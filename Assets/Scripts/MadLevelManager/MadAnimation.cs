using System;
using UnityEngine;

namespace MadLevelManager
{
	public class MadAnimation : MonoBehaviour
	{
		[Serializable]
		public class Action
		{
			[Serializable]
			public class Tint
			{
				public bool useBase;

				public Color color = Color.white;
			}

			public bool enabled;

			public MadiTween.EaseType easeType = MadiTween.EaseType.easeOutElastic;

			public Vector3 move;

			public Vector3 rotate;

			public Vector3 scale = Vector3.one;

			public float time = 1f;

			public bool tintEnabled;

			public Tint tint = new Tint();

			public AudioClip playSound;

			public float playSoundVolume = 1f;
		}

		public Action onMouseEnter;

		public Action onMouseExit;

		public Action onTouchEnter;

		public Action onTouchExit;

		public Action onFocus;

		public Action onFocusLost;

		private MadSprite sprite;

		private Vector3 origPosition;

		private Vector3 origRotation;

		private Vector3 origScale;

		private Color origTint;

		private bool hasOrigs;

		private void Start()
		{
			sprite = GetComponent<MadSprite>();
			if (sprite != null)
			{
				MadSprite madSprite = sprite;
				madSprite.onMouseEnter = (MadSprite.Action)Delegate.Combine(madSprite.onMouseEnter, (MadSprite.Action)delegate
				{
					AnimOnMouseEnter();
				});
				MadSprite madSprite2 = sprite;
				madSprite2.onMouseExit = (MadSprite.Action)Delegate.Combine(madSprite2.onMouseExit, (MadSprite.Action)delegate
				{
					AnimOnMouseExit();
				});
				MadSprite madSprite3 = sprite;
				madSprite3.onTouchEnter = (MadSprite.Action)Delegate.Combine(madSprite3.onTouchEnter, (MadSprite.Action)delegate
				{
					AnimOnTouchEnter();
				});
				MadSprite madSprite4 = sprite;
				madSprite4.onTouchExit = (MadSprite.Action)Delegate.Combine(madSprite4.onTouchExit, (MadSprite.Action)delegate
				{
					AnimOnTouchExit();
				});
				MadSprite madSprite5 = sprite;
				madSprite5.onFocus = (MadSprite.Action)Delegate.Combine(madSprite5.onFocus, (MadSprite.Action)delegate
				{
					PlayOnFocus();
				});
				MadSprite madSprite6 = sprite;
				madSprite6.onFocusLost = (MadSprite.Action)Delegate.Combine(madSprite6.onFocusLost, (MadSprite.Action)delegate
				{
					PlayOnFocusLost();
				});
			}
			else
			{
				UnityEngine.Debug.LogError("This component must be attached with sprite!", this);
			}
		}

		private void AnimOnMouseEnter()
		{
			UpdateOrigs();
			PlayAction(onMouseEnter);
		}

		private void AnimOnMouseExit()
		{
			UpdateOrigs();
			PlayAction(onMouseExit);
		}

		private void AnimOnTouchEnter()
		{
			UpdateOrigs();
			PlayAction(onTouchEnter);
		}

		private void AnimOnTouchExit()
		{
			UpdateOrigs();
			PlayAction(onTouchExit);
		}

		private void PlayOnFocus()
		{
			UpdateOrigs();
			PlayAction(onFocus);
		}

		private void PlayOnFocusLost()
		{
			UpdateOrigs();
			PlayAction(onFocusLost);
		}

		private void UpdateOrigs()
		{
			if (!hasOrigs)
			{
				origPosition = base.transform.localPosition;
				origRotation = base.transform.localRotation.eulerAngles;
				origScale = base.transform.localScale;
				origTint = sprite.tint;
				hasOrigs = true;
			}
		}

		private void PlayAction(Action action)
		{
			if (!action.enabled)
			{
				return;
			}
			sprite.AnimMoveTo(origPosition + action.move, action.time, action.easeType);
			sprite.AnimRotateTo(origRotation + action.rotate, action.time, action.easeType);
			sprite.AnimScaleTo(Vector3.Scale(origScale, action.scale), action.time, action.easeType);
			if (action.tintEnabled)
			{
				if (action.tint.useBase)
				{
					sprite.AnimColorTo(origTint, action.time, action.easeType);
				}
				else
				{
					sprite.AnimColorTo(action.tint.color, action.time, action.easeType);
				}
			}
			if (action.playSound != null)
			{
				AudioSource.PlayClipAtPoint(action.playSound, Camera.main.transform.position, action.playSoundVolume);
			}
		}
	}
}
