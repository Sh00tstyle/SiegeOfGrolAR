using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : Singleton<MenuManager>
{
    Stack<MenuBehaviour> _menuStack;
    Stack<MenuAnimation> _animationStack;


    MenuBehaviour _rootMenu;

    [SerializeField]
    MenuBehaviour _defaultMenu;

    protected override void Initialize()
    {
        _NewMenuRoot(_defaultMenu);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Back();
    }

    void _NewMenuRoot(MenuBehaviour pTargetRoot)
    {
        _rootMenu = pTargetRoot;
        _menuStack = new Stack<MenuBehaviour>();
        _animationStack = new Stack<MenuAnimation>();

    }

    public void GoToMenu(MenuBehaviour pTargetMenu, MenuAnimation pAnimation)
    {
        switch (pAnimation.animation)
        {
            case AnimationOption.INSTANT:
                {
                    pTargetMenu.gameObject.SetActive(true);
                    _GetLastMenu().gameObject.SetActive(true);
                }
                break;
            case AnimationOption.FADEIN:
                pTargetMenu.FadeMenu(pAnimation, _GetLastMenu());
                break;
            case AnimationOption.ANIMATE:
                pTargetMenu.ShowMenu(pAnimation, _GetLastMenu());
                break;
        }

        _menuStack.Push(pTargetMenu);
        _animationStack.Push(pAnimation);
    }

    MenuBehaviour _GetLastMenu()
    {
        MenuBehaviour previousMenu = null;
        if (_menuStack.Count > 0)
            previousMenu = _menuStack.Peek();
        else
            previousMenu = _rootMenu;

        return previousMenu;
    }

    public void Back()
    {
        // If only one item is in the stack, go back to the root
        if (_menuStack.Count == 0)
            return;

        MenuAnimation lastAnimation = _animationStack.Peek();
        MenuBehaviour lastMenu = _GetLastMenu();


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
        _GetLastMenu().ShowMenu(invertedAnimation, lastMenu);
    }


}
