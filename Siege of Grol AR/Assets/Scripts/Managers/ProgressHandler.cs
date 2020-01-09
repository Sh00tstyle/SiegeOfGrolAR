using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressHandler : Singleton<ProgressHandler>
{
    private static int _StoryProgress = 3;
    private static bool _IsHelpingSpy = false;

    private void Awake()
    {
        SetDontDestroyOnLoad();
    }

    public void IncreaseStoryProgress()
    {
        if(_StoryProgress < (int)Progress.Finish)
            ++_StoryProgress;
    }

    public void SetStoryDecision(bool pIsHelpingSpy)
    {
        _IsHelpingSpy = pIsHelpingSpy;
    }

    public int StoryProgressIndex
    {
        get
        {
            return _StoryProgress;
        }
    }

    public bool IsHelpingSpy
    {
        get
        {
            return _IsHelpingSpy;
        }
    }
}

public enum Progress // The progress describes the next assignment that needs to be done (also works with the location index in the GameManager)
{
    Priest = 0,
    Drunkard = 1,
    CannonCommander = 2,
    Matthijs = 3,
    Finish = 4
}