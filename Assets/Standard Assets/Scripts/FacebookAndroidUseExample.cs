using SA.Common.Pattern;
using System.Collections;
using UnityEngine;

public class FacebookAndroidUseExample : MonoBehaviour
{
	private static bool IsUserInfoLoaded;

	private static bool IsFrindsInfoLoaded;

	private static bool IsAuntificated;

	public DefaultPreviewButton[] ConnectionDependedntButtons;

	public DefaultPreviewButton connectButton;

	public SA_Texture avatar;

	public SA_Label Location;

	public SA_Label Language;

	public SA_Label Mail;

	public SA_Label Name;

	public SA_Label f1;

	public SA_Label f2;

	public SA_Texture fi1;

	public SA_Texture fi2;

	public Texture2D ImageToShare;

	public GameObject friends;

	private int startScore = 555;

	private string UNION_ASSETS_PAGE_ID = "1435528379999137";

	private void Awake()
	{
		SPFacebook.OnInitCompleteAction += OnInit;
		SPFacebook.OnFocusChangedAction += OnFocusChanged;
		SPFacebook.OnAuthCompleteAction += OnAuth;
		SPFacebook.OnPostingCompleteAction += OnPost;
		SPFacebook.OnPlayerScoresRequestCompleteAction += OnPlayerScoreRequestComplete;
		SPFacebook.OnAppScoresRequestCompleteAction += OnAppScoreRequestComplete;
		SPFacebook.OnSubmitScoreRequestCompleteAction += OnSubmitScoreRequestComplete;
		SPFacebook.OnDeleteScoresRequestCompleteAction += OnDeleteScoreRequestComplete;
		Singleton<SPFacebook>.Instance.Init();
		SA_StatusBar.text = "initializing Facebook";
	}

	private void HandleOnRevokePermission(FB_Result result)
	{
		UnityEngine.Debug.Log("[HandleOnRevokePermission] result.IsSucceeded: " + result.IsSucceeded + " Responce: " + result.RawData);
	}

	private void FixedUpdate()
	{
		if (IsAuntificated)
		{
			connectButton.text = "Disconnect";
			Name.text = "Player Connected";
			DefaultPreviewButton[] connectionDependedntButtons = ConnectionDependedntButtons;
			foreach (DefaultPreviewButton defaultPreviewButton in connectionDependedntButtons)
			{
				defaultPreviewButton.EnabledButton();
			}
			if (IsUserInfoLoaded && Singleton<SPFacebook>.Instance.userInfo.GetProfileImage(FB_ProfileImageSize.square) != null)
			{
				avatar.texture = Singleton<SPFacebook>.Instance.userInfo.GetProfileImage(FB_ProfileImageSize.square);
				Name.text = Singleton<SPFacebook>.Instance.userInfo.Name + " aka " + Singleton<SPFacebook>.Instance.userInfo.UserName;
				Location.text = Singleton<SPFacebook>.Instance.userInfo.Location;
				Language.text = Singleton<SPFacebook>.Instance.userInfo.Locale;
			}
			if (IsFrindsInfoLoaded)
			{
				friends.SetActive(value: true);
				int num = 0;
				if (Singleton<SPFacebook>.Instance.friendsList != null)
				{
					foreach (FB_UserInfo friends2 in Singleton<SPFacebook>.Instance.friendsList)
					{
						if (num == 0)
						{
							f1.text = friends2.Name;
							if (friends2.GetProfileImage(FB_ProfileImageSize.square) != null)
							{
								fi1.texture = friends2.GetProfileImage(FB_ProfileImageSize.square);
							}
						}
						else
						{
							f2.text = friends2.Name;
							if (friends2.GetProfileImage(FB_ProfileImageSize.square) != null)
							{
								fi2.texture = friends2.GetProfileImage(FB_ProfileImageSize.square);
							}
						}
						num++;
					}
				}
			}
			else
			{
				friends.SetActive(value: false);
			}
		}
		else
		{
			DefaultPreviewButton[] connectionDependedntButtons2 = ConnectionDependedntButtons;
			foreach (DefaultPreviewButton defaultPreviewButton2 in connectionDependedntButtons2)
			{
				defaultPreviewButton2.DisabledButton();
			}
			connectButton.text = "Connect";
			Name.text = "Player Disconnected";
			friends.SetActive(value: false);
		}
	}

	private void PostWithAuthCheck()
	{
		Singleton<SPFacebook>.Instance.FeedShare(string.Empty, "https://example.com/myapp/?storyID=thelarch", "The Larch", "I thought up a witty tagline about larches", "There are a lot of larch trees around here, aren't there?", "https://example.com/myapp/assets/1/larch.jpg", string.Empty, string.Empty, string.Empty);
	}

	private void PostNativeScreenshot()
	{
		StartCoroutine(PostFBScreenshot());
	}

	private void PostImage()
	{
		AndroidSocialGate.StartShareIntent("Hello Share Intent", "This is my text to share", ImageToShare, "facebook.katana");
	}

	private IEnumerator PostFBScreenshot()
	{
		yield return new WaitForEndOfFrame();
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, mipChain: false);
		tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		tex.Apply();
		AndroidSocialGate.StartShareIntent("Hello Share Intent", "This is my text to share", tex, "facebook.katana");
		UnityEngine.Object.Destroy(tex);
	}

	private void Connect()
	{
		if (!IsAuntificated)
		{
			Singleton<SPFacebook>.Instance.Login();
			SA_StatusBar.text = "Log in...";
		}
		else
		{
			LogOut();
			SA_StatusBar.text = "Logged out";
		}
	}

	private void LoadUserData()
	{
		SPFacebook.OnUserDataRequestCompleteAction += OnUserDataLoaded;
		Singleton<SPFacebook>.Instance.LoadUserData();
		SA_StatusBar.text = "Loadin user data..";
	}

	private void PostMessage()
	{
		Singleton<SPFacebook>.Instance.FeedShare(string.Empty, "https://example.com/myapp/?storyID=thelarch", "The Larch", "I thought up a witty tagline about larches", "There are a lot of larch trees around here, aren't there?", "https://example.com/myapp/assets/1/larch.jpg", string.Empty, string.Empty, string.Empty);
		SA_StatusBar.text = "Positng..";
	}

	private void PostScreehShot()
	{
		StartCoroutine(PostScreenshot());
		SA_StatusBar.text = "Positng..";
	}

	private void LoadFriends()
	{
		SPFacebook.OnFriendsDataRequestCompleteAction += OnFriendsDataLoaded;
		int limit = 5;
		Singleton<SPFacebook>.Instance.LoadFrientdsInfo(limit);
		SA_StatusBar.text = "Loading friends..";
	}

	private void AppRequest()
	{
		Singleton<SPFacebook>.Instance.AppRequest("Come play this great game!", null, null, null, null, string.Empty, string.Empty);
	}

	private void GetPermissions()
	{
	}

	private void RevokePermission()
	{
	}

	private void LoadScore()
	{
		Singleton<SPFacebook>.Instance.LoadPlayerScores();
	}

	private void LoadAppScores()
	{
		Singleton<SPFacebook>.Instance.LoadAppScores();
	}

	public void SubmitScore()
	{
		startScore++;
		Singleton<SPFacebook>.Instance.SubmitScore(startScore);
	}

	public void DeletePlayerScores()
	{
		Singleton<SPFacebook>.Instance.DeletePlayerScores();
	}

	public void LikePage()
	{
		Application.OpenURL("https://www.facebook.com/unionassets");
	}

	public void CheckLike()
	{
		UnityEngine.Debug.Log("[CheckLike]");
		if (Singleton<SPFacebook>.Instance.IsUserLikesPage(Singleton<SPFacebook>.Instance.UserId, UNION_ASSETS_PAGE_ID))
		{
			SA_StatusBar.text = "Current user Likes union assets";
			return;
		}
		SPFacebook.OnLikesListLoadedAction += OnLikesLoaded;
		Singleton<SPFacebook>.Instance.LoadLikes(Singleton<SPFacebook>.Instance.UserId, UNION_ASSETS_PAGE_ID);
	}

	private void OnLikesLoaded(FB_Result result)
	{
		UnityEngine.Debug.Log("[OnLikesLoaded] result " + result.RawData);
		if (Singleton<SPFacebook>.Instance.IsUserLikesPage(Singleton<SPFacebook>.Instance.UserId, UNION_ASSETS_PAGE_ID))
		{
			SA_StatusBar.text = "Current user Likes union assets";
		}
		else
		{
			SA_StatusBar.text = "Current user does not like union assets";
		}
	}

	private void OnFocusChanged(bool focus)
	{
		UnityEngine.Debug.Log("FB OnFocusChanged: " + focus);
		if (!focus)
		{
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = 1f;
		}
	}

	private void OnUserDataLoaded(FB_Result result)
	{
		SPFacebook.OnUserDataRequestCompleteAction -= OnUserDataLoaded;
		if (result.IsSucceeded)
		{
			SA_StatusBar.text = "User data loaded";
			IsUserInfoLoaded = true;
			Singleton<SPFacebook>.Instance.userInfo.LoadProfileImage(FB_ProfileImageSize.square);
		}
		else
		{
			SA_StatusBar.text = "Opps, user data load failed, something was wrong";
			UnityEngine.Debug.Log("Opps, user data load failed, something was wrong");
		}
	}

	private void OnFriendsDataLoaded(FB_Result res)
	{
		SPFacebook.OnFriendsDataRequestCompleteAction -= OnFriendsDataLoaded;
		if (res.Error == null)
		{
			foreach (FB_UserInfo friends2 in Singleton<SPFacebook>.Instance.friendsList)
			{
				friends2.LoadProfileImage(FB_ProfileImageSize.square);
			}
			IsFrindsInfoLoaded = true;
		}
		else
		{
			UnityEngine.Debug.Log("Opps, friends data load failed, something was wrong");
		}
	}

	private void OnInit()
	{
		if (Singleton<SPFacebook>.Instance.IsLoggedIn)
		{
			IsAuntificated = true;
		}
		else
		{
			SA_StatusBar.text = "user Login -> fale";
		}
	}

	private void OnAuth(FB_Result result)
	{
		if (Singleton<SPFacebook>.Instance.IsLoggedIn)
		{
			IsAuntificated = true;
			SA_StatusBar.text = "user Login -> true";
		}
		else
		{
			UnityEngine.Debug.Log("Failed to log in");
		}
	}

	private void OnPost(FB_PostResult res)
	{
		if (res.IsSucceeded)
		{
			UnityEngine.Debug.Log("Posting complete");
			UnityEngine.Debug.Log("Posy id: " + res.PostId);
			SA_StatusBar.text = "Posting complete";
		}
		else
		{
			SA_StatusBar.text = "Oops, post failed, something was wrong " + res.Error;
			UnityEngine.Debug.Log("Oops, post failed, something was wrong " + res.Error);
		}
	}

	private void OnPlayerScoreRequestComplete(FB_Result result)
	{
		if (result.IsSucceeded)
		{
			string arg = "Player has scores in " + Singleton<SPFacebook>.Instance.userScores.Count + " apps\n";
			arg = (SA_StatusBar.text = arg + "Current Player Score = " + Singleton<SPFacebook>.Instance.GetCurrentPlayerIntScoreByAppId(Singleton<SPFacebook>.Instance.AppId));
		}
		else
		{
			SA_StatusBar.text = result.RawData;
		}
	}

	private void OnAppScoreRequestComplete(FB_Result result)
	{
		if (result.IsSucceeded)
		{
			string arg = "Loaded " + Singleton<SPFacebook>.Instance.appScores.Count + " scores results\n";
			arg = (SA_StatusBar.text = arg + "Current Player Score = " + Singleton<SPFacebook>.Instance.GetScoreByUserId(Singleton<SPFacebook>.Instance.UserId));
		}
		else
		{
			SA_StatusBar.text = result.RawData;
		}
	}

	private void OnSubmitScoreRequestComplete(FB_Result result)
	{
		if (result.IsSucceeded)
		{
			string arg = "Score successfully submited\n";
			arg = (SA_StatusBar.text = arg + "Current Player Score = " + Singleton<SPFacebook>.Instance.GetScoreByUserId(Singleton<SPFacebook>.Instance.UserId));
		}
		else
		{
			SA_StatusBar.text = result.RawData;
		}
	}

	private void OnDeleteScoreRequestComplete(FB_Result result)
	{
		if (result.IsSucceeded)
		{
			string arg = "Score successfully deleted\n";
			arg = (SA_StatusBar.text = arg + "Current Player Score = " + Singleton<SPFacebook>.Instance.GetScoreByUserId(Singleton<SPFacebook>.Instance.UserId));
		}
		else
		{
			SA_StatusBar.text = result.RawData;
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
		Singleton<SPFacebook>.Instance.PostImage("My app ScreehShot", tex);
		UnityEngine.Object.Destroy(tex);
	}

	private void LogOut()
	{
		IsUserInfoLoaded = false;
		IsAuntificated = false;
		Singleton<SPFacebook>.Instance.Logout();
	}
}
