using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class GetHealthPotion : WalkToTargetAndExecuteAction
    {
        private int hpChange;

        public GetHealthPotion(AutonomousCharacter character, GameObject target) : base("GetHealthPotion", character, target)
        {
            this.hpChange = this.Character.GameManager.characterData.MaxHP - this.Character.GameManager.characterData.HP;
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
                change += -hpChange;
            return change;
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return this.Character.GameManager.characterData.HP < this.Character.GameManager.characterData.MaxHP;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            var hp = (int)worldModel.GetProperty(Properties.HP);
            var maxhp = (int)worldModel.GetProperty(Properties.MAXHP);
            return hp < maxhp;
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.GetHealthPotion(this.Target);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);
            worldModel.SetProperty(Properties.HP, 10);
            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }

		public override float GetH(WorldModel currentState)
		{
            var hp = (int)currentState.GetProperty(Properties.HP);

			if (hp <= 5) {
				hp = 0;
			} else if (hp <= 10)
				hp = 2;
			else
				hp = 4;

            //if the change in HP is big (the current HP is low), we should consider doing it
            return this.ActionWeight*hp + this.DurationWeight*base.GetH(currentState);
		}
    }
}
