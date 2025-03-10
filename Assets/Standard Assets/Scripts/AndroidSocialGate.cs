using System;
using System.Threading;
using UnityEngine;

public class AndroidSocialGate : MonoBehaviour
{
	private static AndroidSocialGate _Instance = null;

	public static event Action<bool, string> OnShareIntentCallback;

	public static void StartGooglePlusShare(string text, Texture2D texture = null)
	{
		CheckAndCreateInstance();
		AN_SocialSharingProxy.StartGooglePlusShareIntent(text, (!(texture == null)) ? Convert.ToBase64String(texture.EncodeToPNG()) : string.Empty);
	}

	public static void StartShareIntent(string caption, string message, string packageNamePattern = "")
	{
		CheckAndCreateInstance();
		StartShareIntentWithSubject(caption, message, string.Empty, packageNamePattern);
	}

	public static void StartShareIntent(string caption, string message, Texture2D texture, string packageNamePattern = "")
	{
		CheckAndCreateInstance();
		StartShareIntentWithSubject(caption, message, string.Empty, texture, packageNamePattern);
	}

	public static void StartShareIntentWithSubject(string caption, string message, string subject, string packageNamePattern = "")
	{
		CheckAndCreateInstance();
		AN_SocialSharingProxy.StartShareIntent(caption, message, subject, packageNamePattern);
	}

	public static void StartShareIntentWithSubject(string caption, string message, string subject, Texture2D texture, string packageNamePattern = "")
	{
		CheckAndCreateInstance();
		byte[] inArray = texture.EncodeToPNG();
		string media = Convert.ToBase64String(inArray);
		AN_SocialSharingProxy.StartShareIntent(caption, message, subject, media, packageNamePattern, (int)AndroidNativeSettings.Instance.ImageFormat, AndroidNativeSettings.Instance.SaveCameraImageToGallery);
	}

	public static void SendMail(string caption, string message, string subject, string recipients, Texture2D texture = null)
	{
		CheckAndCreateInstance();
		if (texture != null)
		{
			byte[] inArray = texture.EncodeToPNG();
			string media = Convert.ToBase64String(inArray);
			AN_SocialSharingProxy.SendMailWithImage(caption, message, subject, recipients, media, (int)AndroidNativeSettings.Instance.ImageFormat, AndroidNativeSettings.Instance.SaveCameraImageToGallery);
		}
		else
		{
			AN_SocialSharingProxy.SendMail(caption, message, subject, recipients);
		}
	}

	public static void ShareTwitterGif(string gifPath, string message)
	{
		AN_SocialSharingProxy.ShareTwitterGif(gifPath, message);
	}

	private static void CheckAndCreateInstance()
	{
		if (_Instance == null)
		{
			_Instance = (UnityEngine.Object.FindObjectOfType(typeof(AndroidSocialGate)) as AndroidSocialGate);
			if (_Instance == null)
			{
				_Instance = new GameObject().AddComponent<AndroidSocialGate>();
				_Instance.gameObject.name = _Instance.GetType().Name;
			}
		}
	}

	private void ShareCallback(string data)
	{
		string[] array = data.Split(new string[1]
		{
			"|"
		}, StringSplitOptions.None);
		bool flag = int.Parse(array[1]) == -1;
		AndroidSocialGate.OnShareIntentCallback(flag, array[0]);
		UnityEngine.Debug.Log("[AndroidSocialGate]ShareCallback Posted:" + flag + " Package:" + array[0]);
	}

	static AndroidSocialGate()
	{
		AndroidSocialGate.OnShareIntentCallback = delegate
		{
		};
	}
}
