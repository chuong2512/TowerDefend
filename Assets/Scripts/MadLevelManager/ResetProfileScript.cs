using System;
using UnityEngine;

namespace MadLevelManager
{
	public class ResetProfileScript : MonoBehaviour
	{
		public bool resetOnRKey;

		private void Start()
		{
			MadSprite sprite = GetComponent<MadSprite>();
			if (sprite != null)
			{
				MadSprite madSprite = sprite;
				madSprite.onMouseEnter = (MadSprite.Action)Delegate.Combine(madSprite.onMouseEnter, (MadSprite.Action)delegate
				{
					sprite.AnimScaleTo(Vector3.one * 1.5f, 1f, MadiTween.EaseType.easeOutElastic);
				});
				MadSprite madSprite2 = sprite;
				madSprite2.onMouseExit = (MadSprite.Action)Delegate.Combine(madSprite2.onMouseExit, (MadSprite.Action)delegate
				{
					sprite.AnimScaleTo(Vector3.one, 1f, MadiTween.EaseType.easeOutElastic);
				});
				MadSprite madSprite3 = sprite;
				MadSprite.Action onMouseDown = madSprite3.onMouseDown;
				MadSprite.Action action = delegate
				{
					MadLevelProfile.Reset();
					MadLevel.ReloadCurrent();
				};
				sprite.onTap = action;
				madSprite3.onMouseDown = (MadSprite.Action)Delegate.Combine(onMouseDown, action);
			}
		}

		private void Update()
		{
			if (resetOnRKey && UnityEngine.Input.GetKey(KeyCode.R))
			{
				MadLevelProfile.Reset();
				MadLevel.ReloadCurrent();
			}
		}
	}
}
