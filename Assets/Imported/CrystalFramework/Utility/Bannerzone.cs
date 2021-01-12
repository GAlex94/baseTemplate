using Crystal;
using UnityEngine;
using UnityEngine.UI;

namespace baseTemplate
{
    public class Bannerzone : MonoBehaviour
    {
        public LayoutElement bannerContainer;
        public float PercentSize;
  
        private void Start()
        {
            UpdateBannerZone.Instance.OnSetAdBannerHeight += SetBannerContainer;
        }

        private void OnEnable()
        {
            SetBannerContainer(UpdateBannerZone.Instance.AdBannerHeight);
        }

        private void OnDestroy()
        {
            if (UpdateBannerZone.IsAwake && UpdateBannerZone.Instance.OnSetAdBannerHeight != null)
                UpdateBannerZone.Instance.OnSetAdBannerHeight -= SetBannerContainer;
        }


        private void SetBannerContainer(float size)
        {
            PercentSize = 15;
            if (bannerContainer == null)
            {
                var rectTransform = transform as RectTransform;
                if (rectTransform != null)
                {
                    var Y = -(size + (size > 0 ? size / 100f * PercentSize : 0f));
                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, Y);
                }
            }
            else
            {
                if (size > 0)
                {
#if UNITY_IOS || UNITY_EDITOR
                    bannerContainer.minHeight = size + (size / 100 * PercentSize) + UpdateBannerZone.Instance.SizeSafeArea;
#else
                     bannerContainer.minHeight = size + (size / 100 * PercentSize) ;
#endif

                }
                else
                {
                    bannerContainer.minHeight = 0;
                }
            }

        }
    }
}