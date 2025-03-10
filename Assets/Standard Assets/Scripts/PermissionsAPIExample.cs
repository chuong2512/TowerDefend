using SA.Common.Pattern;
using System.Collections.Generic;
using UnityEngine;

public class PermissionsAPIExample : MonoBehaviour
{
	private void Awake()
	{
		PermissionsManager.ActionPermissionsRequestCompleted += HandleActionPermissionsRequestCompleted;
	}

	public void CheckPermission()
	{
		UnityEngine.Debug.Log("CheckPermission");
		bool flag = PermissionsManager.IsPermissionGranted(AN_Permission.WRITE_EXTERNAL_STORAGE);
		UnityEngine.Debug.Log(flag);
		flag = PermissionsManager.IsPermissionGranted(AN_Permission.INTERNET);
		UnityEngine.Debug.Log(flag);
		CheckShouldRequestPermission();
	}

	public void RequestPermission()
	{
		Singleton<PermissionsManager>.Instance.RequestPermissions(AN_Permission.WRITE_EXTERNAL_STORAGE, AN_Permission.CAMERA);
	}

	public void CheckShouldRequestPermission()
	{
		UnityEngine.Debug.Log("CheckShouldRequestPermission: " + PermissionsManager.ShouldShowRequestPermission(AN_Permission.WRITE_EXTERNAL_STORAGE).ToString());
	}

	private void HandleActionPermissionsRequestCompleted(AN_GrantPermissionsResult res)
	{
		UnityEngine.Debug.Log("HandleActionPermissionsRequestCompleted");
		foreach (KeyValuePair<AN_Permission, AN_PermissionState> item in res.RequestedPermissionsState)
		{
			UnityEngine.Debug.Log(item.Key.GetFullName() + " / " + item.Value.ToString());
		}
	}
}
