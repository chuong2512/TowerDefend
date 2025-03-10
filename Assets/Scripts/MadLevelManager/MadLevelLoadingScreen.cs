using System;
using UnityEngine;

namespace MadLevelManager
{
	public class MadLevelLoadingScreen : MonoBehaviour
	{
		public int notAsyncLoadingStartFrame = 3;

		public bool asyncLoading;

		public string testModeLevelToLoad = string.Empty;

		[NonSerialized]
		public MadLevelConfiguration.Level nextLevel;

		[NonSerialized]
		public AsyncOperation asyncOperation;

		private int frameNumber;

		private bool testMode;

		public float progress
		{
			get
			{
				if (asyncOperation != null)
				{
					return asyncOperation.progress;
				}
				return 0f;
			}
		}

		public bool isDone
		{
			get
			{
				if (asyncOperation != null)
				{
					return asyncOperation.isDone;
				}
				return false;
			}
		}

		public bool isTestMode => testMode;

		private void Start()
		{
			if (!Application.HasProLicense())
			{
				asyncLoading = false;
			}
			if (!MadLevel.hasExtension)
			{
				InitTestMode();
			}
			InitFinalize();
		}

		private void Update()
		{
			frameNumber++;
			if (!asyncLoading && frameNumber >= notAsyncLoadingStartFrame)
			{
				if (MadLevel.hasExtension && MadLevel.CanContinue())
				{
					MadLevel.Continue();
					return;
				}
				UnityEngine.Debug.LogWarning("Level loading screen is meant to be in extension as 'before' scene.");
				MadLevel.LoadNext();
			}
		}

		private void InitTestMode()
		{
			UnityEngine.Debug.Log("Initializing test mode");
			testMode = true;
			if (string.IsNullOrEmpty(testModeLevelToLoad))
			{
				UnityEngine.Debug.LogError("Test level name not set");
				return;
			}
			nextLevel = MadLevel.activeConfiguration.FindLevelByName(testModeLevelToLoad);
			if (nextLevel == null)
			{
				UnityEngine.Debug.LogError("Cannot find level with name " + testModeLevelToLoad);
			}
		}

		private void InitFinalize()
		{
			if (asyncLoading)
			{
				if (MadLevel.hasExtension && MadLevel.CanContinue())
				{
					asyncOperation = MadLevel.ContinueAsync();
					return;
				}
				UnityEngine.Debug.LogWarning("Level loading screen is meant to be in extension as 'before' scene.");
				asyncOperation = MadLevel.LoadNextAsync();
			}
		}
	}
}
