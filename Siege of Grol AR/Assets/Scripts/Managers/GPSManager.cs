using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSManager : Singleton<GPSManager>
{
    [SerializeField]
    private Transform _player;

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

    [SerializeField]
    private Transform _referenceTransform;

    [SerializeField]
    private double _referenceLatitude = 52.042339;

    [SerializeField]
    private double _referenceLongitude = 6.616444;

    [SerializeField]
    private double _referenceScale = 1100.0;

    private IEnumerator Start()
    {
        if (_useFakeLocation)
            yield return StartCoroutine(InitializeMockLocation());
        else
            yield return StartCoroutine(InitializeLocationService());
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
        _player.position += worldMovement * Time.deltaTime * _movementSpeed;
    }

    public Vector3 GetWorldPosFromGPS(double pLatitude, double pLongitude)
    {
        // Calculate the world position based on a set scale and a reference point in the map that is based on real world GPS coordinates
        Vector3 mapOrigin = _referenceTransform.position;

        double worldXPos = pLatitude - _referenceLatitude;
        double worldZPos = pLongitude - _referenceLongitude;

        worldXPos = worldXPos - mapOrigin.x;
        worldZPos = worldZPos - mapOrigin.z;

        worldXPos *= _referenceScale;
        worldZPos *= _referenceScale;

        worldZPos *= 0.5;

        Vector3 newPos = new Vector3((float)worldXPos, 0.0f, (float)worldZPos);
        newPos.x *= -1;
        newPos = Quaternion.Euler(0.0f, -90.0f, 0.0f) * newPos;

        return newPos;
    }

    public GPSLocation GetGPSFromWorldPos(Vector3 pWorldPos)
    {
        // Reconstruct the GPS coordinates of a world position based on the reference point on the map
        Vector3 mapOrigin = _referenceTransform.position;

        pWorldPos.x *= -1;
        pWorldPos = Quaternion.Euler(0.0f, -90.0f, 0.0f) * pWorldPos;

        double latitude = pWorldPos.x - mapOrigin.x;
        double longitude = pWorldPos.z - mapOrigin.z;

        longitude *= 2.0;

        latitude *= 1.0 / _referenceScale;
        longitude *= 1.0 / _referenceScale;

        latitude += _referenceLatitude;
        longitude += _referenceLongitude;

        return new GPSLocation(longitude, latitude);
    }

    private IEnumerator InitializeMockLocation()
    {
        yield return null;

        Debug.Log("Unable to initialize GPS service, initialized Mock GPS service instead!");
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
                return GetGPSFromWorldPos(_player.position);
        }
    }
}
