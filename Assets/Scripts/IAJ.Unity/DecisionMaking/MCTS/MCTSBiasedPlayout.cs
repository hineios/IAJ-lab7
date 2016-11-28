using Assets.Scripts.GameManager;
using System;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;
using System.Collections;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTSBiasedPlayout : MCTS
    {

        public MCTSBiasedPlayout(CurrentStateWorldModel currentStateWorldModel) : base(currentStateWorldModel)
        {
        }

		protected override Reward Playout(EfficientWorldModel initialPlayoutState)
        {
			EfficientWorldModel roll = initialPlayoutState.GenerateChildWorldModel();
			int playoutReach = 0;
			double sumOfActionsH = 0.0f;
			while (!roll.IsTerminal ()) 
			{
				GOB.Action[] actions = roll.GetExecutableActions ();
				GOB.Action choosenAction = actions[0];
				foreach (GOB.Action action in actions) {
					sumOfActionsH += Math.Exp(action.GetH (roll));
				}
				int i = 0;
				float actionValue = 0.0f;
				double gibbsProb = 20.0;
				double currentGibbsProb = 0.0;
				foreach (GOB.Action action in actions) {
					actionValue = action.GetH (roll);
<<<<<<< HEAD
					currentGibbsProb = Math.Exp(-actionValue) /sumOfActionsH;
					if (currentGibbsProb > gibbsProb) {
=======
					currentGibbsProb = Math.Exp(actionValue) /sumOfActionsH;
					if (currentGibbsProb < gibbsProb) {
>>>>>>> c805cb3ad9dc10e1df02802407a049253cbeee2c
						gibbsProb = currentGibbsProb;
						choosenAction = action;
					}
					i++;
				}
				choosenAction.ApplyActionEffects (roll);
				roll.CalculateNextPlayer();
				playoutReach++;
			}

			if (playoutReach > MaxPlayoutDepthReached)
				MaxPlayoutDepthReached = playoutReach;

			float re = roll.GetScore ();
			Reward reward = new Reward();
			reward.Value = roll.GetScore();
			reward.PlayerID = roll.GetNextPlayer();
			return reward;
        }
    }
}
