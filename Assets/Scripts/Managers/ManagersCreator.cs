using DTools;
using UnityEngine;

namespace baseTemplate
{
    public class ManagersCreator : MonoBehaviour
    {
        [Header("Game managers settings")]
        [SerializeField] private bool isDebug = true;
        [SerializeField] private StateGameEnum curStateGame = StateGameEnum.Menu;
        
        [Header("Sounds")]
        [SerializeField] private AudioClip musicAudioClip;
        [SerializeField] private float musicValue;
        

        [Header("Data managers settings")] 
        [SerializeField] private string profileName = "MainProfile";
        [SerializeField] private bool clearProfile = false;
        [SerializeField] private DefaultProfile defaultProfile = null;

        [Header("GameShop")]
        [SerializeField] private bool isFakeShop = true;
        [SerializeField] private PurchaseConfig purchaseConfig = null;

        public string ProfileName => profileName;

        void Awake()
        {
            if (!GameManager.IsAwake)
            {
                DataManager.Instance.Init(ProfileName, clearProfile, defaultProfile, DLocalizationManager.Instance.language);
                GameManager.Instance.Init(isDebug, isFakeShop, curStateGame, musicAudioClip, musicValue);

                Screen.sleepTimeout = SleepTimeout.NeverSleep;

 
                Application.targetFrameRate = 60;

#if !UNITY_EDITOR
                if (!Debug.isDebugBuild)
                {
					Debug.unityLogger.logEnabled = false;
                }
#endif
            }
        }
    }
}