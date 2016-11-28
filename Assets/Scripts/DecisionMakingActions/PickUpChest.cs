using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class PickUpChest : WalkToTargetAndExecuteAction
    {
		public PickUpChest(AutonomousCharacter character, GameObject target) : base("PickUpChest",character,target)
        {
        }


        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            if (goal.Name == AutonomousCharacter.GET_RICH_GOAL) change -= 5.0f;
            return change;
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return true;
        }

		public override bool CanExecute(EfficientWorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            return true;
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.PickUpChest(this.Target);
        }

		public override void ApplyActionEffects(EfficientWorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            //var goalValue = worldModel.GetGoalValue(AutonomousCharacter.GET_RICH_GOAL);
            //worldModel.SetGoalValue(AutonomousCharacter.GET_RICH_GOAL, goalValue - 5.0f);
		
            var money = (int)worldModel.GetProperty(Properties.MONEY);
            worldModel.SetProperty(Properties.MONEY, money + 5);

            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }

		public override float GetH(EfficientWorldModel currentState)
		{
			var distance = base.GetH (currentState);
			float guardianValue = 40.0f;
			var guardian = currentState.getItemGuardian (this.Target.name);
			if (guardian != null) {
				if (!(bool)currentState.GetProperty (guardian))
					guardianValue = 0.0f;
			}
            //We do want to get rich as fast as possible (h = 0, consider only the durantion of the action)
			return this.DurationWeight*base.GetH(currentState) + guardianValue;
		}
    }
}
