using MadLevelManager;
using SA.Common.Pattern;
using UnityEngine;

public class googleAnalytic : MonoBehaviour
{
	private void Awake()
	{
		Singleton<AndroidGoogleAnalytics>.Instance.StartTracking();
	}

	private void Start()
	{
		Singleton<AndroidGoogleAnalytics>.Instance.SendView(MadLevel.currentLevelName);
	}

	private void Update()
	{
	}
}
