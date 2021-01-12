using UnityEngine;

namespace baseTemplate
{
    [CreateAssetMenu(fileName = "DefaultProfile", menuName = "Data/BasicConfig/DefaultProfile")]
    public class DefaultProfile : ScriptableObject
    {
        public GameData profileData;
    }
}