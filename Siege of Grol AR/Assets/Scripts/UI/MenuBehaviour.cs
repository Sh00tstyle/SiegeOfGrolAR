using DG.Tweening;
using DG.Tweening.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuBehaviour : MonoBehaviour
{
    [SerializeField]
    MenuAnimation[] _animations;

    [SerializeField]
    CanvasGroup _canvasGroup;

    [SerializeField]
    RectTransform _canvasRect;

    Tween _activeTween;

    private void Awake()
    {
        for (int i = 0; i < _animations.Length; ++i)
        {
            _animations[i].SetListener();
        }


    }

    //void _FadeMenu(bool fadeIn)
    //{
    //    activeTween = canvasGroup.DOFade(fadeIn ? 1 : 0, fadeIn ? easeInDuration : easeOutDuration).SetEase(fadeIn ? easeIn : easeOut);

    //    if (!fadeIn)
    //        activeTween.OnComplete(() => this.gameObject.SetActive(false));
    //}

    void _SlideIn(Direction pDirection)
    {

        _activeTween = _canvasRect.DOLocalMove(Vector2.zero, 1);


    }

    //public void PushMenu(MenuBehaviour pPreviousMenu)
    //{
    //    activeTween.Kill();
    //    // E.g. In case we're coming from the left, the old menu should go to the right
    //    Direction pushDirection = Direction.DOWN;
    //    switch (inAnimationDirection)
    //    {
    //        case Direction.UP:
    //            pushDirection = Direction.DOWN;
    //            break;
    //        case Direction.DOWN:
    //            pushDirection = Direction.UP;
    //            break;
    //        case Direction.RIGHT:
    //            pushDirection = Direction.LEFT;
    //            break;
    //        case Direction.LEFT:
    //            pushDirection = Direction.RIGHT;
    //            break;
    //    }
    //    pPreviousMenu.activeTween = pPreviousMenu.containerRect.DOLocalMove(_GetAnimationVector(pushDirection), pPreviousMenu.easeOutDuration)
    //        .SetEase(pPreviousMenu.easeOut)
    //        .OnComplete(() => pPreviousMenu.gameObject.SetActive(false));

    //    _AnimationMenu(true);
    //}

    public void ShowMenu(Ease pEase, float pEaseDuration, AnimationOption pAnimation = AnimationOption.ANIMATE, Direction pDirection = Direction.DOWN)
    {
        _canvasRect.localPosition = -_GetAnimationVector(pDirection);
        Debug.Log("Show menu");
        //activeTween.Kill();
        gameObject.SetActive(true);

        ////else
        _SlideIn(pDirection);

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

    //public void HideMenu()
    //{
    //    activeTween.Kill();
    //    if (outAnimation == AnimationOption.INSTANT)
    //        gameObject.SetActive(false);
    //    else if (outAnimation == AnimationOption.DISSOLVE)
    //        _FadeMenu(false);
    //    else
    //        _AnimationMenu(false);
    //}

}
