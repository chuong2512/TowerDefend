using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TDTK
{
	public class UITextOverlay : MonoBehaviour
	{
		public List<Text> textOverlayList = new List<Text>();

		private Color defaultColor = default(Color);

		private int defaultFontSize = 16;

		public static UITextOverlay instance;

		public float textDuration = 0.5f;

		private float durationMultiplier;

		private void Start()
		{
			instance = this;
			for (int i = 0; i < 20; i++)
			{
				if (i > 0)
				{
					GameObject gameObject = UI.Clone(textOverlayList[0].gameObject, "Text " + i);
					textOverlayList.Add(gameObject.GetComponent<Text>());
				}
				textOverlayList[i].text = string.Empty;
				textOverlayList[i].gameObject.SetActive(value: false);
			}
			durationMultiplier = 1f / textDuration;
			if (textOverlayList.Count > 0)
			{
				defaultColor = textOverlayList[0].color;
				defaultFontSize = textOverlayList[0].fontSize;
			}
		}

		private void OnEnable()
		{
			TextOverlay.onTextOverlayE += NewTextOverlay;
		}

		private void OnDisable()
		{
			TextOverlay.onTextOverlayE -= NewTextOverlay;
		}

		private void NewTextOverlay(TextOverlay overlayInstance)
		{
			if (UIMainControl.EnableTextOverlay())
			{
				Text unusedTextOverlay = GetUnusedTextOverlay();
				unusedTextOverlay.text = overlayInstance.msg;
				if (overlayInstance.useColor)
				{
					unusedTextOverlay.color = overlayInstance.color;
				}
				else
				{
					unusedTextOverlay.color = defaultColor;
				}
				unusedTextOverlay.fontSize = (int)Mathf.Round((float)defaultFontSize * overlayInstance.scale);
				unusedTextOverlay.transform.localPosition = GetScreenPos(overlayInstance.pos);
				unusedTextOverlay.gameObject.SetActive(value: true);
				StartCoroutine(TextOverlayRoutine(overlayInstance.pos, unusedTextOverlay));
			}
		}

		private IEnumerator TextOverlayRoutine(Vector3 pos, Text txt)
		{
			StartCoroutine(ZoomEffect(txt.transform));
			Transform txtT = txt.transform;
			Vector3 movedPos = Vector3.zero;
			float duration = 0f;
			while (duration < 1f)
			{
				Vector3 screenPos = GetScreenPos(pos);
				movedPos += new Vector3(0f, 7.5f * Time.deltaTime, 0f);
				txtT.localPosition = screenPos + movedPos;
				Color color = txt.color;
				color.a = ((!(duration > 0.5f)) ? 1f : ((1f - duration) * 2f));
				txt.color = color;
				duration += Time.deltaTime * durationMultiplier;
				yield return null;
			}
			txt.text = string.Empty;
			txt.fontSize = defaultFontSize;
			txt.gameObject.SetActive(value: false);
		}

		private IEnumerator ZoomEffect(Transform textT)
		{
			float duration2 = 0f;
			Vector3 localScale = textT.localScale;
			float defaultScale = localScale.x;
			while (duration2 < 1f)
			{
				float scale = Mathf.Lerp(1f, 1.5f, duration2);
				textT.localScale = new Vector3(scale, scale, scale) * defaultScale;
				duration2 += Time.deltaTime * durationMultiplier * 6f;
				yield return null;
			}
			duration2 = 0f;
			while (duration2 < 1f)
			{
				float scale2 = Mathf.Lerp(1.5f, 1f, duration2);
				textT.localScale = new Vector3(scale2, scale2, scale2) * defaultScale;
				duration2 += Time.deltaTime * durationMultiplier * 4f;
				yield return null;
			}
			textT.localScale = new Vector3(1f, 1f, 1f) * defaultScale;
		}

		public Vector3 GetScreenPos(Vector3 worldPos)
		{
			Vector3 result = Camera.main.WorldToScreenPoint(worldPos) * UIMainControl.GetScaleFactor();
			result.z = 0f;
			return result;
		}

		private Text GetUnusedTextOverlay()
		{
			for (int i = 0; i < textOverlayList.Count; i++)
			{
				if (textOverlayList[i].text == string.Empty)
				{
					return textOverlayList[i];
				}
			}
			int count = textOverlayList.Count;
			GameObject gameObject = UI.Clone(textOverlayList[0].gameObject, "Text " + textOverlayList.Count);
			textOverlayList.Add(gameObject.GetComponent<Text>());
			textOverlayList[count].text = string.Empty;
			textOverlayList[count].gameObject.SetActive(value: false);
			return textOverlayList[count];
		}
	}
}
