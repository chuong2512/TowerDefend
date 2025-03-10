using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TDTK
{
	public class UIMessage : MonoBehaviour
	{
		[Serializable]
		public class UIMsgItem
		{
			public GameObject rootObj;

			public RectTransform rectT;

			public Text label;

			public CanvasGroup canvasG;

			public UIMsgItem()
			{
			}

			public UIMsgItem(GameObject obj)
			{
				rootObj = obj;
				Init();
			}

			public virtual void Init()
			{
				canvasG = rootObj.GetComponent<CanvasGroup>();
				rectT = rootObj.GetComponent<RectTransform>();
				label = rootObj.GetComponent<Text>();
			}

			public static UIMsgItem Clone(GameObject srcObj, string name = "")
			{
				return new UIMsgItem(UI.Clone(srcObj, name));
			}
		}

		public GameObject messageObj;

		private List<UIMsgItem> msgList = new List<UIMsgItem>();

		private static UIMessage instance;

		private Vector3 scale = new Vector3(1f, 1f, 1f);

		private Vector3 scaleZoomed = new Vector3(1.25f, 1.25f, 1.25f);

		private void Awake()
		{
			instance = this;
			base.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
		}

		private void Start()
		{
			for (int i = 0; i < 15; i++)
			{
				if (i == 0)
				{
					msgList.Add(new UIMsgItem(messageObj));
				}
				else
				{
					msgList.Add(UIMsgItem.Clone(messageObj, "TextMessage" + (i + 1)));
				}
				msgList[i].rootObj.SetActive(value: false);
			}
		}

		private void OnEnable()
		{
			TDTK.onGameMessageE += _DisplayMessage;
		}

		private void OnDisable()
		{
			TDTK.onGameMessageE -= _DisplayMessage;
		}

		public static void DisplayMessage(string msg)
		{
			instance._DisplayMessage(msg);
		}

		private void _DisplayMessage(string msg)
		{
			int unusedTextIndex = GetUnusedTextIndex();
			msgList[unusedTextIndex].label.text = msg;
			StartCoroutine(DisplayItemRoutine(msgList[unusedTextIndex]));
		}

		private IEnumerator DisplayItemRoutine(UIMsgItem item)
		{
			item.rectT.SetAsFirstSibling();
			UIMainControl.FadeIn(item.canvasG, 0.1f, item.rootObj);
			StartCoroutine(ScaleRectTRoutine(item.rectT, 0.1f, scale, scaleZoomed));
			yield return StartCoroutine(UIMainControl.WaitForRealSeconds(0.1f));
			StartCoroutine(ScaleRectTRoutine(item.rectT, 0.25f, scaleZoomed, scale));
			yield return StartCoroutine(UIMainControl.WaitForRealSeconds(0.8f));
			UIMainControl.FadeOut(item.canvasG, 1f, item.rootObj);
		}

		private IEnumerator ScaleRectTRoutine(RectTransform rectt, float dur, Vector3 startS, Vector3 endS)
		{
			float timeMul = 1f / dur;
			float duration = 0f;
			while (duration < 1f)
			{
				rectt.localScale = Vector3.Lerp(startS, endS, duration);
				duration += Time.unscaledDeltaTime * timeMul;
				yield return null;
			}
		}

		private int GetUnusedTextIndex()
		{
			for (int i = 0; i < msgList.Count; i++)
			{
				if (!msgList[i].rootObj.activeInHierarchy)
				{
					return i;
				}
			}
			msgList.Add(UIMsgItem.Clone(messageObj, "TextMessage" + (msgList.Count + 1)));
			return msgList.Count - 1;
		}
	}
}
