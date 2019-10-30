using DG.Tweening;
using DG.Tweening.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBehaviour : MonoBehaviour
{


    public MenuOptions options;


    [SerializeField]
    CanvasGroup canvasGroup;

    [SerializeField]
    Ease easeIn, easeOut;

    [SerializeField]
    float easeInDuration, easeOutDuration;

    Tween _activeTween;


    public void ShowMenu()
    {
        this.gameObject.SetActive(true);

        // Kill any active DOTweens if we're going fast
        _activeTween.Kill();
        _activeTween = canvasGroup.DOFade(1, easeInDuration).SetEase(easeIn);
    }

    public void HideMenu()
    {
        // Kill any active DOTweens if we're going fast
        _activeTween.Kill();
        _activeTween = canvasGroup.DOFade(0, easeOutDuration).SetEase(easeOut).OnComplete(() => this.gameObject.SetActive(false));
    }

}
