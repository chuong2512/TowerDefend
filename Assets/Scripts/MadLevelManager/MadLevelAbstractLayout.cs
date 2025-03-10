using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace MadLevelManager
{
	public abstract class MadLevelAbstractLayout : MadNode
	{
		public delegate void IconActivationEvent(MadLevelIcon icon, string levelName);

		public enum TwoStepActivationType
		{
			Disabled,
			OnlyOnMobiles,
			Always
		}

		public enum LoadLevel
		{
			Immediately,
			WithDelay,
			SendMessage,
			DoNotLoad
		}

		public enum OnMobileBack
		{
			LoadPreviousLevel,
			LoadSpecifiedLevel
		}

		public enum LookLevelType
		{
			FirstLevel,
			LastUnlocked,
			LastCompleted
		}

		public enum LevelsEnumerationType
		{
			Numbers,
			Letters,
			LettersLower,
			Roman
		}

		public enum IconTemplateQuantity
		{
			Single,
			OnePerLevel
		}

		public MadLevelIcon iconTemplate;

		public List<MadLevelIcon> iconTemplates = new List<MadLevelIcon>();

		public IconTemplateQuantity iconTemplateQuantity;

		public bool lookAtLastLevel = true;

		public LookLevelType lookAtLevel;

		public LevelsEnumerationType enumerationType;

		public int enumerationOffset;

		public TwoStepActivationType twoStepActivationType;

		private MadLevelIcon activeIcon;

		public LoadLevel loadLevel;

		public float loadLevelLoadLevelDelay = 1.5f;

		public GameObject loadLevelMessageReceiver;

		public string loadLevelMessageName;

		public bool loadLevelMessageIncludeChildren;

		public bool onIconActivatePlayAudio;

		public AudioClip onIconActivatePlayAudioClip;

		public float onIconActivatePlayAudioVolume = 1f;

		public bool onIconDeactivatePlayAudio;

		public AudioClip onIconDeactivatePlayAudioClip;

		public float onIconDeactivatePlayAudioVolume = 1f;

		private AudioListener cachedAudioListener;

		public bool onIconActivateMessage;

		public GameObject onIconActivateMessageReceiver;

		public string onIconActivateMessageMethodName = "OnIconActivate";

		public bool onIconActivateMessageIncludeChildren;

		public bool onIconDeactivateMessage;

		public GameObject onIconDeactivateMessageReceiver;

		public string onIconDeactivateMessageMethodName = "OnIconDeactivate";

		public bool onIconDeactivateMessageIncludeChildren;

		public bool handleMobileBackButton = true;

		public OnMobileBack handleMobileBackButtonAction;

		public string handleMobileBackButtonLevelName;

		[HideInInspector]
		public MadLevelConfiguration configuration;

		public int configurationGroup;

		public bool useCurrentlyActiveConfiguration;

		[NonSerialized]
		[HideInInspector]
		public bool fullyInitialized;

		public MadLevelConfiguration currentConfiguration
		{
			get
			{
				if (useCurrentlyActiveConfiguration)
				{
					return MadLevel.activeConfiguration;
				}
				return configuration;
			}
		}

		public event IconActivationEvent onIconActivate;

		public event IconActivationEvent onIconDeactivate;

		public bool CurrentConfigurationValid()
		{
			if (!useCurrentlyActiveConfiguration)
			{
				UnityEngine.Debug.LogWarning("Do not use this method when useCurrentyActiveConfiguration is set to false");
				return true;
			}
			if (currentConfiguration == configuration)
			{
				return true;
			}
			if (string.IsNullOrEmpty(MadLevel.currentLevelName))
			{
				return false;
			}
			if (currentConfiguration.FindGroupById(configurationGroup) == null)
			{
				return false;
			}
			return true;
		}

		protected virtual void Update()
		{
			UpdateMultipleIcons();
			UpdateHandleMobileBackButton();
		}

		private void UpdateHandleMobileBackButton()
		{
			if (SystemInfo.deviceType == DeviceType.Handheld && handleMobileBackButton && UnityEngine.Input.GetKey(KeyCode.Escape))
			{
				switch (handleMobileBackButtonAction)
				{
				case OnMobileBack.LoadPreviousLevel:
					MadLevel.LoadPrevious();
					break;
				case OnMobileBack.LoadSpecifiedLevel:
					MadLevel.LoadLevelByName(handleMobileBackButtonLevelName);
					break;
				default:
					UnityEngine.Debug.LogError("Unknown action: " + handleMobileBackButtonAction);
					break;
				}
			}
		}

		public abstract MadLevelIcon GetIcon(string levelName);

		public MadLevelIcon GetFirstIcon()
		{
			string name = MadLevel.activeConfiguration.FindGroupById(configurationGroup).name;
			string levelName = MadLevel.FindFirstLevelName(MadLevel.Type.Level, name);
			return GetIcon(levelName);
		}

		public MadLevelIcon GetLastIcon()
		{
			string name = MadLevel.activeConfiguration.FindGroupById(configurationGroup).name;
			string levelName = MadLevel.FindLastLevelName(MadLevel.Type.Level, name);
			return GetIcon(levelName);
		}

		public MadLevelIcon GetLastCompletedIcon()
		{
			IOrderedEnumerable<MadLevelConfiguration.Level> source = from l in MadLevel.activeConfiguration.levels
				where l.groupId == configurationGroup && l.type == MadLevel.Type.Level && MadLevelProfile.IsCompleted(l.name)
				orderby l.order descending
				select l;
			MadLevelConfiguration.Level level = source.FirstOrDefault();
			if (level != null)
			{
				return MadLevelLayout.current.GetIcon(level.name);
			}
			return null;
		}

		public MadLevelIcon GetLastUnlockedIcon()
		{
			IOrderedEnumerable<MadLevelConfiguration.Level> source = from l in MadLevel.activeConfiguration.levels
				where l.groupId == configurationGroup && l.type == MadLevel.Type.Level && MadLevelProfile.IsLockedSet(l.name) && !MadLevelProfile.IsLocked(l.name)
				orderby l.order descending
				select l;
			MadLevelConfiguration.Level level = source.FirstOrDefault();
			if (level != null)
			{
				return MadLevelLayout.current.GetIcon(level.name);
			}
			return null;
		}

		public MadLevelIcon GetActiveIcon()
		{
			return activeIcon;
		}

		public virtual MadLevelIcon GetCurrentIcon()
		{
			return activeIcon;
		}

		public abstract MadLevelIcon FindClosestIcon(Vector3 position);

		public MadLevelIcon GetNextIcon(MadLevelIcon icon)
		{
			MadLevelConfiguration.Level level = currentConfiguration.FindNextLevel(icon.level.name);
			if (level == null)
			{
				return null;
			}
			return GetIcon(level.name);
		}

		public MadLevelIcon GetPreviousIcon(MadLevelIcon icon)
		{
			MadLevelConfiguration.Level level = currentConfiguration.FindPreviousLevel(icon.level.name);
			if (level == null)
			{
				return null;
			}
			return GetIcon(level.name);
		}

		public MadLevelIcon[] GetAllIcons()
		{
			List<MadLevelIcon> list = MadTransform.FindChildren(base.transform, (MadLevelIcon ic) => MadGameObject.IsActive(ic.gameObject), 3);
			return list.ToArray();
		}

		public abstract void LookAtIcon(MadLevelIcon icon, bool animate = false);

		public bool LookAtLevel(string levelName)
		{
			MadLevelConfiguration.Level level = MadLevel.activeConfiguration.FindLevelByName(levelName);
			if (level.type == MadLevel.Type.Other)
			{
				UnityEngine.Debug.LogWarning("Level " + levelName + " is of wrong type. Won't look at it.");
				return false;
			}
			if (level.type == MadLevel.Type.Extra)
			{
				level = currentConfiguration.FindPreviousLevel(MadLevel.lastPlayedLevelName, MadLevel.Type.Level);
				if (level == null)
				{
					UnityEngine.Debug.Log("Cannot find previous level icon.");
					return false;
				}
			}
			MadLevelIcon icon = GetIcon(levelName);
			if (icon != null)
			{
				LookAtIcon(icon);
				return true;
			}
			UnityEngine.Debug.Log("Cannot find icon for level: " + levelName);
			return false;
		}

		public bool LookAtLastPlayedLevel()
		{
			if (!Application.isPlaying)
			{
				return false;
			}
			string lastPlayedLevelName = MadLevel.lastPlayedLevelName;
			if (!string.IsNullOrEmpty(lastPlayedLevelName))
			{
				return LookAtLevel(lastPlayedLevelName);
			}
			return false;
		}

		protected virtual void OnEnable()
		{
			UpdateMultipleIcons();
			if (currentConfiguration == null)
			{
				if (MadLevel.hasActiveConfiguration)
				{
					configuration = MadLevel.activeConfiguration;
				}
				else
				{
					UnityEngine.Debug.LogWarning("There's no active level configuration. Please prepare one and activate it.");
				}
			}
			else if (!useCurrentlyActiveConfiguration && currentConfiguration != MadLevel.activeConfiguration && Application.isPlaying)
			{
				UnityEngine.Debug.LogWarning("This layout was prepared for different level configuration than the active one. http://goo.gl/AxZqW2", this);
			}
			if (Application.isPlaying && useCurrentlyActiveConfiguration && !CurrentConfigurationValid())
			{
				UnityEngine.Debug.LogError("Your currently active configuration is not compatible with the configuration that this level was prepared for. Please see the layout inspector.");
			}
			MadPanel madPanel = MadTransform.FindParent<MadPanel>(base.transform);
			madPanel.onFocusChanged += delegate(MadSprite sprite)
			{
				if (activeIcon != null && sprite != activeIcon)
				{
					DeactivateActiveIcon();
				}
			};
			onIconActivate += delegate(MadLevelIcon icon, string levelName)
			{
				if (onIconActivateMessage && onIconActivateMessageReceiver != null)
				{
					if (onIconActivateMessageIncludeChildren)
					{
						onIconActivateMessageReceiver.BroadcastMessage(onIconActivateMessageMethodName, icon);
					}
					else
					{
						onIconActivateMessageReceiver.SendMessage(onIconActivateMessageMethodName, icon);
					}
				}
				if (onIconActivatePlayAudio && onIconActivatePlayAudioClip != null && cachedAudioListener != null)
				{
					AudioSource.PlayClipAtPoint(onIconActivatePlayAudioClip, cachedAudioListener.transform.position, onIconActivatePlayAudioVolume);
				}
			};
			onIconDeactivate += delegate(MadLevelIcon icon, string levelName)
			{
				if (onIconDeactivateMessage && onIconDeactivateMessageReceiver != null)
				{
					if (onIconDeactivateMessageIncludeChildren)
					{
						onIconDeactivateMessageReceiver.BroadcastMessage(onIconDeactivateMessageMethodName, icon);
					}
					else
					{
						onIconDeactivateMessageReceiver.SendMessage(onIconDeactivateMessageMethodName, icon);
					}
				}
				if (onIconDeactivatePlayAudio && onIconDeactivatePlayAudioClip != null && cachedAudioListener != null)
				{
					AudioSource.PlayClipAtPoint(onIconDeactivatePlayAudioClip, cachedAudioListener.transform.position, onIconDeactivatePlayAudioVolume);
				}
			};
		}

		protected virtual void Start()
		{
			if (onIconActivatePlayAudio)
			{
				cachedAudioListener = (UnityEngine.Object.FindObjectOfType(typeof(AudioListener)) as AudioListener);
				if (cachedAudioListener == null)
				{
					UnityEngine.Debug.LogError("Cannot find an audio listener for this scene. Audio will not be played");
				}
			}
			if (Application.isPlaying)
			{
				bool flag = false;
				if (lookAtLastLevel)
				{
					flag = LookAtLastPlayedLevel();
				}
				if (!flag)
				{
					LookAtLevel();
				}
			}
		}

		private void LateUpdate()
		{
			fullyInitialized = true;
		}

		private void LookAtLevel()
		{
			switch (lookAtLevel)
			{
			case LookLevelType.FirstLevel:
				LookAtFirstLevel();
				break;
			case LookLevelType.LastUnlocked:
				LookAtLastUnlockedLevel();
				break;
			case LookLevelType.LastCompleted:
				LookAtLastCompletedLevel();
				break;
			default:
				UnityEngine.Debug.LogError("Unknown level type: " + lookAtLevel);
				break;
			}
		}

		private void LookAtLastCompletedLevel()
		{
			MadLevelIcon lastCompletedIcon = GetLastCompletedIcon();
			if (lastCompletedIcon != null)
			{
				LookAtIcon(lastCompletedIcon);
			}
			else
			{
				LookAtFirstLevel();
			}
		}

		private void LookAtFirstLevel()
		{
			MadLevelIcon firstIcon = MadLevelLayout.current.GetFirstIcon();
			LookAtIcon(firstIcon);
		}

		private void LookAtLastUnlockedLevel()
		{
			MadLevelIcon lastUnlockedIcon = GetLastUnlockedIcon();
			if (lastUnlockedIcon != null)
			{
				LookAtIcon(lastUnlockedIcon);
			}
			else
			{
				LookAtFirstLevel();
			}
		}

		public bool CanActivate(MadLevelIcon icon)
		{
			if (icon.locked && !icon.canFocusIfLocked)
			{
				return false;
			}
			return true;
		}

		public virtual void Activate(MadLevelIcon icon)
		{
			if (activeIcon != null && icon != activeIcon)
			{
				DeactivateActiveIcon();
			}
			switch (twoStepActivationType)
			{
			case TwoStepActivationType.Disabled:
				if (!icon.locked)
				{
					StartCoroutine(Activate2nd(icon));
				}
				break;
			case TwoStepActivationType.OnlyOnMobiles:
				if (SystemInfo.deviceType == DeviceType.Handheld)
				{
					Activate1st(icon);
				}
				else if (!icon.locked)
				{
					StartCoroutine(Activate2nd(icon));
				}
				break;
			case TwoStepActivationType.Always:
				Activate1st(icon);
				break;
			default:
				UnityEngine.Debug.LogError("Uknown option: " + twoStepActivationType);
				break;
			}
		}

		private void Activate1st(MadLevelIcon icon)
		{
			if (!CanActivate(icon))
			{
				UnityEngine.Debug.Log("Icon " + icon + " cannot be activated because of layout settings.", icon);
			}
			else if (!icon.hasFocus)
			{
				icon.hasFocus = true;
				if (this.onIconActivate != null)
				{
					this.onIconActivate(icon, icon.level.name);
				}
				activeIcon = icon;
			}
			else if (!icon.locked)
			{
				StartCoroutine(Activate2nd(icon));
			}
		}

		protected void DeactivateActiveIcon()
		{
			MadLevelIcon madLevelIcon = activeIcon;
			activeIcon = null;
			if (this.onIconDeactivate != null)
			{
				this.onIconDeactivate(madLevelIcon, madLevelIcon.level.name);
			}
		}

		private IEnumerator Activate2nd(MadLevelIcon icon)
		{
			switch (loadLevel)
			{
			case LoadLevel.DoNotLoad:
				break;
			case LoadLevel.Immediately:
				icon.LoadLevel();
				break;
			case LoadLevel.WithDelay:
				yield return new WaitForSeconds(loadLevelLoadLevelDelay);
				icon.LoadLevel();
				break;
			case LoadLevel.SendMessage:
				if (loadLevelMessageReceiver == null)
				{
					UnityEngine.Debug.LogError("No send message receiver", this);
				}
				else if (string.IsNullOrEmpty(loadLevelMessageName))
				{
					UnityEngine.Debug.LogError("No sent message name", this);
				}
				else if (loadLevelMessageIncludeChildren)
				{
					loadLevelMessageReceiver.BroadcastMessage(loadLevelMessageName, icon);
				}
				else
				{
					loadLevelMessageReceiver.SendMessage(loadLevelMessageName, icon);
				}
				break;
			default:
				UnityEngine.Debug.LogError("Unknown LoadLevel option: " + loadLevel);
				break;
			}
		}

		protected MadLevelIcon CreateIcon(Transform parent, string name, MadLevelIcon template)
		{
			GameObject gameObject = null;
			if (gameObject != null)
			{
				gameObject.name = name;
				gameObject.transform.parent = parent;
			}
			else
			{
				gameObject = MadTransform.CreateChild(parent, name, template.gameObject);
			}
			return gameObject.GetComponent<MadLevelIcon>();
		}

		protected string GetEnumerationValue(int index)
		{
			switch (enumerationType)
			{
			case LevelsEnumerationType.Numbers:
				return (index + 1 + enumerationOffset).ToString();
			case LevelsEnumerationType.Letters:
				return EnumerationLetter(index + Mathf.Max(enumerationOffset, 0));
			case LevelsEnumerationType.LettersLower:
				return EnumerationLetter(index + Mathf.Max(enumerationOffset, 0)).ToLower();
			case LevelsEnumerationType.Roman:
				return MadMath.ToRoman(index + 1 + Mathf.Max(enumerationOffset, 0));
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private string EnumerationLetter(int number)
		{
			int num = 26;
			string text = string.Empty;
			do
			{
				int num2 = number % num;
				if (!string.IsNullOrEmpty(text))
				{
					num2--;
				}
				text = (char)(65 + num2) + text;
				number /= num;
			}
			while (number > 0);
			return text;
		}

		protected void UpdateMultipleIcons()
		{
			if (Application.isPlaying || iconTemplateQuantity == IconTemplateQuantity.Single || currentConfiguration == null)
			{
				return;
			}
			int num = currentConfiguration.LevelCount(MadLevel.Type.Level, configurationGroup);
			bool flag = false;
			while (iconTemplates.Count < num)
			{
				MadLevelIcon item = null;
				if (iconTemplates.Count > 0)
				{
					item = iconTemplates[iconTemplates.Count - 1];
				}
				iconTemplates.Add(item);
				flag = true;
			}
			while (iconTemplates.Count > num)
			{
				iconTemplates.RemoveAt(iconTemplates.Count - 1);
				flag = true;
			}
		}
	}
}
