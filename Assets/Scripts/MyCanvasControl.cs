using MadLevelManager;
using SA.Common.Pattern;
using SIS;
using System.Collections;
using TDTK;
using UnityEngine;
using UnityEngine.UI;

public class MyCanvasControl : MonoBehaviour
{
	public GameObject VicCanvas;

	public GameObject LoseCanvas;

	public GameObject PauseCanvas;

	public GameObject[] StarList;

	public Text RewardText;

	public int StarNum;

	public int ScoreNum;

	private string InterstitialUnityId = "";

    public Admobs admob;

	private void Awake()
	{
        //if (Singleton<AndroidAdMobController>.Instance.IsInited)
        //{
        //	if (!Singleton<AndroidAdMobController>.Instance.InterstisialUnitId.Equals(InterstitialUnityId))
        //	{
        //		Singleton<AndroidAdMobController>.Instance.SetInterstisialsUnitID(InterstitialUnityId);
        //	}
        //}
        //else
        //{
        //	Singleton<AndroidAdMobController>.Instance.Init(InterstitialUnityId);
        //}
        admob = GameObject.Find("Admob").GetComponent<Admobs>();
		UnityEngine.Debug.Log("sdfdsf");
		UnityEngine.Debug.Log(MadLevel.currentLevelName);
		VicCanvas.SetActive(value: false);
		LoseCanvas.SetActive(value: false);
		PauseCanvas.SetActive(value: false);
		StarList[0].SetActive(value: false);
		StarList[1].SetActive(value: false);
		StarList[2].SetActive(value: false);
	}

	private void Start()
	{
        //Singleton<AndroidAdMobController>.Instance.LoadInterstitialAd();
      //  admob.RequestInterstitial();
		if (ES2.Exists("Ads"))
		{
			int num = ES2.Load<int>("Ads");
			if (num != 0 && num == 1)
			{
				UnityEngine.Debug.Log("Ads have removed");
			}
		}
	}

	private void OnGameOver(bool playerWon)
	{
		if (!ES2.Exists("Ads"))
		{
			return;
		}
		switch (ES2.Load<int>("Ads"))
		{
		case 0:
			if (playerWon)
			{
				StartCoroutine(ShowFullBanner());
			}
			else
			{
				StartCoroutine(ShowFullBanner());
			}
			break;
		case 1:
			UnityEngine.Debug.Log("Ads have removed");
			break;
		}
	}

	private void OnviewFullAds()
	{
		UnityEngine.Debug.Log("Ads Ok");
        //Singleton<AndroidAdMobController>.Instance.ShowInterstitialAd();
       // admob.ShowAds();
		StartCoroutine(ReloadAds());
	}

	private IEnumerator ShowFullBanner()
	{
		yield return new WaitForSeconds(3f);
        //Singleton<AndroidAdMobController>.Instance.ShowInterstitialAd();
        //admob.ShowAds();
        StartCoroutine(ReloadAds());
	}

	private IEnumerator ReloadAds()
	{
		yield return new WaitForSeconds(10f);
        //admob.RequestInterstitial();
		//Singleton<AndroidAdMobController>.Instance.LoadInterstitialAd();
	}

	public void AddStar(int Score)
	{
		if (Score < 10)
		{
			StarList[0].SetActive(value: true);
			MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, "star_1", val: true);
			MadLevelProfile.SetCompleted(MadLevel.currentLevelName, completed: true);
			StarNum = 1;
		}
		else if (Score >= 10 && Score < 20)
		{
			StarList[1].SetActive(value: true);
			StarList[2].SetActive(value: true);
			StarNum = 2;
			MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, "star_1", val: true);
			MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, "star_2", val: true);
			MadLevelProfile.SetCompleted(MadLevel.currentLevelName, completed: true);
		}
		else if (Score >= 20)
		{
			StarList[0].SetActive(value: true);
			StarList[1].SetActive(value: true);
			StarList[2].SetActive(value: true);
			StarNum = 3;
			MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, "star_1", val: true);
			MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, "star_2", val: true);
			MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, "star_3", val: true);
			MadLevelProfile.SetCompleted(MadLevel.currentLevelName, completed: true);
		}
		GetGear(MadLevel.currentLevelName);
		int value = ScoreNum * StarNum;
		DBManager.IncreaseFunds("coins", value);
		RewardText.text = value.ToString() + " Gear";
	}

	public void Hotdemo()
	{
		DBManager.IncreaseFunds("coins", 54);
	}

	public void GetGear(string _LevelName)
	{
		switch (_LevelName)
		{
		case "Level 1":
			ScoreNum = 20;
			break;
		case "Level 2":
			ScoreNum = 22;
			break;
		case "Level 3":
			ScoreNum = 24;
			break;
		case "Level 4":
			ScoreNum = 26;
			break;
		case "Level 5":
			ScoreNum = 28;
			break;
		case "Level 6":
			ScoreNum = 30;
			break;
		case "Level 7":
			ScoreNum = 32;
			break;
		case "Level 8":
			ScoreNum = 34;
			break;
		case "Level 9":
			ScoreNum = 36;
			break;
		case "Level 10":
			ScoreNum = 38;
			break;
		case "Level 11":
			ScoreNum = 40;
			break;
		case "Level 12":
			ScoreNum = 42;
			break;
		case "Level 13":
			ScoreNum = 44;
			break;
		case "Level 14":
			ScoreNum = 46;
			break;
		case "Level 15":
			ScoreNum = 48;
			break;
		case "Level 16":
			ScoreNum = 50;
			break;
		case "Level 17":
			ScoreNum = 52;
			break;
		case "Level 18":
			ScoreNum = 54;
			break;
		case "Level 19":
			ScoreNum = 56;
			break;
		case "Level 20":
			ScoreNum = 58;
			break;
		case "Level 21":
			ScoreNum = 60;
			break;
		case "Level 22":
			ScoreNum = 62;
			break;
		case "Level 23":
			ScoreNum = 64;
			break;
		case "Level 24":
			ScoreNum = 66;
			break;
		case "Level 25":
			ScoreNum = 68;
			break;
		case "Level 26":
			ScoreNum = 70;
			break;
		case "Level 27":
			ScoreNum = 72;
			break;
		case "Level 28":
			ScoreNum = 74;
			break;
		case "Level 29":
			ScoreNum = 76;
			break;
		case "Level 30":
			ScoreNum = 78;
			break;
		case "Level 31":
			ScoreNum = 80;
			break;
		case "Level 32":
			ScoreNum = 82;
			break;
		case "Level 33":
			ScoreNum = 84;
			break;
		case "Level 34":
			ScoreNum = 86;
			break;
		case "Level 35":
			ScoreNum = 88;
			break;
		case "Level 36":
			ScoreNum = 90;
			break;
		case "Level 37":
			ScoreNum = 92;
			break;
		case "Level 38":
			ScoreNum = 94;
			break;
		case "Level 39":
			ScoreNum = 96;
			break;
		case "Level 40":
			ScoreNum = 100;
			break;
		}
	}

	private void OnEnable()
	{
		UIMainControl.OnVictory += OnShow;
		UIMainControl.OnVictory += OnGameOver;
	}

	private void OnDisable()
	{
		UIMainControl.OnVictory -= OnShow;
		UIMainControl.OnVictory -= OnGameOver;
	}

	private void OnShow(bool won)
	{
		Time.timeScale = 1f;
		if (won)
		{
			VicCanvas.SetActive(value: true);
			int playerLife = GameControl.GetPlayerLife();
			AddStar(playerLife);
			return;
		}
		LoseCanvas.SetActive(value: true);
		if (!MadLevelProfile.IsLocked("Level 2"))
		{
			UnityEngine.Debug.Log("xong lv sd2");
		}
	}

	public void OnRestartFc()
	{
		GameControl.RestartScene();
	}

	public void OnGOUpgrade()
	{
		Time.timeScale = 1f;
		LoadingControl._LevelName = "Upgrade";
		Application.LoadLevelAdditiveAsync("Loading2");
	}

	public void GotoMenu()
	{
		Time.timeScale = 1f;
		LoadingControl._LevelName = "LevelSelect";
		Application.LoadLevelAdditiveAsync("Loading2");
	}

	public void NextLvFc()
	{
		Time.timeScale = 1f;
		string nextLevelName = MadLevel.GetNextLevelName();
		if (nextLevelName == "Level 40")
		{
			LoadingControl._LevelName = "LevelSelect";
		}
		else
		{
			LoadingControl._LevelName = nextLevelName;
		}
		Application.LoadLevelAdditiveAsync("Loading2");
	}

	public void onPauseFc()
	{
		UIMainControl.TogglePause();
		PauseCanvas.SetActive(value: true);
	}

	public void OnResume()
	{
		UIMainControl.TogglePause();
		PauseCanvas.SetActive(value: false);
	}
}
