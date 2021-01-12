using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DTools;
using UnityEngine;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

/// <summary>
/// Менеджер игрового контента, связанного с Rewarded Video.
/// </summary>
public class RewardedAds : MonoBehaviour
{
    private static RewardedAds m_instance;
    public static RewardedAds Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<RewardedAds>();
            }
            return m_instance;
        }
    }

    // Настройки запросов к UnityADS.
    private const string PLACEMENT_ID = "rewardedVideo";
    // * периодичность повтора запроса на показ видео.
    private const float REQUEST_PERIOD = 0.2f;
    // * спустя какое время нужно прекратить запросы и выдать ошибку, если видео не загрузилось?
    private const float REQUEST_TIME_LIMIT = 1f;

    /// <summary>
    /// Сценарии показа Rewarded Video.
    /// </summary>
    public enum AdsCase
    {
        MoreCoins, DoubleCoins, ExtraLife, NotEnoughCoins, ShopUnlock
    }

    /// <summary>
    /// Настройки попапа для сценария показа Rewarded Video.
    /// </summary>
    [System.Serializable]
    public class AdsCaseSetup
    {
        public AdsCase id;
       // public UIDialog dialog;
    }

    [Header("Настройки попапов")]
    public AdsCaseSetup[] caseSettings;

    [Header("Сообщения об ошибках (если нужны)")]
    public string msgSkipped;
    public string msgError;

    private bool videoIsRequested;

    /// <summary>
    /// Прямой запрос произвольного показа рекламы.
    /// </summary>
    /// <param name="adsCase">Сценарий показа</param>
    /// <param name="onAnyResult">Колбек на любой исход показа</param>
    /// <param name="onSuccess">Колбек на успешный показ</param>
    /// <param name="userAgreed">Дал ли согласие игрок?</param>
    public void DirectCall(AdsCase adsCase, System.Action onAnyResult, System.Action onSuccess, bool userAgreed = true)
    {
        if (userAgreed)
        {
            ShowRewardedVideo(onAnyResult, onSuccess);
        //  AnalyticsManager.Instance.LogEvent(AnalyticsManager.Event.ClickAdsAgree, adsCase.ToString());
        }
        else
        {
        //  AnalyticsManager.Instance.LogEvent(AnalyticsManager.Event.ClickAdsDisagree, adsCase.ToString());
        }
    }

    /// <summary>
    /// Показать диалог соответствующий настройкам сценария показа.
    /// </summary>
    /// <param name="adsCase">Сценарий показа</param>
    /// <param name="onSuccess">Колбек на успешный показ</param>
    /// <param name="message">Сообщение диалога</param>
    public void ShowDialog(AdsCase adsCase, System.Action onSuccess, string message = null)
    {
        var setup = caseSettings.FirstOrDefault(s => s.id == adsCase);

        if (setup == null)
        {
            //Debug.LogError("ADS: Settings for case \"" + adsCase + "\" NOT FOUND!");
            return;
        }

        /*
        if (adsCase == AdsCase.MoreCoins)
        {
            AnalyticsManager.Instance.LogEvent(AnalyticsManager.Event.ClickMoreCoins);
        }
        */

     /*   var dialog = setup.dialog;
        var localMessage = string.IsNullOrEmpty(message) ? string.Empty : DLocalizationManager.Instance.GetLocalText(message);

        dialog.Define(localMessage,
            new UIDialog.Option
            {
                type = UIDialog.OptionType.Yes,
                action = () =>
                {
                    //  AnalyticsManager.Instance.LogEvent(AnalyticsManager.Event.ClickAdsAgree, adsCase.ToString());
                    ShowRewardedVideo(dialog.HideByAnimation, onSuccess);
                }
            },
            new UIDialog.Option
            {
                type = UIDialog.OptionType.No,
                action = () =>
                {
                    //  AnalyticsManager.Instance.LogEvent(AnalyticsManager.Event.ClickAdsDisagree, adsCase.ToString());
                    dialog.HideByAnimation();
                }
            });
            
        dialog.ShowByAnimation();
        */
    }

    public void ShowRewardedVideo(System.Action onAnyResult, System.Action onSuccess, System.Action onFailed = null, System.Action onSkipped = null)
    {
        if (Debug.isDebugBuild)
        {
            AdmobController.Instance.ShowFakeAd("Incentivized", onSuccess, onFailed, onSkipped);
            return;
        }

        if (videoIsRequested)
        {
            return;
        }
        
        System.Action<ShowResult> onAdsShown = (result) =>
        {/*
#if UNITY_EDITOR
            result = ShowResult.Finished;
#endif*/
            videoIsRequested = false;

            onAnyResult?.Invoke();
            switch (result)
            {
                case ShowResult.Finished:

                    onSuccess?.Invoke();
                    break;

                case ShowResult.Skipped:

                    if (!string.IsNullOrEmpty(msgSkipped))
                    {
                        string msg = DLocalizationManager.Instance.GetLocalText(msgSkipped);
                       ToastMessage.ShowCustom(msg);
                    }
                    onFailed?.Invoke();
                    break;

                case ShowResult.Failed:

                    if (!string.IsNullOrEmpty(msgError))
                    {
                        string msg = DLocalizationManager.Instance.GetLocalText(msgError);
                        ToastMessage.ShowCustom(msg);
                    }
                    onSkipped?.Invoke();
                    break;
            }
        };

        videoIsRequested = true;

        // Только если нет Admob Reward, обращаемся к Unity ADS.
        if (!AdmobController.Instance.RewardAdsOperator(onAdsShown))
        {
#if UNITY_ADS
            StartCoroutine(ShowRewardedVideo(onAdsShown));
#else
            onAdsShown?.Invoke(ShowResult.Failed);
#endif
        }
    }


#if UNITY_ADS
    // Рутина запросов к сервису Unity ADS.
    IEnumerator ShowRewardedVideo(System.Action<ShowResult> onComplete)
    {
        // Должна быть настроена автоматическая инициализация Unity ADS.
        if (!Advertisement.isInitialized)
        {
            //Debug.Log("Unity ADS not initialized!");
            onComplete(ShowResult.Failed);
            yield break;
        }

        // Платформа должна поддерживать Unity ADS.
        if(!Advertisement.isSupported)
        {
            //Debug.Log("Unity ADS not supported!");
            onComplete(ShowResult.Failed);
            yield break;
        }

        // Если всё готово к показу видео, то показываем.
        if (Advertisement.IsReady(PLACEMENT_ID))
        {
            Advertisement.Show(PLACEMENT_ID, new ShowOptions { resultCallback = onComplete });
            yield break;
        }

        // Если видео ещё не готово, периодически повторяем запрос.
        for (float attemptsTime = 0f; attemptsTime < REQUEST_TIME_LIMIT; attemptsTime += REQUEST_PERIOD)
        {
            yield return new WaitForSecondsRealtime(REQUEST_PERIOD);

            if (Advertisement.IsReady(PLACEMENT_ID))
            {
                Advertisement.Show(PLACEMENT_ID, new ShowOptions { resultCallback = onComplete });
                yield break;
            }
        }

        // Не дождались готовности видео.
        //Debug.Log("Failed to load ADS!");
        onComplete(ShowResult.Failed);
    }
#endif
}
