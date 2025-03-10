using ANMiniJSON;
using SA.Common.Pattern;
using SA.Common.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NotificationsExample : MonoBehaviour
{
	public Texture2D bigPicture;

	private int LastNotificationId;

	private void Awake()
	{
		GoogleCloudMessageService.ActionCMDRegistrationResult += HandleActionCMDRegistrationResult;
		GoogleCloudMessageService.ActionCouldMessageLoaded += OnMessageLoaded;
		GoogleCloudMessageService.ActionGCMPushLaunched += HandleActionGCMPushLaunched;
		GoogleCloudMessageService.ActionGCMPushReceived += HandleActionGCMPushReceived;
		Singleton<GoogleCloudMessageService>.Instance.Init();
	}

	private void Toast()
	{
		AndroidToast.ShowToastNotification("Hello Toast", 1);
	}

	private void Local()
	{
		AndroidNotificationBuilder androidNotificationBuilder = new AndroidNotificationBuilder(IdFactory.NextId, "Local Notification Title", "This is local notification", 10);
		androidNotificationBuilder.SetBigPicture(bigPicture);
		Singleton<AndroidNotificationManager>.Instance.ScheduleLocalNotification(androidNotificationBuilder);
	}

	private void LoadLaunchNotification()
	{
		AndroidNotificationManager instance = Singleton<AndroidNotificationManager>.Instance;
		instance.OnNotificationIdLoaded = (Action<int>)Delegate.Combine(instance.OnNotificationIdLoaded, new Action<int>(OnNotificationIdLoaded));
		Singleton<AndroidNotificationManager>.Instance.LocadAppLaunchNotificationId();
	}

	private void CanselLocal()
	{
		Singleton<AndroidNotificationManager>.Instance.CancelLocalNotification(LastNotificationId);
	}

	private void CancelAll()
	{
		Singleton<AndroidNotificationManager>.Instance.CancelAllLocalNotifications();
	}

	private void Reg()
	{
		Singleton<GoogleCloudMessageService>.Instance.RgisterDevice();
	}

	private void LoadLastMessage()
	{
		Singleton<GoogleCloudMessageService>.Instance.LoadLastMessage();
	}

	private void HandleActionGCMPushReceived(string message, Dictionary<string, object> data)
	{
		UnityEngine.Debug.Log("[HandleActionGCMPushReceived]");
		UnityEngine.Debug.Log("Message: " + message);
		foreach (KeyValuePair<string, object> datum in data)
		{
			UnityEngine.Debug.Log("Data Entity: " + datum.Key + " " + datum.Value.ToString());
		}
		AN_PoupsProxy.showMessage(message, Json.Serialize(data));
	}

	private void HandleActionGCMPushLaunched(string message, Dictionary<string, object> data)
	{
		UnityEngine.Debug.Log("[HandleActionGCMPushLaunched]");
		UnityEngine.Debug.Log("Message: " + message);
		foreach (KeyValuePair<string, object> datum in data)
		{
			UnityEngine.Debug.Log("Data Entity: " + datum.Key + " " + datum.Value.ToString());
		}
		AN_PoupsProxy.showMessage(message, Json.Serialize(data));
	}

	private void HandleActionCMDRegistrationResult(GP_GCM_RegistrationResult res)
	{
		if (res.IsSucceeded)
		{
			AN_PoupsProxy.showMessage("Regstred", "GCM REG ID: " + Singleton<GoogleCloudMessageService>.Instance.registrationId);
		}
		else
		{
			AN_PoupsProxy.showMessage("Reg Failed", "GCM Registration failed :(");
		}
	}

	private void OnNotificationIdLoaded(int notificationid)
	{
		AN_PoupsProxy.showMessage("Loaded", "App was laucnhed with notification id: " + notificationid);
	}

	private void OnMessageLoaded(string msg)
	{
		AN_PoupsProxy.showMessage("Message Loaded", "Last GCM Message: " + Singleton<GoogleCloudMessageService>.Instance.lastMessage);
	}
}
