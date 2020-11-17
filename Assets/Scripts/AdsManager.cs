using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance = null;

    public bool isTestMode;

    [Header("Android")]
    public string androidAppId;
    public string androidBannerId;
    public string androidInterstitialId;
    public string androidRewardedId;

    [Header("IOS")]
    public string IOSAppId;
    public string IOSBannerId;
    public string IOSInterstitialId;
    public string IOSRewardedId;

    string appId;
    string bannerAdUnitId;
    string interstitialAdUnitId;
    string rewardedAdUnitId;

    BannerView bannerView;
    InterstitialAd interstitialAd;
    RewardedAd rewardedAd;

    int interstitialInterval = 2;

    [HideInInspector] public int rewardType = 0; // 1: hint  2: extra time



    private void Awake()
    {
        // Initialize the singleton instance.
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {

#if UNITY_ANDROID
        appId = androidAppId;
#elif UNITY_IOS
        appId = IOSAppId;
#else
        appId = "unexpected_platform";
#endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);

        // Test IDs
        if (isTestMode)
        {
            bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
            interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
            rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
        }
        else
        {
#if UNITY_ANDROID
            bannerAdUnitId = androidBannerId;
            interstitialAdUnitId = androidInterstitialId;
            rewardedAdUnitId = androidRewardedId;
#elif UNITY_IOS
            bannerAdUnitId = IOSBannerId;
            interstitialAdUnitId = IOSInterstitialId;
            rewardedAdUnitId = IOSRewardedId;
#endif
        }

        //Request Ads
        RequestBanner();
        RequestInterstitialAd();
        RequestRewardedAd();
    }


    #region BANNER AD
    private void RequestBanner()
    {
        bannerView = new BannerView(bannerAdUnitId, AdSize.Banner, AdPosition.Top);
        bannerView.OnAdLoaded += HandleBannerOnAdLoaded;
        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);
    }

    private void HandleBannerOnAdLoaded(object sender, EventArgs e)
    {
        bannerView.Show();
    }

    public void HideBanner()
    {
        bannerView.Hide();
    }

    public void ShowBanner()
    {
        bannerView.Show();
    }
    #endregion


    #region INTERSTITIAL AD
    private void RequestInterstitialAd()
    {
        interstitialAd = new InterstitialAd(interstitialAdUnitId);

        interstitialAd.OnAdLoaded += HandleInterstitialLoaded;
        interstitialAd.OnAdOpening += HandleInterstitialOpened;
        interstitialAd.OnAdClosed += HandleInterstitialClosed;
        interstitialAd.OnAdFailedToLoad += HandleInterstitialFailed;

        AdRequest request = new AdRequest.Builder().Build();
        interstitialAd.LoadAd(request);
    }

    // Reset the counter for failed ads
    private void HandleInterstitialLoaded(object sender, EventArgs args)
    {
        PlayerPrefs.SetInt("InterstitialFailed", 0);
    }

    // Reset the counter for ad clicks
    private void HandleInterstitialOpened(object sender, EventArgs args)
    {
        PlayerPrefs.SetInt("ClickCounter", 0);
    }

    // Request new interstital ad
    private void HandleInterstitialClosed(object sender, EventArgs args)
    {
        RequestInterstitialAd();
    }

    // Try to load new ad if ad is failed (limit the trial with 3 times to save data)
    private void HandleInterstitialFailed(object sender, AdFailedToLoadEventArgs args)
    {
        StartCoroutine(InterstitialFailedAsync());
    }

    IEnumerator InterstitialFailedAsync()
    {
        yield return new WaitForSeconds(3f);
        int trial = PlayerPrefs.GetInt("InterstitialFailed", 0);
        trial++;
        if (trial < 3)
            RequestInterstitialAd();
    }

    // ADJUSTS THE INTERSTITIAL FREQUENCY DEPENDING ON THE CURRENT LEVEL PLAYED
    public void ShowInterstitialAd()
    {
        int levelIndex = PlayerPrefs.GetInt("LastLevelIndex", 1);

        // Set interstitial interval according to the last selected level
        if (levelIndex >= 1 && levelIndex <= 10)
            interstitialInterval = 3;
        else if (levelIndex > 10 && levelIndex <= 20)
            interstitialInterval = 2;
        else
            interstitialInterval = 1;

        // Increment the counter
        int counter = PlayerPrefs.GetInt("ClickCounter", 0);
        counter++;
        PlayerPrefs.SetInt("ClickCounter", counter);

        // If counter reaches ... and ad is loaded then show it
        if (interstitialAd.IsLoaded() && counter >= interstitialInterval)
            interstitialAd.Show();
    }
    #endregion


    #region REWARDED AD
    private void RequestRewardedAd()
    {
        rewardedAd = new RewardedAd(rewardedAdUnitId);

        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;
        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;

        AdRequest request = new AdRequest.Builder().Build();
        rewardedAd.LoadAd(request);
    }

    // Reset the counter for failed ads
    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        PlayerPrefs.SetInt("RewardedFailed", 0);
    }

    // Give extra time to the user as a reward and resume the game OR show hint
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        FindObjectOfType<LevelManager>().ShowHintPanel();
    }

    // Request new rewarded ad
    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        RequestRewardedAd();
    }

    // Try to load new ad if ad is failed (limit the trial with 3 times)
    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        StartCoroutine(RewardedFailedAsync());
    }

    IEnumerator RewardedFailedAsync()
    {
        yield return new WaitForSeconds(3f);
        int trial = PlayerPrefs.GetInt("RewardedFailed", 0);
        trial++;
        if (trial < 3)
            RequestRewardedAd();
    }

    public void ShowRewardedAd()
    {
        if (rewardedAd.IsLoaded())
            rewardedAd.Show();
    }

    #endregion
    
}
