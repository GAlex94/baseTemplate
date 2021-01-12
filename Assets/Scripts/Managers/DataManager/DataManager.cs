using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Common;
using DTools;
using UnityEngine;

namespace baseTemplate
{
    public class DataManager : Singleton<DataManager>
    {
        private string _profileName;
        private bool _clearProfileOnStart;

        [SerializeField] private GameData _data = new GameData();

        private bool _dataDirty = false;

        private DefaultProfile _defaultProfile;

        private List<ILanguageListener> _languageListeners = new List<ILanguageListener>();

        public GameData GetCurrentData => _data;
        public PlayerData PlayerData => _data.PlayerData;


        private string FilePath => Path.Combine(Application.persistentDataPath, _profileName + ".json");
        public bool IsAdsDisabled => _data.PlayerData.adsDisabled;


        #region MainLogic

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            if (_clearProfileOnStart)
            {
                Clear();
            }
            else
            {
                Load();
            }
        }

        public void Init(string profileName, bool clearProfileOnStart, DefaultProfile defaultProfile,
            string instanceLanguage)
        {
            this._profileName = profileName;
            this._clearProfileOnStart = clearProfileOnStart;
            this._defaultProfile = defaultProfile;
            this._defaultProfile.profileData.TypeLanguage =
                instanceLanguage == "RU" ? TypeLanguageEnum.Russian : TypeLanguageEnum.English;

            if (!Debug.isDebugBuild)
                this._clearProfileOnStart = false;
        }

        public void Clear()
        {
            _data = _defaultProfile != null ? _defaultProfile.profileData : new GameData();

            Save();

            if (File.Exists(FilePath))
            {
                Load();
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("Profile not saved! Check file system!");
#endif
                _data = new GameData();
            }

            SetSoundValue(_data.SoundValue, false);
            SetMusicValue(_data.MusicValue, false);
        }

        private void CheckFirstSession()
        {
            if (_clearProfileOnStart)
            {
                PlayerPrefs.DeleteAll();
            }

            if (!PlayerPrefs.HasKey("FirstSession"))
            {
                PlayerPrefs.SetInt("FirstSession", 1);
            }

            PlayerPrefs.Save();
            Debug.Log("Set default quality");
        }

        [ContextMenu("Save")]
        public void Save()
        {
            File.WriteAllText(FilePath, Base64.Encode(JsonUtility.ToJson(_data, false)));
        }

        private void Load()
        {
            if (File.Exists(FilePath))
            {
                _data = JsonUtility.FromJson<GameData>(Base64.Decode(File.ReadAllText(FilePath)));

                UpdateRuntimeByLoadedData();
            }
            else
            {
                Clear();
            }

            //TODO: костыль для проверки первой сессии
            CheckFirstSession();
        }

        private void SetDataDirty()
        {
            if (_dataDirty == false)
            {
                _dataDirty = true;
                Invoke("DefferSave", 1.0f);
            }
        }

        private void DefferSave()
        {
            Save();
            _dataDirty = false;
        }

        private void UpdateRuntimeByLoadedData()
        {
            SetSoundValue(_data.SoundValue, false);
            SetMusicValue(_data.MusicValue, false);
        }

        #endregion

        #region Sound and Music

        public void SetSoundValue(float soundValue, bool autoSave = true)
        {
            _data.SoundValue = soundValue;
            SoundManager.globalSoundsVolume = soundValue;
            if (autoSave)
                Save();
        }

        public float SoundValue
        {
            get { return _data.SoundValue; }
        }

        public void SetMusicValue(float musicValue, bool autoSave = true)
        {
            _data.MusicValue = musicValue;
            SoundManager.globalMusicVolume = musicValue;
            if (autoSave)
                Save();
        }

        public float MusicValue
        {
            get { return _data.MusicValue; }
        }

        #endregion

        #region Language

        public void SetLanguage(TypeLanguageEnum language, bool autoSave = true)
        {
            _data.TypeLanguage = language;
            switch (language)
            {
                case TypeLanguageEnum.English:
                    PlayerPrefs.SetInt("language", 1);
                    break;
                case TypeLanguageEnum.Russian:
                    PlayerPrefs.SetInt("language", 0);
                    break;
            }

            DLocalizationManager.Instance.ChangeLanguage(language == TypeLanguageEnum.Russian ? "RU" : "EN");

            if (_languageListeners.Count > 0)
            {
                _languageListeners.ForEach(curListener => curListener.OnLanguageChange(_data.TypeLanguage));
            }

            if (autoSave)
            {
                SetDataDirty();
            }
        }

        public void AddLanguageListener(ILanguageListener listener)
        {
            if (!_languageListeners.Contains(listener))
                _languageListeners.Add(listener);
        }

        public void RemoveLanguageListener(ILanguageListener listener)
        {
            _languageListeners.Remove(listener);
        }

        #endregion

        #region Control Main Data Game

        [ContextMenu("Delete")]
        public void DeleteSave()
        {
            if (PlayerPrefs.HasKey("FirstSession"))
            {
                PlayerPrefs.DeleteKey("FirstSession");
            }

            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }

        public void DisableAds()
        {
            _data.PlayerData.adsDisabled = true;
            AdmobController.Instance.RemoveAds();
            SetDataDirty();
        }

        public void UnlockAllStory()
        {
          Debug.LogError("Need unlock all stories");
        }
        #endregion


    }
}
