using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace baseTemplate
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "SettingGame", menuName = "Data/Editors/SettingGame")]
    public class SettingGameEditor : ScriptableObject
    {
        public DefaultProfile DefaultProfile;

        public static SettingGameEditor instance;

        public static SettingGameEditor Instance
        {
            get
            {
                if (instance == null) instance = AssetDatabase.LoadAssetAtPath("Assets/Config/Editor/Resources/SettingGame.asset", typeof(SettingGameEditor)) as SettingGameEditor;
                return instance;
            }
        }
    }
}
#endif