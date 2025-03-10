using UnityEngine;

namespace MadLevelManager
{
	public class MadLevelPropertyCondition : MonoBehaviour
	{
		public enum PropertyType
		{
			Completed,
			Locked,
			LevelNumber,
			Custom
		}

		public enum Comparer
		{
			IsEqual,
			IsNotEqual,
			IsGreater,
			IsGreaterOrEqual,
			IsLower,
			IsLowerOrEqual
		}

		public enum Action
		{
			Show,
			Hide
		}

		public PropertyType propertyType;

		public string customPropertyName;

		public Comparer comparer;

		public string rightSideValue = string.Empty;

		public Action action;

		private MadSprite sprite;

		private MadLevelIcon icon;

		private void Start()
		{
			sprite = GetComponent<MadSprite>();
			if (sprite == null)
			{
				UnityEngine.Debug.LogError("Condition needs a MadSprite to be attached to this game object.", this);
				return;
			}
			icon = MadTransform.FindParent<MadLevelIcon>(base.transform);
			if (icon == null)
			{
				UnityEngine.Debug.LogError("Condition need to be set under MadLevelIcon.", this);
			}
			else
			{
				Apply();
			}
		}

		private void Apply()
		{
			string propertyValue = GetPropertyValue();
			if (propertyValue == null)
			{
				ApplyAction(Opposite(action));
			}
			else if (Compare(propertyValue))
			{
				ApplyAction(action);
			}
			else
			{
				ApplyAction(Opposite(action));
			}
		}

		private Action Opposite(Action action)
		{
			switch (action)
			{
			case Action.Hide:
				return Action.Show;
			case Action.Show:
				return Action.Hide;
			default:
				UnityEngine.Debug.LogError("Unknown action: " + action, this);
				return Action.Hide;
			}
		}

		private string GetPropertyValue()
		{
			string name = icon.level.name;
			switch (propertyType)
			{
			case PropertyType.Completed:
				return MadLevelProfile.IsCompleted(name).ToString();
			case PropertyType.Locked:
				return MadLevelProfile.IsLocked(name).ToString();
			case PropertyType.LevelNumber:
				return icon.levelNumber.text;
			case PropertyType.Custom:
				return MadLevelProfile.GetLevelAny(name, customPropertyName, null);
			default:
				UnityEngine.Debug.LogError("Unknown property type: " + propertyType);
				return null;
			}
		}

		private bool Compare(string leftSideValue)
		{
			switch (comparer)
			{
			case Comparer.IsEqual:
				return leftSideValue == rightSideValue;
			case Comparer.IsNotEqual:
				return leftSideValue != rightSideValue;
			case Comparer.IsGreater:
				return CompareDoubles(leftSideValue, rightSideValue) == 1;
			case Comparer.IsGreaterOrEqual:
			{
				int num2 = CompareDoubles(leftSideValue, rightSideValue);
				return num2 == 1 || num2 == 0;
			}
			case Comparer.IsLower:
				return CompareDoubles(leftSideValue, rightSideValue) == -1;
			case Comparer.IsLowerOrEqual:
			{
				int num = CompareDoubles(leftSideValue, rightSideValue);
				return num == -1 || num == 0;
			}
			default:
				UnityEngine.Debug.LogError("Unknown comparer: " + comparer);
				return false;
			}
		}

		private int CompareDoubles(string a, string b)
		{
			if (!double.TryParse(a, out double result))
			{
				return -2;
			}
			if (!double.TryParse(b, out double result2))
			{
				return -2;
			}
			if (result < result2)
			{
				return -1;
			}
			if (result > result2)
			{
				return 1;
			}
			return 0;
		}

		private void ApplyAction(Action action)
		{
			switch (action)
			{
			case Action.Show:
				sprite.visible = true;
				break;
			case Action.Hide:
				sprite.visible = false;
				break;
			}
		}
	}
}
