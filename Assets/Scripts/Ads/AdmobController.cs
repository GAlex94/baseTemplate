using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GoogleMobileAds.Api;
using System;
using baseTemplate;

#if UNITY_ADS
using UnityEngine.Advertisements;
#else
[System.Serializable]
public enum ShowResult
{
    Failed, Skipped, Finished
}
#endif

public class AdmobController : MonoBehaviour
{
    private static class AdmobReward
    {
        private static string rewardId;
        private static Func<AdRequest> GetAdRequest;

        private static RewardBasedVideoAd rewardBasedVideo;
        private static int attemptsToLoadRewardBasedVideo;
        private static Action<ShowResult> callback;

        private static bool wasCurrentVideoRewarded;

        public static void Init(string rewardId, Func<AdRequest> FuncAdRequest)
        {
            if (rewardBasedVideo != null)
            {
                return;
            }

            AdmobReward.rewardId = rewardId;
            GetAdRequest = FuncAdRequest;

            // Get singleton reward based video ad reference.
            rewardBasedVideo = RewardBasedVideoAd.Instance;
#if UNITY_IOS
            rewardBasedVideo.OnAdOpening += (_, __) => isShowing = true;
            rewardBasedVideo.OnAdClosed += (_, __) => isShowing = false;
#endif
            // Called when an ad request has successfully loaded.
            rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
            // Called when an ad request failed to load.
            rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
            // Called when an ad is shown.
            rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
            // Called when the ad starts to play.
            rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
            // Called when the user should be rewarded for watching a video.
            rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
            // Called when the ad is closed.
            rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
            // Called when the ad click caused the user to leave the application.
            rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;

            attemptsToLoadRewardBasedVideo = 0;
            RequestRewardBasedVideo();
        }

        public static bool ShowReward(Action<ShowResult> callback)
        {
            try
            {
                AdmobReward.callback = callback;

#if UNITY_EDITOR
                Debug.Log("ADS (EDITOR): AdMob Reward Finished");
                callback(ShowResult.Finished);
                RequestRewardBasedVideo();
                return true;
#endif
                bool isLoaded = rewardBasedVideo.IsLoaded();
                if (isLoaded)
                {
                    rewardBasedVideo.Show();
                }
                return isLoaded;
            }
            catch
            {
                return false;
            }
        }

        private static void RequestRewardBasedVideo()
        {
            attemptsToLoadRewardBasedVideo++;
            rewardBasedVideo.LoadAd(GetAdRequest(), rewardId);
        }

        private static void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
        {
            attemptsToLoadRewardBasedVideo = 0;
        }

        private static void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            Debug.Log("REWARDED VIDEO LOADING FAIL: " + e.Message);
            HandleRewardAdsActionDelayed = RequestRewardBasedVideo;
        }

        private static void HandleRewardBasedVideoOpened(object sender, EventArgs e)
        {

        }

        private static void HandleRewardBasedVideoStarted(object sender, EventArgs e)
        {
            wasCurrentVideoRewarded = false;
        }

        private static void HandleRewardBasedVideoRewarded(object sender, Reward e)
        {
            HandleRewardAdsAction += () =>
            {
                if (callback != null)
                {
                    wasCurrentVideoRewarded = true;
                    callback(ShowResult.Finished);
                    RequestRewardBasedVideo();
                }
            };
        }

        private static void HandleRewardBasedVideoClosed(object sender, EventArgs e)
        {
            HandleRewardAdsAction += () =>
            {
                if (!wasCurrentVideoRewarded && callback != null)
                {
                    callback(ShowResult.Skipped);
                    RequestRewardBasedVideo();
                }
            };
        }

        private static void HandleRewardBasedVideoLeftApplication(object sender, EventArgs e)
        {

        }


        public static bool IncentivizedAvailable
        {
            get
            {
                if (Debug.isDebugBuild)
                    return true;
                else
                    return rewardBasedVideo.IsLoaded();
            }
        }
    }

    [Serializable]
    public enum AdsEvent
    {
        SceneAndPopup
    }

    [Serializable]
    public class Rule
    {
        public AdsEvent adsEvent;
        public int skip;
        public int rate;
        [Tooltip("Задержка перед показом")]
        public float delay;
    }

    public bool isEnabled = true;
    public bool needAppId;
    public bool needRewardId;
    public bool isUnityPersonal;
    public bool isManualBannerSchedule;

    public string appIdAndroid;
    public string bannerIdAndroid;
    public string interIdAndroid;
    public string startIdAndroid;
    public string rewardIdAndroid;
    public string unityIdAndroid;

    public string appIdIos;
    public string bannerIdIos;
    public string interIdIos;
    public string rewardIdIos;
    public string unityIdIos;
    public float min_dp = 50;

    [SerializeField] private float _delayAfterReward = 20;
    private float _lastRewardShowingTime = Single.NegativeInfinity;

    string appId, bannerId, interId, startId, rewardId, unityId;

    public List<Rule> rules;

    private bool inited = false;
    private InterstitialAd interstitial;
    private InterstitialAd interstitialStart;

    private BannerView adBannerView;
    private bool isBannerLoaded;
    public Action<float> OnSetAdBannerHeight;

    public bool IncentivizedAvailable()
    {
        if (needRewardId && !string.IsNullOrEmpty(rewardId))
        {
            return AdmobReward.IncentivizedAvailable;
        }

        return false;
    }

   public float AdBannerHeight
    {
        get
        {
            return UpdateBannerZone.Instance.AdBannerHeight;
        }
        private set
        {
            var minDp = Mathf.RoundToInt(min_dp * Screen.dpi / 160);
            UpdateBannerZone.Instance.AdBannerHeight = DataManager.Instance.IsAdsDisabled ? 0 : Mathf.Max(value, minDp);
            OnSetAdBannerHeight?.Invoke(value);
        }
    }

    private bool m_isBannerVisible = true;

    public bool IsBannerVisible
    {
        get { return m_isBannerVisible; }
        set
        {
            if (m_isBannerVisible != value)
            {
                m_isBannerVisible = value;
                if (value && isEnabled)
                {
                    adBannerView?.Show();
                    if (isBannerLoaded)
                    {
                        var minDp = Mathf.RoundToInt(min_dp * Screen.dpi / 160);
                        AdBannerHeight = adBannerView?.GetHeightInPixels() ?? Mathf.Max(AdBannerHeight, minDp);
                    }
                }
                else
                {
                    adBannerView?.Hide();
                    AdBannerHeight = 0;
                }
            }
        }
    }

    private Dictionary<AdsEvent, Rule> rulesIndex;
    private Dictionary<Rule, int> showCount;

    public Func<Action<ShowResult>, bool> RewardAdsOperator;
    private static Action HandleRewardAdsAction;
    private static Action HandleRewardAdsActionDelayed;
    private Coroutine RewardAdsActionDelayedRoutine;

    public static AdmobController Instance;

    IEnumerator Start()
    {
        Debug.Log("Start Admob");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            yield return null;

            rulesIndex = new Dictionary<AdsEvent, Rule>();
            showCount = new Dictionary<Rule, int>();
            foreach (var rule in rules)
            {
                rulesIndex.Add(rule.adsEvent, rule);
                showCount.Add(rule, 0);
            }

#if UNITY_ANDROID
            appId = appIdAndroid;
			bannerId = bannerIdAndroid;
			interId = interIdAndroid;
            startId = startIdAndroid;
            rewardId = rewardIdAndroid;
			unityId = unityIdAndroid;
#elif UNITY_IOS
            appId = appIdIos;
            bannerId = bannerIdIos;
            interId = interIdIos;
            rewardId = rewardIdIos;
            unityId = unityIdIos;
#endif
          
            if (needAppId)
            {
                MobileAds.Initialize(initStatus => { });
            }

            if (needRewardId && !string.IsNullOrEmpty(rewardId))
            {
                AdmobReward.Init(rewardId, GetAdRequest);
                RewardAdsOperator = AdmobReward.ShowReward;
            }
            else
            {
                RewardAdsOperator = (_) => false;
            }
#if !UNITY_EDITOR
            if (Debug.isDebugBuild)
            {
                yield break;
            }

#endif

            if (DataManager.Instance.IsAdsDisabled)
            {
                isEnabled = false;
                yield break;
            }

            if (isUnityPersonal)
            {
#if UNITY_ADS
                Debug.Log("ADS: UnityADS manual initialization.");
                Advertisement.Initialize(unityId, false);
#endif
            }
            Init(true);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
#if UNITY_IOS
        if (isShowing)
        {
            return;
        }
#endif

        if (HandleRewardAdsAction != null)
        {
            HandleRewardAdsAction.Invoke();
            HandleRewardAdsAction = null;
        }

        if (HandleRewardAdsActionDelayed != null && RewardAdsActionDelayedRoutine == null)
        {
            RewardAdsActionDelayedRoutine = StartCoroutine(DelayedRewardAdsAction(5f));
        }
    }

    public void ShowEvent(AdsEvent adsEvent)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        if (!rulesIndex.ContainsKey(adsEvent))
        {
            Debug.LogError("ADS: \"" + adsEvent + "\": Rule is not defined!");
            return;
        }

        var rule = rulesIndex[adsEvent];
        showCount[rule]++;

        var count = showCount[rule] - rule.skip;
        Debug.Log("ADS: \"" + adsEvent + "\": Counter = " + count + ", Rate = " + rule.rate + ".");

        if (count <= 0)
        {
            Debug.Log("ADS: \"" + adsEvent + "\": " + showCount[rule] + " call is skipped (first " + rule.skip + " calls are skipped).");
            return;
        }

        if (count % rule.rate == 0)
        {
            Debug.Log("ADS: \"" + adsEvent + "\": Counter is multiple of " + rule.rate + ", so ADS is meant to be shown.");
            Show(rule.delay, false);
        }
    }

    public void Init(bool need)
    {
        if (need && !inited)
        {
            inited = true;
            RequestInterstitial();
            RequestStartInterstitial();
            RequestBanner(60000);
            AdBannerHeight = 0;
        }
        else
        {
            RemoveAds();
        }
    }

    public void RemoveAds()
    {
        isEnabled = false;

        adBannerView?.Hide();
        AdBannerHeight = 0;
    }

    private void RequestBanner(int msPeriod)
    {
        if (isEnabled)
        {
            AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            adBannerView  = new BannerView(bannerId, adaptiveSize, AdPosition.Bottom);

            adBannerView.OnAdLoaded += (sender, args) => 
            {
                if (IsBannerVisible)
                {
                    AdBannerHeight = adBannerView.GetHeightInPixels();
                }

                isBannerLoaded = true;
                Debug.Log("ADS: Banner LoadAd SUCCESS.");
            };
            adBannerView.OnAdFailedToLoad += (sender, args) => 
            {
                //AdBannerHeight = 0;
                Debug.Log("ADS: Banner LoadAd FAILED.");
            };

            if (isManualBannerSchedule)
            {
                StartCoroutine(ScheduleBannerAds(msPeriod));
            }
            else
            {
                adBannerView.LoadAd(GetAdRequest());
            }

        //  AsyncUtility.Repeat(() => bannerView.LoadAd(GetAdRequest()), () => Debug.Log("ADS: Next banner load in " + msPeriod + " ms."), msPeriod);  
        }
        else
        {
            Debug.Log("ADS: Banner NOT REQUIRED (ads disabled).");
        }
    }

    IEnumerator ScheduleBannerAds(int msPeriod)
    {
        while (true)
        {
            adBannerView.LoadAd(GetAdRequest());
            yield return new WaitForSeconds(msPeriod / 1000f);
        }
    }

    IEnumerator DelayedRewardAdsAction(float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);

        HandleRewardAdsActionDelayed.Invoke();
        HandleRewardAdsActionDelayed = null;
        RewardAdsActionDelayedRoutine = null;
    }

    AdRequest GetAdRequest()
    {
        return new AdRequest.Builder()
            .AddTestDevice(AdRequest.TestDeviceSimulator)      // Simulator.
            .AddTestDevice("A0AB3BAEACD14BE788D16BBD4844AC62") // Alexandr Android
            .AddTestDevice("7F3E67A8CFBBB26929A9E263AED54BE9") // Dmitriy Android
            .AddTestDevice("d77ff4e98f70f895205fda47fdbe9d4c") // Dmitriy iPhone
            .AddTestDevice("482E4B7C6DD7C82D54082EE2C4CFD4D3") // Samsung Galaxy S6 edge
            .AddTestDevice("98FE38230F3C3CE8BC4AF41C10436CBC") // Samsung Galaxy S8
            .AddTestDevice("025221E858E022C2050B79E12742B34B") // new device
            .AddTestDevice("5BCB41EA19ACC3FC09CDE36CA70A5B80") // new device
			.AddTestDevice("6508FDDB21AD82EFBA99476779A0C365") // new device
            .AddTestDevice("A5986FAEEC47427984CABECC7916D9ECR") // new device grigoriy
            .AddTestDevice("C78978D2CE505E782343DF9020AC1C8C") // Dima
            .AddTestDevice("BE04CA8A1BA46F21DB10C0B4E87FB16F") // Sergey
            .AddTestDevice("DA651CD1E3E31704864F2818FCCE9D6D") // Xiaomi
            .AddTestDevice("5E4D28A9BFEBB3267D8D73D03D703FA8") // Dima
            .Build();
    }

    private void RequestInterstitial()
    {
        if (isEnabled)
        {
            // Initialize an InterstitialAd.
            interstitial = new InterstitialAd(interId);
            // Load the interstitial with the request.
            interstitial.LoadAd(GetAdRequest());
#if UNITY_ANDROID
            interstitial.OnAdOpening += HandleAdOpening;
            interstitial.OnAdClosed += HandleAdClosed;
#endif
        }
    }

    private void HandleAdOpening(object sender, EventArgs args)
    {
        interstitial.OnAdOpening -= HandleAdOpening;
    }

    public void HandleAdClosed(object sender, EventArgs args)
    {
        if (isEnabled == true)
        {
            interstitial.OnAdClosed -= HandleAdClosed;

#if UNITY_IOS
            interstitial?.Destroy();
#endif

            RequestInterstitial();
        }
    }


    private void RequestStartInterstitial()
    {
        if (isEnabled)
        {
            // Initialize an InterstitialAd.
            interstitialStart = new InterstitialAd(startId);
            // Load the interstitial with the request.
            interstitialStart.LoadAd(GetAdRequest());

#if UNITY_ANDROID
            interstitialStart.OnAdOpening += HandleAdStartOpening;
            interstitialStart.OnAdClosed += HandleAdStartClosed;
#endif

        }
    }
    
    public void HandleAdStartClosed(object sender, EventArgs args)
    {
        if (isEnabled == true)
        {
            interstitialStart.OnAdClosed -= HandleAdStartClosed;

#if UNITY_IOS
            interstitialStart?.Destroy();
#endif

            Time.timeScale = 1;
        }
    }

    private void HandleAdStartOpening(object sender, EventArgs args)
    {
        interstitialStart.OnAdOpening -= HandleAdStartOpening;
    }


    private bool showAdsInGame;
    public void Show(float time, bool inGameAds)
    {
        if (isEnabled == false)
        {
            Debug.Log("ADS: Disabled.");
            HandleAdClosed(null, null);
            return;
        }

        if (!inGameAds && showAdsInGame)
        {
            showAdsInGame = false;
            return;
        }

        showAdsInGame = inGameAds;

        if (time > 0)
        {
            StartCoroutine(WaitAndShowAd(time));
        }
        else
        {
            ShowNow();
        }
    }
   
    public void ShowStart()
    {
        if (isEnabled == false)
        {
            HandleAdStartClosed(null, null);
            return;
        }

        ShowNowStart();
    }

    IEnumerator WaitAndShowAd(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        ShowNow();
    }

    private int countShowAds;


    private GameObject adCanvasGO;
    private FakeAdsPanel fakeAdevert;

    private void Awake()
    {
        adCanvasGO = transform.Find("AdCanvas").gameObject;
        fakeAdevert = adCanvasGO.GetComponent<FakeAdsPanel>();
        CloseFakeAd();

    }

    public void ShowFakeAd(string message = null, Action finishAction = null, Action cancelAction = null, Action skippAction = null)
    {
        isShowing = true;
        fakeAdevert.Init(message, finishAction, cancelAction, skippAction, CloseFakeAd);
        adCanvasGO.SetActive(true);
    }

    private void CloseFakeAd()
    {
        isShowing = false;
        adCanvasGO.SetActive(false);
    }


    private void ShowNow()
    {
        if (Time.time <= _lastRewardShowingTime + _delayAfterReward)
        {
            return;
        }

        if (Debug.isDebugBuild)
        {
            ShowFakeAd("Interstitial");
        }

        if (interstitial != null && interstitial.IsLoaded())
        {
            Debug.LogError("ADS: Interstitial starts.");

#if UNITY_IOS
            interstitial.OnAdOpening += (_, __) => isShowing = true;
            interstitial.OnAdClosed += (_, __) => isShowing = false;
            interstitial.OnAdClosed += HandleAdClosed;
#endif
            interstitial.Show();

            countShowAds++;
            if (countShowAds >= 5)
            {
                countShowAds = 0;
  //              StartCoroutine(Wt());
            }

            _lastRewardShowingTime = Time.time;
        }
        else
        {
            Debug.Log("ADS: Interstitial is null or not yet loaded.");
        }
    }

    private void ShowNowStart()
    {
        if (Debug.isDebugBuild)
        {
            ShowFakeAd("InterstitialStart");
        }

        if (interstitialStart != null && interstitialStart.IsLoaded())
        {
            Debug.Log("ADS: Start Interstitial starts.");
#if UNITY_IOS
            interstitialStart.OnAdOpening += (_, __) => isShowing = true;
            interstitialStart.OnAdClosed += (_, __) => isShowing = false;
            interstitialStart.OnAdClosed += HandleAdStartClosed;
#endif
            interstitialStart.Show();

            _lastRewardShowingTime = Time.time;
        }
        else
        {
            Debug.LogError("ADS: Start Interstitial is null or not yet loaded.");
        }
    }

    static bool _isShowing;
    public static bool isShowing
    {
        get => _isShowing;
        set
        {
            _isShowing = value;
            AudioListener.volume = value ? 0 : 1;
            Time.timeScale = value ? 0 : 1;
        }
    }

    public bool IsStartShow { get; set; }
    public bool IsFirstGame { get; set; }
}
