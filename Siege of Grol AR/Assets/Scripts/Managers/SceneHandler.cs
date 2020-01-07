using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : Singleton<SceneHandler>
{
    private Coroutine _sceneLoadingRoutine = null;
    private bool _debug = false;

    private void Awake()
    {
        SetDontDestroyOnLoad();
    }

#if DEBUG
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || (Input.touchCount >= 4 && !_debug))
        {
            _debug = true;

            ProgressHandler.Instance.IncreaseStoryProgress();
            LoadScene(Scenes.Map);
        }

        if (Input.touchCount <= 3 && _debug)
            _debug = false;
    }
#endif

    public void LoadSceneWithDelay(Scenes pTargetScene, float pDelay)
    {
        if(_sceneLoadingRoutine != null)
        {
            Debug.LogError("Unable to load scene with delay, another loading routine is already running!");
            return;
        }

        _sceneLoadingRoutine = StartCoroutine(LoadSceneWithDelayInternally(pTargetScene, pDelay));
    }

    public void LoadScene(Scenes pTargetScene)
    {
        Debug.Log("Loading scene at build index: " + pTargetScene);

        KillAllCoroutines();
        SceneManager.LoadScene((int)pTargetScene, LoadSceneMode.Single);

        _sceneLoadingRoutine = null;
    }

    private IEnumerator LoadSceneWithDelayInternally(Scenes pTargetScene, float pDelay)
    {
        Debug.Log("Waiting " + pDelay + " seconds before loading scene " + pTargetScene);
        yield return new WaitForSecondsRealtime(pDelay);

        LoadScene(pTargetScene);
    }

    private void KillAllCoroutines()
    {
        DOTween.KillAll();

        if(NavigationManager.Instance != null)
            NavigationManager.Instance.StopAllCoroutines();

        if (CameraManager.Instance != null)
            CameraManager.Instance.StopAllCoroutines();

        if (GPSManager.Instance != null)
            GPSManager.Instance.StopAllCoroutines();
    }
}

public enum Scenes 
{
    Map = 0,
    Dialog = 1,
    CannonInteraction = 2
}