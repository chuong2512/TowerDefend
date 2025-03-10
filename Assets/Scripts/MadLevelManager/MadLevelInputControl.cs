using System;
using System.Collections.Generic;
using UnityEngine;

namespace MadLevelManager
{
	public class MadLevelInputControl : MonoBehaviour
	{
        /*
		public abstract class TraverseRule
		{
			[Flags]
			public enum Capability
			{
				CanGoLeft = 0x1,
				CanGoRight = 0x2,
				CanGoUp = 0x4,
				CanGoDown = 0x8,
				CanGoAnywhere = 0xF
			}

			public readonly int capFlags;

			public bool canGoLeft => (capFlags & 1) != 0;

			public bool canGoRight => (capFlags & 2) != 0;

			public bool canGoUp => (capFlags & 4) != 0;

			public bool canGoDown => (capFlags & 8) != 0;

			public TraverseRule(params Capability[] capabilities)
			{
				for (int i = 0; i < capabilities.Length; i++)
				{
					capFlags |= (int)capabilities[i];
				}
			}

			public abstract MadLevelIcon LeftIcon(MadLevelIcon current);

			public abstract MadLevelIcon TopIcon(MadLevelIcon current);

			public abstract MadLevelIcon RightIcon(MadLevelIcon current);

			public abstract MadLevelIcon BottomIcon(MadLevelIcon current);
		}

		public class SimpleTraverseRule : TraverseRule
		{
			public SimpleTraverseRule()
				: base(Capability.CanGoLeft, Capability.CanGoRight)
			{
			}

			public MadLevelIcon LeftIcon(MadLevelIcon current)
			{
				MadLevelAbstractLayout current2 = MadLevelLayout.current;
				return current2.GetPreviousIcon(current);
			}

			public MadLevelIcon RightIcon(MadLevelIcon current)
			{
				MadLevelAbstractLayout current2 = MadLevelLayout.current;
				return current2.GetNextIcon(current);
			}

			public MadLevelIcon TopIcon(MadLevelIcon current)
			{
				throw new NotImplementedException();
			}

			public MadLevelIcon BottomIcon(MadLevelIcon current)
			{
				throw new NotImplementedException();
			}
		}

		public class DirectionTraverseRule : TraverseRule
		{
			public DirectionTraverseRule()
				: base(Capability.CanGoAnywhere)
			{
			}

			public MadLevelIcon LeftIcon(MadLevelIcon current)
			{
				return FindBest(current, Vector3.left, 45f);
			}

			public MadLevelIcon RightIcon(MadLevelIcon current)
			{
				return FindBest(current, Vector3.right, 45f);
			}

			public MadLevelIcon TopIcon(MadLevelIcon current)
			{
				return FindBest(current, Vector3.up, 45f);
			}

			public MadLevelIcon BottomIcon(MadLevelIcon current)
			{
				return FindBest(current, Vector3.down, 45f);
			}

			public MadLevelIcon FindBest(MadLevelIcon origin, Vector2 direction, float toleranceAngle)
			{
				List<MadLevelIcon> list = FindAll(origin, direction, toleranceAngle);
				if (list.Count == 0)
				{
					return null;
				}
				if (list.Count == 1)
				{
					return list[0];
				}
				float num = float.MaxValue;
				MadLevelIcon result = null;
				MadLevelAbstractLayout current = MadLevelLayout.current;
				for (int i = 0; i < list.Count; i++)
				{
					MadLevelIcon madLevelIcon = list[i];
					if (current.CanActivate(madLevelIcon))
					{
						Vector3 v = madLevelIcon.transform.position - origin.transform.position;
						float num2 = Vector2.Angle(direction, v);
						float magnitude = v.magnitude;
						float num3 = magnitude + magnitude * (num2 / toleranceAngle) * 2f;
						if (num3 < num)
						{
							result = madLevelIcon;
							num = num3;
						}
					}
				}
				return result;
			}

			public List<MadLevelIcon> FindAll(MadLevelIcon origin, Vector2 direction, float toleranceAngle)
			{
				List<MadLevelIcon> list = new List<MadLevelIcon>();
				MadLevelAbstractLayout current = MadLevelLayout.current;
				MadLevelIcon[] allIcons = current.GetAllIcons();
				foreach (MadLevelIcon madLevelIcon in allIcons)
				{
					if (!(madLevelIcon == origin))
					{
						Vector2 to = madLevelIcon.transform.position - origin.transform.position;
						if (Vector2.Angle(direction, to) <= toleranceAngle)
						{
							list.Add(madLevelIcon);
						}
					}
				}
				return list;
			}
		}

		public enum InputMode
		{
			KeyCodes,
			InputAxes
		}

		public enum ActivateOnStart
		{
			First,
			LastUnlocked,
			LastCompleted,
			PreviouslyPlayedOrFirst,
			PreviouslyPlayedOrLastUnlocked,
			PreviouslyPlayedOrLastCompleted
		}

		public InputMode inputMode;

		public KeyCode keycodeLeft = KeyCode.LeftArrow;

		public KeyCode keycodeRight = KeyCode.RightArrow;

		public KeyCode keycodeUp = KeyCode.UpArrow;

		public KeyCode keycodeDown = KeyCode.DownArrow;

		public KeyCode keycodeEnter = KeyCode.Return;

		public string axisHorizontal = "Horizontal";

		public string axisVertical = "Vertical";

		public string axisEnter = "Fire1";

		public TraverseRule traverseRule = new DirectionTraverseRule();

		public ActivateOnStart activateOnStart = ActivateOnStart.LastCompleted;

		public bool onlyOnMobiles;

		public bool repeat = true;

		public float repeatInterval = 0.5f;

		private bool isMobile;

		private float lastActionTime;

		private bool keyDown;

		private bool firstUpdateExecuted;

		private bool isLeft
		{
			get
			{
				switch (inputMode)
				{
				case InputMode.KeyCodes:
					return UnityEngine.Input.GetKey(keycodeLeft);
				case InputMode.InputAxes:
					return UnityEngine.Input.GetAxis(axisHorizontal) < 0f;
				default:
					UnityEngine.Debug.LogError("Unknown input mode: " + inputMode);
					return false;
				}
			}
		}

		private bool isRight
		{
			get
			{
				switch (inputMode)
				{
				case InputMode.KeyCodes:
					return UnityEngine.Input.GetKey(keycodeRight);
				case InputMode.InputAxes:
					return UnityEngine.Input.GetAxis(axisHorizontal) > 0f;
				default:
					UnityEngine.Debug.LogError("Unknown input mode: " + inputMode);
					return false;
				}
			}
		}

		private bool isDown
		{
			get
			{
				switch (inputMode)
				{
				case InputMode.KeyCodes:
					return UnityEngine.Input.GetKey(keycodeDown);
				case InputMode.InputAxes:
					return UnityEngine.Input.GetAxis(axisVertical) < 0f;
				default:
					UnityEngine.Debug.LogError("Unknown input mode: " + inputMode);
					return false;
				}
			}
		}

		private bool isUp
		{
			get
			{
				switch (inputMode)
				{
				case InputMode.KeyCodes:
					return UnityEngine.Input.GetKey(keycodeUp);
				case InputMode.InputAxes:
					return UnityEngine.Input.GetAxis(axisVertical) > 0f;
				default:
					UnityEngine.Debug.LogError("Unknown input mode: " + inputMode);
					return false;
				}
			}
		}

		private bool isEnter
		{
			get
			{
				switch (inputMode)
				{
				case InputMode.KeyCodes:
					return UnityEngine.Input.GetKeyDown(keycodeEnter);
				case InputMode.InputAxes:
					return UnityEngine.Input.GetAxis(axisEnter) > 0f;
				default:
					UnityEngine.Debug.LogError("Unknown input mode: " + inputMode);
					return false;
				}
			}
		}

		private void FirstUpdate()
		{
			MadLevelAbstractLayout current = MadLevelLayout.current;
			if (current.twoStepActivationType == MadLevelAbstractLayout.TwoStepActivationType.Disabled)
			{
				UnityEngine.Debug.LogError("Input controller cannot work when two step activation is disabled! Please enable it!", this);
				MadGameObject.SetActive(base.gameObject, active: false);
			}
			else if (current.twoStepActivationType == MadLevelAbstractLayout.TwoStepActivationType.OnlyOnMobiles && !onlyOnMobiles)
			{
				UnityEngine.Debug.LogError("Two step activation is set to work on mobiles, but input controler is not!", this);
				MadGameObject.SetActive(base.gameObject, active: false);
			}
			else
			{
				isMobile = (SystemInfo.deviceType == DeviceType.Handheld);
				DoActivateOnStart();
			}
		}

		private void DoActivateOnStart()
		{
			MadLevelAbstractLayout current = MadLevelLayout.current;
			MadLevelIcon madLevelIcon = null;
			switch (activateOnStart)
			{
			case ActivateOnStart.First:
				madLevelIcon = current.GetFirstIcon();
				break;
			case ActivateOnStart.LastCompleted:
				madLevelIcon = current.GetLastCompletedIcon();
				break;
			case ActivateOnStart.LastUnlocked:
				madLevelIcon = current.GetLastUnlockedIcon();
				break;
			case ActivateOnStart.PreviouslyPlayedOrFirst:
				madLevelIcon = current.GetIcon(MadLevel.lastPlayedLevelName);
				break;
			case ActivateOnStart.PreviouslyPlayedOrLastCompleted:
				madLevelIcon = current.GetIcon(MadLevel.lastPlayedLevelName);
				if (madLevelIcon == null)
				{
					madLevelIcon = current.GetLastCompletedIcon();
				}
				break;
			case ActivateOnStart.PreviouslyPlayedOrLastUnlocked:
				madLevelIcon = current.GetIcon(MadLevel.lastPlayedLevelName);
				if (madLevelIcon == null)
				{
					madLevelIcon = current.GetLastUnlockedIcon();
				}
				break;
			default:
				UnityEngine.Debug.LogError("Unknown activateOnStart: " + activateOnStart);
				break;
			}
			if (madLevelIcon == null)
			{
				madLevelIcon = current.GetFirstIcon();
			}
			if (madLevelIcon != null && !madLevelIcon.hasFocus)
			{
				Activate(madLevelIcon);
			}
		}

		private void Update()
		{
			if (!firstUpdateExecuted)
			{
				firstUpdateExecuted = true;
				FirstUpdate();
			}
			if (onlyOnMobiles && !isMobile)
			{
				return;
			}
			MadLevelIcon madLevelIcon = null;
			MadLevelIcon madLevelIcon2 = ActiveIcon();
			if ((isLeft || isRight || isUp || isDown) && madLevelIcon2 == null)
			{
				MadLevelAbstractLayout current = MadLevelLayout.current;
				MadLevelIcon firstIcon = current.GetFirstIcon();
				Activate(firstIcon);
				return;
			}
			if (isLeft && traverseRule.canGoLeft)
			{
				if (CanExecuteAction())
				{
					madLevelIcon = traverseRule.LeftIcon(madLevelIcon2);
					keyDown = true;
					lastActionTime = Time.time;
				}
			}
			else if (isRight && traverseRule.canGoRight)
			{
				if (CanExecuteAction())
				{
					madLevelIcon = traverseRule.RightIcon(madLevelIcon2);
					keyDown = true;
					lastActionTime = Time.time;
				}
			}
			else if (isUp && traverseRule.canGoUp)
			{
				if (CanExecuteAction())
				{
					madLevelIcon = traverseRule.TopIcon(madLevelIcon2);
					keyDown = true;
					lastActionTime = Time.time;
				}
			}
			else if (isDown && traverseRule.canGoDown)
			{
				if (CanExecuteAction())
				{
					madLevelIcon = traverseRule.BottomIcon(madLevelIcon2);
					keyDown = true;
					lastActionTime = Time.time;
				}
			}
			else if (isEnter)
			{
				Activate(madLevelIcon2);
			}
			else
			{
				keyDown = false;
			}
			if (madLevelIcon != null && madLevelIcon != madLevelIcon2)
			{
				Activate(madLevelIcon);
			}
		}

		private bool CanExecuteAction()
		{
			if (!keyDown)
			{
				return true;
			}
			if (!repeat)
			{
				return false;
			}
			if (lastActionTime + repeatInterval <= Time.time)
			{
				return true;
			}
			return false;
		}

		private MadLevelIcon ActiveIcon()
		{
			MadLevelAbstractLayout current = MadLevelLayout.current;
			return current.GetActiveIcon();
		}

		private void ActivateCurrentLevel()
		{
			MadLevelAbstractLayout current = MadLevelLayout.current;
			MadLevelIcon activeIcon = current.GetActiveIcon();
			Activate(activeIcon);
		}

		private void Activate(MadLevelIcon icon)
		{
			if (icon == null)
			{
				return;
			}
			MadLevelAbstractLayout current = MadLevelLayout.current;
			if (current.CanActivate(icon))
			{
				current.Activate(icon);
				if (current is MadLevelFreeLayout)
				{
					MadLevelFreeLayout madLevelFreeLayout = current as MadLevelFreeLayout;
					madLevelFreeLayout.LookAtIcon(icon, MadiTween.EaseType.easeOutCubic, 1f);
				}
				else if (current is MadLevelGridLayout)
				{
					MadLevelGridLayout madLevelGridLayout = current as MadLevelGridLayout;
					madLevelGridLayout.LookAtIconAnimate(icon);
				}
			}
		}
        */
	}
}
