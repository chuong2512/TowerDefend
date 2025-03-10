using SA.Common.Pattern;
using System;
using UnityEngine;

public class PreviewSceneController : MonoBehaviour
{
	public SA_Label title;

	private void Awake()
	{
		title.text = "Android Native Unity3d Plugin (9.1)";
	}

	private void Start()
	{
		Singleton<ImmersiveMode>.Instance.EnableImmersiveMode();
	}

	public void SendMail()
	{
		AndroidSocialGate.SendMail("Send Mail", string.Empty, "Android Native Plugin Question", "stans.assets@gmail.com");
	}

	public void SendBug()
	{
		AN_LicenseManager.OnLicenseRequestResult = (Action<AN_LicenseRequestResult>)Delegate.Combine(AN_LicenseManager.OnLicenseRequestResult, new Action<AN_LicenseRequestResult>(LicenseRequestResult));
		Singleton<AN_LicenseManager>.Instance.StartLicenseRequest();
	}

	private void LicenseRequestResult(AN_LicenseRequestResult result)
	{
		UnityEngine.Debug.Log("LicenseRequestResult " + result.ToString());
	}

	public void OpenDocs()
	{
		string url = "http://goo.gl/pTcIR8";
		Application.OpenURL(url);
	}

	public void OpenAssetStore()
	{
		string url = "http://goo.gl/g8LWlC";
		Application.OpenURL(url);
	}

	public void MorePlugins()
	{
		string url = "http://goo.gl/MgEirV";
		Application.OpenURL(url);
	}
}
