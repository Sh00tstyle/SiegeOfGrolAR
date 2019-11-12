using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : Singleton<MenuManager>
{
    public MenuBehaviour narrationMenu;

    Stack<MenuBehaviour> _menuStack;
    Stack<MenuAnimation> _animationStack;


    MenuBehaviour LastMenu
    {
        get
        {
            MenuBehaviour previousMenu = null;
            if (_menuStack.Count > 0)
                previousMenu = _menuStack.Peek();
            else
                previousMenu = _rootMenu;

            return previousMenu;
        }
    }


    MenuBehaviour _rootMenu;

    [SerializeField]
    MenuBehaviour _startingMenu, _narrationMenu;

    [SerializeField]
    MenuAnimation _defaultAnimation;

    private void Awake()
    {
        NewMenuRoot(_startingMenu);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Tween activeTween = LastMenu.activeTween;
            if (activeTween != null && !activeTween.IsPlaying())
                Back();
        }

    }

    void NewMenuRoot(MenuBehaviour pTargetRoot)
    {
        _rootMenu = pTargetRoot;
        _menuStack = new Stack<MenuBehaviour>();
        _animationStack = new Stack<MenuAnimation>();
    }


    public void GoToMenu(MenuBehaviour pTargetMenu, MenuAnimation pAnimation = null)
    {
        if (pAnimation == null)
            pAnimation = _defaultAnimation;

        AnimateMenu(pTargetMenu, pAnimation);

    }

    public void GoToMenu(MenuTypes pMenu = MenuTypes.NARRATIONMENU, MenuAnimation pAnimation = null)
    {
        if (pAnimation == null)
            pAnimation = _defaultAnimation;

        MenuBehaviour targetMenu = null;
        switch (pMenu)
        {
            case MenuTypes.NARRATIONMENU:
                targetMenu = _narrationMenu;
                break;
            case MenuTypes.STARTINGMENU:
                targetMenu = _startingMenu;
                break;
        }
        AnimateMenu(targetMenu, pAnimation);

    }

    void AnimateMenu(MenuBehaviour pTargetMenu, MenuAnimation pAnimation = null)
    {
        switch (pAnimation.animation)
        {
            case AnimationOption.INSTANT:
                {
                    pTargetMenu.gameObject.SetActive(true);
                    LastMenu.gameObject.SetActive(true);
                }
                break;
            case AnimationOption.FADEIN:
                pTargetMenu.FadeMenu(pAnimation, pAnimation.stackOptions.HasFlag(StackOptions.OVERLAY) ? null : LastMenu, true);
                break;
            case AnimationOption.FADEOUT:
                pTargetMenu.FadeMenu(pAnimation, pAnimation.stackOptions.HasFlag(StackOptions.OVERLAY) ? null : LastMenu, false);
                break;
            case AnimationOption.ANIMATE:
                pTargetMenu.ShowMenu(pAnimation, LastMenu);
                break;
            case AnimationOption.DISMISS:
                pTargetMenu.HideMenu(pAnimation);
                break;
        }

        if (pAnimation.stackOptions.HasFlag(StackOptions.PUSHTOSTACK))
        {
            _menuStack.Push(pTargetMenu);
            _animationStack.Push(pAnimation);
        }
        if (pTargetMenu == _startingMenu || pAnimation.stackOptions.HasFlag(StackOptions.CLEARSTACK))
            NewMenuRoot(pTargetMenu);
    }


    public void Back()
    {
        // If only one item is in the stack, go back to the root
        if (_menuStack.Count == 0)
            return;

        MenuAnimation lastAnimation = _animationStack.Peek();
        MenuBehaviour lastMenu = LastMenu;


        // Invert last animation direction, in case it has one
        MenuAnimation invertedAnimation = new MenuAnimation();
        switch (lastAnimation.direction)
        {
            case Direction.UP:
                invertedAnimation.direction = Direction.DOWN;
                break;
            case Direction.DOWN:
                invertedAnimation.direction = Direction.UP;
                break;
            case Direction.RIGHT:
                invertedAnimation.direction = Direction.LEFT;
                break;
            case Direction.LEFT:
                invertedAnimation.direction = Direction.RIGHT;
                break;
            default:
                break;
        }

        // Move back the last menu and its animation
        lastMenu.HideMenu(invertedAnimation);
        _menuStack.Pop();
        _animationStack.Pop();

        //// Show the menu before that, in case it was slid back
        MenuBehaviour newMenu = LastMenu;
        newMenu.ShowMenu(invertedAnimation, lastMenu);

        if (newMenu == _startingMenu)
            NewMenuRoot(newMenu);
    }


}
