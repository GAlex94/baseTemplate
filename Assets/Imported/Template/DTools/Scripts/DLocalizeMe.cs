using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
#if UNITY_2018_2_OR_NEWER
using TMPro;
#endif

namespace DTools
{
    /// <summary>
    /// Самолокализуемый текст. Если не указан список замен, само содержимое кодовой строки будет считаться ключом локализации.
    /// </summary>
    /// <remarks>
    /// Можно указать ссылку на конкретный источник текстов. 
    /// Если источник текстов не указан, будем искать перевод в "*/Resources/Localization.xml". 
    /// 
    /// TODO: описать принцип работы замен.
    /// 
    /// Кейс локализации по "мультиключу" здесь не рассматриваем.
    /// </remarks>
    public class DLocalizeMe : MonoBehaviour
    {
        [System.Serializable]
        public class Substitute
        {
            private static Dictionary<string, System.Func<string>> SpecialResults;
            public static void RegisterSpecial(string match, System.Func<string> funcReplace)
            {
                if (SpecialResults == null)
                {
                    SpecialResults = new Dictionary<string, System.Func<string>>();
                }

                if (SpecialResults.ContainsKey(match))
                {
                    if (funcReplace == null)
                    {
                        SpecialResults.Remove(match);
                    }
                    else
                    {
                        SpecialResults[match] = funcReplace;
                    }
                }
                else
                {
                    SpecialResults.Add(match, funcReplace);
                }
            }

            public string code;
            public string result;

            public string Apply(DTextSource textSource, string input)
            {
                if (string.IsNullOrEmpty(code) || string.IsNullOrWhiteSpace(code))
                {
                    return input;
                }

                string finalResult;
                if (SpecialResults.ContainsKey(result))
                {
                    finalResult = SpecialResults[result]();
                }
                else
                {
                    if (textSource != null)
                    {
                        finalResult = textSource.GetLocalText(result);
                    }
                    else
                    {
                        finalResult = DLocalizationManager.Instance.GetLocalText(result);
                    }
                }
                return input.Replace(code, finalResult);
            }
        }

        public DTextSource textSource;
        public string code;
        public List<Substitute> substitutes;

        private bool testedForTextComponent;
        private bool hasTextComponent;
        private bool isTextPro;
        private Text localizedText;
#if UNITY_2018_2_OR_NEWER
        private TMP_Text localizedTextPro;
#endif

        private void SetLocalizedText(string text)
        {
            if (!testedForTextComponent)
            {
                localizedText = GetComponent<Text>();
                hasTextComponent = localizedText;
                isTextPro = false;

#if UNITY_2018_2_OR_NEWER
                localizedTextPro = GetComponent<TMP_Text>();
                hasTextComponent = localizedText || localizedTextPro;
                isTextPro = localizedTextPro;
#endif
                testedForTextComponent = true;
            }
            if (!hasTextComponent)
            {
                return;
            }

            if (isTextPro)
            {
#if UNITY_2018_2_OR_NEWER
                localizedTextPro.text = text;
#endif
            }
            else
            {
                localizedText.text = text;
            }
        }

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            if (substitutes != null && substitutes.Count > 0)
            {
                string result = code;
                foreach (var substitute in substitutes)
                {
                    result = substitute.Apply(textSource, result);
                }
                SetLocalizedText(result);
            }
            else
            {
                if (textSource != null)
                {
                    SetLocalizedText(textSource.GetLocalText(code));
                }
                else
                {
                    SetLocalizedText(DLocalizationManager.Instance.GetLocalText(code));
                }
            }
        }
    }
}
