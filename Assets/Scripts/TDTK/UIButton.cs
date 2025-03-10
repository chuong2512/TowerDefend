using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TDTK
{
	[Serializable]
	public class UIButton : UIObject
	{
		[HideInInspector]
		public Text labelAlt;

		[HideInInspector]
		public Text labelAlt2;

		private Image imgHoverHighlight;

		private Image imgDisHighlight;

		[HideInInspector]
		public Image imgHighlight;

		[HideInInspector]
		public Button button;

		public UIButton()
		{
		}

		public UIButton(GameObject obj)
		{
			rootObj = obj;
			Init();
		}

		public override void Init()
		{
			base.Init();
			button = rootObj.GetComponent<Button>();
			IEnumerator enumerator = rectT.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					if (transform.name == "TextAlt")
					{
						labelAlt = transform.GetComponent<Text>();
					}
					if (transform.name == "TextAlt2")
					{
						labelAlt2 = transform.GetComponent<Text>();
					}
					if (transform.name == "Highlight")
					{
						imgHighlight = transform.GetComponent<Image>();
					}
					if (transform.name == "HoverHighlight")
					{
						imgHoverHighlight = transform.GetComponent<Image>();
					}
					if (transform.name == "DisableHighlight")
					{
						imgDisHighlight = transform.GetComponent<Image>();
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

		public new static UIButton Clone(GameObject srcObj, string name = "", Vector3 posOffset = default(Vector3))
		{
			GameObject obj = UI.Clone(srcObj, name, posOffset);
			return new UIButton(obj);
		}

		public void SetActive(bool flag)
		{
			if (flag)
			{
				imgHoverHighlight.enabled = false;
			}
			if (flag)
			{
				imgDisHighlight.enabled = false;
			}
			rootObj.SetActive(flag);
		}
	}
}
