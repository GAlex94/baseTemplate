using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Substitute = DTools.DLocalizeMe.Substitute;

namespace DTools
{
    [CustomEditor(typeof(DLocalizeMe))]
    public class DLocalizeMeEditor : Editor
    {
        private bool editorNeedTextSourceLink;
        private bool editorNeedSubstitutes;

        public override void OnInspectorGUI()
        {
            DLocalizeMe source = target as DLocalizeMe;

            EditorGUILayout.Separator();

            source.code = EditorGUILayout.DelayedTextField("Кодовая строка:", source.code);           
            
            bool needSourceLink = source.textSource == null ? editorNeedTextSourceLink : true;
            using (new EditorGUI.DisabledScope(source.textSource != null))
            {
                editorNeedTextSourceLink = EditorGUILayout.Toggle("Спец источник?", needSourceLink);
            }

            if (editorNeedTextSourceLink)
            {
                source.textSource = (DTextSource)EditorGUILayout.ObjectField("Источник", source.textSource, typeof(DTextSource), true);
            }

            if (source.substitutes == null)
            {
                source.substitutes = new List<Substitute>();
            }

            bool needSubstitutes = source.substitutes.Count == 0 ? editorNeedSubstitutes : true;
            using (new EditorGUI.DisabledScope(source.substitutes.Count != 0))
            {
                editorNeedSubstitutes = EditorGUILayout.Toggle("Нужны замены?", needSubstitutes);
            }

            if (needSubstitutes)
            {
                int count = source.substitutes.Count;
                float tableWidth = (Screen.width - 100f) / 2;
                float tableHeight = count * 20;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("#", GUILayout.Width(20f));
                EditorGUILayout.LabelField("Код замены", GUILayout.Width(tableWidth));
                EditorGUILayout.LabelField("Результат", GUILayout.Width(tableWidth));
                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < count; i++)
                {
                    var substitute = source.substitutes[i];

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField((i + 1).ToString() + ")", GUILayout.Width(20f), GUILayout.Height(tableHeight / count));
                    substitute.code = EditorGUILayout.DelayedTextField(substitute.code, GUILayout.Width(tableWidth), GUILayout.Height(tableHeight / count));
                    substitute.result = EditorGUILayout.DelayedTextField(substitute.result, GUILayout.Width(tableWidth), GUILayout.Height(tableHeight / count));
                    if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(tableHeight / count)))
                    {
                        source.substitutes.RemoveAt(i);
                        count--;
                    };

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Новая замена", GUILayout.ExpandWidth(true), GUILayout.Height(20f)))
                {
                    source.substitutes.Add(new Substitute());
                };
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
