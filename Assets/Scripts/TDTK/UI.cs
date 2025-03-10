using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TDTK
{
	public class UI
	{
		public static bool IsCursorOnUI(int inputID = -1)
		{
			EventSystem current = EventSystem.current;
			return current.IsPointerOverGameObject(inputID);
		}

		public static GameObject GetHoveredUIElement(Vector2 cursorPos)
		{
			if (EventSystem.current == null)
			{
				return null;
			}
			List<RaycastResult> list = new List<RaycastResult>();
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.position = cursorPos;
			EventSystem.current.RaycastAll(pointerEventData, list);
			return (list.Count <= 0) ? null : list[0].gameObject;
		}

		public static GameObject Clone(GameObject srcObj, string name = "", Vector3 posOffset = default(Vector3))
		{
			GameObject gameObject = Object.Instantiate(srcObj);
			gameObject.name = ((!(name == string.Empty)) ? name : srcObj.name);
			gameObject.transform.SetParent(srcObj.transform.parent);
			gameObject.transform.localPosition = srcObj.transform.localPosition + posOffset;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			return gameObject;
		}
	}
}
