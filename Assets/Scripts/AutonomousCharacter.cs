using System.Collections.Generic;
using Assets.Scripts.DecisionMakingActions;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Pathfinding;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using RAIN.Navigation;
using RAIN.Navigation.NavMesh;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.HPStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS;
using Assets.Scripts.GameManager;

namespace Assets.Scripts
{
    public class AutonomousCharacter : MonoBehaviour
    {
        //constants
        public const string SURVIVE_GOAL = "Survive";
        public const string GAIN_XP_GOAL = "GainXP";
        public const string BE_QUICK_GOAL = "BeQuick";
        public const string GET_RICH_GOAL = "GetRich";

        public const float DECISION_MAKING_INTERVAL = 10.0f;
        //public fields to be set in Unity Editor
        public GameManager.GameManager GameManager;
        public Text SurviveGoalText;
        public Text GainXPGoalText;
        public Text BeQuickGoalText;
        public Text GetRichGoalText;
        public Text TotalProcessingTimeText;
        public Text BestDiscontentmentText;
        public Text ProcessedActionsText;
        public Text BestActionText;
        public bool MCTSActive;


        public Goal BeQuickGoal { get; private set; }
        public Goal SurviveGoal { get; private set; }
        public Goal GetRichGoal { get; private set; }
        public Goal GainXPGoal { get; private set; }
        public List<Goal> Goals { get; set; }
        public List<Action> Actions { get; set; }
        public Action CurrentAction { get; private set; }
        public DynamicCharacter Character { get; private set; }
        public DepthLimitedGOAPDecisionMaking GOAPDecisionMaking { get; set; }
        public MCTS MCTSDecisionMaking { get; set; }
        public AStarPathfinding AStarPathFinding;

        //private fields for internal use only
        private Vector3 startPosition;
        private GlobalPath currentSolution;
        private GlobalPath currentSmoothedSolution;
        private NavMeshPathGraph navMesh;
        
        private bool draw;
        private float nextUpdateTime = 0.0f;
        private float previousGold = 0.0f;
        private int previousXP = 0;
        private Vector3 previousTarget;



        public void Initialize(NavMeshPathGraph navMeshGraph, AStarPathfinding pathfindingAlgorithm)
        {
            this.MCTSActive = true; //change this if you want to try DL-GOAP
            this.draw = false;
            this.navMesh = navMeshGraph;
            this.AStarPathFinding = pathfindingAlgorithm;
            this.AStarPathFinding.NodesPerSearch = 100;
        }

        public void Start()
        {
            this.draw = true;

            this.navMesh = NavigationManager.Instance.NavMeshGraphs[0];
            this.Character = new DynamicCharacter(this.gameObject);

            var clusterGraph = Resources.Load<ClusterGraph>("ClusterGraph");
       
            this.Initialize(NavigationManager.Instance.NavMeshGraphs[0], new NodeArrayAStarPathFinding(NavigationManager.Instance.NavMeshGraphs[0], new GatewayHeuristic(clusterGraph)));


            //initialization of the GOB decision making
            //let's start by creating 4 main goals
            
            this.SurviveGoal = new Goal(SURVIVE_GOAL, 2.0f);

            this.GainXPGoal = new Goal(GAIN_XP_GOAL, 1.0f)
            {
                ChangeRate = 0.1f
            };

            this.GetRichGoal = new Goal(GET_RICH_GOAL, 1.0f)
            {
                InsistenceValue = 5.0f,
                ChangeRate = 0.2f
            };

            this.BeQuickGoal = new Goal(BE_QUICK_GOAL, 1.0f)
            {
                ChangeRate = 0.1f
            };

            this.Goals = new List<Goal>();
            this.Goals.Add(this.SurviveGoal);
            this.Goals.Add(this.BeQuickGoal);
            this.Goals.Add(this.GetRichGoal);
            this.Goals.Add(this.GainXPGoal);

            //initialize the available actions

            this.Actions = new List<Action>();

            this.Actions.Add(new LevelUp(this));

            foreach (var chest in GameObject.FindGameObjectsWithTag("Chest"))
            {
                this.Actions.Add(new PickUpChest(this, chest));
            }

            foreach (var potion in GameObject.FindGameObjectsWithTag("ManaPotion"))
            {
                this.Actions.Add(new GetManaPotion(this, potion));
            }

            foreach (var potion in GameObject.FindGameObjectsWithTag("HealthPotion"))
            {
                this.Actions.Add(new GetHealthPotion(this, potion));
            }

            foreach (var enemy in GameObject.FindGameObjectsWithTag("Skeleton"))
            {
                this.Actions.Add(new SwordAttack(this, enemy));
                this.Actions.Add(new Fireball(this, enemy));
            }

            foreach (var enemy in GameObject.FindGameObjectsWithTag("Orc"))
            {
                this.Actions.Add(new SwordAttack(this, enemy));
                this.Actions.Add(new Fireball(this, enemy));
            }

            foreach (var enemy in GameObject.FindGameObjectsWithTag("Dragon"))
            {
                this.Actions.Add(new SwordAttack(this, enemy));
                this.Actions.Add(new Fireball(this, enemy));
            }

            var worldModel = new CurrentStateWorldModel(this.GameManager, this.Actions, this.Goals);
            this.GOAPDecisionMaking = new DepthLimitedGOAPDecisionMaking(worldModel,this.Actions,this.Goals);
            this.MCTSDecisionMaking = new MCTS(worldModel);
            this.MCTSDecisionMaking.MaxIterations = 5000;
            this.MCTSDecisionMaking.MaxIterationsProcessedPerFrame = 25;
        }

        void Update()
        {
            if (Time.time > this.nextUpdateTime || this.GameManager.WorldChanged)
            {
                this.GameManager.WorldChanged = false;
                this.nextUpdateTime = Time.time + DECISION_MAKING_INTERVAL;

                //first step, perceptions
                //update the agent's goals based on the state of the world
                this.SurviveGoal.InsistenceValue = this.GameManager.characterData.MaxHP - this.GameManager.characterData.HP;

                this.BeQuickGoal.InsistenceValue += DECISION_MAKING_INTERVAL * 0.1f;
                if(this.BeQuickGoal.InsistenceValue > 10.0f)
                {
                    this.BeQuickGoal.InsistenceValue = 10.0f;
                }

                this.GainXPGoal.InsistenceValue += 0.1f; //increase in goal over time
                if(this.GameManager.characterData.XP > this.previousXP)
                {
                    this.GainXPGoal.InsistenceValue -= this.GameManager.characterData.XP - this.previousXP;
                    this.previousXP = this.GameManager.characterData.XP;
                }

                this.GetRichGoal.InsistenceValue += 0.1f; //increase in goal over time
                if (this.GetRichGoal.InsistenceValue > 10)
                {
                    this.GetRichGoal.InsistenceValue = 10.0f;
                }

                if (this.GameManager.characterData.Money > this.previousGold)
                {
                    this.GetRichGoal.InsistenceValue -= this.GameManager.characterData.Money - this.previousGold;
                    this.previousGold = this.GameManager.characterData.Money;
                }

                this.SurviveGoalText.text = "Survive: " + this.SurviveGoal.InsistenceValue;
                this.GainXPGoalText.text = "Gain XP: " + this.GainXPGoal.InsistenceValue.ToString("F1");
                this.BeQuickGoalText.text = "Be Quick: " + this.BeQuickGoal.InsistenceValue.ToString("F1");
                this.GetRichGoalText.text = "GetRich: " + this.GetRichGoal.InsistenceValue.ToString("F1");

                //initialize Decision Making Proccess
                this.CurrentAction = null;
                if(this.MCTSActive)
                {
                    this.MCTSDecisionMaking.InitializeMCTSearch();
                }
                else
                {
                    this.GOAPDecisionMaking.InitializeDecisionMakingProcess();
                }
            }

            if(this.MCTSActive)
            {
                this.UpdateMCTS();
            }
            else
            {
                this.UpdateDLGOAP();
            }

            if(this.CurrentAction != null)
            {
                if(this.CurrentAction.CanExecute())
                {
                    this.CurrentAction.Execute();
                }
            }

            //call the pathfinding method if the user specified a new goal
            if (this.AStarPathFinding.InProgress)
            {
                var finished = this.AStarPathFinding.Search(out this.currentSolution);
                if (finished && this.currentSolution != null)
                {
                    //lets smooth out the Path
                    this.startPosition = this.Character.KinematicData.position;
                    this.currentSmoothedSolution = StringPullingPathSmoothing.SmoothPath(this.Character.KinematicData, this.currentSolution);
                    this.currentSmoothedSolution.CalculateLocalPathsFromPathPositions(this.Character.KinematicData.position);
                    this.Character.Movement = new DynamicFollowPath(this.Character.KinematicData, this.currentSmoothedSolution)
                    {
                        MaxAcceleration = 200.0f,
                        MaxSpeed = 40.0f
                    };
                }
            }


            this.Character.Update();
        }

        private void UpdateMCTS()
        {
            if (this.MCTSDecisionMaking.InProgress)
            {
                var action = this.MCTSDecisionMaking.Run();
                if (action != null)
                {
                    this.CurrentAction = action;
                }
            }

            this.TotalProcessingTimeText.text = "Process. Time: " + this.MCTSDecisionMaking.TotalProcessingTime.ToString("F");
            
            this.ProcessedActionsText.text = "Max Depth: " + this.MCTSDecisionMaking.MaxPlayoutDepthReached.ToString();

            if (this.MCTSDecisionMaking.BestFirstChild != null)
            {
                var q = this.MCTSDecisionMaking.BestFirstChild.Q / this.MCTSDecisionMaking.BestFirstChild.N;
                this.BestDiscontentmentText.text = "Best Exp. Q value: " + q.ToString("F");
                var actionText = "";
                foreach (var action in this.MCTSDecisionMaking.BestActionSequence)
                {
                    actionText += "\n" + action.Name;
                }
                this.BestActionText.text = "Best Action Sequence: " + actionText;
            }
            else
            {
                this.BestActionText.text = "Best Action Sequence:\nNone";
            }
        }

        private void UpdateDLGOAP()
        {
            if (this.GOAPDecisionMaking.InProgress)
            {
                //choose an action using the GOB Decision Making process
                var action = this.GOAPDecisionMaking.ChooseAction();
                if (action != null)
                {
                    this.CurrentAction = action;
                }
            }

            this.TotalProcessingTimeText.text = "Process. Time: " + this.GOAPDecisionMaking.TotalProcessingTime.ToString("F");
            this.BestDiscontentmentText.text = "Best Discontentment: " + this.GOAPDecisionMaking.BestDiscontentmentValue.ToString("F");
            this.ProcessedActionsText.text = "Act. comb. processed: " + this.GOAPDecisionMaking.TotalActionCombinationsProcessed;

            if (this.GOAPDecisionMaking.BestAction != null)
            {
                var actionText = "";
                foreach (var action in this.GOAPDecisionMaking.BestActionSequence)
                {
                    actionText += "\n" + action.Name;
                }
                this.BestActionText.text = "Best Action Sequence: " + actionText;
            }
            else
            {
                this.BestActionText.text = "Best Action Sequence:\nNone";
            }
        }

        public void StartPathfinding(Vector3 targetPosition)
        {
            //if the targetPosition received is the same as a previous target, then this a request for the same target
            //no need to redo the pathfinding search
            if(!this.previousTarget.Equals(targetPosition))
            {
                this.AStarPathFinding.InitializePathfindingSearch(this.Character.KinematicData.position, targetPosition);
                this.previousTarget = targetPosition;
            }
        }
    }
}
