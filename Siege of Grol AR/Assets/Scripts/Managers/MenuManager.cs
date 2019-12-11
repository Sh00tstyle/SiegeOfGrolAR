using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;

public class MenuManager : Singleton<MenuManager>
{
    [SerializeField]
    private MenuBehaviour _startingMenu, _narrationMenu, _popupPrefab, _mainMenu;

    [SerializeField]
    private MenuAnimation _defaultAnimation, _popupAnimation;

    private MenuBehaviour _rootMenu;
    private Stack<MenuBehaviour> _menuStack;
    private Stack<MenuAnimation> _animationStack;

    private MenuBehaviour _popupMenu;
    private Popup _popupScript;


    private bool _InsidePopup;

    private void Awake()
    {
        if (_startingMenu != null)
        {
            NewMenuRoot(_startingMenu);
            GoToMenu(_startingMenu);
        }
        else
        {
            Debug.LogWarning("MenuManager::Unable to initialize starting menu, the starting menu was null! This could lead to errors in the appliaction.");
        }

    }

    private void Update()
    {
        if (_InsidePopup)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Tween activeTween = LastMenu.activeTween;
            if (activeTween != null && !activeTween.IsPlaying())
                Back();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            ShowPopup("Header", "Body", "Open settings", null);
        }

    }

    public void GoToMenu(MenuBehaviour pTargetMenu, MenuAnimation pAnimation = null)
    {
        if (pAnimation == null)
            pAnimation = _defaultAnimation;

        if (!pTargetMenu.gameObject.activeSelf)
            pTargetMenu.gameObject.SetActive(true);

        AnimateMenu(pTargetMenu, pAnimation);

    }

    public void GoToMenu(MenuTypes pMenu, MenuAnimation pAnimation = null)
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
            case MenuTypes.MAINMENU:
                targetMenu = _mainMenu;
                break;
        }

        AnimateMenu(targetMenu, pAnimation);
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

        // Show the menu before that, in case it was slid back
        MenuBehaviour newMenu = LastMenu;
        newMenu.ShowMenu(invertedAnimation, lastMenu);

        if (newMenu == _startingMenu)
            NewMenuRoot(newMenu);
    }

    private void NewMenuRoot(MenuBehaviour pTargetRoot)
    {
        _rootMenu = pTargetRoot;

        if (_menuStack == null)
            _menuStack = new Stack<MenuBehaviour>();
        else
            _menuStack.Clear();

        if (_animationStack == null)
            _animationStack = new Stack<MenuAnimation>();
        else
            _animationStack.Clear();
    }

    private void AnimateMenu(MenuBehaviour pTargetMenu, MenuAnimation pAnimation = null)
    {
        switch (pAnimation.animation)
        {
            case AnimationOption.INSTANT:
                pTargetMenu.gameObject.SetActive(true);
                LastMenu.gameObject.SetActive(true);
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

    public MenuBehaviour LastMenu
    {
        get
        {
            if (_menuStack == null)
            {
                Debug.LogError("MenuManager::Unable to retrieve LastMenu, _menuStack was null");
                return null;
            }

            if (_menuStack.Count > 0)
                return _menuStack.Peek();
            else
                return _rootMenu;
        }
    }

    public void ShowPopup(string pPopupHeader, string pPopupBody, string pButtonText, Action pAction)
    {
        // Instantiate menu if _popupObject is non existent in scene
        if (_popupScript == null || _popupMenu == null)
        {
            // Instantiate the GameObject
            _popupMenu = Instantiate(_popupPrefab).GetComponent<MenuBehaviour>();
            // Get popup script
            _popupScript = _popupMenu.GetComponentInChildren<Popup>();
        }


        // Show the popup menu
        _popupMenu.FadeMenu(_popupAnimation, null, true);
        _InsidePopup = true;

        // Set text to popup
        _popupScript.buttonText.text = pButtonText;
        _popupScript.popupHeader.text = pPopupHeader;
        _popupScript.popupBody.text = pPopupBody;

        // Clear all previously used actions
        _popupScript.button.onClick.RemoveAllListeners();

        // Add action to button onClick, also add the hiding of the menu
        _popupScript.button.onClick.AddListener(() =>
        {
            // Fade out Menu
            _popupMenu.FadeMenu(_popupAnimation, null, false);
            _InsidePopup = false;
        });
        if (pAction != null)
            _popupScript.button.onClick.AddListener(() => pAction());
    }



}
