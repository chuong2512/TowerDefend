using SA.Common.Pattern;
using UnityEngine;

public class AnalyticsUseExample : MonoBehaviour
{
	private void Awake()
	{
		Singleton<AndroidGoogleAnalytics>.Instance.StartTracking();
	}

	private void Start()
	{
		Singleton<AndroidGoogleAnalytics>.Instance.SendView("Home Screen");
		Singleton<AndroidGoogleAnalytics>.Instance.SendTiming("App Started", (long)Time.time);
	}

	public void PurchaseTackingExample()
	{
		Singleton<AndroidGoogleAnalytics>.Instance.CreateTransaction("0_123456", "In-app Store", 2.1f, 0.17f, 0f, "USD");
		Singleton<AndroidGoogleAnalytics>.Instance.CreateItem("0_123456", "Level Pack: Space", "L_789", "Game expansions", 1.99f, 1, "USD");
	}
}
