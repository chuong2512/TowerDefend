using UnityEngine;
using UnityEngine.EventSystems;

namespace TDTK
{
	public class UIItemCallback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		private Callback enterCB;

		private Callback exitCB;

		private CallbackInputDependent downCB;

		private CallbackInputDependent upCB;

		private GameObject thisObj;

		public void SetEnterCallback(Callback callback)
		{
			enterCB = callback;
		}

		public void SetExitCallback(Callback callback)
		{
			exitCB = callback;
		}

		public void SetDownCallback(CallbackInputDependent callback)
		{
			downCB = callback;
		}

		public void SetUpCallback(CallbackInputDependent callback)
		{
			upCB = callback;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (enterCB != null)
			{
				enterCB(thisObj);
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (exitCB != null)
			{
				exitCB(thisObj);
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (downCB != null)
			{
				downCB(thisObj, eventData.pointerId);
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (upCB != null)
			{
				upCB(thisObj, eventData.pointerId);
			}
		}

		private void Awake()
		{
			thisObj = base.gameObject;
		}
	}
}
