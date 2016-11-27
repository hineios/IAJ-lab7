using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.GOB.Action;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTSRAVE : MCTS
    {
        protected const float b = 1;
        protected List<Pair<int,Action>> ActionHistory { get; set; }
        public MCTSRAVE(CurrentStateWorldModel worldModel) :base(worldModel)
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
            float beta = node.NRAVE / (node.N + node.NRAVE + (4*node.N*node.NRAVE*b*b));
            float betaMinus = 1 - beta;

            //step 2, calculate the MCTS value, the RAVE value, and the UCT for each child and determine the best one
            foreach (MCTSNode child in node.ChildNodes)
            {
                RAVEValue = child.QRAVE/child.NRAVE ;
                MCTSValue = child.Q/child.N;
                UCTValue = (betaMinus*MCTSValue + beta*RAVEValue) + C * (float)Math.Sqrt(Math.Log(child.Parent.N) / child.N);
                if (UCTValue > bestUCT)
                {
                    bestNode = child;
                    bestUCT = UCTValue;
                }
            }
            return bestNode;
        }


        protected override Reward Playout(WorldModel initialPlayoutState)
        {
            //throw new NotImplementedException();
            ActionHistory = new List<Pair<int, GOB.Action>>();
            WorldModel model = initialPlayoutState.GenerateChildWorldModel();
            Action action;

            int playoutReach = 0;

            while (!model.IsTerminal())
            {
                //Select a random Action
                action = model.GetExecutableActions()[RandomGenerator.Next(0, model.GetExecutableActions().Length)];
                ActionHistory.Add(new Pair<int, GOB.Action>(model.GetNextPlayer(), action));
                action.ApplyActionEffects(model);
                model.CalculateNextPlayer();
                playoutReach += 1;
            }

            if (playoutReach > MaxPlayoutDepthReached)
                MaxPlayoutDepthReached = playoutReach;

            Reward r = new Reward();
            r.Value = model.GetScore();
            r.PlayerID = model.GetNextPlayer();
            return r;
        }

        protected override void Backpropagate(MCTSNode node, Reward reward)
        {
            while (node != null)
            {
                node.N += 1;
                node.Q += reward.GetRewardForNode(node);
                if(node.Parent != null)
                    ActionHistory.Add(new Pair<int, GOB.Action>(node.Parent.PlayerID, node.Action));
                node = node.Parent;

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
