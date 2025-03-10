using System;
using System.Collections.Generic;
using UnityEngine;

namespace MadLevelManager
{
	[ExecuteInEditMode]
	public class MadLevelIcon : MadSprite
	{
		public bool hasLevelConfiguration;

		public int levelIndex;

		public string levelSceneName;

		public string levelArguments;

		public MadLevelProperty completedProperty;

		public MadLevelProperty lockedProperty;

		public MadText levelNumber;

		public bool canFocusIfLocked = true;

		public List<MadLevelIcon> unlockOnComplete = new List<MadLevelIcon>();

		private List<MadLevelIcon> unlockOnCompleteBackRef = new List<MadLevelIcon>();

		public List<GameObject> showWhenLevelLocked = new List<GameObject>();

		public List<GameObject> showWhenLevelUnlocked = new List<GameObject>();

		public List<GameObject> showWhenLevelNotCompleted = new List<GameObject>();

		public List<GameObject> showWhenLevelCompleted = new List<GameObject>();

		[HideInInspector]
		public int version;

		private bool justEnabled;

		private bool _isTemplate;

		private bool _isTemplateCached;

		private MadLevelConfiguration _configuration;

		private int _levelGroup = -1;

		public bool isTemplate
		{
			get
			{
				if (!_isTemplateCached)
				{
					_isTemplate = (MadTransform.FindParent<MadLevelAbstractLayout>(base.transform) == null);
					_isTemplateCached = true;
				}
				return _isTemplate;
			}
		}

		public MadLevelConfiguration configuration
		{
			get
			{
				if (_configuration == null)
				{
					MadLevelAbstractLayout madLevelAbstractLayout = MadTransform.FindParent<MadLevelAbstractLayout>(base.transform);
					if (madLevelAbstractLayout != null)
					{
						_configuration = madLevelAbstractLayout.currentConfiguration;
					}
				}
				return _configuration;
			}
			set
			{
				_configuration = value;
			}
		}

		public int levelGroup
		{
			get
			{
				if (_levelGroup == -1)
				{
					MadLevelAbstractLayout madLevelAbstractLayout = MadTransform.FindParent<MadLevelAbstractLayout>(base.transform);
					_levelGroup = madLevelAbstractLayout.configurationGroup;
				}
				return _levelGroup;
			}
			set
			{
				_levelGroup = value;
			}
		}

		public bool generated => hasLevelConfiguration;

		public bool completed
		{
			get
			{
				return MadLevelProfile.IsCompleted(level.name);
			}
			set
			{
				if (completedProperty != null)
				{
					completedProperty.propertyEnabled = value;
				}
				if (value)
				{
					UnlockOnComplete();
				}
				MadLevelProfile.SetCompleted(level.name, value);
				ChangeState(showWhenLevelCompleted, value);
				ChangeState(showWhenLevelNotCompleted, !value);
			}
		}

		public bool locked
		{
			get
			{
				if (lockedProperty != null)
				{
					return lockedProperty.propertyEnabled;
				}
				if (!justEnabled)
				{
					return MadLevelProfile.IsLocked(level.name);
				}
				if (!MadLevelProfile.IsLocked(level.name))
				{
					return false;
				}
				for (int i = 0; i < unlockOnCompleteBackRef.Count; i++)
				{
					MadLevelIcon madLevelIcon = unlockOnCompleteBackRef[i];
					if (madLevelIcon.completed)
					{
						return false;
					}
				}
				return true;
			}
			set
			{
				if (MadGameObject.IsActive(base.gameObject))
				{
					if (lockedProperty != null)
					{
						lockedProperty.propertyEnabled = value;
					}
					if (Application.isPlaying)
					{
						MadLevelProfile.SetLocked(level.name, value);
					}
					ChangeState(showWhenLevelLocked, value);
					ChangeState(showWhenLevelUnlocked, !value);
					if (!value && !isTemplate && !canFocusIfLocked)
					{
						MadSprite component = GetComponent<MadSprite>();
						component.eventFlags |= EventFlags.Focus;
					}
				}
			}
		}

		public List<MadLevelProperty> properties => MadTransform.FindChildren<MadLevelProperty>(base.transform);

		public MadLevelConfiguration.Level level
		{
			get
			{
				if (configuration != null)
				{
					return configuration.GetLevel(MadLevel.Type.Level, levelGroup, levelIndex);
				}
				return null;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			Upgrade();
			justEnabled = true;
			for (int i = 0; i < unlockOnComplete.Count; i++)
			{
				MadLevelIcon madLevelIcon = unlockOnComplete[i];
				if (madLevelIcon != null && !madLevelIcon.unlockOnCompleteBackRef.Contains(this))
				{
					madLevelIcon.unlockOnCompleteBackRef.Add(this);
				}
			}
		}

		private void Upgrade()
		{
			if (version == 0 && MadLevelProfile.IsLevelSet(base.name) && !MadLevelProfile.IsLevelSet(level.name))
			{
				MadLevelProfile.RenameLevel(base.name, level.name);
			}
			version = 1;
		}

		protected override void Update()
		{
			base.Update();
			if (!justEnabled)
			{
				return;
			}
			if (Application.isPlaying)
			{
				if (isTemplate)
				{
					MadGameObject.SetActive(base.gameObject, active: false);
				}
				if (completedProperty == null && level != null)
				{
					completed = MadLevelProfile.IsCompleted(level.name);
				}
				base.onMouseUp = (Action)Delegate.Combine(base.onMouseUp, (Action)delegate
				{
					Activate();
				});
				base.onTap = (Action)Delegate.Combine(base.onTap, (Action)delegate
				{
					Activate();
				});
				if (!isTemplate && !canFocusIfLocked && locked)
				{
					MadSprite component = GetComponent<MadSprite>();
					component.eventFlags &= (EventFlags)(-9);
				}
			}
			if (level != null)
			{
				ChangeState(showWhenLevelLocked, locked);
				ChangeState(showWhenLevelUnlocked, !locked);
				ChangeState(showWhenLevelCompleted, completed);
				ChangeState(showWhenLevelNotCompleted, !completed);
				if (!locked && Application.isPlaying)
				{
					MadLevelProfile.SetLocked(level.name, locked: false);
				}
			}
			justEnabled = false;
		}

		public void Activate()
		{
			MadLevelAbstractLayout madLevelAbstractLayout = MadTransform.FindParent<MadLevelAbstractLayout>(base.transform);
			madLevelAbstractLayout.Activate(this);
		}

		public void ApplyConnections()
		{
			foreach (MadLevelProperty property in properties)
			{
				property.ApplyConnections();
			}
		}

		public MadLevelProperty.SpecialType TypeFor(MadLevelProperty property)
		{
			if (property == completedProperty)
			{
				return MadLevelProperty.SpecialType.Completed;
			}
			if (property == lockedProperty)
			{
				return MadLevelProperty.SpecialType.Locked;
			}
			if (property.gameObject == levelNumber.gameObject)
			{
				return MadLevelProperty.SpecialType.LevelNumber;
			}
			return MadLevelProperty.SpecialType.Regular;
		}

		public void UpdateProperty(string propertyName, bool state)
		{
			MadLevelProperty[] componentsInChildren = GetComponentsInChildren<MadLevelProperty>();
			bool flag = false;
			MadLevelProperty[] array = componentsInChildren;
			foreach (MadLevelProperty madLevelProperty in array)
			{
				if (madLevelProperty.name == propertyName)
				{
					madLevelProperty.propertyEnabled = state;
					flag = true;
				}
			}
			if (!flag)
			{
				UnityEngine.Debug.LogError("Cannot find property '" + propertyName + "'", base.gameObject);
			}
		}

		public void LoadLevel()
		{
			if (hasLevelConfiguration)
			{
				MadLevelConfiguration.Level level = configuration.GetLevel(MadLevel.Type.Level, levelGroup, levelIndex);
				MadLevel.LoadLevelByName(level.name);
			}
			else if (!string.IsNullOrEmpty(levelSceneName))
			{
				MadLevelProfile.recentLevelSelected = this.level.name;
				MadLevel.currentLevelName = this.level.name;
				MadLevel.arguments = string.Empty;
				UnityEngine.SceneManagement.SceneManager.LoadScene(levelSceneName);
			}
			else
			{
				UnityEngine.Debug.LogError("Level scene name not set. I don't know what to load!");
			}
		}

		private void OnPropertyChange(MadLevelProperty property)
		{
			if (property.specialType == MadLevelProperty.SpecialType.Completed)
			{
				UnlockOnComplete();
			}
		}

		private void UnlockOnComplete()
		{
			if (unlockOnComplete != null)
			{
				foreach (MadLevelIcon item in unlockOnComplete)
				{
					item.locked = false;
				}
			}
		}

		private void Enable(List<GameObject> objects)
		{
			ChangeState(objects, state: true);
		}

		private void Disable(List<GameObject> objects)
		{
			ChangeState(objects, state: true);
		}

		private void ChangeState(List<GameObject> objects, bool state)
		{
			for (int i = 0; i < objects.Count; i++)
			{
				GameObject gameObject = objects[i];
				if (!(gameObject == null))
				{
					gameObject.active = state;
					MadSprite component = gameObject.GetComponent<MadSprite>();
					if (component != null)
					{
						component.visible = state;
					}
				}
			}
		}
	}
}
