using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

namespace TDTK
{
	public class UIMainControl : MonoBehaviour
	{
		public delegate void VicAction(bool playerWon);

		[Space(5f)]
		[Tooltip("Check to make the force the UI into Perk Menu only.\nIntended for inter-level scene to unlock new perk in for design which involve persistent perk unlock.")]
		public bool perkMenuOnly;

		[Space(5f)]
		[Tooltip("Check to enable touch mode (optional mode intend for touch input)\n\nwhen using touch mode, build and ability button wont be trigger immediately as soon as they are click.\n\nInstead the first click will only bring up the tooltip, a second click will then confirm the button click")]
		public bool touchMode;

		[Space(5f)]
		[Tooltip("Check to use pie-menu arrangement for the build-buttons.\nOnly applicable when using PointNBuild Mode")]
		public bool usePieBuildMenu;

		[Space(5f)]
		[Tooltip("Check to show hit-point overlay on top of each unit")]
		public bool enableHPOverlay = true;

		[Tooltip("Check to show damage overlay of each attack when hitting a target")]
		public bool enableTextOverlay = true;

		[Space(5f)]
		[Tooltip("Check to have the game over menu show the 'next level button' only whent the level is won")]
		public bool alwaysShowNextButton = true;

		[Space(10f)]
		[Tooltip("The blur image effect component on the main ui camera (optional)")]
		public BlurOptimized uiBlurEffect;

		[Space(10f)]
		[Tooltip("Check to have the camera auto center on build point or selected tower\nOnly available when using default CameraControl")]
		public bool autoCenterCamera;

		[Space(10f)]
		[Tooltip("Check to disable auto scale up of UIElement when the screen resolution exceed reference resolution specified in CanvasScaler/nRecommended to have this set to false when building for mobile")]
		public bool limitScale = true;

		[Tooltip("The CanvasScaler components of all the canvas. Required to have the floating UI elements appear in the right screen position")]
		public List<CanvasScaler> scalerList = new List<CanvasScaler>();

		private bool enableInput = true;

		private static UIMainControl instance;

		[CompilerGenerated]
		private static TDTK.FPSModeHandler _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static TDTK.FPSModeHandler _003C_003Ef__mg_0024cache1;

		public static event VicAction OnVictory;

		public static bool InTouchMode()
		{
			return instance.touchMode;
		}

		public static bool UsePieMenu()
		{
			return instance.usePieBuildMenu;
		}

		public static bool EnableHPOverlay()
		{
			return instance.enableHPOverlay;
		}

		public static bool EnableTextOverlay()
		{
			return instance.enableTextOverlay;
		}

		public static bool AlwaysShowNextButton()
		{
			return instance.alwaysShowNextButton;
		}

		public static float GetScaleFactor()
		{
			if (instance.scalerList.Count == 0)
			{
				return 1f;
			}
			if (instance.scalerList[0].uiScaleMode == CanvasScaler.ScaleMode.ConstantPixelSize)
			{
				return 1f / instance.scalerList[0].scaleFactor;
			}
			if (instance.scalerList[0].uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
			{
				Vector2 referenceResolution = instance.scalerList[0].referenceResolution;
				return referenceResolution.x / (float)Screen.width;
			}
			return 1f;
		}

		public static void EnableInput()
		{
			instance.enableInput = true;
		}

		public static void DisableInput()
		{
			instance.enableInput = false;
		}

		private void Awake()
		{
			instance = this;
		}

		private void Start()
		{
			if (limitScale)
			{
				for (int i = 0; i < scalerList.Count; i++)
				{
					float num = Screen.width;
					Vector2 referenceResolution = scalerList[i].referenceResolution;
					if (num >= referenceResolution.x)
					{
						instance.scalerList[i].uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
					}
					else
					{
						instance.scalerList[i].uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
					}
				}
			}
			if (perkMenuOnly)
			{
				UIHUD.GetInstance().gameObject.SetActive(value: false);
				UIBuildButton.GetInstance().gameObject.SetActive(value: false);
				UIAbilityButton.GetInstance().gameObject.SetActive(value: false);
				UITowerView.GetInstance().gameObject.SetActive(value: false);
				UIPerkMenu.DisableCloseButton();
				OnPerkMenu();
			}
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
			}
			if (enableInput && UnityEngine.Input.touchCount <= 1 && !UI.IsCursorOnUI(0) && !UI.IsCursorOnUI() && Input.GetMouseButtonDown(0))
			{
				OnCursorDown(UnityEngine.Input.mousePosition);
			}
		}

		private void OnCursorDown(Vector3 cursorPos)
		{
			UnitTower unitTower = GameControl.Select(cursorPos);
			GameControl.SelectTower(unitTower);
			if (unitTower != null)
			{
				if (!BuildManager.UseDragNDrop())
				{
					UIBuildButton.Hide();
				}
				if (autoCenterCamera)
				{
					CameraControl.SetPosition(unitTower.thisT.position);
				}
				UITowerView.Show(unitTower);
				return;
			}
			UITowerView.Hide();
			if (!BuildManager.UseDragNDrop())
			{
				BuildInfo buildInfo = BuildManager.CheckBuildPoint(cursorPos);
				UIBuildButton.Show(buildInfo);
				if (buildInfo.status == _TileStatus.Available && autoCenterCamera)
				{
					CameraControl.SetPosition(buildInfo.position);
				}
			}
		}

		public static void ClearSelectedTower()
		{
			GameControl.SelectTower();
			UITowerView.Hide();
		}

		private void OnEnable()
		{
			TDTK.onGameOverE += OnGameOver;
			TDTK.onFPSModeE += OnFPSMode;
		}

		private void OnDisable()
		{
			TDTK.onGameOverE -= OnGameOver;
			TDTK.onFPSModeE -= OnFPSMode;
		}

		public void OnGameOver(bool won)
		{
			if (UIMainControl.OnVictory != null)
			{
				UIMainControl.OnVictory(won);
			}
		}

		private IEnumerator GameOverDelay(bool won)
		{
			yield return StartCoroutine(WaitForRealSeconds(0.1f));
			CameraControl.FadeBlur(uiBlurEffect, 0f, 2f);
			CameraControl.TurnBlurOn();
			UIGameOver.Show(won);
		}

		public static void OnFPSMode(bool flag)
		{
			if (flag)
			{
				UIBuildButton.Hide();
				UIAbilityButton.Hide();
				UIFPS.Show();
			}
			else
			{
				UIBuildButton.Show();
				UIAbilityButton.Show();
				UIFPS.Hide();
			}
		}

		public static void OnPerkMenu()
		{
			instance._OnPerkMenu();
		}

		public void _OnPerkMenu()
		{
			UITowerView.Hide();
			CameraControl.FadeBlur(uiBlurEffect, 0f, 2f);
			CameraControl.TurnBlurOn();
			GameControl.PauseGame();
			UIPerkMenu.Show();
			Time.timeScale = 0f;
		}

		public static void ClosePerkMenu()
		{
			instance.StartCoroutine(instance._ClosePerkMenu());
		}

		private IEnumerator _ClosePerkMenu()
		{
			CameraControl.FadeBlur(uiBlurEffect, 2f);
			CameraControl.TurnBlurOff();
			GameControl.ResumeGame();
			UIPerkMenu.Hide();
			yield return StartCoroutine(WaitForRealSeconds(0.25f));
			Time.timeScale = 1f;
		}

		public static void TogglePause()
		{
			instance._TogglePause();
		}

		public void _TogglePause()
		{
			if (GameControl.IsGamePlaying())
			{
				PauseGame();
			}
			else if (GameControl.IsGamePaused())
			{
				ResumeGame();
			}
		}

		public static void PauseGame()
		{
			instance._PauseGame();
		}

		public void _PauseGame()
		{
			UnityEngine.Debug.Log("_PauseGame");
			GameControl.PauseGame();
		}

		public static void ResumeGame()
		{
			instance.StartCoroutine(instance._ResumeGame());
		}

		private IEnumerator _ResumeGame()
		{
			UnityEngine.Debug.Log("_ResumeGame");
			GameControl.ResumeGame();
			yield return StartCoroutine(WaitForRealSeconds(0.25f));
		}

		public static IEnumerator WaitForRealSeconds(float time)
		{
			float start = Time.realtimeSinceStartup;
			while (Time.realtimeSinceStartup < start + time)
			{
				yield return null;
			}
		}

		public static void FadeOut(CanvasGroup canvasGroup, float duration = 0.25f, GameObject obj = null)
		{
			instance.StartCoroutine(instance._FadeOut(canvasGroup, 1f / duration, obj));
		}

		private IEnumerator _FadeOut(CanvasGroup canvasGroup, float timeMul, GameObject obj)
		{
			float duration = 0f;
			while (duration < 1f)
			{
				canvasGroup.alpha = Mathf.Lerp(1f, 0f, duration);
				duration += Time.unscaledDeltaTime * timeMul;
				yield return null;
			}
			canvasGroup.alpha = 0f;
			if (obj != null)
			{
				obj.SetActive(value: false);
			}
		}

		public static void FadeIn(CanvasGroup canvasGroup, float duration = 0.25f, GameObject obj = null)
		{
			instance.StartCoroutine(instance._FadeIn(canvasGroup, 1f / duration, obj));
		}

		private IEnumerator _FadeIn(CanvasGroup canvasGroup, float timeMul, GameObject obj)
		{
			if (obj != null)
			{
				obj.SetActive(value: true);
			}
			float duration = 0f;
			while (duration < 1f)
			{
				canvasGroup.alpha = Mathf.Lerp(0f, 1f, duration);
				duration += Time.unscaledDeltaTime * timeMul;
				yield return null;
			}
			canvasGroup.alpha = 1f;
		}
	}
}
