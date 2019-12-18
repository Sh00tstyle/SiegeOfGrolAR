using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : Singleton<SceneHandler>
{
    private bool _debugToggle = false;

    private Coroutine _sceneLoadingRoutine = null;

    private void Awake()
    {
        SetDontDestroyOnLoad();
    }

    private void Update()
    {
        // DEBUG
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_debugToggle)
            {
                LoadScene(1);
            }
            else
            {
                LoadScene(0);
            }

            _debugToggle = !_debugToggle;
        }
    }

    public void LoadSceneWithDelay(int pSceneBuildIndex, float pDelay)
    {
        if(_sceneLoadingRoutine != null)
        {
            Debug.LogError("Unable to load scene with delay, another loading routine is already running!");
            return;
        }

        _sceneLoadingRoutine = StartCoroutine(LoadSceneWithDelayInternally(pSceneBuildIndex, pDelay));
    }

    public void LoadScene(int pSceneBuildIndex)
    {
        Debug.Log("Loading scene at build index: " + pSceneBuildIndex);

        KillAllCoroutines();
        SceneManager.LoadScene(pSceneBuildIndex, LoadSceneMode.Single);

        _sceneLoadingRoutine = null;
    }

    private IEnumerator LoadSceneWithDelayInternally(int pSceneBuildIndex, float pDelay)
    {
        Debug.Log("Waiting " + pDelay + " seconds before loading scene " + pSceneBuildIndex);
        yield return new WaitForSecondsRealtime(pDelay);

        LoadScene(pSceneBuildIndex);
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
