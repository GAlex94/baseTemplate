using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using UnityEngine;

namespace DTools
{
    /// <summary>
    /// Представление гугл-таблицы локализации. 
    /// </summary>
    /// <remarks>
    /// В инспекторе в поле "XML-ресурс" нужно прописать ресурс-путь до XML-версии листа гугл-таблицы.
    /// После инициализации можно будет выбрать язык перевода из доступных столбцов.
    /// В финальной версии обязательно ставим галочку "Системный язык".
    /// </remarks>
    public class DTextSource : MonoBehaviour
    {
        protected const string DEFAULT_LANGUAGE_CODE = "EN";

        // В этом словаре должны быть все используемые языки.
        // Любой столбец из гугл-таблицы, которого нет в этом словаре, будет считаться ключом.
        protected readonly Dictionary<SystemLanguage, string> _SystemLanguagesToCodes = new Dictionary<SystemLanguage, string>()
        {
            { SystemLanguage.English, "EN" },
            { SystemLanguage.Russian, "RU" },
            { SystemLanguage.ChineseSimplified, "CN" }, 
            { SystemLanguage.Japanese, "JP" },
            { SystemLanguage.German, "DE" },
            { SystemLanguage.Portuguese, "PT"},
      //      { SystemLanguage.Korean, "KO"},
     //       { SystemLanguage.French, "FR"}
        };

        /// <summary>
        /// Конструктор составного ключа из произвольного количества столбцов.
        /// </summary>
        public class Key
        {
            public class EqualityComparer : IEqualityComparer<Key>
            {
                public bool Equals(Key x, Key y)
                {
                    System.Func<Key, Key, bool> elementsEqual = (a, b) =>
                    {
                        for (int i = 0; i < a._allKeys.Count; i++)
                        {
                            if (!string.Equals(a._allKeys[i], b._allKeys[i]))
                            {
                                return false;
                            }
                        }
                        return true;
                    };

                    bool result = x._allKeys != null && y._allKeys != null && x._allKeys.Count == y._allKeys.Count && elementsEqual(x, y);

                    return result;
                }

                public int GetHashCode(Key obj)
                {
                    int hash = 19;
                    foreach (var foo in obj._allKeys)
                    {
                        hash = hash * 31 + foo.GetHashCode();
                    }
                    return hash;
                }
            }

            public List<string> _allKeys;

            private Key(string single)
            {
                _allKeys = new List<string>();
                _allKeys.Add(single);
            }

            private Key(IEnumerable<string> multiple)
            {
                _allKeys = new List<string>();
                foreach (var single in multiple)
                {
                    _allKeys.Add(single);
                }
            }

            public static implicit operator List<string>(Key key)
            {
                return key._allKeys;
            }

            public static implicit operator Key(List<string> list)
            {
                return new Key(list);
            }

            public static implicit operator Key(string single)
            {
                return new Key(single);
            }

            public override string ToString()
            {
                if (_allKeys == null || _allKeys.Count == 0)
                {
                    return "";
                }

                string result = "[" + _allKeys[0] + "]";
                for (int i = 1; i < _allKeys.Count; i++)
                {
                    result += ", " + _allKeys[i];
                }
                return result;
            }
        }

        // Сопоставление ключа строке перевода на выбранный язык.
        protected Dictionary<Key, string> dictionary;
        public int KeysTotal
        {
            get
            {
                return dictionary == null ? 0 : dictionary.Count;
            }
        }

        public string resource;

        public List<string> extraKeys;
        public List<string> languageCodes;

        public string language;
        public bool runtimeLanguageDefinition = true;

        private bool inited = false;

        public void Init(bool forced = false)
        {
            if (inited && !forced)
            {
                return;
            }

            if (runtimeLanguageDefinition)
            {
#if UNITY_EDITOR
                const string EDITOR_PREFS_KEY = "DToolsLocalizationDefault";
                string editorLanguage = UnityEditor.EditorPrefs.GetString(EDITOR_PREFS_KEY, "EN");

                if (_SystemLanguagesToCodes.ContainsValue(editorLanguage))
                {
                    language = editorLanguage;
                }
                else
                {
                    language = DEFAULT_LANGUAGE_CODE;
                }
#else
                if (!_SystemLanguagesToCodes.TryGetValue(Application.systemLanguage, out language))
                {
                    language = DEFAULT_LANGUAGE_CODE;
                }
#endif
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(((TextAsset)(Resources.Load(resource, typeof(TextAsset)))).text);

            var singleNode = doc.DocumentElement.SelectSingleNode("*");
            var allFields = new List<string>();

            for (int i = 0; i < singleNode.Attributes.Count; i++)
            {
                allFields.Add(singleNode.Attributes[i].Name);
            }

            languageCodes = allFields.Where(f => _SystemLanguagesToCodes.Values.Contains(f)).ToList();
            extraKeys = allFields.Where(f => !_SystemLanguagesToCodes.Values.Contains(f)).ToList();

            if (string.IsNullOrEmpty(language) || !languageCodes.Contains(language))
            {
                language = DEFAULT_LANGUAGE_CODE;
            }

            var nodeList = doc.DocumentElement.SelectNodes("*[@" + language + "]");
            dictionary = new Dictionary<Key, string>(nodeList.Count, new Key.EqualityComparer());
            for (int i = 0; i < nodeList.Count; i++)
            {
                Key key = nodeList[i].Name;
                key._allKeys.AddRange(extraKeys.Select(k => nodeList[i].Attributes[k].InnerXml));

                string value = nodeList[i].Attributes[language].InnerXml;
                if (value == "")
                {
                    value = nodeList[i].Attributes[DEFAULT_LANGUAGE_CODE].InnerXml;
                }

                if (!dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, value);
                }
                else
                {
                    Debug.Log("В словаре локализации есть дубликаты! Они будут пропущены.");
                }
            }

            inited = true;
        }

        // Метод на случай если в ходе игры нужно поменять язык.
        public void ChangeLanguage(string other)
        {
            if (!languageCodes.Contains(other))
            {
                Debug.Log("Язык сменён на неизвестый, будет использован язык по умолчанию.");
            }

            language = other;

            inited = false;
            Init();
        }

        // Вызывать вручную нужно только этот метод.
        /// <summary>
        /// Получить строку по ключу локализации, простому или составному.
        /// </summary>
        /// <param name="key">Простой ключ (первый столбец)</param>
        /// <param name="moreKeys"></param>
        /// <returns></returns>
        public string GetLocalText(string key, params string[] moreKeys)
        {
            Init();

            if (moreKeys == null || moreKeys.Length == 0)
            {
                return GetLocalText((Key)key);
            }
            else
            {
                var keyList = new List<string>();
                keyList.Add(key);
                keyList.AddRange(moreKeys);

                return GetLocalText(keyList);
            }
        }

        /// <summary>
        /// Получить строку локализации не по ключу, но по индексу записи в словаре.
        /// </summary>
        /// <param name="index">Индекс записи</param>
        /// <returns></returns>
        public string GetLocalText(int index)
        {
            Init();

            if (index >= 0 && index < KeysTotal)
            {
                return dictionary.ElementAt(index).Value;
            }
            return "";
        }

        private string GetLocalText(Key key)
        {
            string result;
            if (dictionary.TryGetValue(key, out result))
            {
                return result;
            }
            Debug.LogError("Не найден ключ локализации: " + key);
            return "";
        }
    }
}