using DG.Tweening;
using DG.Tweening.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class MenuBehaviour : MonoBehaviour
{
    public List<MenuAnimation> animations;

    [SerializeField]
    CanvasGroup _canvasGroup;

    [SerializeField]
    RectTransform _canvasRect;

    public Tween activeTween;

    private void Awake()
    {
        for (int i = 0; i < animations.Count; ++i)
            animations[i].SetListener();
    }

    public void FadeMenu(MenuAnimation pAnimation, MenuBehaviour pLastMenu, bool pFadeIn)
    {
        // In case pLastMenu has an active tween, kill it
        if (pLastMenu != null && pLastMenu.activeTween != null)
            pLastMenu.activeTween.Kill();

        // Kill any previous animations on object
        activeTween.Kill();

        // Enable GameObject
        this.gameObject.SetActive(true);

        // Check if alpha if 1, in case set it to 0;
        if (pFadeIn)
        {
            if (_canvasGroup.alpha > 0)
                _canvasGroup.alpha = 0;
        }

        activeTween = _canvasGroup.DOFade(pFadeIn ? 1 : 0, pAnimation.easeDuration).OnComplete(() =>
        {
            if (pLastMenu != null)
                pLastMenu.gameObject.SetActive(false);
            if(!pFadeIn)
                this.gameObject.SetActive(false);
        });

        // Apply DOTween Ease or custom Curve
        _SetEase(activeTween, pAnimation);
    }

    public void ShowMenu(MenuAnimation pAnimation, MenuBehaviour pLastMenu)
    {
        // Enable GameObject
        this.gameObject.SetActive(true);

        // In case pLastMenu has an active tween, kill it. Elsewise place it out of frame.
        pLastMenu.activeTween.Kill();
        _canvasRect.localPosition = -_GetAnimationVector(pAnimation.direction);

        // Kill any previous animations on object
        activeTween.Kill();
        activeTween = _canvasRect.DOLocalMove(Vector2.zero, pAnimation.easeDuration).OnComplete(() =>
        {
            if (pLastMenu != null && !pAnimation.stackOptions.HasFlag(StackOptions.OVERLAY))
                pLastMenu.gameObject.SetActive(false);
        });

        // Apply DOTween Ease or custom Curve
        _SetEase(activeTween, pAnimation);

    }

    void _SetEase(Tween tween, MenuAnimation pAnimation)
    {
        if (pAnimation.ease != Ease.Unset)
            tween.SetEase(pAnimation.ease);
        else
            tween.SetEase(pAnimation.customCurve);
    }

    public Tween HideMenu(MenuAnimation pAnimation)
    {
        // Move outside of frame
        return _canvasRect.DOLocalMove(_GetAnimationVector(pAnimation.direction), pAnimation.easeDuration)
            .SetEase(pAnimation.ease).OnComplete(() => this.gameObject.SetActive(false));

    }

    Vector2 _GetAnimationVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.LEFT:
                return Vector2.left * 300;
            case Direction.RIGHT:
                return Vector2.right * 300;
            case Direction.UP:
                return Vector2.up * 600;
            case Direction.DOWN:
                return Vector2.down * 600;
        }
        return Vector2.zero;
    }

}
