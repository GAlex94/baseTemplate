using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTools
{
    /// <summary>
    /// Автоматически генерируемый TextSource, который будет искать данные в файле "*/Resources/Localization.xml".
    /// </summary>
    public class DLocalizationManager : DTextSource
    {
        private static DLocalizationManager instance;
        public static DLocalizationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<DLocalizationManager>();
                }
                if (instance == null)
                {
                    instance = new GameObject("Auto_DLocalizationManager").AddComponent<DLocalizationManager>();
                    instance.resource = "Localization";
                    instance.runtimeLanguageDefinition = true;
                    instance.Init();
                }
                return instance;
            }
        }
    }
}