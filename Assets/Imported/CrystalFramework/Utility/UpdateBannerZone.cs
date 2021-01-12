using System;
using UnityEngine;

namespace baseTemplate
{
    public class UpdateBannerZone : Singleton<UpdateBannerZone>
    {
        public Action<float> OnSetAdBannerHeight;
        private float mAdBannerHeight;
      
        
        private float sizeSafeArea = 0;
        public void AddSafeAreaZone(float size)
        {
            sizeSafeArea = size;
            OnSetAdBannerHeight?.Invoke(mAdBannerHeight);
        }

#if UNITY_EDITOR
        public float mAdBannerHeightTest;
#endif
        public float AdBannerHeight
        {
            get
            {
                return mAdBannerHeight;
            }
            set
            {
                float bannerUnitsHeight = value;
                if (GUIController.IsAwake)
                {
                    float bannerScreenPart = bannerUnitsHeight / GUIController.Instance.CanvasRectTransform.GetComponent<Canvas>()
                                                 .pixelRect.height;
                    bannerUnitsHeight = bannerScreenPart * (GUIController.Instance.CanvasRectTransform).sizeDelta.y;
                }
                mAdBannerHeight = bannerUnitsHeight;
                OnSetAdBannerHeight?.Invoke(mAdBannerHeight);
            }
        }

        public float SizeSafeArea => sizeSafeArea;

#if UNITY_EDITOR
        [ContextMenu("Test")]
        public void SetTest()
        {
            AdBannerHeight = mAdBannerHeightTest;
        }
#endif

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}