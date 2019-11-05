using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        // The menu for the narration should be opened here

        GameManager.Instance.StartInteraction(); // DEBUG: This should be called from a menu
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
