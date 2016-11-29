using System.Collections.Generic;
using Assets.Scripts.GameManager;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.GOB.Action;
using Assets.Scripts.IAJ.Unity.Utils;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    class MCTSRAVEandBiasedPlayout : MCTS
    {
        protected const float b = 1;
        protected List<Pair<int, Action>> ActionHistory { get; set; }

        public const int TIME = 200;
        public MCTSRAVEandBiasedPlayout(CurrentStateWorldModel currentStateWorldModel) : base(currentStateWorldModel)
        {
        }

        protected override MCTSNode BestUCTChild(MCTSNode node)
        {
            float MCTSValue;
            float RAVEValue;
            float UCTValue;
            float bestUCT = float.MinValue;
            MCTSNode bestNode = null;

            //step 1, calculate beta and 1-beta. beta does not change from child to child. So calculate this only once
            float beta = node.NRAVE / (node.N + node.NRAVE + (4 * node.N * node.NRAVE * b * b));
            float betaMinus = 1 - beta;

            //step 2, calculate the MCTS value, the RAVE value, and the UCT for each child and determine the best one
            foreach (MCTSNode child in node.ChildNodes)
            {
                RAVEValue = child.QRAVE / child.NRAVE;
                MCTSValue = child.Q / child.N;
                UCTValue = (betaMinus * MCTSValue + beta * RAVEValue) + C * (float)Math.Sqrt(Math.Log(child.Parent.N) / child.N);
                if (UCTValue > bestUCT)
                {
                    bestNode = child;
                    bestUCT = UCTValue;
                }
            }
            return bestNode;
        }


		protected override Reward Playout(EfficientWorldModel initialPlayoutState)
        {
            //throw new NotImplementedException();
            ActionHistory = new List<Pair<int, GOB.Action>>();
			EfficientWorldModel roll = initialPlayoutState.GenerateChildWorldModel();
            Action choosenAction;
            GOB.Action[] actions;
            double sumOfActionsH = 0.0f;

            int playoutReach = 0;

            while (!roll.IsTerminal())
            {
                // Biased Playout phase
                actions = roll.GetExecutableActions();
                choosenAction = actions[0];
                foreach (GOB.Action action in actions)
                {
                    sumOfActionsH += Math.Exp(action.GetH(roll));
                }
                int i = 0;
                float actionValue = 0.0f;
                double gibbsProb = 20.0;
                double currentGibbsProb = 0.0;
                foreach (GOB.Action action in actions)
                {
                    actionValue = action.GetH(roll);
                    currentGibbsProb = Math.Exp(actionValue) / sumOfActionsH;
                    if (currentGibbsProb < gibbsProb)
                    {
                        gibbsProb = currentGibbsProb;
                        choosenAction = action;
                    }
                    i++;
                }

                //RAVE history, for selection
                ActionHistory.Add(new Pair<int, GOB.Action>(roll.GetNextPlayer(), choosenAction));
                choosenAction.ApplyActionEffects(roll);
                roll.CalculateNextPlayer();
                playoutReach += 1;
            }

            if (playoutReach > MaxPlayoutDepthReached)
                MaxPlayoutDepthReached = playoutReach;

            Reward r = new Reward();
            r.Value = roll.GetScore();
            r.PlayerID = roll.GetNextPlayer();
            return r;
        }
        
        protected override void Backpropagate(MCTSNode node, Reward reward)
        {
            while (node != null)
            {
                node.N += 1;
                node.Q += reward.GetRewardForNode(node);
                if (node.Parent != null)
                    ActionHistory.Add(new Pair<int, GOB.Action>(node.Parent.PlayerID, node.Action));
                node = node.Parent;

                // RAVE stuff
                if (node != null)
                {
                    var p = node.PlayerID;
                    foreach (var c in node.ChildNodes)
                    {
                        if (ActionHistory.Contains(new Pair<int, GOB.Action>(p, c.Action)))
                        {
                            c.NRAVE += 1;
                            c.QRAVE += reward.GetRewardForNode(c);
                        }
                    }
                }
            }
        }
    }
}
