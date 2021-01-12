using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DTools
{
    [CustomEditor(typeof(DTextSource))]
    public class DTextSourceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Color errorColor = new Color32(0xFF, 0xBA, 0xBE, 0xFF);
            DTextSource source = target as DTextSource;

            using (new EditorGUI.DisabledGroupScope(true))
            {
                MonoScript script = MonoScript.FromMonoBehaviour(source);
                script = EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false) as MonoScript;
            }

            if (string.IsNullOrEmpty(source.resource))
            {
                UnityEngine.GUI.backgroundColor = errorColor;
                source.resource = EditorGUILayout.TextField("XML-ресурс", source.resource);
                UnityEngine.GUI.backgroundColor = Color.white;
            }
            else
            {
                source.resource = EditorGUILayout.TextField("XML-ресурс", source.resource);

                if (GUILayout.Button("Инициализировать", GUILayout.ExpandWidth(true), GUILayout.Height(20f)))
                {
                    source.Init();
                }
                EditorGUILayout.Separator();

                if (source.languageCodes != null && source.languageCodes.Count > 0)
                {
                    source.runtimeLanguageDefinition = EditorGUILayout.Toggle("Системный язык?", source.runtimeLanguageDefinition);

                    if (!source.runtimeLanguageDefinition)
                    {
                        int selected = Mathf.Max(0, source.languageCodes.IndexOf(source.language));
                        selected = EditorGUILayout.Popup("Язык", selected, source.languageCodes.ToArray());
                        source.language = source.languageCodes[selected];
                    }
                }
            }
        }
    }
}