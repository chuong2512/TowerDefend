using SA.Common.Pattern;
using UnityEngine;

public class AndroidGoogleAdsExample_old : MonoBehaviour
{
	private const string MY_BANNERS_AD_UNIT_ID = "ca-app-pub-6101605888755494/1824764765";

	private const string MY_INTERSTISIALS_AD_UNIT_ID = "ca-app-pub-6101605888755494/3301497967";

	private GUIStyle style;

	private GUIStyle style2;

	private GoogleMobileAdBanner banner1;

	private GoogleMobileAdBanner banner2;

	private bool IsInterstisialsAdReady;

	private void Start()
	{
		Singleton<AndroidAdMobController>.Instance.Init("ca-app-pub-6101605888755494/1824764765");
		Singleton<AndroidAdMobController>.Instance.SetInterstisialsUnitID("ca-app-pub-6101605888755494/3301497967");
		Singleton<AndroidAdMobController>.Instance.SetGender(GoogleGender.Male);
		Singleton<AndroidAdMobController>.Instance.AddKeyword("game");
		Singleton<AndroidAdMobController>.Instance.SetBirthday(1989, AndroidMonth.MARCH, 18);
		Singleton<AndroidAdMobController>.Instance.TagForChildDirectedTreatment(tagForChildDirectedTreatment: false);
		Singleton<AndroidAdMobController>.Instance.AddTestDevice("6B9FA8031AEFDC4758B7D8987F77A5A6");
		Singleton<AndroidAdMobController>.Instance.OnInterstitialLoaded += OnInterstisialsLoaded;
		Singleton<AndroidAdMobController>.Instance.OnInterstitialOpened += OnInterstisialsOpen;
		InitStyles();
	}

	private void InitStyles()
	{
		style = new GUIStyle();
		style.normal.textColor = Color.white;
		style.fontSize = 16;
		style.fontStyle = FontStyle.BoldAndItalic;
		style.alignment = TextAnchor.UpperLeft;
		style.wordWrap = true;
		style2 = new GUIStyle();
		style2.normal.textColor = Color.white;
		style2.fontSize = 12;
		style2.fontStyle = FontStyle.Italic;
		style2.alignment = TextAnchor.UpperLeft;
		style2.wordWrap = true;
	}

	private void OnGUI()
	{
		float num = 20f;
		float num2 = 10f;
		GUI.Label(new Rect(num2, num, Screen.width, 40f), "Interstisal Example", style);
		num += 40f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Start Interstitial Ad"))
		{
			Singleton<AndroidAdMobController>.Instance.StartInterstitialAd();
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Load Interstitial Ad"))
		{
			Singleton<AndroidAdMobController>.Instance.LoadInterstitialAd();
		}
		num2 += 170f;
		GUI.enabled = IsInterstisialsAdReady;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Show Interstitial Ad"))
		{
			Singleton<AndroidAdMobController>.Instance.ShowInterstitialAd();
		}
		GUI.enabled = true;
		num += 80f;
		num2 = 10f;
		GUI.Label(new Rect(num2, num, Screen.width, 40f), "Banners Example", style);
		GUI.enabled = false;
		if (banner1 == null)
		{
			GUI.enabled = true;
		}
		num += 40f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Banner Custom Pos"))
		{
			banner1 = Singleton<AndroidAdMobController>.Instance.CreateAdBanner(300, 100, GADBannerSize.BANNER);
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Banner Top Left"))
		{
			banner1 = Singleton<AndroidAdMobController>.Instance.CreateAdBanner(TextAnchor.UpperLeft, GADBannerSize.BANNER);
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Banner Top Center"))
		{
			banner1 = Singleton<AndroidAdMobController>.Instance.CreateAdBanner(TextAnchor.UpperCenter, GADBannerSize.BANNER);
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Banner Top Right"))
		{
			banner1 = Singleton<AndroidAdMobController>.Instance.CreateAdBanner(TextAnchor.UpperRight, GADBannerSize.BANNER);
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Banner Bottom Left"))
		{
			banner1 = Singleton<AndroidAdMobController>.Instance.CreateAdBanner(TextAnchor.LowerLeft, GADBannerSize.BANNER);
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Banner Bottom Center"))
		{
			banner1 = Singleton<AndroidAdMobController>.Instance.CreateAdBanner(TextAnchor.LowerCenter, GADBannerSize.BANNER);
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Banner Bottom Right"))
		{
			banner1 = Singleton<AndroidAdMobController>.Instance.CreateAdBanner(TextAnchor.LowerRight, GADBannerSize.BANNER);
		}
		GUI.enabled = false;
		if (banner1 != null && banner1.IsLoaded)
		{
			GUI.enabled = true;
		}
		num += 80f;
		num2 = 10f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Refresh"))
		{
			banner1.Refresh();
		}
		GUI.enabled = false;
		if (banner1 != null && banner1.IsLoaded && banner1.IsOnScreen)
		{
			GUI.enabled = true;
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Hide"))
		{
			banner1.Hide();
		}
		GUI.enabled = false;
		if (banner1 != null && banner1.IsLoaded && !banner1.IsOnScreen)
		{
			GUI.enabled = true;
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Show"))
		{
			banner1.Show();
		}
		GUI.enabled = false;
		if (banner1 != null)
		{
			GUI.enabled = true;
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Destroy"))
		{
			Singleton<AndroidAdMobController>.Instance.DestroyBanner(banner1.id);
			banner1 = null;
		}
		GUI.enabled = true;
		num += 80f;
		num2 = 10f;
		GUI.Label(new Rect(num2, num, Screen.width, 40f), "Banner 2", style);
		GUI.enabled = false;
		if (banner2 == null)
		{
			GUI.enabled = true;
		}
		num += 40f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Smart Banner"))
		{
			banner2 = Singleton<AndroidAdMobController>.Instance.CreateAdBanner(TextAnchor.LowerLeft, GADBannerSize.SMART_BANNER);
		}
		GUI.enabled = false;
		if (banner2 != null && banner2.IsLoaded)
		{
			GUI.enabled = true;
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Refresh"))
		{
			banner2.Refresh();
		}
		GUI.enabled = false;
		if (banner2 != null && banner2.IsLoaded && banner2.IsOnScreen)
		{
			GUI.enabled = true;
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Hide"))
		{
			banner2.Hide();
		}
		GUI.enabled = false;
		if (banner2 != null && banner2.IsLoaded && !banner2.IsOnScreen)
		{
			GUI.enabled = true;
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Show"))
		{
			banner2.Show();
		}
		GUI.enabled = false;
		if (banner2 != null)
		{
			GUI.enabled = true;
		}
		num2 += 170f;
		if (GUI.Button(new Rect(num2, num, 150f, 50f), "Destroy"))
		{
			Singleton<AndroidAdMobController>.Instance.DestroyBanner(banner2.id);
			banner2 = null;
		}
		GUI.enabled = true;
	}

	private void OnInterstisialsLoaded()
	{
		IsInterstisialsAdReady = true;
	}

	private void OnInterstisialsOpen()
	{
		IsInterstisialsAdReady = false;
	}
}
