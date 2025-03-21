using UnityEngine;

public class AndroidNativeExampleBase : MonoBehaviour
{
	public virtual void Awake()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			UnityEngine.Debug.LogWarning("The Android Native Example Scene will only work on Real Android Device");
		}
	}
}
