using SA.Common.Models;
using System;
using System.Collections.Generic;

public class AN_GrantPermissionsResult : Result
{
	private Dictionary<AN_Permission, AN_PermissionState> _RequestedPermissionsState = new Dictionary<AN_Permission, AN_PermissionState>();

	public Dictionary<AN_Permission, AN_PermissionState> RequestedPermissionsState => _RequestedPermissionsState;

	public AN_GrantPermissionsResult(string[] permissionsList, string[] resultsList)
	{
		int num = 0;
		foreach (string fullName in permissionsList)
		{
			AN_Permission permissionByName = PermissionsManager.GetPermissionByName(fullName);
			int value = Convert.ToInt32(resultsList[num]);
			_RequestedPermissionsState.Add(permissionByName, (AN_PermissionState)value);
			num++;
		}
	}
}
