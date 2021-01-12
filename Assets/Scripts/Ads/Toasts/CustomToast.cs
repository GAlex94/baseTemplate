using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
/*
 * Объект CustomToast помещён внутри префаба "Prefabs/Common/Canvas Inter Screen.prefab".
 * 
 * Таким образом обеспечено присутствие CustomToast на всех сценах и поверх всего UI.
 *  
 */

/// <summary>
/// Упрощенная версия VToast.
/// </summary>
public class CustomToast : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Text text;
    public float showTime = 0.8f;

    private float animaTime = 0.2f;
    private float disTimer;

    void Start()
    {
        canvasGroup.alpha = 0f;
    }

    public void Show(string message)
    {
        DOTween.Kill(this);

        canvasGroup.alpha = 0;
        text.text = message;

        var sq = DOTween.Sequence();

        sq.Append(canvasGroup.DOFade(1f, animaTime).SetUpdate(true));
        sq.AppendInterval(showTime);
        sq.Append(canvasGroup.DOFade(0f, animaTime).SetUpdate(true));

        sq.SetId(this);
    }
}