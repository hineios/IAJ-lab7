using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class GetHealthPotion : WalkToTargetAndExecuteAction
    {
        public GetHealthPotion(AutonomousCharacter character, GameObject target) : base("GetHealthPotion",character,target)
        {
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
