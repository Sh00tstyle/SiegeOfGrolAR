using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class NavigationManager : Singleton<NavigationManager>
{
    [Serializable]
    public struct GPSLocation
    {
        public double latitude;
        public double longitude;


        public GPSLocation(double latitude, double longitude) : this()
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }
    }

    const string apiKey = "5b3ce3597851110001cf6248296d7129fb1343a4bf2e43eca3d631a1";
    const string baseURI = "https://api.openrouteservice.org/v2/directions/";

    [SerializeField] 
    private LineRenderer _navigateLR, _pastLR;

    [SerializeField] 
    private GameObject _player;

    private Vector3[] _receivedPath;
    private Tween _movementTween;

    // Testing purpose
    private int index;

    [ContextMenu("Request")]
    void Awake()
    {
        StartCoroutine(GetDirections(
        new GPSLocation(6.616444, 52.042944),
        new GPSLocation(6.617697, 52.04234)
        ));
    }

    public void SetPath(LineRenderer pLineRenderer, Vector3[] pPath)
    {
        pLineRenderer.positionCount = pPath.Length;
        pLineRenderer.SetPositions(pPath);
    }

    public void UpdateLineRenderers()
    {
        // Check the closest point where the player is
        int pathIndex = GetPathIndexRelativeToPlayer(_player.transform.position);

        // Add one extra index for the player's location
        Vector3[] navigated = new Vector3[pathIndex + 1];
        Vector3[] toNavigate = new Vector3[_receivedPath.Length - pathIndex + 1];

        // Extract data from receivedPath
        for (int i = 0; i < pathIndex; ++i)
            navigated[i] = _receivedPath[i];
        for (int i = 0; i < _receivedPath.Length - pathIndex; ++i)
            toNavigate[i + 1] = _receivedPath[i + pathIndex];

        // Apply arrays to line renderers
        SetPath(_navigateLR, toNavigate);
        SetPath(_pastLR, navigated);

        ConnectLineRenderers();
    }

    [ContextMenu("Animate player to random location")]
    public void UpdatePlayerPosition()
    {
        // Testing purpose
        Vector3 newPosition = _receivedPath[index++];

        _movementTween = _player.transform.DOMove(newPosition, 0.6f).SetEase(Ease.InSine)
            .OnUpdate(() => ConnectLineRenderers())
            .OnComplete(() => { UpdateLineRenderers(); UpdatePlayerPosition(); });
    }

    public string LocationToString(GPSLocation location)
    {
        return String.Format("{0},{1}", location.latitude.ToString(CultureInfo.InvariantCulture), location.longitude.ToString(CultureInfo.InvariantCulture));
    }

    private IEnumerator GetDirections(GPSLocation pStart, GPSLocation pEnd)
    {
        string request = String.Format("foot-walking?api_key={0}&start={1}&end={2}", apiKey, LocationToString(pStart), LocationToString(pEnd));
        using (UnityWebRequest webRequest = UnityWebRequest.Get(baseURI + request))
        {
            // In case we will ever switch to JSON POSTs, we would need to use an authorization key instead of ?api_key=
            //webRequest.SetRequestHeader("Authorization", apiKey);

            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
                Debug.Log("Error receiving navigation: " + webRequest.error);
            else
            {
                // Parse JSON to class
                NavigationResponse response = JsonConvert.DeserializeObject<NavigationResponse>(webRequest.downloadHandler.text);
                // Store received path and apply it to the line renderer

                if (response == null)
                {
                    Debug.LogError("The web request response was null");
                    yield break;
                }

                if (response.features == null)
                {
                    Debug.LogError("The features was null");
                    yield break;
                }

                if (response.features != null && response.features.Count <= 0)
                {
                    Debug.LogError("Features list was empty");
                    yield break;
                }

                _receivedPath = GeometryToVector3(response.features[0]);
                SetPath(_navigateLR, _receivedPath);

                _pastLR.positionCount = 2;
                _pastLR.SetPosition(0, _receivedPath[0]);
                _pastLR.SetPosition(1, _receivedPath[0]);
            }
        }
    }

    private void ConnectLineRenderers()
    {
        _navigateLR.SetPosition(0, _player.transform.position);
        _pastLR.SetPosition(_pastLR.positionCount - 1, _player.transform.position);
    }

    private int GetPathIndexRelativeToPlayer(Vector3 pPlayerPosition)
    {
        int index = 0;
        float smallestDistance = Mathf.Infinity;

        pPlayerPosition.y = 0.0f; // Move all positions to compare to the same Y position (might alter the players transform.position, but i don't think so)

        Vector3 currentPosition;
        Vector3 deltaVector;

        for (int i = 0; i < _receivedPath.Length; ++i)
        {
            currentPosition = _receivedPath[i];
            currentPosition.y = 0.0f;

            deltaVector = currentPosition - pPlayerPosition;
            float distance = deltaVector.sqrMagnitude;

            if (distance < smallestDistance)
            {
                index = i;
                smallestDistance = distance;
            }
        }

        return index;
    }


    private Vector3[] GeometryToVector3(Feature feature)
    {
        Vector3[] geometry = new Vector3[feature.geometry.coordinates.Count];

        if(GPSManager.Instance == null)
        {
            Debug.LogError("The GPSManager was null");
        }

        for(int i = 0; i < geometry.Length; ++i)
        {
            Vector3 worldPos = GPSManager.Instance.GetWorldPosFromGPS(feature.geometry.coordinates[i][1], feature.geometry.coordinates[i][0], new Vector3(1.0f, 0.0f, 0.55f));
            worldPos.x *= -1;
            worldPos = Quaternion.Euler(0.0f, -90.0f, 0.0f) * worldPos;

            geometry[i] = worldPos;
        }

        return geometry;
    }

}
