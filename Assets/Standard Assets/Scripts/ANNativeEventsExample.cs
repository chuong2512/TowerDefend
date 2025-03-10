using SA.Common.Pattern;
using System;
using UnityEngine;

public class ANNativeEventsExample : MonoBehaviour
{
	private void Start()
	{
		AndroidApp instance = Singleton<AndroidApp>.Instance;
		instance.OnActivityResult = (Action<AndroidActivityResult>)Delegate.Combine(instance.OnActivityResult, new Action<AndroidActivityResult>(OnActivityResult));
	}

	private void OnStop()
	{
		UnityEngine.Debug.Log("Activity event: OnStop");
	}

	private void OnStart()
	{
		UnityEngine.Debug.Log("Activity event: OnStart");
	}

	private void OnNewIntent()
	{
		UnityEngine.Debug.Log("Activity event: OnNewIntent");
	}

	private void OnActivityResult(AndroidActivityResult result)
	{
		UnityEngine.Debug.Log("Activity event: OnActivityResult");
		UnityEngine.Debug.Log("result.code: " + result.code);
		UnityEngine.Debug.Log("result.requestId: " + result.requestId);
	}
}
