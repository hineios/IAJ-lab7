using System;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System.Collections.Generic;
using Assets.Scripts.GameManager;
using System.Linq;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
	public class EfficientWorldModel
	{
		private const int WORLD_ARRAY_SIZE = 22;

		private const int HP = 0;
		private const int MANA = 1;
		private const int MAXHP = 2;
		private const int XP = 3;
		private const int TIME = 4;
		private const int MONEY = 5;
		private const int LEVEL = 6;
		private const int POSITION = 7;
		private const int CHEST_ONE = 8;
		private const int CHEST_TWO = 9;
		private const int CHEST_THREE = 10;
		private const int CHEST_FOUR = 11;
		private const int CHEST_FIVE = 12;
		private const int MANA_ONE = 13;
		private const int MANA_TWO = 14;
		private const int HEALTH_ONE = 15;
		private const int HEALTH_TWO = 16;
		private const int SKELETON_ONE = 17;
		private const int SKELETON_TWO = 18;
		private const int ORC_ONE = 19;
		private const int ORC_TWO = 20;
		private const int DRAGON = 21;

		// These positions tell if enemies are active (Alive) or inactive (Dead)
		private const string SKELETON_ONE_LABEL = "Skeleton (2)";
		private const string SKELETON_TWO_LABEL = "Skeleton (3)";
		private const string ORC_ONE_LABEL = "Orc";
		private const string ORC_TWO_LABEL = "Orc (1)";
		private const string DRAGON_LABEL = "Dragon";

		// These positions tell if items are active (not picked) or inactive (picked)
		private const string CHEST_ONE_LABEL = "Chest";
		private const string CHEST_TWO_LABEL = "Chest (1)";
		private const string CHEST_THREE_LABEL = "Chest (2)";
		private const string CHEST_FOUR_LABEL = "Chest (4)";
		private const string CHEST_FIVE_LABEL = "Chest (3)";
		private const string MANA_ONE_LABEL = "ManaPotion";
		private const string MANA_TWO_LABEL = "ManaPotion (1)";
		private const string HEALTH_ONE_LABEL = "HealthPotion";
		private const string HEALTH_TWO_LABEL = "HealthPotion (1)";

		private Dictionary<string, string> guardians;

		private List<Action> Actions { get; set; }
		protected IEnumerator<Action> ActionEnumerator { get; set; }

		protected EfficientWorldModel Parent { get; set; }

		private Dictionary<string, float> GoalValues { get; set; } 

		public Object[] worldState = new object[WORLD_ARRAY_SIZE];

		public EfficientWorldModel (List<Action> actions, GameManager.GameManager manager)
		{
			this.guardians = new Dictionary<string, string> (5);
			this.GoalValues = new Dictionary<string, float>();
			createInitialInformation (manager);
			this.Actions = actions;
			this.ActionEnumerator = actions.GetEnumerator();
		}


		public EfficientWorldModel (EfficientWorldModel worldModel)
		{
			worldModel.worldState.CopyTo (this.worldState, 0);
			this.guardians = new Dictionary<string, string>(worldModel.guardians);
			this.GoalValues = new Dictionary<string, float>();
			this.Actions = worldModel.Actions;
			this.ActionEnumerator = this.Actions.GetEnumerator();
		}

		private void createInitialInformation(GameManager.GameManager manager)
		{
			worldState [HP] = manager.characterData.HP;
			worldState [MANA] = manager.characterData.Mana;
			worldState [MAXHP] = manager.characterData.MaxHP;
			worldState [XP] = manager.characterData.XP;
			worldState [TIME] = manager.characterData.Time;
			worldState [MONEY] = manager.characterData.Money;
			worldState [LEVEL] = manager.characterData.Level;
			worldState [POSITION] = manager.characterData.CharacterGameObject.transform.position;
			int i;
			for (i = 8; i < WORLD_ARRAY_SIZE; i++) {
				worldState [i] = true;
			}
			guardians.Add (CHEST_ONE_LABEL, SKELETON_ONE_LABEL);
			guardians.Add (CHEST_TWO_LABEL, ORC_TWO_LABEL);
			guardians.Add (CHEST_THREE_LABEL, ORC_ONE_LABEL);
			guardians.Add (CHEST_FOUR_LABEL, DRAGON_LABEL);
			guardians.Add (CHEST_FIVE_LABEL, SKELETON_TWO_LABEL);
		}


		public virtual EfficientWorldModel GenerateChildWorldModel()
		{
			return new EfficientWorldModel(this);
		}


		public virtual object GetProperty(string propertyName)
		{
			object result = null;
			switch (propertyName) {
			case Properties.HP:
				result = this.worldState [HP];
				break;
			case Properties.LEVEL:
				result = this.worldState [LEVEL];
				break;
			case Properties.MANA:
				result = this.worldState [MANA];
				break;
			case Properties.MAXHP:
				result = this.worldState [MAXHP];
				break;
			case Properties.MONEY:
				result = this.worldState [MONEY];
				break;
			case Properties.POSITION:
				result = this.worldState [POSITION];
				break;
			case Properties.TIME:
				result = this.worldState [TIME];
				break;
			case Properties.XP:
				result = this.worldState [XP];
				break;
			case CHEST_ONE_LABEL:
				result = this.worldState [CHEST_ONE];
				break;
			case CHEST_TWO_LABEL:
				result = this.worldState [CHEST_TWO];
				break;
			case CHEST_THREE_LABEL:
				result = this.worldState [CHEST_THREE];
				break;
			case CHEST_FOUR_LABEL:
				result = this.worldState [CHEST_FOUR];
				break;
			case CHEST_FIVE_LABEL:
				result = this.worldState [CHEST_FIVE];
				break;
			case MANA_ONE_LABEL:
				result = this.worldState [MANA_ONE];
				break;
			case MANA_TWO_LABEL:
				result = this.worldState [MANA_TWO];
				break;
			case HEALTH_ONE_LABEL:
				result = this.worldState [HEALTH_ONE];
				break;
			case HEALTH_TWO_LABEL:
				result = this.worldState [HEALTH_TWO];
				break;
			case SKELETON_ONE_LABEL:
				result = this.worldState [SKELETON_ONE];
				break;
			case SKELETON_TWO_LABEL:
				result = this.worldState [SKELETON_TWO];
				break;
			case ORC_ONE_LABEL:
				result = this.worldState [ORC_ONE];
				break;
			case ORC_TWO_LABEL:
				result = this.worldState [ORC_TWO];
				break;
			case DRAGON_LABEL:
				result = this.worldState [DRAGON];
				break;
			}

			return result;
		}

		public virtual void SetProperty(string propertyName, object value)
		{
			switch (propertyName) {
			case Properties.HP:
				this.worldState [HP] = value;
				break;
			case Properties.LEVEL:
				this.worldState [LEVEL] = value;
				break;
			case Properties.MANA:
				this.worldState [MANA] = value;
				break;
			case Properties.MAXHP:
				this.worldState [MAXHP] = value;
				break;
			case Properties.MONEY:
				this.worldState [MONEY] = value;
				break;
			case Properties.POSITION:
				this.worldState [POSITION] = value;
				break;
			case Properties.TIME:
				this.worldState [TIME] = value;
				break;
			case Properties.XP:
				this.worldState [XP] = value;
				break;
			case CHEST_ONE_LABEL:
				this.worldState [CHEST_ONE] = value;
				break;
			case CHEST_TWO_LABEL:
				this.worldState [CHEST_TWO] = value;
				break;
			case CHEST_THREE_LABEL:
				this.worldState [CHEST_THREE] = value;
				break;
			case CHEST_FOUR_LABEL:
				this.worldState [CHEST_FOUR] = value;
				break;
			case CHEST_FIVE_LABEL:
				this.worldState [CHEST_FIVE] = value;
				break;
			case MANA_ONE_LABEL:
				this.worldState [MANA_ONE] = value;
				break;
			case MANA_TWO_LABEL:
				this.worldState [MANA_TWO] = value;
				break;
			case HEALTH_ONE_LABEL:
				this.worldState [HEALTH_ONE] = value;
				break;
			case HEALTH_TWO_LABEL:
				this.worldState [HEALTH_TWO] = value;
				break;
			case SKELETON_ONE_LABEL:
				this.worldState [SKELETON_ONE] = value;
				break;
			case SKELETON_TWO_LABEL:
				this.worldState [SKELETON_TWO] = value;
				break;
			case ORC_ONE_LABEL:
				this.worldState [ORC_ONE] = value;
				break;
			case ORC_TWO_LABEL:
				this.worldState [ORC_TWO] = value;
				break;
			case DRAGON_LABEL:
				this.worldState [DRAGON] = value;
				break;
			}
		}


		public string getItemGuardian(string itemName)
		{
			if (guardians.ContainsKey (itemName))
				return guardians [itemName];
			else
				return null;
		}


		public virtual Action GetNextAction()
		{
			Action action = null;
			//returns the next action that can be executed or null if no more executable actions exist
			if (this.ActionEnumerator.MoveNext())
			{
				action = this.ActionEnumerator.Current;
			}

			while (action != null && !action.CanExecute(this))
			{
				if (this.ActionEnumerator.MoveNext())
				{
					action = this.ActionEnumerator.Current;    
				}
				else
				{
					action = null;
				}
			}

			return action;
		}

		public virtual Action[] GetExecutableActions()
		{
			return this.Actions.Where(a => a.CanExecute(this)).ToArray();
		}

		public virtual bool IsTerminal()
		{
			return true;
		}


		public virtual float GetScore()
		{
			return 0.0f;
		}

		public virtual int GetNextPlayer()
		{
			return 0;
		}

		public virtual void CalculateNextPlayer()
		{
		}


		public float CalculateDiscontentment(List<Goal> goals)
		{
			return 0.0f;
		}
	}
}

