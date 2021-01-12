using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

#if UNITY_EDITOR
namespace baseTemplate
{
    [CustomEditor(typeof(SettingGameEditor))]
    public class SettingGameEditorGUI : Editor
    {
        SettingGameEditor SettingGameAsset;
        Color originalGUIColor;
        private bool isDisplayDeffaultInSpector;
        [MenuItem("Tools/Edit Game Settings", false, -100)]
        public static void OpenRCCSettings()
        {
            Selection.activeObject = SettingGameEditor.Instance;
        }

        public override void OnInspectorGUI()
        {

            serializedObject.Update();
            SettingGameAsset = (SettingGameEditor) target;

            originalGUIColor = GUI.color;
            EditorGUIUtility.labelWidth = 250;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Настройки игры", EditorStyles.boldLabel);
            GUI.color = new Color(.75f, 1f, .75f);
            EditorGUILayout.LabelField("Доступ ко всем настройкам", EditorStyles.helpBox);
            GUI.color = originalGUIColor;

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Включить отображение стадартного инспектора", EditorStyles.label, GUILayout.Width(300f));
            isDisplayDeffaultInSpector = EditorGUILayout.Toggle(isDisplayDeffaultInSpector, GUILayout.Width(150f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (isDisplayDeffaultInSpector)
            {
                DrawDefaultInspector();
                EditorGUILayout.Space();
            }
 


            GUI.color = new Color(.70f, 0.8f, 0.76f, 1f);

            if (GUILayout.Button("Дефолтный профиль"))
            {
                Selection.activeObject = SettingGameAsset.DefaultProfile;
            }
            EditorGUILayout.Space();

        }
    }
}
#endif