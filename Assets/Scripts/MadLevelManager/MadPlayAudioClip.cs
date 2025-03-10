using System;
using UnityEngine;

namespace MadLevelManager
{
	public class MadPlayAudioClip : MonoBehaviour
	{
		public enum EventType
		{
			OnMouseEnter,
			OnMouseExit,
			OnMouseDown,
			OnMouseUp,
			OnTouchEnter,
			OnTouchExit,
			OnFocus,
			OnFocusLost
		}

		public EventType eventType;

		public AudioClip audioClip;

		public float volume = 1f;

		private void OnEnable()
		{
			MadSprite component = GetComponent<MadSprite>();
			if (component == null)
			{
				UnityEngine.Debug.LogError("This component requires MadSprite!");
				return;
			}
			switch (eventType)
			{
			case EventType.OnMouseEnter:
			{
				MadSprite madSprite8 = component;
				madSprite8.onMouseEnter = (MadSprite.Action)Delegate.Combine(madSprite8.onMouseEnter, new MadSprite.Action(Invoke));
				break;
			}
			case EventType.OnMouseExit:
			{
				MadSprite madSprite7 = component;
				madSprite7.onMouseExit = (MadSprite.Action)Delegate.Combine(madSprite7.onMouseExit, new MadSprite.Action(Invoke));
				break;
			}
			case EventType.OnMouseDown:
			{
				MadSprite madSprite6 = component;
				madSprite6.onMouseDown = (MadSprite.Action)Delegate.Combine(madSprite6.onMouseDown, new MadSprite.Action(Invoke));
				break;
			}
			case EventType.OnMouseUp:
			{
				MadSprite madSprite5 = component;
				madSprite5.onMouseUp = (MadSprite.Action)Delegate.Combine(madSprite5.onMouseUp, new MadSprite.Action(Invoke));
				break;
			}
			case EventType.OnTouchEnter:
			{
				MadSprite madSprite4 = component;
				madSprite4.onTouchEnter = (MadSprite.Action)Delegate.Combine(madSprite4.onTouchEnter, new MadSprite.Action(Invoke));
				break;
			}
			case EventType.OnTouchExit:
			{
				MadSprite madSprite3 = component;
				madSprite3.onTouchExit = (MadSprite.Action)Delegate.Combine(madSprite3.onTouchExit, new MadSprite.Action(Invoke));
				break;
			}
			case EventType.OnFocus:
			{
				MadSprite madSprite2 = component;
				madSprite2.onFocus = (MadSprite.Action)Delegate.Combine(madSprite2.onFocus, new MadSprite.Action(Invoke));
				break;
			}
			case EventType.OnFocusLost:
			{
				MadSprite madSprite = component;
				madSprite.onFocusLost = (MadSprite.Action)Delegate.Combine(madSprite.onFocusLost, new MadSprite.Action(Invoke));
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void Invoke(MadSprite sprite)
		{
			Camera camera = Camera.main;
			if (camera == null)
			{
				camera = (UnityEngine.Object.FindObjectOfType(typeof(Camera)) as Camera);
			}
			AudioSource.PlayClipAtPoint(audioClip, camera.transform.position, volume);
		}
	}
}
