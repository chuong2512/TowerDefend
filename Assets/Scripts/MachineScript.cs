using MadLevelManager;
using SA.Common.Pattern;
using SIS;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class MachineScript : MonoBehaviour
{
	public Sprite[] _Sprite;

	public ParticleSystem _ParCoin;

	private float _SpinTime = 3.9f;

	private bool SpinStart;

	public GameObject SpinFx;

	public GameObject Candy;

	public Text TextSpinNum;

	private int SpinNum;

	public GameObject SpinAdsPanel;

	public Text SpinAdsPanelTxt;

	public Text GearTxt;

	private int SpinAds;

	private int _Candy;

	private int KetQua;

	private Image ChangeSkin;

	public Text CheckInternet;

	private bool internetStatus = true;

	public GameObject RateBtn;

	private string AdmobID = "ca-app-pub-4997313761585174/8522778810";

	private string RewardAdsID = "ca-app-pub-4997313761585174/1594441173";

	private string UnityRewardID = "1566891";

	private void Awake()
	{
		StartCoroutine(CheckConnectionToMasterServer());
		if (Singleton<AndroidAdMobController>.Instance.IsInited)
		{
			if (!Singleton<AndroidAdMobController>.Instance.InterstisialUnitId.Equals(AdmobID))
			{
				Singleton<AndroidAdMobController>.Instance.SetInterstisialsUnitID(AdmobID);
			}
		}
		else
		{
			Singleton<AndroidAdMobController>.Instance.Init(AdmobID);
		}
	}

	private void Start()
	{
		if (ES2.Exists("Rate"))
		{
			if (ES2.Load<bool>("Rate"))
			{
				RateBtn.SetActive(value: false);
			}
			else
			{
				RateBtn.SetActive(value: true);
			}
		}
		else
		{
			ES2.Save(param: false, "Rate");
		}
		internetStatus = false;
		Singleton<AndroidAdMobController>.Instance.LoadInterstitialAd();
		_ParCoin.GetComponent<ParticleSystem>();
		ChangeSkin = Candy.GetComponent<Image>();
		SpinFx.SetActive(value: false);
	}

	private void HandleOnRewarded(string NameReward, int NumReward)
	{
	}

	private void HandleOnRewardedVideoAdClosed()
	{
	}

	private void HandleOnRewardedVideoLoaded()
	{
	}

	private void Update()
	{
		if (!SpinStart)
		{
			return;
		}
		SpinFx.SetActive(value: true);
		_SpinTime -= Time.deltaTime;
		if (_SpinTime < 0f)
		{
			if (SpinAds >= 3)
			{
				SpinAds = 0;
				ShowBanner();
			}
			_SpinTime = 3.9f;
			SpinStart = false;
			SpinFx.SetActive(value: false);
			Candy.SetActive(value: true);
			KetQua = UnityEngine.Random.Range(0, 1000);
			if (KetQua >= 0 && KetQua <= 300)
			{
				ChangeSkin.sprite = _Sprite[0];
				DBManager.IncreaseFunds("coins", 2);
				UnityEngine.Debug.Log("Add 2 Gear");
			}
			else if (KetQua >= 301 && KetQua <= 700)
			{
				ChangeSkin.sprite = _Sprite[1];
				DBManager.IncreaseFunds("coins", 5);
				UnityEngine.Debug.Log("Add 5 Gear");
			}
			if (KetQua >= 701 && KetQua <= 970)
			{
				_ParCoin.Play();
				ChangeSkin.sprite = _Sprite[2];
				DBManager.IncreaseFunds("coins", 10);
				UnityEngine.Debug.Log("Add 10 Gear");
			}
			if (KetQua >= 971 && KetQua <= 1000)
			{
				_ParCoin.Play();
				ChangeSkin.sprite = _Sprite[3];
				DBManager.IncreaseFunds("coins", 50);
				UnityEngine.Debug.Log("Add 50 Gear");
			}
		}
	}

	private IEnumerator CheckConnectionToMasterServer()
	{
		Ping pingMasterServer = new Ping("8.8.8.8");
		float startTime = Time.time;
		while (!pingMasterServer.isDone && Time.time < startTime + 5f)
		{
			yield return new WaitForSeconds(5f);
		}
		if (pingMasterServer.isDone && pingMasterServer.time > 2)
		{
			UnityEngine.Debug.Log("IntenetON");
			CheckInternet.text = "Internet Ready! Spin Ready!";
			internetStatus = true;
			RateScript.InternetCheck = true;
		}
		if (pingMasterServer.isDone && pingMasterServer.time <= 1)
		{
			CheckInternet.text = "Opps! Please check internet connection!";
			UnityEngine.Debug.Log("IntenetOFF");
			internetStatus = false;
			RateScript.InternetCheck = false;
		}
	}

	public void SpinBtn()
	{
		if (!SpinStart && internetStatus)
		{
			Singleton<AndroidAdMobController>.Instance.LoadInterstitialAd();
			SpinAds++;
			SpinStart = true;
			Candy.SetActive(value: false);
		}
	}

	public void CloseAdsPanel()
	{
		SpinAdsPanel.SetActive(value: false);
	}

	public void RateBtnFc()
	{
		if (ES2.Load<bool>("Rate"))
		{
			RateBtn.SetActive(value: false);
		}
		else if (internetStatus)
		{
			Application.OpenURL("https://play.google.com");
			ES2.Save(param: true, "Rate");
			RateBtn.SetActive(value: false);
			DBManager.IncreaseFunds("coins", 200);
		}
	}

	public void ShowBanner()
	{
		Singleton<AndroidAdMobController>.Instance.ShowInterstitialAd();
	}

	public void ShowRewardedVideo()
	{
		string placementId = "rewardedVideo";
		/*ShowOptions showOptions = new ShowOptions();
		showOptions.resultCallback = HandleShowResult;
		if (Advertisement.IsReady(placementId))
		{
			Advertisement.Show(placementId, showOptions);
		}*/
	}

	private IEnumerator ShowAdWhenReady()
	{
		float currentTimeScale = Time.timeScale;
		Time.timeScale = 0f;
		yield return null;
		/*while (Advertisement.isShowing)
		{
			yield return null;
		}*/
		Time.timeScale = currentTimeScale;
	}

	/*private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			SpinAdsPanel.SetActive(value: false);
			SpinNum += 3;
			TextSpinNum.text = "Spin x" + SpinNum.ToString();
			ES2.Save(SpinNum, "SpinNumber");
			Advertisement.Initialize(UnityRewardID, testMode: false);
			break;
		case ShowResult.Skipped:
			SpinAdsPanelTxt.text = "Video was skipped!";
			break;
		case ShowResult.Failed:
			SpinAdsPanelTxt.text = "Video failed to show!";
			break;
		}
	}*/

	public void EscapeBtn()
	{
		MadLevel.LoadLevelByName("Start");
	}
}
