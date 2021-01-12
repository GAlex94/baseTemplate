using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace baseTemplate
{
    public class GUIScreen : MonoBehaviour
    {
        [SerializeField] private ScreenLayer guiLayer = ScreenLayer.None;

        [SerializeField] private EffectType appearEffect = EffectType.None;

        [SerializeField] private EffectType fadeEffect = EffectType.None;
        [SerializeField] protected RectTransform panelScreenTransform;
        [SerializeField] protected CanvasGroup canvasGroup;

        private bool showed = false;
        [SerializeField] protected bool ignoreLastScreen = false;


        private enum EffectType
        {
            Scale, Alpha, None
        }

        public bool IsShowed => showed;

        public ScreenLayer ScreenLayer => guiLayer;

        public int OffsetZ { get; set; } = 0;

        public bool IgnoreLastScreen => ignoreLastScreen;

        public void Show()
        {
            showed = true;
            ApplyEffect(true);
            StartCoroutine(OnShowNextFrame());
        }

        private IEnumerator OnShowNextFrame()
        {
            yield return null;
            OnShow();
        }

        public void Hide()
        {
            showed = false;
            ApplyEffect(false);
            OnHide();
        }
        
        protected virtual void OnShow()
        {

        }

        protected virtual void OnHide()
        {

        }

        protected virtual void EndEffect()
        {

        }

        private void ApplyEffect(bool isAppear)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();

            if (isAppear)
            {
                gameObject.SetActive(true);
                switch (appearEffect)
                {
                    case EffectType.Scale:
                    {
                        if (panelScreenTransform != null)
                        {
                            panelScreenTransform.DOKill();
                            panelScreenTransform.localScale = Vector3.zero;
                            panelScreenTransform.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutExpo)
                                .OnComplete(() =>
                                {
                                    panelScreenTransform.localScale = Vector3.one;
                                    EndEffect();
                                });
                        }
                        else
                        {
                            transform.DOKill();
                            transform.localScale = Vector3.zero;
                            transform.DOScale(Vector3.one, .2f).SetEase(Ease.InOutExpo).OnComplete(() =>
                            {
                                transform.localScale = Vector3.one;
                                EndEffect();
                            });
                        }

                        break;
                    }

                    case EffectType.Alpha:
                    {
                        if (panelScreenTransform != null)
                        {
                            panelScreenTransform.DOKill();
                            panelScreenTransform.gameObject.SetActive(true);
                        }

                        if (canvasGroup != null)
                        {
                            canvasGroup.DOKill();
                            canvasGroup.DOFade(0, 0f);
                            canvasGroup.DOFade(1, 0.2f).OnComplete(() =>
                            {
                                canvasGroup.alpha = 1;
                                EndEffect();
                            });
                        }
                        else
                        {
                            Debug.LogError("Need canvasGroup");
                            appearEffect = EffectType.None;
                            ApplyEffect(true);
                        }

                        break;
                    }

                    case EffectType.None:
                    {
                        if (panelScreenTransform != null)
                        {
                            panelScreenTransform.DOKill();
                            panelScreenTransform.localScale = Vector3.one;
                            panelScreenTransform.gameObject.SetActive(true);
                        }
                        else
                        {
                            transform.DOKill();
                            transform.localScale = Vector3.one;
                        }

                        if (canvasGroup != null)
                        {
                            canvasGroup.DOKill();
                            canvasGroup.DOFade(1, 0f);
                        }

                        break;
                    }
                }
            }
            else
            {
                switch (fadeEffect)
                {
                    case EffectType.Scale:
                    {

                        if (panelScreenTransform != null)
                        {
                            panelScreenTransform.DOKill();
                            panelScreenTransform.localScale = Vector3.one;
                            panelScreenTransform.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InOutExpo)
                                .OnComplete(() => { this.gameObject.SetActive(false); });
                        }
                        else
                        {
                            transform.DOKill();
                            transform.localScale = Vector3.one;
                            transform.DOScale(Vector3.zero, .2f).SetEase(Ease.InOutExpo).OnComplete(() =>
                            {
                                this.gameObject.SetActive(false);
                            });
                        }

                        break;
                    }

                    case EffectType.Alpha:
                    {

                        if (panelScreenTransform != null)
                        {
                            panelScreenTransform.DOKill();
                            panelScreenTransform.gameObject.SetActive(false);
                        }

                        if (canvasGroup != null)
                        {
                            canvasGroup.DOKill();
                            canvasGroup.DOFade(1, 0f);
                            canvasGroup.DOFade(0, 0.2f).OnComplete(() =>
                            {
                                canvasGroup.alpha = 0;
                                this.gameObject.SetActive(false);
                            });
                        }
                        else
                        {
                            Debug.LogError("Need canvasGroup");
                            appearEffect = EffectType.None;
                            ApplyEffect(false);
                        }

                        break;
                    }

                    case EffectType.None:
                    {
                        if (panelScreenTransform != null)
                        {
                            panelScreenTransform.DOKill();
                            panelScreenTransform.localScale = Vector3.zero;
                            panelScreenTransform.gameObject.SetActive(false);
                        }
                        else
                        {
                            transform.DOKill();
                            transform.localScale = Vector3.zero;
                        }

                        if (canvasGroup != null)
                        {
                            canvasGroup.DOKill();
                            canvasGroup.DOFade(0, 0f);
                        }

                        gameObject.SetActive(false);
                        break;
                    }
                }
            }
        }

        public virtual void BackButton()
        {
            GUIController.Instance.ShowScreen<ScreenMessageExit>();
        }

        public virtual void ShowTips()
        {
        }
    }
}
