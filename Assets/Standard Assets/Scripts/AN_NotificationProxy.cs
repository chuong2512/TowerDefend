using System;
using UnityEngine;

public class AN_NotificationProxy
{
	private const string CLASS_NAME = "com.androidnative.features.notifications.LocalNotificationManager";

	private static void CallActivityFunction(string methodName, params object[] args)
	{
		AN_ProxyPool.CallStatic("com.androidnative.features.notifications.LocalNotificationManager", methodName, args);
	}

	public static void ShowToastNotification(string text, int duration)
	{
		CallActivityFunction("ShowToastNotification", text, duration.ToString());
	}

	public static void HideAllNotifications()
	{
		CallActivityFunction("HideAllNotifications");
	}

	public static void requestCurrentAppLaunchNotificationId()
	{
		CallActivityFunction("requestCurrentAppLaunchNotificationId");
	}

	public static void ScheduleLocalNotification(AndroidNotificationBuilder builder)
	{
		object[] obj = new object[14]
		{
			builder.Title,
			builder.Message,
			builder.Time.ToString(),
			builder.Id.ToString(),
			builder.Icon,
			builder.Sound,
			builder.Vibration.ToString(),
			builder.ShowIfAppForeground.ToString(),
			builder.Repeating,
			builder.RepeatDelay,
			builder.LargeIcon,
			(!(builder.BigPicture == null)) ? Convert.ToBase64String(builder.BigPicture.EncodeToPNG()) : string.Empty,
			null,
			null
		};
		object text;
		if (builder.Color == null)
		{
			text = string.Empty;
		}
		else
		{
			object[] array = new object[4];
			Color value = builder.Color.Value;
			array[0] = 255f * value.a;
			Color value2 = builder.Color.Value;
			array[1] = 255f * value2.r;
			Color value3 = builder.Color.Value;
			array[2] = 255f * value3.g;
			Color value4 = builder.Color.Value;
			array[3] = 255f * value4.b;
			text = string.Format("{0}|{1}|{2}|{3}", array);
		}
		obj[12] = text;
		obj[13] = builder.WakeLockTime;
		CallActivityFunction("ScheduleLocalNotification", obj);
	}

	public static void CanselLocalNotification(int id)
	{
		CallActivityFunction("canselLocalNotification", id.ToString());
	}

	public static void InitPushNotifications(string smallIcon, string largeIcon, string sound, bool vibration, bool showWhenAppForeground, bool replaceOldNotificationWithNew, string color)
	{
		CallActivityFunction("InitPushNotifications", smallIcon, largeIcon, sound, vibration.ToString(), showWhenAppForeground.ToString(), replaceOldNotificationWithNew.ToString(), color);
	}

	public static void GCMRgisterDevice(string senderId)
	{
		CallActivityFunction("GCMRgisterDevice", senderId);
	}

	public static void GCMLoadLastMessage()
	{
		CallActivityFunction("GCMLoadLastMessage");
	}

	public static void GCMRemoveLastMessageInfo()
	{
		CallActivityFunction("GCMRemoveLastMessageInfo");
	}
}
