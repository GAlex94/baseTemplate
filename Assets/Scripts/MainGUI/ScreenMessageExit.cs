using UnityEngine;
using UnityEngine.UI;

namespace baseTemplate
{
    public class ScreenMessageExit : GUIScreen
    {
        [Header("ScreenMessage")] 
        [SerializeField] private Button closeButton;

        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;

        [Header("Sound")] [SerializeField] private AudioClip buttonClickAudioClip;
        [SerializeField] private AudioClip openScreenAudioClip;

        private void ClosePopup()
        {
            if (buttonClickAudioClip != null) SoundManager.PlaySound(buttonClickAudioClip);
            GUIController.Instance.HideScreen<ScreenMessageExit>();
        }

        private void Awake()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(ClosePopup);
            }

            noButton.onClick.RemoveAllListeners();
            noButton.onClick.AddListener(ClosePopup);

            yesButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(ExitGame);
        }

        protected override void EndEffect()
        {
            if (openScreenAudioClip != null) SoundManager.PlaySound(openScreenAudioClip);
            Time.timeScale = 0;
        }

        protected override void OnHide()
        {
            Time.timeScale = 1;
        }

        private void ExitGame()
        {
            Application.Quit();
        }

        public override void BackButton()
        {
            ClosePopup();
        }
    }
}