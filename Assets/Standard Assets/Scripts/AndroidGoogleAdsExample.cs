using SA.Common.Pattern;
using UnityEngine;

public class AndroidGoogleAdsExample : MonoBehaviour
{
	private const string MY_BANNERS_AD_UNIT_ID = "ca-app-pub-6101605888755494/1824764765";

	private const string MY_INTERSTISIALS_AD_UNIT_ID = "ca-app-pub-6101605888755494/3301497967";

	private const string MY_REWARDED_VIDEO_AD_UNIT_ID = "ca-app-pub-6101605888755494/5378283960";

	private GoogleMobileAdBanner banner1;

	private GoogleMobileAdBanner banner2;

	private bool IsInterstisialsAdReady;

	public DefaultPreviewButton ShowIntersButton;

	public DefaultPreviewButton[] b1CreateButtons;

	public DefaultPreviewButton b1Hide;

	public DefaultPreviewButton b1Show;

	public DefaultPreviewButton b1Refresh;

	public DefaultPreviewButton ChangePost1;

	public DefaultPreviewButton ChangePost2;

	public DefaultPreviewButton b1Destroy;

	public DefaultPreviewButton[] b2CreateButtons;

	public DefaultPreviewButton b2Hide;

	public DefaultPreviewButton b2Show;

	public DefaultPreviewButton b2Refresh;

	public DefaultPreviewButton b2Destroy;

	private void Start()
	{
		AndroidAdMob.Client.Init("ca-app-pub-6101605888755494/1824764765");
		AndroidAdMob.Client.SetInterstisialsUnitID("ca-app-pub-6101605888755494/3301497967");
		Singleton<AndroidAdMobController>.Instance.SetRewardedVideoAdUnitID("ca-app-pub-6101605888755494/5378283960");
		AndroidAdMob.Client.SetGender(GoogleGender.Male);
		AndroidAdMob.Client.AddKeyword("game");
		AndroidAdMob.Client.SetBirthday(1989, AndroidMonth.MARCH, 18);
		AndroidAdMob.Client.TagForChildDirectedTreatment(tagForChildDirectedTreatment: false);
		AndroidAdMob.Client.OnInterstitialLoaded += OnInterstisialsLoaded;
		AndroidAdMob.Client.OnInterstitialOpened += OnInterstisialsOpen;
		Singleton<AndroidAdMobController>.Instance.OnRewardedVideoLoaded += HandleOnRewardedVideoLoaded;
		Singleton<AndroidAdMobController>.Instance.OnRewardedVideoAdClosed += HandleOnRewardedVideoAdClosed;
		AndroidAdMob.Client.OnAdInAppRequest += OnInAppRequest;
	}

	private void HandleOnRewardedVideoAdClosed()
	{
	}

	private void HandleOnRewardedVideoLoaded()
	{
	}

	private void StartInterstitialAd()
	{
		AndroidAdMob.Client.StartInterstitialAd();
	}

	private void LoadInterstitialAd()
	{
		AndroidAdMob.Client.LoadInterstitialAd();
	}

	private void ShowInterstitialAd()
	{
		AndroidAdMob.Client.ShowInterstitialAd();
	}

	private void LoadRewardedVideoAd()
	{
		Singleton<AndroidAdMobController>.Instance.LoadRewardedVideo();
	}

	private void ShowRewardedVideoAd()
	{
		Singleton<AndroidAdMobController>.Instance.ShowRewardedVideo();
	}

	private void CreateBannerCustomPos()
	{
		banner1 = AndroidAdMob.Client.CreateAdBanner(300, 100, GADBannerSize.BANNER);
	}

	private void CreateBannerUpperLeft()
	{
		banner1 = AndroidAdMob.Client.CreateAdBanner(TextAnchor.UpperLeft, GADBannerSize.BANNER);
	}

	private void CreateBannerUpperCneter()
	{
		banner1 = AndroidAdMob.Client.CreateAdBanner(TextAnchor.UpperCenter, GADBannerSize.BANNER);
	}

	private void CreateBannerBottomLeft()
	{
		banner1 = AndroidAdMob.Client.CreateAdBanner(TextAnchor.LowerLeft, GADBannerSize.BANNER);
	}

	private void CreateBannerBottomCenter()
	{
		banner1 = AndroidAdMob.Client.CreateAdBanner(TextAnchor.LowerCenter, GADBannerSize.BANNER);
	}

	private void CreateBannerBottomRight()
	{
		banner1 = Singleton<AndroidAdMobController>.Instance.CreateAdBanner(TextAnchor.LowerRight, GADBannerSize.BANNER);
	}

	private void B1Hide()
	{
		banner1.Hide();
	}

	private void B1Show()
	{
		banner1.Show();
	}

	private void B1Refresh()
	{
		banner1.Refresh();
	}

	private void B1Destrouy()
	{
		AndroidAdMob.Client.DestroyBanner(banner1.id);
		banner1 = null;
	}

	private void SmartTOP()
	{
		banner2 = AndroidAdMob.Client.CreateAdBanner(TextAnchor.UpperCenter, GADBannerSize.SMART_BANNER);
	}

	private void SmartBottom()
	{
		banner2 = AndroidAdMob.Client.CreateAdBanner(TextAnchor.LowerCenter, GADBannerSize.SMART_BANNER);
	}

	private void B2Hide()
	{
		banner2.Hide();
	}

	private void B2Show()
	{
		banner2.Show();
	}

	private void B2Refresh()
	{
		banner2.Refresh();
	}

	private void B2Destrouy()
	{
		AndroidAdMob.Client.DestroyBanner(banner2.id);
		banner2 = null;
	}

	private void ChnagePostToMiddle()
	{
		banner1.SetBannerPosition(TextAnchor.MiddleCenter);
	}

	private void ChangePostRandom()
	{
		banner1.SetBannerPosition(UnityEngine.Random.Range(0, Screen.width), UnityEngine.Random.Range(0, Screen.height));
	}

	private void FixedUpdate()
	{
		if (IsInterstisialsAdReady)
		{
			ShowIntersButton.EnabledButton();
		}
		else
		{
			ShowIntersButton.DisabledButton();
		}
		if (banner1 != null)
		{
			DefaultPreviewButton[] array = b1CreateButtons;
			foreach (DefaultPreviewButton defaultPreviewButton in array)
			{
				defaultPreviewButton.DisabledButton();
			}
			b1Destroy.EnabledButton();
			if (banner1.IsLoaded)
			{
				b1Refresh.EnabledButton();
				ChangePost1.EnabledButton();
				ChangePost2.EnabledButton();
				if (banner1.IsOnScreen)
				{
					b1Hide.EnabledButton();
					b1Show.DisabledButton();
				}
				else
				{
					b1Hide.DisabledButton();
					b1Show.EnabledButton();
				}
			}
			else
			{
				b1Refresh.DisabledButton();
				ChangePost1.DisabledButton();
				ChangePost2.DisabledButton();
				b1Hide.DisabledButton();
				b1Show.DisabledButton();
			}
		}
		else
		{
			DefaultPreviewButton[] array2 = b1CreateButtons;
			foreach (DefaultPreviewButton defaultPreviewButton2 in array2)
			{
				defaultPreviewButton2.EnabledButton();
			}
			b1Hide.DisabledButton();
			b1Show.DisabledButton();
			b1Refresh.DisabledButton();
			b1Destroy.DisabledButton();
		}
		if (banner2 != null)
		{
			DefaultPreviewButton[] array3 = b2CreateButtons;
			foreach (DefaultPreviewButton defaultPreviewButton3 in array3)
			{
				defaultPreviewButton3.DisabledButton();
			}
			b2Destroy.EnabledButton();
			if (banner2.IsLoaded)
			{
				b2Refresh.EnabledButton();
				if (banner2.IsOnScreen)
				{
					b2Hide.EnabledButton();
					b2Show.DisabledButton();
				}
				else
				{
					b2Hide.DisabledButton();
					b2Show.EnabledButton();
				}
			}
			else
			{
				b2Refresh.DisabledButton();
				b2Hide.DisabledButton();
				b2Show.DisabledButton();
			}
		}
		else
		{
			DefaultPreviewButton[] array4 = b2CreateButtons;
			foreach (DefaultPreviewButton defaultPreviewButton4 in array4)
			{
				defaultPreviewButton4.EnabledButton();
			}
			b2Hide.DisabledButton();
			b2Show.DisabledButton();
			b2Refresh.DisabledButton();
			b2Destroy.DisabledButton();
		}
	}

	private void OnInterstisialsLoaded()
	{
		IsInterstisialsAdReady = true;
	}

	private void OnInterstisialsOpen()
	{
		IsInterstisialsAdReady = false;
	}

	private void OnInAppRequest(string productId)
	{
		AN_PoupsProxy.showMessage("In App Request", "In App Request for product Id: " + productId + " received");
		AndroidAdMob.Client.RecordInAppResolution(GADInAppResolution.RESOLUTION_SUCCESS);
	}
}
