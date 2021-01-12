using System;
using UnityEngine;
using UnityEngine.UI;

public class FakeAdsPanel: MonoBehaviour
{
    public Button CancelButton;
    public Button SkippButton;
    public Button CloseButton;

    public Text fakeAdevertText;
    private Action finishAction, cancelAction, skippAction, close;

    public void Awake()
    {
        CloseButton.onClick.AddListener(() =>
        {
            if (finishAction != null) finishAction();
            if (close != null) close();
        });

        SkippButton.onClick.AddListener(() =>
        {
            if (skippAction != null) skippAction();
            if (close != null) close();
        });

        CancelButton.onClick.AddListener(() =>
        {
            if (cancelAction != null) cancelAction();
            if (close != null) close();
        });
    }

    internal void Init(string message, Action finishAction, Action cancelAction, Action skippAction, Action close)
    {
        fakeAdevertText.text = message;
        this.finishAction = finishAction;
        this.cancelAction = cancelAction;
        CancelButton.gameObject.SetActive(cancelAction != null);
        this.skippAction = skippAction;
        SkippButton.gameObject.SetActive(skippAction != null);
        this.close = close;
    }


}