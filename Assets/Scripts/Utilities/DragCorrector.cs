using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(EventSystem))]
    [AddComponentMenu("UI/Extensions/DragCorrector")]
    public class DragCorrector : MonoBehaviour
    {
        public int baseTH = 6;
        public int basePPI = 210;
        public int dragTH = 0;

        void Start()
        {
            dragTH = baseTH * (int)Screen.dpi / basePPI;

            EventSystem es = GetComponent<EventSystem>();

            if (es)
            {
                es.pixelDragThreshold = dragTH;
            }
        }
    }
}