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

    Tween _activeTween;

    private void Awake()
    {
        for (int i = 0; i < animations.Count; ++i)
            animations[i].SetListener();
    }

    public void FadeMenu(MenuAnimation pAnimation, MenuBehaviour pLastMenu)
    {
        // Kill any previous animations on object
        _activeTween.Kill();

        // Enable GameObject
        this.gameObject.SetActive(true);

        // Check if alpha if 1, in case set it to 0;
        if (_canvasGroup.alpha > 0)
            _canvasGroup.alpha = 0;

        _activeTween = _canvasGroup.DOFade(1, pAnimation.easeDuration).OnComplete(() =>
        {
            if (pLastMenu != null)
                pLastMenu.gameObject.SetActive(false);
        });

        // Apply DOTween Ease or custom Curve
        _SetEase(_activeTween, pAnimation);
    }

    public void ShowMenu(MenuAnimation pAnimation, MenuBehaviour pLastMenu)
    {
        // Enable GameObject
        this.gameObject.SetActive(true);

        // Kill any previous animations on object
        _activeTween.Kill();

        // Set position outside of frame
        _canvasRect.localPosition = -_GetAnimationVector(pAnimation.direction);
        _activeTween = _canvasRect.DOLocalMove(Vector2.zero, pAnimation.easeDuration).OnComplete(() =>
        {
            if (pLastMenu != null)
                pLastMenu.gameObject.SetActive(false);
        });

        // Apply DOTween Ease or custom Curve
        _SetEase(_activeTween, pAnimation);

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
