using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] LineRenderer _navigateLR, _pastLR;
    [SerializeField] GameObject _player;

    const string apiKey = "5b3ce3597851110001cf6248296d7129fb1343a4bf2e43eca3d631a1";
    const string baseURI = "https://api.openrouteservice.org/v2/directions/";

    Vector3[] _receivedPath;
    Tween _movementTween;

    // Testing purpose
    int index;

    [ContextMenu("Request")]
    void Awake()
    {
        /**
            StartCoroutine(GetDirections(
        new GPSLocation(52.041095, 6.617560),
        new GPSLocation(52.198434, 6.879558)
        ));
        /**/

                StartCoroutine(GetDirections(
        new GPSLocation(6.616444, 52.042944),
        new GPSLocation(6.6177933, 52.0424283)
        ));
    }

    [System.Serializable]
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

    IEnumerator GetDirections(GPSLocation pStart, GPSLocation pEnd)
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

                if(response == null)
                {
                    Debug.LogError("The web request response was null");
                    yield break;
                }

                if (response.features == null)
                {
                    Debug.LogError("The features was null");
                    yield break;
                }

                if(response.features != null && response.features.Count <= 0)
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

    void ConnectLineRenderers()
    {
        _navigateLR.SetPosition(0, _player.transform.position);
        _pastLR.SetPosition(_pastLR.positionCount - 1, _player.transform.position);
    }

    // Perhaps not that efficient?
    int GetPathIndexRelativeToPlayer(Vector3 pPlayerPosition)
    {
        int index = 0;
        float smallestDistance = Mathf.Infinity;
        Vector3 position = Vector3.zero;
        for (int i = 0; i < _receivedPath.Length; ++i)
        {
            position = _receivedPath[i];
            float distance = Mathf.Abs(Vector2.Distance(position, pPlayerPosition));
            if (distance < smallestDistance)
            {
                index = i;
                smallestDistance = distance;
            }
        }
        return index;
    }


    // Replace with Nico's function
    Vector3[] GeometryToVector3(Feature feature)
    {
        Vector3[] geometry = new Vector3[feature.geometry.coordinates.Count];

        if(GPSManager.Instance == null)
        {
            Debug.LogError("The GPSManager was null");
        }

        for(int i = 0; i < geometry.Length; ++i)
            geometry[i] = GPSManager.Instance.GetWorldPosFromGPS(feature.geometry.coordinates[i][1], feature.geometry.coordinates[i][0]);

        /**
        for (int i = 0; i < geometry.Length; ++i)
            geometry[i] = new Vector3((float)feature.geometry.coordinates[i][0] * 18, 0, (float)feature.geometry.coordinates[i][1] * 36);
        /**/

        return geometry;
    }

    string LocationToString(GPSLocation location)
    {
        return String.Format("{0},{1}", location.latitude.ToString(CultureInfo.InvariantCulture), location.longitude.ToString(CultureInfo.InvariantCulture));
    }


}
