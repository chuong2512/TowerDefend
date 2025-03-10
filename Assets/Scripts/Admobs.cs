using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using SIS;

public class Admobs : MonoBehaviour
{
    public string appid_android;
    private string banner_adid_android;
    public string inter_adid_android;
    public string reward_adid_android;
    public string appid_ios;
    private string banner_adid_ios;
    public string inter_adid_ios;
    public string reward_adid_ios;
    public int timerset;
    private int timer;
    private readonly TimeSpan APPOPEN_TIMEOUT = TimeSpan.FromHours(4);
    private DateTime appOpenExpireTime;
    private float deltaTime;
    private bool isShowingAppOpenAd;
    public UnityEvent OnAdLoadedEvent;
    public UnityEvent OnAdFailedToLoadEvent;
    public UnityEvent OnAdOpeningEvent;
    public UnityEvent OnAdFailedToShowEvent;
    public UnityEvent OnUserEarnedRewardEvent;
    public UnityEvent OnAdClosedEvent;
    public bool showFpsMeter = true;
    public Text fpsMeter;
    public Text statusText;
    //public RewardedAd rewardedAd;
   // public InterstitialAd interstitialAd;
    //public BannerView bannerView;
    //public BannerView exitDialogAd;
    //public int coinToAdd = 100;
   
    
    // Use this for initialization
    public void Start()
    {

        timer = PlayerPrefs.GetInt("xtimer", timerset);

    }
}
