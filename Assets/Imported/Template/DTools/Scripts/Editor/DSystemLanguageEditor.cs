using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DTools
{
    public static class DSystemLanguageEditor
    {
        private const string MENU_PATH = "Tools/Локализация/Системный Язык [Editor Only]/";
        private const string EDITOR_PREFS_KEY = "DToolsLocalizationDefault";

        [MenuItem(MENU_PATH + "English")]
        public static void EnglishSet()
        {
            EditorPrefs.SetString(EDITOR_PREFS_KEY, "EN");
            UpdateLocalizationLanguage();
        }

        [MenuItem(MENU_PATH + "English", true)]
        public static bool EnglishValidate()
        {
            bool check = EditorPrefs.GetString(EDITOR_PREFS_KEY, "EN") == "EN";
            Menu.SetChecked(MENU_PATH + "English", check);
            return true;
        }

        [MenuItem(MENU_PATH + "Русский")]
        public static void RussianSet()
        {
            EditorPrefs.SetString(EDITOR_PREFS_KEY, "RU");
            UpdateLocalizationLanguage();
        }

        [MenuItem(MENU_PATH + "Русский", true)]
        public static bool RussianValidate()
        {
            bool check = EditorPrefs.GetString(EDITOR_PREFS_KEY, "RU") == "RU";
            Menu.SetChecked(MENU_PATH + "Русский", check);
            return true;
        }


        [MenuItem(MENU_PATH + "Немецкий")]
        public static void GermanSet()
        {
            EditorPrefs.SetString(EDITOR_PREFS_KEY, "DE");
            UpdateLocalizationLanguage();
        }

        [MenuItem(MENU_PATH + "Немецкий", true)]
        public static bool GermanValidate()
        {
            bool check = EditorPrefs.GetString(EDITOR_PREFS_KEY, "DE") == "DE";
            Menu.SetChecked(MENU_PATH + "Немецкий", check);
            return true;
        }


        [MenuItem(MENU_PATH + "Португальский")]
        public static void PortugueseSet()
        {
            EditorPrefs.SetString(EDITOR_PREFS_KEY, "PT");
            UpdateLocalizationLanguage();
        }

        [MenuItem(MENU_PATH + "Португальский", true)]
        public static bool PortugueseValidate()
        {
            bool check = EditorPrefs.GetString(EDITOR_PREFS_KEY, "PT") == "PT";
            Menu.SetChecked(MENU_PATH + "Португальский", check);
            return true;
        }
        /*
        [MenuItem(MENU_PATH + "Корейский")]
        public static void KoreanSet()
        {
            EditorPrefs.SetString(EDITOR_PREFS_KEY, "KO");
            UpdateLocalizationLanguage();
        }

        [MenuItem(MENU_PATH + "Корейский", true)]
        public static bool KoreanValidate()
        {
            bool check = EditorPrefs.GetString(EDITOR_PREFS_KEY, "KO") == "KO";
            Menu.SetChecked(MENU_PATH + "Корейский", check);
            return true;
        }

        [MenuItem(MENU_PATH + "Японский")]
        public static void JapaneseSet()
        {
            EditorPrefs.SetString(EDITOR_PREFS_KEY, "JP");
            UpdateLocalizationLanguage();
        }

        [MenuItem(MENU_PATH + "Японский", true)]
        public static bool JapaneseValidate()
        {
            bool check = EditorPrefs.GetString(EDITOR_PREFS_KEY, "JP") == "JP";
            Menu.SetChecked(MENU_PATH + "Японский", check);
            return true;
        }
        */
        private static void UpdateLocalizationLanguage()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            var textSources = Object.FindObjectsOfType<DTextSource>();
            var selfLocalizedTexts = Object.FindObjectsOfType<DLocalizeMe>();

            foreach (var source in textSources)
            {
                source.Init(forced: true);
            }
            foreach (var text in selfLocalizedTexts)
            {
                text.Init();
            }
        }
    }
}
