using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : Singleton<MenuManager>
{
    Stack<MenuBehaviour> _menuStack;

    [SerializeField]
    MenuBehaviour _defaultMenu;


    protected override void Initialize()
    {
        _menuStack = new Stack<MenuBehaviour>();
        _menuStack.Push(_defaultMenu);
        _defaultMenu.ShowMenu();
    }

    public void GoToMenu(MenuBehaviour pTargetMenu)
    {
        //// Hide and disable last menu
        //if (!pTargetMenu.stackOptions.HasFlag(StackOptions.OVERLAY))
        //    _menuStack.Peek().HideMenu();

        //// Enable new menu and add to stack if neccesary
        //if (pTargetMenu.stackOptions.HasFlag(StackOptions.PUSHTOSTACK))
        //    _menuStack.Push(pTargetMenu);

        if (pTargetMenu.inAnimation == AnimationOption.PUSH)
            pTargetMenu.PushMenu(_menuStack.Peek());
        else
            _menuStack.Peek().HideMenu();

        //if (pTargetMenu.stackOptions.HasFlag(StackOptions.CLEARSTACK))
        //{
        //    _menuStack = new Stack<MenuBehaviour>();
        //    _menuStack.Push(pTargetMenu);
        //}
        _menuStack.Push(pTargetMenu);
        pTargetMenu.ShowMenu();
    }

    public void Back()
    {
        // If only one item is in the stack, there is no point going back 
        if (_menuStack.Count <= 1)
            return;

        // Hide and disable current last menu
        _menuStack.Peek().HideMenu();
        _menuStack.Pop();
        // Enable last menu
        _menuStack.Peek().ShowMenu();
    }
}
