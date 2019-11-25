using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSManager : Singleton<GPSManager>
{
    [SerializeField]
    private bool _useFakeLocation = true;

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
    private float _referenceScale = 1100.0f;

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

    private IEnumerator InitializeMockLocation()
    {
        yield return null;

        Debug.Log("Did not initialize the GPS service, you chose to use a fake location!");
    }

    private IEnumerator InitializeLocationService()
    {

        if (Input.location.isEnabledByUser) // The GPS service was not enabled in the device settings
            yield break;

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
            yield break;
        }

        if(Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Failed to initialize the location service!");
            yield break;
        }
        else
        {
            // Location service initialized and data could be retrieved
            Debug.Log("Initialized location services");
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }
    }

    public Vector3 GetWorldPosFromGPS(double pLatitude, double pLongitude, Vector3? pScale = null)
    {
        // Calculate the world position based on a set scale and a reference point in the map that is based on real world GPS coordinates
        Vector3 mapOrigin = _referenceTransform.position;

        double deltaLatitude = pLatitude - _referenceLatitude;
        double deltaLongitude = pLongitude - _referenceLongitude;

        double worldXPos = (deltaLatitude - mapOrigin.x);
        double worldZPos = (deltaLongitude - mapOrigin.z);

        if(pScale == null)
        {
            worldXPos *= _referenceScale;
            worldZPos *= _referenceScale;
        }
        else
        {
            worldXPos *= _referenceScale * pScale.Value.x;
            worldZPos *= _referenceScale * pScale.Value.z;
        }

        Vector3 newPos = new Vector3((float)worldXPos, 0.0f, (float)worldZPos);

        return newPos;
    }

    public LocationInfo? CurrentLocationData // The "?" indicates that the type can be nulled, so in this case the returned struct can be null
    {
        get
        {
            if(Input.location.status == LocationServiceStatus.Running) 
                return Input.location.lastData;
            else
                return null;
        }
    }
}
