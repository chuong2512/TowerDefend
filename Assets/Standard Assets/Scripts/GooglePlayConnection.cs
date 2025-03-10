using SA.Common.Pattern;
using System;
using System.Threading;
using UnityEngine;

public class GooglePlayConnection : Singleton<GooglePlayConnection>
{
	private bool _IsInitialized;

	private static GPConnectionState _State;

	public bool IsConnected => State == GPConnectionState.STATE_CONNECTED;

	[Obsolete("state is deprecated, please use State instead.")]
	public static GPConnectionState state => State;

	public static GPConnectionState State => _State;

	[Obsolete("isInitialized is deprecated, please use IsInitialized instead.")]
	public bool isInitialized => IsInitialized;

	public bool IsInitialized => _IsInitialized;

	public static event Action<GooglePlayConnectionResult> ActionConnectionResultReceived;

	public static event Action<GPConnectionState> ActionConnectionStateChanged;

	public static event Action ActionPlayerConnected;

	public static event Action ActionPlayerDisconnected;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Singleton<GooglePlayManager>.Instance.Create();
		Init();
	}

	private void Init()
	{
		string text = string.Empty;
		if (AndroidNativeSettings.Instance.EnableGamesAPI)
		{
			text += "GamesAPI";
		}
		if (AndroidNativeSettings.Instance.EnablePlusAPI)
		{
			text += "PlusAPI";
		}
		if (AndroidNativeSettings.Instance.EnableDriveAPI)
		{
			text += "DriveAPI";
		}
		if (AndroidNativeSettings.Instance.EnableAppInviteAPI)
		{
			text += "AppInvite";
		}
		AN_GMSGeneralProxy.setConnectionParams(AndroidNativeSettings.Instance.ShowConnectingPopup);
		AN_GMSGeneralProxy.playServiceInit(text);
	}

	[Obsolete("connect is deprecated, please use Connect instead.")]
	public void connect()
	{
		Connect();
	}

	public void Connect()
	{
		Connect(null);
	}

	[Obsolete("connect is deprecated, please use Connect instead.")]
	public void connect(string accountName)
	{
		Connect(accountName);
	}

	public void Connect(string accountName)
	{
		if (_State != GPConnectionState.STATE_CONNECTED && _State != GPConnectionState.STATE_CONNECTING)
		{
			OnStateChange(GPConnectionState.STATE_CONNECTING);
			if (accountName != null)
			{
				AN_GMSGeneralProxy.playServiceConnect(accountName);
			}
			else
			{
				AN_GMSGeneralProxy.playServiceConnect();
			}
		}
	}

	[Obsolete("disconnect is deprecated, please use Disconnect instead.")]
	public void disconnect()
	{
		Disconnect();
	}

	public void Disconnect()
	{
		if (_State != GPConnectionState.STATE_DISCONNECTED && _State != GPConnectionState.STATE_CONNECTING)
		{
			OnStateChange(GPConnectionState.STATE_DISCONNECTED);
			AN_GMSGeneralProxy.playServiceDisconnect();
		}
	}

	public static bool CheckState()
	{
		GPConnectionState state = _State;
		if (state == GPConnectionState.STATE_CONNECTED)
		{
			return true;
		}
		return false;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		AN_GMSGeneralProxy.OnApplicationPause(pauseStatus);
	}

	private void OnPlayServiceDisconnected(string data)
	{
		OnStateChange(GPConnectionState.STATE_DISCONNECTED);
	}

	private void OnConnectionResult(string resultCode)
	{
		UnityEngine.Debug.Log("[OnConnectionResult] resultCode " + resultCode);
		GooglePlayConnectionResult googlePlayConnectionResult = new GooglePlayConnectionResult();
		googlePlayConnectionResult.code = (GP_ConnectionResultCode)Convert.ToInt32(resultCode);
		if (googlePlayConnectionResult.IsSuccess)
		{
			OnStateChange(GPConnectionState.STATE_CONNECTED);
		}
		else
		{
			OnStateChange(GPConnectionState.STATE_DISCONNECTED);
		}
		GooglePlayConnection.ActionConnectionResultReceived(googlePlayConnectionResult);
	}

	private void OnStateChange(GPConnectionState connectionState)
	{
		_State = connectionState;
		switch (_State)
		{
		case GPConnectionState.STATE_CONNECTED:
			GooglePlayConnection.ActionPlayerConnected();
			break;
		case GPConnectionState.STATE_DISCONNECTED:
			GooglePlayConnection.ActionPlayerDisconnected();
			break;
		}
		GooglePlayConnection.ActionConnectionStateChanged(_State);
		UnityEngine.Debug.Log("Play Serice Connection State -> " + _State.ToString());
	}

	static GooglePlayConnection()
	{
		GooglePlayConnection.ActionConnectionResultReceived = delegate
		{
		};
		GooglePlayConnection.ActionConnectionStateChanged = delegate
		{
		};
		GooglePlayConnection.ActionPlayerConnected = delegate
		{
		};
		GooglePlayConnection.ActionPlayerDisconnected = delegate
		{
		};
		_State = GPConnectionState.STATE_UNCONFIGURED;
	}
}
