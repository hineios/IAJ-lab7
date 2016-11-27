using Assets.Scripts.GameManager;
using System;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTSBiasedPlayout : MCTS
    {
		public const int TIME = 200;

        public MCTSBiasedPlayout(CurrentStateWorldModel currentStateWorldModel) : base(currentStateWorldModel)
        {
        }

        protected override Reward Playout(WorldModel initialPlayoutState)
        {
			//Debug.Log("Playout MCTS Biased");
			WorldModel roll = initialPlayoutState.GenerateChildWorldModel();


			while (!roll.IsTerminal ()) 
			{
				

			}

			Reward reward = new Reward();
			reward.Value = roll.GetScore();
			return reward;
        }

        protected MCTSNode Expand(WorldModel parentState, GOB.Action action)
        {
			WorldModel futureState = parentState.GenerateChildWorldModel();
			action.ApplyActionEffects(futureState);
			//futureState.CalculateNextPlayer();
			MCTSNode child = new MCTSNode(futureState);
			child.Action = action;
			return child;
        }



		private float actionScore(WorldModel world)
		{
			int mana = 0;
			int hp = 0;
			int money = 0;
			float timeLeft = 0.0f;
			float result = 0.0f;
			mana = (int) world.GetProperty (Properties.MANA);
			hp =(int) world.GetProperty (Properties.HP);
			money =(int) world.GetProperty (Properties.MONEY);
			timeLeft = TIME -(float) world.GetProperty (Properties.TIME);
			if (hp > 0 && timeLeft > 0) {
				result = hp * 0.3f + mana * 0.2f + money * 0.5f;
			}
				
			return result;
		}
    }
}
