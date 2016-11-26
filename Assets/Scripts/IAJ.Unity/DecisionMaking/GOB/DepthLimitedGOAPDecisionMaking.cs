using Assets.Scripts.GameManager;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class DepthLimitedGOAPDecisionMaking
    {
        public const int MAX_DEPTH = 3;
        public int ActionCombinationsProcessedPerFrame { get; set; }
        public float TotalProcessingTime { get; set; }
        public int TotalActionCombinationsProcessed { get; set; }
        public bool InProgress { get; set; }

        public CurrentStateWorldModel InitialWorldModel { get; set; }
        private List<Action> Actions { get; set; }
        private List<Goal> Goals { get; set; }
        private WorldModel[] Models { get; set; }
        private Action[] ActionPerLevel { get; set; }
        public Action[] BestActionSequence { get; private set; }
        public Action BestAction { get; private set; }
        public float BestDiscontentmentValue { get; private set; }
        private int CurrentDepth {  get; set; }

        public DepthLimitedGOAPDecisionMaking(CurrentStateWorldModel currentStateWorldModel, List<Action> actions, List<Goal> goals)
        {
            this.ActionCombinationsProcessedPerFrame = 200;
            this.Actions = actions;
            this.Goals = goals;
            this.InitialWorldModel = currentStateWorldModel;
        }

        public void InitializeDecisionMakingProcess()
        {
            this.InProgress = true;
            this.TotalProcessingTime = 0.0f;
            this.TotalActionCombinationsProcessed = 0;
            this.CurrentDepth = 0;
            this.Models = new WorldModel[MAX_DEPTH + 1];
            this.Models[0] = this.InitialWorldModel;
            this.ActionPerLevel = new Action[MAX_DEPTH];
            this.BestActionSequence = new Action[MAX_DEPTH];
            this.BestAction = null;
            this.BestDiscontentmentValue = float.MaxValue;
            this.InitialWorldModel.Initialize();
        }

        public Action ChooseAction()
        {
            var processedActions = 0;
            var combinations = 0;

            var startTime = Time.realtimeSinceStartup;

            float bestValue = float.MaxValue;
            while(CurrentDepth >= 0)
            {
                if(CurrentDepth >= MAX_DEPTH)
                {
                    var currentValue = Models[CurrentDepth].CalculateDiscontentment(Goals);
                    if(currentValue < bestValue)
                    {
                        bestValue = currentValue;
                        BestAction = ActionPerLevel[0];
                        BestDiscontentmentValue = currentValue;
                        ActionPerLevel.CopyTo(BestActionSequence, 0);
                    }
                    CurrentDepth -= 1;
                    combinations += 1;
                    TotalActionCombinationsProcessed += 1;
                    continue;
                }

                //if (combinations >= ActionCombinationsProcessedPerFrame) return this.BestAction;

                var NextAction = Models[CurrentDepth].GetNextAction();
                if (NextAction != null)
                {
                    Models[CurrentDepth + 1] = Models[CurrentDepth].GenerateChildWorldModel();
                    NextAction.ApplyActionEffects(Models[CurrentDepth + 1]);
                    ActionPerLevel[CurrentDepth] = NextAction;
                    CurrentDepth += 1;
                    processedActions += 1;
                }
                else
                {
                    CurrentDepth -= 1;
                }
            }

            this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
            this.InProgress = false;

            return this.BestAction;
        }
    }
}
