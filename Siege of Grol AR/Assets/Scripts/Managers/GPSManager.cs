using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class GPSManager : Singleton<GPSManager>
{
    [SerializeField]
    private GameObject _debugCanvas;

    [SerializeField]
    private bool _useFakeLocation = true;

    [SerializeField]
    private float _movementSpeed = 1.0f;

    [SerializeField]
    private float _gpsAccuracyInMeters = 10.0f;

    [SerializeField]
    private float _gpsUpdateDistanceInMeters = 10.0f;

    [SerializeField]
    private int _gpsMaxInitializationTime = 20;

    private IEnumerator Start()
    {
        if (_useFakeLocation)
            yield return StartCoroutine(InitializeMockLocation());
        else
            yield return StartCoroutine(InitializeLocationService());

        NavigationManager.Instance.RequestNewNavigationPath();
    }

    private void OnDestroy()
    {
        ShutdownLocationService();
    }

    public void ShutdownLocationService()
    {
        if (!_useFakeLocation && Input.location.status == LocationServiceStatus.Running)
            Input.location.Stop();
    }

    public void MoveJoystickPlayer(Vector3 deltaMovement)
    {
        Vector3 worldMovement = new Vector3(deltaMovement.x, 0.0f, deltaMovement.y);
        Quaternion rotation = Quaternion.Euler(0.0f, CameraManager.Instance.RotationXAxis, 0.0f); //align the movement vector to on the camera orientation

        worldMovement = rotation * worldMovement;

        NavigationManager.Instance.Player.position += worldMovement * Time.deltaTime * _movementSpeed;
    }

    private IEnumerator InitializeMockLocation()
    {
        yield return null;

        Debug.Log("Unable to initialize GPS service, initialized Mock GPS service instead!");

        // Switch to Debug mode by spawning the debug joystick and moving the player to the world origin
        Instantiate(_debugCanvas);
        NavigationManager.Instance.Player.transform.position = new Vector3(0.0f, 0.1f, 0.0f);
    }

    private IEnumerator InitializeLocationService()
    {
        if (Input.location.isEnabledByUser) // The GPS service was not enabled in the device settings
        {
            Debug.Log("Unable to load GPS service, it was not enabled by the user");

            yield return StartCoroutine(InitializeMockLocation());
            yield break;
        }

        // Trying to initialize the location service
        Input.location.Start(_gpsAccuracyInMeters, _gpsUpdateDistanceInMeters);

        int waitTime = _gpsMaxInitializationTime;
        WaitForSeconds waitForOneSecond = new WaitForSeconds(1.0f);

        // Wait until the service initializes
        while(Input.location.status == LocationServiceStatus.Initializing && waitTime > 0)
        {
            yield return waitForOneSecond;
            --waitTime;
        }

        // Service did not initialize within the given timeframe
        if(waitTime < 1)
        {
            Debug.Log("GPS initialization timed out");

            yield return StartCoroutine(InitializeMockLocation());
            yield break;
        }

        if(Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Failed to initialize the location service!");

            yield return StartCoroutine(InitializeMockLocation());
            yield break;
        }
        else
        {
            // Location service initialized and data could be retrieved
            Debug.Log("Initialized location services");
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }
    }

    public GPSLocation CurrentGPSLocation
    {
        get
        {
            if(!_useFakeLocation && Input.location.status == LocationServiceStatus.Running)
                return new GPSLocation(Input.location.lastData.longitude, Input.location.lastData.latitude);
            else
                return NavigationManager.Instance.GetGPSFromWorldPos(NavigationManager.Instance.Player.position);
        }
    }
}
