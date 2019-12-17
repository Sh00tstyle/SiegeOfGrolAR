﻿using UnityEngine;

public class Narrator : MonoBehaviour
{
    [SerializeField]
    private string[] _narrative;

    [SerializeField]
    private string _hint;

    private int _currentNarrativeIndex;

    private void Awake()
    {
        _currentNarrativeIndex = 0;
    }

    public void OpenMenu()
    {
        Debug.Log("Opening Narration of " + name);

        MenuManager.Instance.GoToMenu(MenuTypes.NARRATIONMENU);
    }

    public string GetNextText()
    {
        ++_currentNarrativeIndex;

        if (_currentNarrativeIndex > _narrative.Length)
            return ""; // no more lines left

        return _narrative[_currentNarrativeIndex - 1];
    }

    public string GetHintText()
    {
        return _hint;
    }
}
