using SA.Common.Pattern;
using UnityEngine;

public class GoogleLogin : MonoBehaviour
{
	private void Awake()
	{
		Singleton<GooglePlayConnection>.Instance.Connect();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void BxhBtn()
	{
		Singleton<GooglePlayManager>.Instance.ShowLeaderBoardsUI();
	}

	public void ConnectBtn()
	{
		Singleton<GooglePlayConnection>.Instance.Connect();
	}
}
