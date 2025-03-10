using System;
using UnityEngine;

namespace MadLevelManager
{
	public class MadEvent : MonoBehaviour
	{
		public enum EventType
		{
			OnMouseEnter,
			OnMouseExit,
			OnMouseDown,
			OnMouseUp,
			OnTap,
			OnFocusGain,
			OnFocusLost
		}

		public EventType eventType;

		public string message;

		public GameObject messageReceiver;

		private void Start()
		{
			MadSprite component = GetComponent<MadSprite>();
			if (component == null)
			{
				UnityEngine.Debug.LogError("This component must be attached along with MadSprite component!");
				return;
			}
			switch (eventType)
			{
			case EventType.OnMouseEnter:
			{
				MadSprite madSprite7 = component;
				madSprite7.onMouseEnter = (MadSprite.Action)Delegate.Combine(madSprite7.onMouseEnter, (MadSprite.Action)delegate
				{
					Invoke();
				});
				break;
			}
			case EventType.OnMouseExit:
			{
				MadSprite madSprite6 = component;
				madSprite6.onMouseExit = (MadSprite.Action)Delegate.Combine(madSprite6.onMouseExit, (MadSprite.Action)delegate
				{
					Invoke();
				});
				break;
			}
			case EventType.OnMouseDown:
			{
				MadSprite madSprite5 = component;
				madSprite5.onMouseDown = (MadSprite.Action)Delegate.Combine(madSprite5.onMouseDown, (MadSprite.Action)delegate
				{
					Invoke();
				});
				break;
			}
			case EventType.OnMouseUp:
			{
				MadSprite madSprite4 = component;
				madSprite4.onMouseUp = (MadSprite.Action)Delegate.Combine(madSprite4.onMouseUp, (MadSprite.Action)delegate
				{
					Invoke();
				});
				break;
			}
			case EventType.OnTap:
			{
				MadSprite madSprite3 = component;
				madSprite3.onTap = (MadSprite.Action)Delegate.Combine(madSprite3.onTap, (MadSprite.Action)delegate
				{
					Invoke();
				});
				break;
			}
			case EventType.OnFocusGain:
			{
				MadSprite madSprite2 = component;
				madSprite2.onFocus = (MadSprite.Action)Delegate.Combine(madSprite2.onFocus, (MadSprite.Action)delegate
				{
					Invoke();
				});
				break;
			}
			case EventType.OnFocusLost:
			{
				MadSprite madSprite = component;
				madSprite.onFocusLost = (MadSprite.Action)Delegate.Combine(madSprite.onFocusLost, (MadSprite.Action)delegate
				{
					Invoke();
				});
				break;
			}
			default:
				UnityEngine.Debug.LogError("Unknown event type: " + eventType);
				break;
			}
		}

		private void Invoke()
		{
			if (messageReceiver != null)
			{
				messageReceiver.SendMessage(message);
			}
			else
			{
				SendMessage(message);
			}
		}
	}
}
