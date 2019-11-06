using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class MenuAnimation
{

    public StackOptions stackOptions = StackOptions.PUSHTOSTACK;
    public MenuBehaviour menu;
    public AnimationOption animation = AnimationOption.INSTANT;
    public AnimationCurve customCurve = null;
    public Direction direction;
    public Button triggerButton;
    public Ease ease = Ease.InSine;
    public float easeDuration = 0.5f;
    public SwipeDetection swipeDetection;

    public void SetListener()
    {
        // In case no triggers assigned don't add any listeners
        if (!triggerButton && !swipeDetection)
            return;

        if (swipeDetection != null)
            swipeDetection.AddListener(direction, () => MenuManager.Instance.GoToMenu(menu, this));

        if (triggerButton != null)
            triggerButton.onClick.AddListener(() => MenuManager.Instance.GoToMenu(menu, this));
    }
}

