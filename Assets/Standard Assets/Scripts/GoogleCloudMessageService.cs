using ANMiniJSON;
using SA.Common.Pattern;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GoogleCloudMessageService : Singleton<GoogleCloudMessageService>
{
	private string _lastMessage = string.Empty;

	private string _registrationId = string.Empty;

	public string registrationId => _registrationId;

	public string lastMessage => _lastMessage;

	public static event Action<string> ActionCouldMessageLoaded;

	public static event Action<GP_GCM_RegistrationResult> ActionCMDRegistrationResult;

	public static event Action<string, Dictionary<string, object>> ActionGCMPushLaunched;

	public static event Action<string, Dictionary<string, object>> ActionGCMPushReceived;

	public static event Action<string, Dictionary<string, object>> ActionParsePushReceived;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void Init()
	{
		switch (AndroidNativeSettings.Instance.PushService)
		{
		case AN_PushNotificationService.Google:
			InitPushNotifications();
			break;
		case AN_PushNotificationService.OneSignal:
			InitOneSignalNotifications();
			break;
		case AN_PushNotificationService.Parse:
			InitParsePushNotifications();
			break;
		}
	}

	public void InitOneSignalNotifications()
	{
	}

	public void InitPushNotifications()
	{
		AN_NotificationProxy.InitPushNotifications((!(AndroidNativeSettings.Instance.PushNotificationSmallIcon == null)) ? AndroidNativeSettings.Instance.PushNotificationSmallIcon.name.ToLower() : string.Empty, (!(AndroidNativeSettings.Instance.PushNotificationLargeIcon == null)) ? AndroidNativeSettings.Instance.PushNotificationLargeIcon.name.ToLower() : string.Empty, (!(AndroidNativeSettings.Instance.PushNotificationSound == null)) ? AndroidNativeSettings.Instance.PushNotificationSound.name : string.Empty, AndroidNativeSettings.Instance.EnableVibrationPush, AndroidNativeSettings.Instance.ShowPushWhenAppIsForeground, AndroidNativeSettings.Instance.ReplaceOldNotificationWithNew, $"{255f * AndroidNativeSettings.Instance.PushNotificationColor.a}|{255f * AndroidNativeSettings.Instance.PushNotificationColor.r}|{255f * AndroidNativeSettings.Instance.PushNotificationColor.g}|{255f * AndroidNativeSettings.Instance.PushNotificationColor.b}");
	}

	public void InitPushNotifications(string smallIcon, string largeIcon, string sound, bool enableVibrationPush, bool showWhenAppForeground, bool replaceOldNotificationWithNew, string color)
	{
		AN_NotificationProxy.InitPushNotifications(smallIcon, largeIcon, sound, enableVibrationPush, showWhenAppForeground, replaceOldNotificationWithNew, color);
	}

	public void InitParsePushNotifications()
	{
		ParsePushesStub.InitParse();
		ParsePushesStub.OnPushReceived += HandleOnPushReceived;
	}

	public void RgisterDevice()
	{
		AN_NotificationProxy.GCMRgisterDevice(AndroidNativeSettings.Instance.GCM_SenderId);
	}

	public void LoadLastMessage()
	{
		AN_NotificationProxy.GCMLoadLastMessage();
	}

	public void RemoveLastMessageInfo()
	{
		AN_NotificationProxy.GCMRemoveLastMessageInfo();
	}

	private void HandleOnPushReceived(string stringPayload, Dictionary<string, object> payload)
	{
		GoogleCloudMessageService.ActionParsePushReceived(stringPayload, payload);
	}

	private void GCMNotificationCallback(string data)
	{
		UnityEngine.Debug.Log("[GCMNotificationCallback] JSON Data: " + data);
		string[] array = data.Split(new string[1]
		{
			"|"
		}, StringSplitOptions.None);
		string arg = array[0];
		Dictionary<string, object> arg2 = Json.Deserialize(array[1]) as Dictionary<string, object>;
		GoogleCloudMessageService.ActionGCMPushReceived(arg, arg2);
	}

	private void GCMNotificationLaunchedCallback(string data)
	{
		UnityEngine.Debug.Log("[GCMNotificationLaunchedCallback] JSON Data: " + data);
		string[] array = data.Split(new string[1]
		{
			"|"
		}, StringSplitOptions.None);
		string arg = array[0];
		Dictionary<string, object> arg2 = Json.Deserialize(array[1]) as Dictionary<string, object>;
		GoogleCloudMessageService.ActionGCMPushLaunched(arg, arg2);
	}

	private void OnLastMessageLoaded(string data)
	{
		_lastMessage = data;
		GoogleCloudMessageService.ActionCouldMessageLoaded(lastMessage);
	}

	private void OnRegistrationReviced(string regId)
	{
		_registrationId = regId;
		GoogleCloudMessageService.ActionCMDRegistrationResult(new GP_GCM_RegistrationResult(_registrationId));
	}

	private void OnRegistrationFailed()
	{
		GoogleCloudMessageService.ActionCMDRegistrationResult(new GP_GCM_RegistrationResult());
	}

	static GoogleCloudMessageService()
	{
		GoogleCloudMessageService.ActionCouldMessageLoaded = delegate
		{
		};
		GoogleCloudMessageService.ActionCMDRegistrationResult = delegate
		{
		};
		GoogleCloudMessageService.ActionGCMPushLaunched = delegate
		{
		};
		GoogleCloudMessageService.ActionGCMPushReceived = delegate
		{
		};
		GoogleCloudMessageService.ActionParsePushReceived = delegate
		{
		};
	}
}
