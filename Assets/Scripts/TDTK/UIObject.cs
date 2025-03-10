using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TDTK
{
	[Serializable]
	public class UIObject
	{
		public GameObject rootObj;

		[HideInInspector]
		public Transform rootT;

		[HideInInspector]
		public RectTransform rectT;

		[HideInInspector]
		public Image imgRoot;

		[HideInInspector]
		public Image imgIcon;

		[HideInInspector]
		public Text label;

		[HideInInspector]
		public UIItemCallback itemCallback;

		public UIObject()
		{
		}

		public UIObject(GameObject obj)
		{
			rootObj = obj;
			Init();
		}

		public virtual void Init()
		{
			rootT = rootObj.transform;
			rectT = rootObj.GetComponent<RectTransform>();
			imgRoot = rootObj.GetComponent<Image>();
			IEnumerator enumerator = rectT.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					if (transform.name == "Image")
					{
						imgIcon = transform.GetComponent<Image>();
					}
					else if (transform.name == "Text")
					{
						label = transform.GetComponent<Text>();
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public void SetCallback(Callback enter = null, Callback exit = null, CallbackInputDependent down = null, CallbackInputDependent up = null)
		{
			itemCallback = rootObj.AddComponent<UIItemCallback>();
			itemCallback.SetEnterCallback(enter);
			itemCallback.SetExitCallback(exit);
			itemCallback.SetDownCallback(down);
			itemCallback.SetUpCallback(up);
		}

		public static UIObject Clone(GameObject srcObj, string name = "", Vector3 posOffset = default(Vector3))
		{
			GameObject obj = UI.Clone(srcObj, name, posOffset);
			return new UIObject(obj);
		}
	}
}
