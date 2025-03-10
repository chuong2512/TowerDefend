using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MadLevelManager
{
	public class MadLevelQuery
	{
		private enum Selector
		{
			All,
			Groups,
			Levels
		}

		private delegate void PropertyProcessor(string level, string name, string propertyValue);

		private Selector selector;

		private string[] selectorGroupName;

		private string[] selectorLevelName;

		private bool hasLevelType;

		private MadLevel.Type levelType;

		private string[] propertyName;

		public MadLevelQuery ForAll()
		{
			selector = Selector.All;
			return this;
		}

		public MadLevelQuery ForGroup(params string[] groupName)
		{
			selectorGroupName = groupName;
			selector = Selector.Groups;
			return this;
		}

		public MadLevelQuery ForLevel(params string[] levelName)
		{
			selectorLevelName = levelName;
			selector = Selector.Levels;
			return this;
		}

		public MadLevelQuery OfLevelType(MadLevel.Type levelType)
		{
			hasLevelType = true;
			this.levelType = levelType;
			return this;
		}

		public MadLevelQuery SelectProperty(params string[] propertyName)
		{
			this.propertyName = propertyName;
			return this;
		}

		public int CountLevels()
		{
			return GetLevelNames().Count;
		}

		public void SetLocked(bool val)
		{
			List<string> levelNames = GetLevelNames();
			for (int i = 0; i < levelNames.Count; i++)
			{
				string levelName = levelNames[i];
				MadLevelProfile.SetLocked(levelName, val);
			}
		}

		public void SetCompleted(bool val)
		{
			List<string> levelNames = GetLevelNames();
			for (int i = 0; i < levelNames.Count; i++)
			{
				string levelName = levelNames[i];
				MadLevelProfile.SetCompleted(levelName, val);
			}
		}

		public int CountProperties()
		{
			if (propertyName == null || propertyName.Length == 0)
			{
				UnityEngine.Debug.LogError("Missing SelectProperty() directive");
				return 0;
			}
			return GetLevelNames().Count * propertyName.Length;
		}

		public int CountEnabled()
		{
			return Count(bool.TrueString);
		}

		public int CountDisabled()
		{
			return Count(bool.FalseString);
		}

		public void SetEnabled()
		{
			SetBoolean(val: true);
		}

		public void SetDisabled()
		{
			SetBoolean(val: false);
		}

		public void SetBoolean(bool val)
		{
			ProcessProperties(delegate(string levelName, string propertyName, string v)
			{
				MadLevelProfile.SetLevelBoolean(levelName, propertyName, val);
			});
		}

		public int CountLocked()
		{
			List<string> levelNames = GetLevelNames();
			int num = 0;
			MadLevelAbstractLayout madLevelAbstractLayout = MadLevelLayout.TryGet();
			for (int i = 0; i < levelNames.Count; i++)
			{
				string levelName = levelNames[i];
				if (madLevelAbstractLayout != null)
				{
					MadLevelIcon icon = madLevelAbstractLayout.GetIcon(levelName);
					if (icon != null)
					{
						if (icon.locked)
						{
							num++;
						}
						continue;
					}
				}
				if (MadLevelProfile.IsLockedSet(levelName))
				{
					if (MadLevelProfile.IsLocked(levelName))
					{
						num++;
					}
				}
				else if (MadLevel.activeConfiguration.FindLevelByName(levelName).lockedByDefault)
				{
					num++;
				}
			}
			return num;
		}

		public int CountUnlocked()
		{
			return CountLevels() - CountLocked();
		}

		public int CountCompleted()
		{
			List<string> levelNames = GetLevelNames();
			int num = 0;
			for (int i = 0; i < levelNames.Count; i++)
			{
				if (MadLevelProfile.IsCompleted(levelNames[i]))
				{
					num++;
				}
			}
			return num;
		}

		public int CountNotCompleted()
		{
			return CountLevels() - CountCompleted();
		}

		public int SumIntegers()
		{
			int sum = 0;
			ProcessProperties(delegate(string ln, string pn, string value)
			{
				if (int.TryParse(value, out int result))
				{
					sum += result;
				}
				else if (!string.IsNullOrEmpty(value))
				{
					UnityEngine.Debug.LogError("Cannot parse property value '" + value + "' to integer.");
				}
			});
			return sum;
		}

		public float SumFloats()
		{
			float sum = 0f;
			ProcessProperties(delegate(string ln, string pn, string value)
			{
				if (float.TryParse(value, out float result))
				{
					sum += result;
				}
				else if (!string.IsNullOrEmpty(value))
				{
					UnityEngine.Debug.LogError("Cannot parse property value '" + value + "' to float.");
				}
			});
			return sum;
		}

		private int Count(string val)
		{
			int result = 0;
			if (!ProcessProperties(delegate(string ln, string pn, string value)
			{
				if (value == val)
				{
					result++;
				}
			}))
			{
				return 0;
			}
			return result;
		}

		private bool ProcessProperties(PropertyProcessor processor)
		{
			List<string> levelNames = GetLevelNames();
			if (propertyName == null || propertyName.Length == 0)
			{
				UnityEngine.Debug.LogError("Missing SelectProperty() directive");
				return false;
			}
			for (int i = 0; i < levelNames.Count; i++)
			{
				string text = levelNames[i];
				for (int j = 0; j < propertyName.Length; j++)
				{
					string text2 = propertyName[j];
					if (!MadLevelProfile.IsLevelPropertySet(text, text2))
					{
						processor(text, text2, null);
					}
					else
					{
						processor(text, text2, MadLevelProfile.GetLevelAny(text, text2));
					}
				}
			}
			return true;
		}

		private List<string> GetLevelNames()
		{
			MadLevelConfiguration activeConfiguration = MadLevel.activeConfiguration;
			switch (selector)
			{
			case Selector.All:
				if (!hasLevelType)
				{
					return (from l in activeConfiguration.levels
						select l.name).ToList();
				}
				return (from l in activeConfiguration.levels
					where l.type == levelType
					select l.name).ToList();
			case Selector.Groups:
				if (!hasLevelType)
				{
					return (from l in activeConfiguration.levels
						where Array.IndexOf(selectorGroupName, l.GetGroup().name) != -1
						select l.name).ToList();
				}
				return (from l in activeConfiguration.levels
					where l.type == levelType && Array.IndexOf(selectorGroupName, l.GetGroup().name) != -1
					select l.name).ToList();
			case Selector.Levels:
				if (!hasLevelType)
				{
					return (from l in activeConfiguration.levels
						where Array.IndexOf(selectorLevelName, l.name) != -1
						select l.name).ToList();
				}
				return (from l in activeConfiguration.levels
					where l.type == levelType && Array.IndexOf(selectorLevelName, l.name) != -1
					select l.name).ToList();
			default:
				UnityEngine.Debug.LogError("Unknown selector: " + selector);
				return new List<string>();
			}
		}
	}
}
