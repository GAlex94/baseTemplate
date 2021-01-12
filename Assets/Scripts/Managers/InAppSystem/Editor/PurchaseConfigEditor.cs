using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace baseTemplate
{
    [CustomEditor(typeof(PurchaseConfig))]
    public class PurchaseConfigEditor : Editor
    {
        PurchaseConfig prop;
        Color orgColor;
        private bool isDisplayDeffaultInSpector;
        List<PurchaseInfo> emptyInfos = new List<PurchaseInfo>();

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            prop = (PurchaseConfig) target;
            orgColor = GUI.color;
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Настройки инапов", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Включить отображение стадартного инспектора", EditorStyles.label,
                GUILayout.Width(300f));
            isDisplayDeffaultInSpector = EditorGUILayout.Toggle(isDisplayDeffaultInSpector, GUILayout.Width(150f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (isDisplayDeffaultInSpector)
            {
                DrawDefaultInspector();
                EditorGUILayout.Space();
            }


            EditorGUIUtility.labelWidth = 250;
            GUI.color = orgColor;
            emptyInfos = new List<PurchaseInfo>();
            GUILayout.Label("Продукты: ", EditorStyles.boldLabel);
            for (int i = 0; i < prop.purchases.Length; i++)
            {

                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(prop.purchases[i].NameEditor, EditorStyles.boldLabel);
                if (GUILayout.Button("[Удалить]", GUILayout.Width(80f)))
                {
                    Remove(i);
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                if (i >= prop.purchases.Length)
                    return;

                prop.purchases[i].NameEditor = EditorGUILayout.TextField("Название в редакторе",
                    prop.purchases[i].NameEditor, GUILayout.Width(500f));
                prop.purchases[i].type = (GameProductType) EditorGUILayout.EnumPopup("Группа продукта",
                    prop.purchases[i].type, GUILayout.Width(500f));
                prop.purchases[i].gameProduct = (GameProductEnum) EditorGUILayout.EnumPopup("Тип продукта",
                    prop.purchases[i].gameProduct, GUILayout.Width(500f));
                prop.purchases[i].productTitle = EditorGUILayout.TextField("Ключ продукта",
                    prop.purchases[i].productTitle, GUILayout.Width(500f));
                prop.purchases[i].productDescr = EditorGUILayout.TextField("Ключ описания",
                    prop.purchases[i].productDescr, GUILayout.Width(500f));
                prop.purchases[i].behavior =
                    (UnityEngine.Purchasing.ProductType) EditorGUILayout.EnumPopup("Тип покупки",
                        prop.purchases[i].behavior, GUILayout.Width(500f));
                prop.purchases[i].typeBuyProduct = (BuyProductType) EditorGUILayout.EnumPopup("Валюта покупки",
                    prop.purchases[i].typeBuyProduct, GUILayout.Width(500f));

                if (prop.purchases[i].typeBuyProduct == BuyProductType.Real)
                {
                    prop.purchases[i].productId = EditorGUILayout.TextField("Id продукта (Android)",
                        prop.purchases[i].productId, GUILayout.Width(500f));
                    prop.purchases[i].productIdIOS = EditorGUILayout.TextField("Id продукта iOS",
                        prop.purchases[i].productIdIOS, GUILayout.Width(500f));
                }
                else
                {
                    if (prop.purchases[i].typeBuyProduct != BuyProductType.WatchAds && prop.purchases[i].typeBuyProduct != BuyProductType.Real)
                    {
                        prop.purchases[i].cost =
                            EditorGUILayout.IntField("Стоимость", prop.purchases[i].cost, GUILayout.Width(500f));
                    }
                }

                prop.purchases[i].productIcon = (Sprite) EditorGUILayout.ObjectField("Иконка",
                    prop.purchases[i].productIcon, typeof(Sprite), false, GUILayout.Width(500f));
                EditorGUILayout.EndVertical();

            }


            GUI.color = Color.green;
            GUI.color = orgColor;

            EditorGUILayout.Space();
            if (GUILayout.Button("Добавить новый продукт"))
            {
                AddNew();
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

        void AddNew()
        {
            emptyInfos.Clear();
            emptyInfos.AddRange(prop.purchases);
            PurchaseInfo newEmpty = new PurchaseInfo();
            emptyInfos.Add(newEmpty);
            prop.purchases = emptyInfos.ToArray();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(prop);
        }

        void Remove(int index)
        {
            emptyInfos.Clear();
            emptyInfos.AddRange(prop.purchases);
            emptyInfos.RemoveAt(index);
            prop.purchases = emptyInfos.ToArray();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(prop);

        }

    }
}



