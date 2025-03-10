using System;
using System.Threading;
using UnityEngine;

public class InitAndroidInventoryTask : MonoBehaviour
{
	public event Action ActionComplete;

	public event Action ActionFailed;

	public InitAndroidInventoryTask()
	{
		this.ActionComplete = delegate
		{
		};
		this.ActionFailed = delegate
		{
		};
		
	}

	public static InitAndroidInventoryTask Create()
	{
		return new GameObject("InitAndroidInventoryTask").AddComponent<InitAndroidInventoryTask>();
	}

	public void Run()
	{
		UnityEngine.Debug.Log("InitAndroidInventoryTask task started");
		if (AndroidInAppPurchaseManager.Client.IsConnected)
		{
			OnBillingConnected(null);
			return;
		}
		AndroidInAppPurchaseManager.ActionBillingSetupFinished += OnBillingConnected;
		if (!AndroidInAppPurchaseManager.Client.IsConnectingToServiceInProcess)
		{
			AndroidInAppPurchaseManager.Client.Connect();
		}
	}

	private void OnBillingConnected(BillingResult result)
	{
		UnityEngine.Debug.Log("OnBillingConnected");
		if (result == null)
		{
			OnBillingConnectFinished();
			return;
		}
		AndroidInAppPurchaseManager.ActionBillingSetupFinished -= OnBillingConnected;
		if (result.IsSuccess)
		{
			OnBillingConnectFinished();
			return;
		}
		UnityEngine.Debug.Log("OnBillingConnected Failed");
		this.ActionFailed();
	}

	private void OnBillingConnectFinished()
	{
		UnityEngine.Debug.Log("OnBillingConnected COMPLETE");
		if (AndroidInAppPurchaseManager.Client.IsInventoryLoaded)
		{
			UnityEngine.Debug.Log("IsInventoryLoaded COMPLETE");
			this.ActionComplete();
			return;
		}
		AndroidInAppPurchaseManager.ActionRetrieveProducsFinished += OnRetrieveProductsFinised;
		if (!AndroidInAppPurchaseManager.Client.IsProductRetrievingInProcess)
		{
			AndroidInAppPurchaseManager.Client.RetrieveProducDetails();
		}
	}

	private void OnRetrieveProductsFinised(BillingResult result)
	{
		UnityEngine.Debug.Log("OnRetrieveProductsFinised");
		AndroidInAppPurchaseManager.ActionRetrieveProducsFinished -= OnRetrieveProductsFinised;
		if (result.IsSuccess)
		{
			UnityEngine.Debug.Log("OnRetrieveProductsFinised COMPLETE");
			this.ActionComplete();
		}
		else
		{
			UnityEngine.Debug.Log("OnRetrieveProductsFinised FAILED");
			this.ActionFailed();
		}
	}
}
