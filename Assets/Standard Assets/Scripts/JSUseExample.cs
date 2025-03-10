using System;
using UnityEngine;

[Serializable]
public class JSUseExample : MonoBehaviour
{
	public void OnGUI()
	{
		if (GUI.Button(new Rect(10f, 70f, 200f, 70f), "Connect"))
		{
			gameObject.SendMessage("ConnectToPlaySertivce");
		}
	}

	public void PlayerConnectd()
	{
		UnityEngine.Debug.Log("Player Connected Event received");
	}

	public void PlayerDisconected()
	{
		UnityEngine.Debug.Log("Player Disconected Event received");
	}

	public void Main()
	{
	}
}
