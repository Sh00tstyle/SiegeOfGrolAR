using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    private bool _debugToggle = false;

    private void Awake()
    {
        if (_Instance != null)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
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

    public void LoadScene(int pSceneBuildIndex)
    {
        Debug.Log("Loading scene at build index: " + pSceneBuildIndex);

        KillAllCoroutines();
        SceneManager.LoadScene(pSceneBuildIndex, LoadSceneMode.Single);
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
