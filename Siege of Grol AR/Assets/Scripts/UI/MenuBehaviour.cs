using DG.Tweening;
using DG.Tweening.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuBehaviour : MonoBehaviour
{
    [Header("Animation")]
    public StackOptions stackOptions;
    public AnimationOption inAnimation, outAnimation;
    public Direction inAnimationDirection, outAnimationDirection;

    [SerializeField]
    Ease easeIn, easeOut;
    [SerializeField]
    float easeInDuration, easeOutDuration;


    [Header("Canvas objects")]
    [SerializeField]
    RectTransform containerRect;
    [SerializeField]
    CanvasGroup canvasGroup;



    public Tween activeTween;


    private void Awake()
    {
        if (inAnimation == AnimationOption.ANIMATE || inAnimation == AnimationOption.PUSH)
            containerRect.localPosition = _GetAnimationVector(inAnimationDirection);
    }

    void _FadeMenu(bool fadeIn)
    {
        activeTween = canvasGroup.DOFade(fadeIn ? 1 : 0, fadeIn ? easeInDuration : easeOutDuration).SetEase(fadeIn ? easeIn : easeOut);

        if (!fadeIn)
            activeTween.OnComplete(() => this.gameObject.SetActive(false));
    }

    void _AnimationMenu(bool animationIn)
    {
        if (canvasGroup.alpha < 1)
            canvasGroup.alpha = 1;

        activeTween = containerRect.DOLocalMove(
            animationIn ? Vector2.zero : _GetAnimationVector(outAnimationDirection),
            animationIn ? easeInDuration : easeOutDuration
           ).SetEase(animationIn ? easeIn : easeOut);

        if (!animationIn)
            activeTween.OnComplete(() => this.gameObject.SetActive(false));
    }

    public void PushMenu(MenuBehaviour pPreviousMenu)
    {
        activeTween.Kill();
        // E.g. In case we're coming from the left, the old menu should go to the right
        Direction pushDirection = Direction.DOWN;
        switch (inAnimationDirection)
        {
            case Direction.UP:
                pushDirection = Direction.DOWN;
                break;
            case Direction.DOWN:
                pushDirection = Direction.UP;
                break;
            case Direction.RIGHT:
                pushDirection = Direction.LEFT;
                break;
            case Direction.LEFT:
                pushDirection = Direction.RIGHT;
                break;
        }
        pPreviousMenu.activeTween = pPreviousMenu.containerRect.DOLocalMove(_GetAnimationVector(pushDirection), pPreviousMenu.easeOutDuration)
            .SetEase(pPreviousMenu.easeOut)
            .OnComplete(() => pPreviousMenu.gameObject.SetActive(false));

        _AnimationMenu(true);
    }

    public void ShowMenu()
    {
        activeTween.Kill();
        this.gameObject.SetActive(true);

        //else
        _AnimationMenu(true);

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

    public void HideMenu()
    {
        activeTween.Kill();
        if (outAnimation == AnimationOption.INSTANT)
            gameObject.SetActive(false);
        else if (outAnimation == AnimationOption.DISSOLVE)
            _FadeMenu(false);
        else
            _AnimationMenu(false);
    }

}
