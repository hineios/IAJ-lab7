using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class Fireball : WalkToTargetAndExecuteAction
    {
        private int xpChange;

        public Fireball(AutonomousCharacter character, GameObject target) : base("Fireball",character,target)
        {
            //TODO: implement
            throw new NotImplementedException();
        }

        public override float GetGoalChange(Goal goal)
        {
            //TODO: implement
            throw new NotImplementedException();

        }

        public override bool CanExecute()
        {
            //TODO: implement
            throw new NotImplementedException();
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            //TODO: implement
            throw new NotImplementedException();
        }

        public override void Execute()
        {
            //TODO: implement
            throw new NotImplementedException();
        }
        

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            //TODO: implement
            throw new NotImplementedException();
        }
    }
}
