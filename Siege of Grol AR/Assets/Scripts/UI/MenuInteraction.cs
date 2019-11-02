using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class MenuAnimation
{
    [SerializeField]
    MenuBehaviour menu;
    [SerializeField]
    AnimationOption option;
    [SerializeField]
    Direction direction;
    [SerializeField]
    Button triggerButton;
    [SerializeField]
    Ease ease = Ease.InSine;
    [SerializeField]
    float easeDuration;

    [SerializeField]
    Direction swipeDirection;
    [SerializeField]
    SwipeDetection swipeDetection;

    public void SetListener()
    {
        // In case no triggers assigned don't add any listeners
        if (!triggerButton && !swipeDetection)
            return;

        if (swipeDetection != null)
            swipeDetection.AddListener(swipeDirection, () => menu.ShowMenu(ease, easeDuration, option, direction));

        if (triggerButton != null)
            triggerButton.onClick.AddListener(() => menu.ShowMenu(ease, easeDuration, option, direction));
    }
}

