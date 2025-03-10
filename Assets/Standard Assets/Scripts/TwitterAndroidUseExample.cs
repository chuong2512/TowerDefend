using SA.Common.Pattern;
using System.Collections;
using UnityEngine;

public class TwitterAndroidUseExample : MonoBehaviour
{
	private static bool IsUserInfoLoaded;

	private static bool IsAuthenticated;

	public Texture2D ImageToShare;

	public DefaultPreviewButton connectButton;

	public SA_Texture avatar;

	public SA_Label Location;

	public SA_Label Language;

	public SA_Label Status;

	public SA_Label Name;

	public DefaultPreviewButton[] AuthDependedButtons;

	private void Awake()
	{
		Singleton<AndroidTwitterManager>.Instance.OnTwitterInitedAction += OnTwitterInitedAction;
		Singleton<AndroidTwitterManager>.Instance.OnPostingCompleteAction += OnPostingCompleteAction;
		Singleton<AndroidTwitterManager>.Instance.OnUserDataRequestCompleteAction += OnUserDataRequestCompleteAction;
		Singleton<AndroidTwitterManager>.Instance.OnAuthCompleteAction += OnAuthCompleteAction;
		Singleton<AndroidTwitterManager>.Instance.Init();
	}

	private void FixedUpdate()
	{
		if (IsAuthenticated)
		{
			connectButton.text = "Disconnect";
			Name.text = "Player Connected";
			DefaultPreviewButton[] authDependedButtons = AuthDependedButtons;
			foreach (DefaultPreviewButton defaultPreviewButton in authDependedButtons)
			{
				defaultPreviewButton.EnabledButton();
			}
			if (IsUserInfoLoaded)
			{
				if (Singleton<AndroidTwitterManager>.Instance.userInfo.profile_image != null)
				{
					avatar.texture = Singleton<AndroidTwitterManager>.Instance.userInfo.profile_image;
				}
				Name.text = Singleton<AndroidTwitterManager>.Instance.userInfo.name + " aka " + Singleton<AndroidTwitterManager>.Instance.userInfo.screen_name;
				Location.text = Singleton<AndroidTwitterManager>.Instance.userInfo.location;
				Language.text = Singleton<AndroidTwitterManager>.Instance.userInfo.lang;
				Status.text = Singleton<AndroidTwitterManager>.Instance.userInfo.status.text;
			}
		}
		else
		{
			DefaultPreviewButton[] authDependedButtons2 = AuthDependedButtons;
			foreach (DefaultPreviewButton defaultPreviewButton2 in authDependedButtons2)
			{
				defaultPreviewButton2.DisabledButton();
			}
			connectButton.text = "Connect";
			Name.text = "Player Disconnected";
		}
	}

	private void Connect()
	{
		if (!IsAuthenticated)
		{
			Singleton<AndroidTwitterManager>.Instance.AuthenticateUser();
		}
		else
		{
			LogOut();
		}
	}

	private void PostWithAuthCheck()
	{
		Singleton<AndroidTwitterManager>.Instance.PostWithAuthCheck("Hello, I'am posting this from my app");
	}

	private void PostNativeScreenshot()
	{
		StartCoroutine(PostTWScreenshot());
	}

	private void PostMSG()
	{
		AndroidSocialGate.StartShareIntent("Hello Share Intent", "This is my text to share", "twi");
	}

	private void PostImage()
	{
		AndroidSocialGate.StartShareIntent("Hello Share Intent", "This is my text to share", ImageToShare, "twi");
	}

	private IEnumerator PostTWScreenshot()
	{
		yield return new WaitForEndOfFrame();
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, mipChain: false);
		tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		tex.Apply();
		AndroidSocialGate.StartShareIntent("Hello Share Intent", "This is my text to share", tex, "twi");
		UnityEngine.Object.Destroy(tex);
	}

	private void LoadUserData()
	{
		Singleton<AndroidTwitterManager>.Instance.LoadUserData();
	}

	private void Test()
	{
		TW_OAuthAPIRequest tW_OAuthAPIRequest = TW_OAuthAPIRequest.Create();
		tW_OAuthAPIRequest.Send("https://api.twitter.com/1.1/statuses/home_timeline.json");
		tW_OAuthAPIRequest.OnResult += OnResult;
	}

	private void OnResult(TW_APIRequstResult result)
	{
		UnityEngine.Debug.Log("Is Request Succeeded: " + result.IsSucceeded);
		UnityEngine.Debug.Log("Responce data:");
		UnityEngine.Debug.Log(result.responce);
	}

	private void PostMessage()
	{
		Singleton<AndroidTwitterManager>.Instance.Post("Hello, I'am posting this from my app");
	}

	private void PostScreehShot()
	{
		StartCoroutine(PostScreenshot());
	}

	private void OnUserDataRequestCompleteAction(TWResult result)
	{
		if (result.IsSucceeded)
		{
			IsUserInfoLoaded = true;
			Singleton<AndroidTwitterManager>.Instance.userInfo.LoadProfileImage();
			Singleton<AndroidTwitterManager>.Instance.userInfo.LoadBackgroundImage();
		}
		else
		{
			UnityEngine.Debug.Log("Opps, user data load failed, something was wrong");
		}
	}

	private void OnPostingCompleteAction(TWResult result)
	{
		if (result.IsSucceeded)
		{
			UnityEngine.Debug.Log("Congrats. You just posted something to Twitter!");
		}
		else
		{
			UnityEngine.Debug.Log("Oops! Posting failed. Something went wrong.");
		}
	}

	private void OnAuthCompleteAction(TWResult result)
	{
		if (result.IsSucceeded)
		{
			OnAuth();
		}
	}

	private void OnTwitterInitedAction(TWResult result)
	{
		if (Singleton<AndroidTwitterManager>.Instance.IsAuthed)
		{
			OnAuth();
		}
	}

	private void OnAuth()
	{
		IsAuthenticated = true;
	}

	private void RetrieveTimeLine()
	{
		TW_UserTimeLineRequest tW_UserTimeLineRequest = TW_UserTimeLineRequest.Create();
		tW_UserTimeLineRequest.ActionComplete += OnTimeLineRequestComplete;
		tW_UserTimeLineRequest.AddParam("screen_name", "unity3d");
		tW_UserTimeLineRequest.AddParam("count", "1");
		tW_UserTimeLineRequest.Send();
	}

	private void UserLookUpRequest()
	{
		TW_UsersLookUpRequest tW_UsersLookUpRequest = TW_UsersLookUpRequest.Create();
		tW_UsersLookUpRequest.ActionComplete += OnLookUpRequestComplete;
		tW_UsersLookUpRequest.AddParam("screen_name", "unity3d");
		tW_UsersLookUpRequest.Send();
	}

	private void FriedsidsRequest()
	{
		TW_FriendsIdsRequest tW_FriendsIdsRequest = TW_FriendsIdsRequest.Create();
		tW_FriendsIdsRequest.ActionComplete += OnIdsLoaded;
		tW_FriendsIdsRequest.AddParam("screen_name", "unity3d");
		tW_FriendsIdsRequest.Send();
	}

	private void FollowersidsRequest()
	{
		TW_FollowersIdsRequest tW_FollowersIdsRequest = TW_FollowersIdsRequest.Create();
		tW_FollowersIdsRequest.ActionComplete += OnIdsLoaded;
		tW_FollowersIdsRequest.AddParam("screen_name", "unity3d");
		tW_FollowersIdsRequest.Send();
	}

	private void TweetSearch()
	{
		TW_SearchTweetsRequest tW_SearchTweetsRequest = TW_SearchTweetsRequest.Create();
		tW_SearchTweetsRequest.ActionComplete += OnSearchRequestComplete;
		tW_SearchTweetsRequest.AddParam("q", "@noradio");
		tW_SearchTweetsRequest.AddParam("count", "1");
		tW_SearchTweetsRequest.Send();
	}

	private void OnIdsLoaded(TW_APIRequstResult result)
	{
		if (result.IsSucceeded)
		{
			AN_PoupsProxy.showMessage("Ids Request Succeeded", "Totals ids loaded: " + result.ids.Count);
			UnityEngine.Debug.Log(result.ids.Count);
		}
		else
		{
			UnityEngine.Debug.Log(result.responce);
			AN_PoupsProxy.showMessage("Ids Request Failed", result.responce);
		}
	}

	private void OnLookUpRequestComplete(TW_APIRequstResult result)
	{
		if (result.IsSucceeded)
		{
			string str = "User Id: ";
			str += result.users[0].id;
			str += "\n";
			str = str + "User Name:" + result.users[0].name;
			AN_PoupsProxy.showMessage("User Info Loaded", str);
			UnityEngine.Debug.Log(str);
		}
		else
		{
			UnityEngine.Debug.Log(result.responce);
			AN_PoupsProxy.showMessage("User Info Failed", result.responce);
		}
	}

	private void OnSearchRequestComplete(TW_APIRequstResult result)
	{
		if (result.IsSucceeded)
		{
			string str = "Tweet text:\n";
			str += result.tweets[0].text;
			AN_PoupsProxy.showMessage("Tweet Search Request Succeeded", str);
			UnityEngine.Debug.Log(str);
		}
		else
		{
			UnityEngine.Debug.Log(result.responce);
			AN_PoupsProxy.showMessage("Tweet Search Request Failed", result.responce);
		}
	}

	private void OnTimeLineRequestComplete(TW_APIRequstResult result)
	{
		if (result.IsSucceeded)
		{
			string str = "Last Tweet text:\n";
			str += result.tweets[0].text;
			AN_PoupsProxy.showMessage("Time Line Request Succeeded", str);
			UnityEngine.Debug.Log(str);
		}
		else
		{
			UnityEngine.Debug.Log(result.responce);
			AN_PoupsProxy.showMessage("Time Line Request Failed", result.responce);
		}
	}

	private IEnumerator PostScreenshot()
	{
		yield return new WaitForEndOfFrame();
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, mipChain: false);
		tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		tex.Apply();
		Singleton<AndroidTwitterManager>.Instance.Post("My app ScreehShot", tex);
		UnityEngine.Object.Destroy(tex);
	}

	private void LogOut()
	{
		IsUserInfoLoaded = false;
		IsAuthenticated = false;
		Singleton<AndroidTwitterManager>.Instance.LogOut();
	}
}
