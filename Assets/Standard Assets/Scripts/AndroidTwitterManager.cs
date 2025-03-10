using SA.Common.Pattern;
using System;
using System.Reflection;
using System.Threading;
using UnityEngine;

public class AndroidTwitterManager : Singleton<AndroidTwitterManager>, TwitterManagerInterface
{
	private bool _IsAuthed;

	private bool _IsInited;

	private string _AccessToken = string.Empty;

	private string _AccessTokenSecret = string.Empty;

	private TwitterUserInfo _userInfo;

	public bool IsAuthed => _IsAuthed;

	public bool IsInited => _IsInited;

	public TwitterUserInfo userInfo => _userInfo;

	public string AccessToken => _AccessToken;

	public string AccessTokenSecret => _AccessTokenSecret;

	public event Action OnTwitterLoginStarted;

	public event Action OnTwitterLogOut;

	public event Action OnTwitterPostStarted;

	public event Action<TWResult> OnTwitterInitedAction;

	public event Action<TWResult> OnAuthCompleteAction;

	public event Action<TWResult> OnPostingCompleteAction;

	public event Action<TWResult> OnUserDataRequestCompleteAction;

	public AndroidTwitterManager()
	{
		this.OnTwitterLoginStarted = delegate
		{
		};
		this.OnTwitterLogOut = delegate
		{
		};
		this.OnTwitterPostStarted = delegate
		{
		};
		this.OnTwitterInitedAction = delegate
		{
		};
		this.OnAuthCompleteAction = delegate
		{
		};
		this.OnPostingCompleteAction = delegate
		{
		};
		this.OnUserDataRequestCompleteAction = delegate
		{
		};
		
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void Init()
	{
		try
		{
			Type type = Type.GetType("AN_SoomlaGrow");
			MethodInfo method = type.GetMethod("Init", BindingFlags.Static | BindingFlags.Public);
			method.Invoke(null, null);
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError("AndroidNative: Soomla Initalization failed" + ex.Message);
		}
		Init(SocialPlatfromSettings.Instance.TWITTER_CONSUMER_KEY, SocialPlatfromSettings.Instance.TWITTER_CONSUMER_SECRET);
	}

	public void Init(string consumer_key, string consumer_secret)
	{
		if (!_IsInited)
		{
			_IsInited = true;
			AndroidNative.TwitterInit(consumer_key, consumer_secret);
		}
	}

	public void AuthenticateUser()
	{
		this.OnTwitterLoginStarted();
		if (_IsAuthed)
		{
			OnAuthSuccess();
		}
		else
		{
			AndroidNative.AuthificateUser();
		}
	}

	public void LoadUserData()
	{
		if (_IsAuthed)
		{
			AndroidNative.LoadUserData();
			return;
		}
		UnityEngine.Debug.LogWarning("Auth user before loadin data, fail event generated");
		TWResult obj = new TWResult(IsResSucceeded: false, null);
		this.OnUserDataRequestCompleteAction(obj);
	}

	public void Post(string status)
	{
		this.OnTwitterPostStarted();
		if (!_IsAuthed)
		{
			UnityEngine.Debug.LogWarning("Auth user before posting data, fail event generated");
			TWResult obj = new TWResult(IsResSucceeded: false, null);
			this.OnPostingCompleteAction(obj);
		}
		else
		{
			AndroidNative.TwitterPost(status);
		}
	}

	public void Post(string status, Texture2D texture)
	{
		this.OnTwitterPostStarted();
		if (!_IsAuthed)
		{
			UnityEngine.Debug.LogWarning("Auth user before posting data, fail event generated");
			TWResult obj = new TWResult(IsResSucceeded: false, null);
			this.OnPostingCompleteAction(obj);
		}
		else
		{
			byte[] inArray = texture.EncodeToPNG();
			string data = Convert.ToBase64String(inArray);
			AndroidNative.TwitterPostWithImage(status, data);
		}
	}

	public TwitterPostingTask PostWithAuthCheck(string status)
	{
		return PostWithAuthCheck(status, null);
	}

	public TwitterPostingTask PostWithAuthCheck(string status, Texture2D texture)
	{
		TwitterPostingTask twitterPostingTask = TwitterPostingTask.Cretae();
		twitterPostingTask.Post(status, texture, this);
		return twitterPostingTask;
	}

	public void LogOut()
	{
		this.OnTwitterLogOut();
		_IsAuthed = false;
		AndroidNative.LogoutFromTwitter();
	}

	private void OnInited(string data)
	{
		if (data.Equals("1"))
		{
			_IsAuthed = true;
		}
		TWResult obj = new TWResult(IsResSucceeded: true, null);
		this.OnTwitterInitedAction(obj);
	}

	private void OnAuthSuccess()
	{
		_IsAuthed = true;
		TWResult obj = new TWResult(IsResSucceeded: true, null);
		this.OnAuthCompleteAction(obj);
	}

	private void OnAuthFailed()
	{
		TWResult obj = new TWResult(IsResSucceeded: false, null);
		this.OnAuthCompleteAction(obj);
	}

	private void OnPostSuccess()
	{
		TWResult obj = new TWResult(IsResSucceeded: true, null);
		this.OnPostingCompleteAction(obj);
	}

	private void OnPostFailed()
	{
		TWResult obj = new TWResult(IsResSucceeded: false, null);
		this.OnPostingCompleteAction(obj);
	}

	private void OnUserDataLoaded(string data)
	{
		_userInfo = new TwitterUserInfo(data);
		TWResult obj = new TWResult(IsResSucceeded: true, data);
		this.OnUserDataRequestCompleteAction(obj);
	}

	private void OnUserDataLoadFailed()
	{
		TWResult obj = new TWResult(IsResSucceeded: false, null);
		this.OnUserDataRequestCompleteAction(obj);
	}

	private void OnAuthInfoReceived(string data)
	{
		UnityEngine.Debug.Log("OnAuthInfoReceived");
		string[] array = data.Split("|"[0]);
		_AccessToken = array[0];
		_AccessTokenSecret = array[1];
	}
}
