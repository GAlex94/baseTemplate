using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace baseTemplate
{
    public class ScreenFakeLoading : GUIScreen
    {
        [Header("ScreenFakeLoading")] 
        [SerializeField] private Slider _progressSlider;

        [SerializeField] private float _fakeTimeValue;
        [SerializeField] private float _fakeHelperTimeValue;
        [SerializeField] private float _startSliderValue;
        [SerializeField] private float _startHelperSliderValue;


        [Header("Debug")] [SerializeField] private Text _loadingGameDataText;

        private Action _fakeEndAction;

        public void Init(Action endFakeAction)
        {
            _fakeEndAction = endFakeAction;

            StartCoroutine(LoadGame());
        }

        private IEnumerator LoadGame()
        {
            var currentTimeWait = 0f;
            var fakeTime = _fakeTimeValue;
            var fakeValue = _startSliderValue;
            if (GameManager.IsAwake && !GameManager.IsFirstStart)
            {
                fakeTime = _fakeHelperTimeValue;
                fakeValue = _startHelperSliderValue;
                GameManager.IsFirstStart = true;
            }

            _progressSlider.value = fakeValue;

            yield return new WaitForSecondsRealtime(0.1f);

            while (currentTimeWait < fakeTime)
            {
                yield return new WaitForEndOfFrame();
                currentTimeWait += Time.deltaTime;
                _progressSlider.value = fakeValue + (1 - fakeValue) * currentTimeWait / fakeTime;
            }

            _fakeEndAction?.Invoke();
            GUIController.Instance.HideScreen(this);
        }

    }
}