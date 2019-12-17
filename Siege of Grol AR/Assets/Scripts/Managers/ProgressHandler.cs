using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressHandler : Singleton<ProgressHandler>
{
    private static int _StoryProgress = 0;

    private void Awake()
    {
        SetDontDestroyOnLoad();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            IncreaseStoryProgress();

            Debug.Log("Increased story progress to " + _StoryProgress);
        }
    }
    public void IncreaseStoryProgress()
    {
        if(_StoryProgress < (int)Progress.Finish)
            ++_StoryProgress;
    }

    public int StoryProgress
    {
        get
        {
            return _StoryProgress;
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