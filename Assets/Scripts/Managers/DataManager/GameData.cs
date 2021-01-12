using System;

namespace baseTemplate
{
    [Serializable]
    public class GameData
    {
        public string Version;
        public float SoundValue;
        public float MusicValue;
        public PlayerData PlayerData;
        public TypeLanguageEnum TypeLanguage;

        public GameData()
        {
            Version = "1.0";
            SoundValue = 1;
            MusicValue = 1;
            TypeLanguage = TypeLanguageEnum.English;
        }
    }

    #region PlayerData

    [Serializable]
    public class PlayerData
    {
        public bool adsDisabled;
        PlayerData()
        {
            adsDisabled = false;
        }
    }
    #endregion

   
    public enum TypeLanguageEnum
    {
        English,
        Russian,
    }
}