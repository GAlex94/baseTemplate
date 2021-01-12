using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace DTools
{
    public class GUILocalizationTool : MonoBehaviour
    {
        [SerializeField]
        LocalizedText[] localizedTexts;

        [SerializeField]
        LocalizedTextMP[] localizedTextMPs;
        private void Start()
        {
            Localize();
        }

        void Localize()
        {
            foreach (var text in localizedTexts)
            {
                text.Localize();
            }

            foreach (var text in localizedTextMPs)
            {
                text.Localize();
            }
        }
    }

    [Serializable]
    public abstract class Localized
    {
         public string key;

        public abstract void Localize();
    }

    [Serializable]
    public class LocalizedText : Localized
    {
        public Text text;

        public override void Localize()
        {
            text.text = DLocalizationManager.Instance.GetLocalText(key);
        }
    }

    [Serializable]
    public class LocalizedTextMP : Localized
    {
        public TextMeshProUGUI text;

        public override void Localize()
        {
            text.text = DLocalizationManager.Instance.GetLocalText(key);
        }
    }
}




