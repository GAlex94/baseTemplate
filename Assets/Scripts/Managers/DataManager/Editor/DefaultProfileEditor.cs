using UnityEditor;
using UnityEngine;

namespace baseTemplate
{
    [CustomEditor(typeof(DefaultProfile))]
    public class DefaultProfileEditor : Editor
    {
        DefaultProfile prop;
        Color orgColor;
        private bool isDisplayDeffaultInSpector;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            prop = (DefaultProfile)target;
            orgColor = GUI.color;
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Настройки профиля по умолчанию", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Данные настройки будут применимы при первом запуске игры", EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Включить отображение стадартного инспектора", EditorStyles.label, GUILayout.Width(300f));
            isDisplayDeffaultInSpector = EditorGUILayout.Toggle(isDisplayDeffaultInSpector, GUILayout.Width(150f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

       
            EditorGUIUtility.labelWidth = 250;
            GUI.color = orgColor;
            EditorGUILayout.LabelField("Profile Data", EditorStyles.boldLabel);
            prop.profileData.Version = EditorGUILayout.TextField("Версия ", prop.profileData.Version);
            prop.profileData.SoundValue = EditorGUILayout.Slider("Звук ", prop.profileData.SoundValue, 0f,1f);
            prop.profileData.MusicValue = EditorGUILayout.Slider("Музыка ", prop.profileData.MusicValue,0f, 1f);
            prop.profileData.TypeLanguage = (TypeLanguageEnum)EditorGUILayout.EnumPopup("Язык", prop.profileData.TypeLanguage);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Player Data", EditorStyles.boldLabel);
            prop.profileData.PlayerData.adsDisabled = EditorGUILayout.Toggle("Отключение рекламы", prop.profileData.PlayerData.adsDisabled);


            EditorGUILayout.Space();

            if(isDisplayDeffaultInSpector)
            {
                DrawDefaultInspector();
            }
            EditorGUILayout.Space();
            GUI.color = new Color(.70f, 0.8f, 0.76f, 1f);

            if (GUILayout.Button("<-- Все настройки игры"))
            {
                Selection.activeObject = SettingGameEditor.Instance;
            }
            
            GUI.color = orgColor;
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(prop);


        }
    }
}