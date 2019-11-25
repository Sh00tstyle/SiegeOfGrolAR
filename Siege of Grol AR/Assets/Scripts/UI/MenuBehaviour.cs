using DG.Tweening;
using DG.Tweening.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuBehaviour : MonoBehaviour
{
    public List<MenuAnimation> animations;
    public Tween activeTween;

    [SerializeField]
    private CanvasScaler _canvasScaler;

    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private RectTransform _canvasRect;

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
        gameObject.SetActive(true);

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
                gameObject.SetActive(false);
        });

        // Apply DOTween Ease or custom Curve
        SetEase(activeTween, pAnimation);
    }

    public void ShowMenu(MenuAnimation pAnimation, MenuBehaviour pLastMenu)
    {
        // Enable GameObject
        gameObject.SetActive(true);

        // In case pLastMenu has an active tween, kill it. Elsewise place it out of frame.
        pLastMenu.activeTween.Kill();
        _canvasRect.localPosition = -GetAnimationVector(pAnimation.direction);

        // Kill any previous animations on object
        activeTween.Kill();
        activeTween = _canvasRect.DOLocalMove(Vector2.zero, pAnimation.easeDuration).OnComplete(() =>
        {
            if (pLastMenu != null && !pAnimation.stackOptions.HasFlag(StackOptions.OVERLAY))
                pLastMenu.gameObject.SetActive(false);
        });

        // Apply DOTween Ease or custom Curve
        SetEase(activeTween, pAnimation);

    }

    public Tween HideMenu(MenuAnimation pAnimation)
    {
        // Move outside of frame
        return _canvasRect.DOLocalMove(GetAnimationVector(pAnimation.direction), pAnimation.easeDuration)
            .SetEase(pAnimation.ease).OnComplete(() => this.gameObject.SetActive(false));

    }

    private void SetEase(Tween tween, MenuAnimation pAnimation)
    {
        if (pAnimation.ease != Ease.Unset)
            tween.SetEase(pAnimation.ease);
        else
            tween.SetEase(pAnimation.customCurve);
    }

    private Vector2 GetAnimationVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.LEFT:
                return Vector2.left * _canvasScaler.referenceResolution.x;

            case Direction.RIGHT:
                return Vector2.right * _canvasScaler.referenceResolution.x;

            case Direction.UP:
                return Vector2.up * _canvasScaler.referenceResolution.y;

            case Direction.DOWN:
                return Vector2.down * _canvasScaler.referenceResolution.y;

            default:
                return Vector2.zero;
        }
    }

}
