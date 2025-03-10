using System;
using System.Collections.Generic;
using UnityEngine;

namespace MadLevelManager
{
	public class MadAnimator : MonoBehaviour
	{
		[Serializable]
		public class Action
		{
			public List<AnimationRef> playAnimations = new List<AnimationRef>();

			public List<AnimationRef> stopAnimations = new List<AnimationRef>();

			public bool stopAllAnimations;

			public void Execute(GameObject parent)
			{
				if (stopAllAnimations)
				{
					StopAllAnimations(parent);
				}
				else
				{
					StopAnimations(parent);
				}
				PlayAnimations(parent);
			}

			private void PlayAnimations(GameObject parent)
			{
				for (int i = 0; i < playAnimations.Count; i++)
				{
					AnimationRef animationRef = playAnimations[i];
					string name = animationRef.name;
					bool fromTheBeginning = animationRef.fromTheBeginning;
					if (MadAnim.PlayAnimation(parent, name, fromTheBeginning) == 0)
					{
						UnityEngine.Debug.LogWarning("There's no animation with name '" + name + "'.");
					}
				}
			}

			private void StopAnimations(GameObject parent)
			{
				for (int i = 0; i < stopAnimations.Count; i++)
				{
					AnimationRef animationRef = stopAnimations[i];
					string name = animationRef.name;
					if (MadAnim.StopAnimation(parent, name) == 0)
					{
						UnityEngine.Debug.LogWarning("There's no animation with name '" + name + "'.");
					}
				}
			}

			private void StopAllAnimations(GameObject parent)
			{
				List<MadAnim> list = MadAnim.AllAnimations(parent);
				for (int i = 0; i < list.Count; i++)
				{
					MadAnim madAnim = list[i];
					madAnim.Stop();
				}
			}
		}

		[Serializable]
		public class AnimationRef
		{
			public string name;

			public bool fromTheBeginning = true;
		}

		public Action onMouseEnter = new Action();

		public Action onMouseExit = new Action();

		public Action onMouseDown = new Action();

		public Action onMouseUp = new Action();

		public Action onTouchEnter = new Action();

		public Action onTouchExit = new Action();

		public Action onFocus = new Action();

		public Action onFocusLost = new Action();

		protected virtual void Start()
		{
			MadSprite component = GetComponent<MadSprite>();
			if (component != null)
			{
				MadSprite madSprite = component;
				madSprite.onMouseEnter = (MadSprite.Action)Delegate.Combine(madSprite.onMouseEnter, (MadSprite.Action)delegate
				{
					onMouseEnter.Execute(base.gameObject);
				});
				MadSprite madSprite2 = component;
				madSprite2.onMouseExit = (MadSprite.Action)Delegate.Combine(madSprite2.onMouseExit, (MadSprite.Action)delegate
				{
					onMouseExit.Execute(base.gameObject);
				});
				MadSprite madSprite3 = component;
				madSprite3.onMouseDown = (MadSprite.Action)Delegate.Combine(madSprite3.onMouseDown, (MadSprite.Action)delegate
				{
					onMouseDown.Execute(base.gameObject);
				});
				MadSprite madSprite4 = component;
				madSprite4.onMouseUp = (MadSprite.Action)Delegate.Combine(madSprite4.onMouseUp, (MadSprite.Action)delegate
				{
					onMouseUp.Execute(base.gameObject);
				});
				MadSprite madSprite5 = component;
				madSprite5.onTouchEnter = (MadSprite.Action)Delegate.Combine(madSprite5.onTouchEnter, (MadSprite.Action)delegate
				{
					onTouchEnter.Execute(base.gameObject);
				});
				MadSprite madSprite6 = component;
				madSprite6.onTouchExit = (MadSprite.Action)Delegate.Combine(madSprite6.onTouchExit, (MadSprite.Action)delegate
				{
					onTouchExit.Execute(base.gameObject);
				});
				MadSprite madSprite7 = component;
				madSprite7.onFocus = (MadSprite.Action)Delegate.Combine(madSprite7.onFocus, (MadSprite.Action)delegate
				{
					onFocus.Execute(base.gameObject);
				});
				MadSprite madSprite8 = component;
				madSprite8.onFocusLost = (MadSprite.Action)Delegate.Combine(madSprite8.onFocusLost, (MadSprite.Action)delegate
				{
					onFocusLost.Execute(base.gameObject);
				});
			}
			else
			{
				UnityEngine.Debug.LogError("This component must be attached with sprite!", this);
			}
		}

		private void Update()
		{
		}
	}
}
