using SA.Common.Pattern;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class TW_APIRequest : MonoBehaviour
{
	private bool IsFirst;

	private string GetParams;

	private string requestUrl;

	private Dictionary<string, string> Headers;

	public event Action<TW_APIRequstResult> ActionComplete;

	protected TW_APIRequest()
	{
		this.ActionComplete = delegate
		{
		};
		IsFirst = true;
		GetParams = string.Empty;
		Headers = new Dictionary<string, string>();
		
	}

	public void Send()
	{
		if (Singleton<TwitterApplicationOnlyToken>.Instance.currentToken == null)
		{
			Singleton<TwitterApplicationOnlyToken>.Instance.ActionComplete += OnTokenLoaded;
			Singleton<TwitterApplicationOnlyToken>.Instance.RetrieveToken();
		}
		else
		{
			StartCoroutine(Request());
		}
	}

	public void AddParam(string name, int value)
	{
		AddParam(name, value.ToString());
	}

	public void AddParam(string name, string value)
	{
		if (!IsFirst)
		{
			GetParams += "&";
		}
		else
		{
			GetParams += "?";
		}
		GetParams = GetParams + name + "=" + value;
		IsFirst = false;
	}

	protected void SendCompleteResult(TW_APIRequstResult res)
	{
		this.ActionComplete(res);
	}

	protected void SetUrl(string url)
	{
		requestUrl = url;
	}

	private IEnumerator Request()
	{
		requestUrl += GetParams;
		Headers.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
		Headers.Add("Authorization", "Bearer " + Singleton<TwitterApplicationOnlyToken>.Instance.currentToken);
		WWW www = new WWW(requestUrl, null, Headers);
		yield return www;
		if (www.error == null)
		{
			OnResult(www.text);
		}
		else
		{
			this.ActionComplete(new TW_APIRequstResult(IsResSucceeded: false, www.error));
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	protected abstract void OnResult(string data);

	private void OnTokenLoaded()
	{
		Singleton<TwitterApplicationOnlyToken>.Instance.ActionComplete -= OnTokenLoaded;
		if (Singleton<TwitterApplicationOnlyToken>.Instance.currentToken != null)
		{
			StartCoroutine(Request());
		}
		else
		{
			this.ActionComplete(new TW_APIRequstResult(IsResSucceeded: false, "Retirving auth token failed"));
		}
	}
}
