
using SA.Common.Pattern;
using SIS;
using System;
using System.Collections;
using TDTK;
using UnityEngine;
using UnityEngine.UI;

public class StartControl : MonoBehaviour
{
	public Text AudioText;

	public static int AdsCount;

	public GameObject AdsObj;

	public Text CheckInternet;

	private int UserAchive;

	private void Awake()
	{
		
		if (!ES2.Exists("Gear"))
		{
			ES2.Save(0, "Gear");
			ES2.Save(0, "Ads");
			ES2.Save(0, "Tutorial");
			ES2.Save(param: false, "KeyFirstPurchase");
			ES2.Save(param: false, "KeydayCurrent");
			ES2.Save(param: false, "KeyFirstPlay");
			ES2.Save(DateTime.Now, "KeyLastDay");
			ES2.Save(param: false, "KeyHack");
			ES2.Save(0, "KeyHighScore");
			ES2.Save(0, "GunJetRange");
			ES2.Save(0, "GunJetDamage");
			ES2.Save(0, "GunJetCooldown");
			ES2.Save(0, "GunJetCrift");
			ES2.Save(0, "GunJetStun");
			ES2.Save(0, "SlowRange");
			ES2.Save(0, "SlowCooldown");
			ES2.Save(0, "SlowDamage");
			ES2.Save(0, "SlowTime");
			ES2.Save(0, "CannonRange");
			ES2.Save(0, "CannonDamage");
			ES2.Save(0, "CannonCooldown");
			ES2.Save(0, "CannonCrift");
			ES2.Save(0, "CannonStun");
			ES2.Save(0, "LaserRange");
			ES2.Save(0, "LaserDamage");
			ES2.Save(0, "LaserCooldown");
			ES2.Save(0, "LaserCrift");
			ES2.Save(0, "LaserStun");
			ES2.Save(0, "BombRange");
			ES2.Save(0, "BombDamage");
			ES2.Save(0, "BombCooldown");
			ES2.Save(0, "BombCrift");
			ES2.Save(0, "BombStun");
			ES2.Save(0, "SunrayRange");
			ES2.Save(0, "SunrayDamage");
			ES2.Save(0, "SunrayCooldown");
			ES2.Save(0, "SunrayCrift");
			ES2.Save(0, "SunrayStun");
			ES2.Save(0, "BeamRange");
			ES2.Save(0, "BeamDamage");
			ES2.Save(0, "BeamCooldown");
			ES2.Save(0, "BeamCrift");
			ES2.Save(0, "BeamStun");
			ES2.Save(0, "AtomRange");
			ES2.Save(0, "AtomDamage");
			ES2.Save(0, "AtomCooldown");
			ES2.Save(0, "AtomStun");
		}
	}

	private void Start()
	{
	}

	public void AniveBtn()
	{
		Singleton<GooglePlayManager>.Instance.ShowAchievementsUI();
	}

	public void BxhBtn()
	{
		Singleton<GooglePlayManager>.Instance.ShowLeaderBoardsUI();
	}

	public void ConnectBtn()
	{
		Singleton<GooglePlayConnection>.Instance.Connect();
	}

	private void Update()
	{
	}

	public void AddGear()
	{
		DBManager.IncreaseFunds("coins", 100000);
	}

	public void GoFacebook()
	{
		Application.OpenURL("https://www.facebook.com");
	}

	public void AudioControlFc()
	{
		switch (ES2.Load<int>("Audio"))
		{
		case 0:
			ES2.Save(1, "Audio");
			AudioText.text = "Sound On";
			AudioManager.SetSFXVolume(1f);
			AudioManager.SetMusicVolume(1f);
			break;
		case 1:
			ES2.Save(0, "Audio");
			AudioText.text = "Sound Off";
			AudioManager.SetSFXVolume(0f);
			AudioManager.SetMusicVolume(0f);
			break;
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
			CheckInternet.text = "Internet Is On";
		}
		if (pingMasterServer.isDone && pingMasterServer.time <= 1)
		{
			CheckInternet.text = "Internet Is Off";
			UnityEngine.Debug.Log("IntenetOFF");
		}
	}
}
