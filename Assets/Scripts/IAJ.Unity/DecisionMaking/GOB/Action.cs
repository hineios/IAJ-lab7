using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class Action
    {
        protected float DurationWeight = 0.5f;
        protected float ActionWeight = 0.5f;
        
        public string Name { get; set; }
        private Dictionary<Goal, float> GoalEffects { get; set; }
        public float Duration { get; set; }

        public Action(string name)
        {
            this.Name = name;
            this.GoalEffects = new Dictionary<Goal, float>();
        }

        public void AddEffect(Goal goal, float goalChange)
        {
            this.GoalEffects[goal] = goalChange;
        }

        public virtual float GetGoalChange(Goal goal)
        {
            if (this.GoalEffects.ContainsKey(goal))
            {
                return this.GoalEffects[goal];
            }
            else return 0.0f;
        }

        public virtual float GetDuration()
        {
            return this.Duration;
        }

        public virtual float GetDuration(WorldModel worldModel)
        {
            return this.Duration;
        }

        public virtual bool CanExecute(WorldModel woldModel)
        {
            return true;
        }

        public virtual bool CanExecute()
        {
            return true;
        }

        public virtual void Execute()
        {
        }

        public virtual void ApplyActionEffects(WorldModel worldModel)
        {
        }

		public virtual float GetH(WorldModel currentState)
		{
            //Keep in mind that low H value means an action is desirable
            //Also keep in mind that this method is only called if the action is executable
			return 0.0f;
		}
    }
}
