using Assets.Scripts.GameManager;
using System;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTSBiasedPlayout : MCTS
    {

        public MCTSBiasedPlayout(CurrentStateWorldModel currentStateWorldModel) : base(currentStateWorldModel)
        {
			this.MaxPlayoutDepthReached = 0;
        }

        protected override Reward Playout(WorldModel initialPlayoutState)
        {
			//Debug.Log("Playout MCTS Biased");
			WorldModel roll = initialPlayoutState.GenerateChildWorldModel();

			int playoutReach = 0;
			float sumOfActionsH = 0.0f;
			while (!roll.IsTerminal ()) 
			{
				GOB.Action[] actions = roll.GetExecutableActions ();
				GOB.Action choosenAction = actions[0];
				foreach (GOB.Action action in actions) {
					sumOfActionsH += action.GetH (roll);
				}
				float actionValue = 0.0f;
				float gibbsProb = 0.0f;
				float currentGibbsProb = 0.0f;
				foreach (GOB.Action action in actions) {
					actionValue = action.GetH (roll);
					currentGibbsProb = actionValue / sumOfActionsH;
					if (currentGibbsProb > gibbsProb) {
						gibbsProb = currentGibbsProb;
						choosenAction = action;
					}
				}

				choosenAction.ApplyActionEffects (roll);
				roll.CalculateNextPlayer();
				playoutReach++;
			}

			if (playoutReach > MaxPlayoutDepthReached)
				MaxPlayoutDepthReached = playoutReach;

			Reward reward = new Reward();
			reward.Value = roll.GetScore();
			reward.PlayerID = roll.GetNextPlayer();
			return reward;
        }
			
    }
}
