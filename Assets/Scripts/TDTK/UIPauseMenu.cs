using System.Collections;
using UnityEngine;

namespace TDTK
{
	public class UIPauseMenu : MonoBehaviour
	{
		private GameObject thisObj;

		private RectTransform rectT;

		private CanvasGroup canvasGroup;

		private static UIPauseMenu instance;

		public void Awake()
		{
			instance = this;
			thisObj = base.gameObject;
			rectT = thisObj.GetComponent<RectTransform>();
			canvasGroup = thisObj.GetComponent<CanvasGroup>();
			if (canvasGroup == null)
			{
				canvasGroup = thisObj.AddComponent<CanvasGroup>();
			}
			canvasGroup.alpha = 0f;
			rectT.anchoredPosition = new Vector3(0f, 99999f, 0f);
		}

		public void OnResumeButton()
		{
			UIMainControl.ResumeGame();
		}

		public void OnRestartButton()
		{
			GameControl.RestartScene();
		}

		public void OnOptionButton()
		{
		}

		public void OnMenuButton()
		{
			GameControl.LoadMainMenu();
		}

		public static void Show()
		{
			instance._Show();
		}

		public void _Show()
		{
			rectT.localPosition = new Vector3(0f, 0f, 0f);
			UIMainControl.FadeIn(canvasGroup);
		}

		public static void Hide()
		{
			instance._Hide();
		}

		public void _Hide()
		{
			UIMainControl.FadeOut(canvasGroup);
			StartCoroutine(DelayHide());
		}

		private IEnumerator DelayHide()
		{
			yield return new WaitForSeconds(0.25f);
			rectT.localPosition = new Vector3(-5000f, -5000f, 0f);
		}
	}
}
