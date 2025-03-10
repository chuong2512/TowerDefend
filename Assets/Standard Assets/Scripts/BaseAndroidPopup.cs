using System;
using System.Threading;
using UnityEngine;

public class BaseAndroidPopup : MonoBehaviour
{
	public string title;

	public string message;

	public event Action<AndroidDialogResult> ActionComplete;

	public BaseAndroidPopup()
	{
		this.ActionComplete = delegate
		{
		};
		
	}

	public void onDismissed(string data)
	{
		this.ActionComplete(AndroidDialogResult.CLOSED);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	protected void DispatchAction(AndroidDialogResult res)
	{
		this.ActionComplete(res);
	}
}
