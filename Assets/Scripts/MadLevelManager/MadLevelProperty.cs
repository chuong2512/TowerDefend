using System;
using System.Collections.Generic;
using UnityEngine;

namespace MadLevelManager
{
	[ExecuteInEditMode]
	public class MadLevelProperty : MadNode
	{
		public enum Type
		{
			Bool,
			Integer,
			Float,
			String
		}

		public enum SpecialType
		{
			Regular,
			Locked,
			Completed,
			LevelNumber
		}

		[HideInInspector]
		public bool _propertyEnabled = true;

		public MadSprite[] showWhenEnabled = new MadSprite[0];

		public MadSprite[] showWhenDisabled = new MadSprite[0];

		public bool textFromProperty;

		public string textPropertyName;

		private bool justAwaken;

		private bool onFirstUpdate = true;

		private MadSprite _sprite;

		private MadLevelIcon _icon;

		public SpecialType specialType => icon.TypeFor(this);

		private MadSprite sprite
		{
			get
			{
				if (_sprite == null)
				{
					_sprite = GetComponent<MadSprite>();
				}
				return _sprite;
			}
		}

		public MadLevelIcon icon
		{
			get
			{
				if (_icon == null)
				{
					_icon = MadTransform.FindParent<MadLevelIcon>(base.transform);
				}
				return _icon;
			}
		}

		public bool propertyEnabled
		{
			get
			{
				return _propertyEnabled;
			}
			set
			{
				UpdateEnabled(value);
			}
		}

		public bool linked => linkage != null;

		public MadLevelProperty linkage
		{
			get
			{
				List<MadLevelProperty> properties = icon.properties;
				foreach (MadLevelProperty item in properties)
				{
					int num = Array.FindIndex(item.showWhenEnabled, (MadSprite sprite) => sprite != null && sprite.gameObject == base.gameObject);
					if (num != -1)
					{
						return item;
					}
					num = Array.FindIndex(item.showWhenDisabled, (MadSprite sprite) => sprite != null && sprite.gameObject == base.gameObject);
					if (num != -1)
					{
						return item;
					}
				}
				return null;
			}
		}

		private bool propertySet
		{
			get
			{
				if (Application.isPlaying)
				{
					return IsLevelBooleanSet();
				}
				return false;
			}
		}

		private bool persistent => specialType != SpecialType.LevelNumber;

		private void Awake()
		{
			justAwaken = true;
		}

		private void OnEnable()
		{
			onFirstUpdate = true;
		}

		private void Start()
		{
		}

		private void Update()
		{
			if (!onFirstUpdate)
			{
				return;
			}
			if (Application.isPlaying && icon.level != null && persistent)
			{
				if (propertySet)
				{
					propertyEnabled = GetLevelBoolean();
				}
				else if (justAwaken)
				{
					SetLevelBoolean(propertyEnabled);
				}
				else
				{
					propertyEnabled = false;
				}
			}
			if (textFromProperty && sprite is MadText)
			{
				MadText madText = sprite as MadText;
				MadLevelConfiguration.Level level = icon.level;
				string text = madText.text = MadLevelProfile.GetLevelAny(level.name, textPropertyName);
			}
			onFirstUpdate = false;
			justAwaken = false;
		}

		private void UpdateEnabled(bool enabled)
		{
			if (_propertyEnabled == enabled)
			{
				return;
			}
			ApplyConnections(enabled);
			if (sprite != null)
			{
				if (!Application.isPlaying)
				{
					sprite.visible = enabled;
				}
				else
				{
					sprite.visible = enabled;
				}
			}
			_propertyEnabled = enabled;
			if (Application.isPlaying && persistent)
			{
				SetLevelBoolean(enabled);
				SendMessageUpwards("OnPropertyChange", this);
			}
		}

		public void ApplyConnections()
		{
			ApplyConnections(propertyEnabled);
		}

		private void ApplyConnections(bool enabled)
		{
			MadSprite[] array;
			MadSprite[] array2;
			if (enabled)
			{
				array = showWhenEnabled;
				array2 = showWhenDisabled;
			}
			else
			{
				array = showWhenDisabled;
				array2 = showWhenEnabled;
			}
			if (array2 != null)
			{
				MadSprite[] array3 = array2;
				foreach (MadSprite madSprite in array3)
				{
					if (!(madSprite == null))
					{
						MadLevelProperty component = madSprite.GetComponent<MadLevelProperty>();
						if (component != null)
						{
							component.propertyEnabled = false;
						}
						else
						{
							madSprite.visible = false;
						}
					}
				}
			}
			if (array == null)
			{
				return;
			}
			MadSprite[] array4 = array;
			foreach (MadSprite madSprite2 in array4)
			{
				if (!(madSprite2 == null))
				{
					MadLevelProperty component2 = madSprite2.GetComponent<MadLevelProperty>();
					if (component2 != null)
					{
						component2.propertyEnabled = true;
					}
					else
					{
						madSprite2.visible = true;
					}
				}
			}
		}

		private bool GetLevelBoolean()
		{
			string name = icon.level.name;
			switch (specialType)
			{
			case SpecialType.Regular:
				return MadLevelProfile.GetLevelBoolean(name, base.name);
			case SpecialType.LevelNumber:
				MadDebug.Assert(condition: false, "Level numbers are not persistent!");
				return false;
			case SpecialType.Locked:
				return MadLevelProfile.IsLocked(name);
			case SpecialType.Completed:
				return MadLevelProfile.IsCompleted(name);
			default:
				MadDebug.Assert(condition: false, "Unknown special type: " + specialType);
				return false;
			}
		}

		private void SetLevelBoolean(bool val)
		{
			string name = icon.level.name;
			switch (specialType)
			{
			case SpecialType.Regular:
				MadLevelProfile.SetLevelBoolean(name, base.name, val);
				break;
			case SpecialType.LevelNumber:
				MadDebug.Assert(condition: false, "Level numbers are not persistent!");
				break;
			case SpecialType.Locked:
				MadLevelProfile.SetLocked(name, val);
				break;
			case SpecialType.Completed:
				MadLevelProfile.SetCompleted(name, val);
				break;
			default:
				MadDebug.Assert(condition: false, "Unknown special type: " + specialType);
				break;
			}
		}

		private bool IsLevelBooleanSet()
		{
			string name = icon.level.name;
			switch (specialType)
			{
			case SpecialType.Regular:
				return MadLevelProfile.IsLevelPropertySet(name, base.name);
			case SpecialType.LevelNumber:
				MadDebug.Assert(condition: false, "Level numbers are not persistent!");
				return false;
			case SpecialType.Locked:
				return MadLevelProfile.IsLockedSet(name);
			case SpecialType.Completed:
				return MadLevelProfile.IsCompletedSet(name);
			default:
				MadDebug.Assert(condition: false, "Unknown special type: " + specialType);
				return false;
			}
		}
	}
}
