using UnityEngine;

public class TWAPITest : MonoBehaviour
{
	private void Start()
	{
		TW_OAuthAPIRequest tW_OAuthAPIRequest = TW_OAuthAPIRequest.Create();
		tW_OAuthAPIRequest.AddParam("count", 1);
		tW_OAuthAPIRequest.Send("https://api.twitter.com/1.1/statuses/home_timeline.json");
		tW_OAuthAPIRequest.OnResult += OnResult;
	}

	private void OnResult(TW_APIRequstResult result)
	{
		UnityEngine.Debug.Log("Is Request Succeeded: " + result.IsSucceeded);
		UnityEngine.Debug.Log("Responce data:");
		UnityEngine.Debug.Log(result.responce);
	}
}
