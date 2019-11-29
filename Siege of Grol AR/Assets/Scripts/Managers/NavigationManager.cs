﻿using DG.Tweening;
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
    const string apiKey = "5b3ce3597851110001cf62488cba7d2491cc44c19424c9e70c00f54e";
    const string baseURI = "https://api.openrouteservice.org/v2/directions/";

    [SerializeField]
    private Transform _referenceTransform;

    [SerializeField]
    private double _referenceLatitude = 52.042944;

    [SerializeField]
    private double _referenceLongitude = 6.616444;

    [SerializeField]
    private double _referenceScale = 2000.0;

    [SerializeField] 
    private LineRenderer _remainingPathLR, _navigatedPathLR;

    [SerializeField] 
    private Transform _player;

    [SerializeField]
    private float _maxStrayDistance = 0.5f;

    [SerializeField]
    private float _minSegmentDistance = 0.1f;

    private Vector3[] _receivedPath;

    private Coroutine _directionRoutine;
    private Coroutine _strayDetectionRoutine;
    private Coroutine _lineRendererUpdateRoutine;

    public void RequestNewNavigationPath()
    {
        GPSLocation reconstructedPlayerLocation = GetGPSFromWorldPos(_player.position);
        GPSLocation destinationLocation = new GPSLocation(GameManager.Instance.CurrentLocation);

        GetDirections(reconstructedPlayerLocation, destinationLocation);
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

    private void InitializePathLineRenderers(Vector3 pDestinationPos)
    {
        // Check the closest point where the player is
        int pathIndex = GetPathIndexRelativeToPlayer(_player.position);

        // Add one extra index for the player's location
        Vector3[] remaining = new Vector3[_receivedPath.Length + 2]; // The path and one extra for the player and the destination
        Vector3[] navigated = new Vector3[1]; // Only one for the player

        // Extract data from receivedPath
        remaining[0] = pDestinationPos;
        remaining[remaining.Length - 1] = _player.position;

        for (int i = 0; i < _receivedPath.Length; ++i)
            remaining[i + 1] = _receivedPath[i];

        navigated[0] = _player.position;

        // Apply arrays to line renderers
        SetPath(_remainingPathLR, remaining);
        SetPath(_navigatedPathLR, navigated);
    }

    private void SetPath(LineRenderer pLineRenderer, Vector3[] pPath)
    {
        pLineRenderer.positionCount = pPath.Length;
        pLineRenderer.SetPositions(pPath);
    }

    private void GetDirections(GPSLocation pStart, GPSLocation pEnd)
    {
        if (_directionRoutine != null)
            StopCoroutine(_directionRoutine);

        _directionRoutine = StartCoroutine(GetDirectionsInternal(pStart, pEnd));
    }

    private IEnumerator GetDirectionsInternal(GPSLocation pStart, GPSLocation pEnd)
    {
        string request = String.Format("foot-walking?api_key={0}&start={1}&end={2}", apiKey, pStart, pEnd);

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
                SetPath(_remainingPathLR, _receivedPath);

                _navigatedPathLR.positionCount = 2;
                _navigatedPathLR.SetPosition(0, _receivedPath[0]);
                _navigatedPathLR.SetPosition(1, _receivedPath[0]);

                Vector3 destinationPos = GetWorldPosFromGPS(pEnd.latitude, pEnd.longitude);

                InitializePathLineRenderers(destinationPos);

                if (_strayDetectionRoutine != null)
                    StopCoroutine(_strayDetectionRoutine);

                if (_lineRendererUpdateRoutine != null)
                    StopCoroutine(_lineRendererUpdateRoutine);

                _strayDetectionRoutine = StartCoroutine(StrayDetectionRoutine());
                _lineRendererUpdateRoutine = StartCoroutine(LineRendererUpdateRoutine());
            }
        }
    }

    private IEnumerator StrayDetectionRoutine()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);

        Vector3 closestPathSegmentPos;
        Vector3 distanceVector;

        while (true)
        {
            closestPathSegmentPos = _receivedPath[GetPathIndexRelativeToPlayer(_player.position)];
            distanceVector = _player.position - closestPathSegmentPos;

            if(distanceVector.magnitude >= _maxStrayDistance)
            {
                // Player strayed outside of the max stray radius
                RequestNewNavigationPath();
                yield break;
            }

            yield return waitForSeconds;
        }
    }

    private IEnumerator LineRendererUpdateRoutine()
    {
        Vector3 currentSegmentPosition;
        Vector3 distanceVector;

        while (true)
        {
            currentSegmentPosition = _remainingPathLR.GetPosition(_remainingPathLR.positionCount - 2); // Take the position in front of the player (player is last)
            distanceVector = _player.position - currentSegmentPosition;

            if(distanceVector.magnitude <= _minSegmentDistance)
            {
                ++_navigatedPathLR.positionCount;
                _navigatedPathLR.SetPosition(_navigatedPathLR.positionCount - 1, currentSegmentPosition);

                if(_remainingPathLR.positionCount > 1)
                {
                    --_remainingPathLR.positionCount;
                    _remainingPathLR.SetPosition(_remainingPathLR.positionCount - 1, _player.position);
                }
                else
                {
                    Debug.Log("No more path segments found");
                    yield break;
                }
            }

            yield return null;
        }
    }

    private int GetPathIndexRelativeToPlayer(Vector3 pPlayerPosition)
    {
        int index = 0;
        float smallestDistance = Mathf.Infinity;

        pPlayerPosition.y = 0.0f; // Move all positions to compare to the same Y position

        Vector3 currentPosition;
        Vector3 distanceVector;

        for (int i = 0; i < _receivedPath.Length; ++i)
        {
            currentPosition = _receivedPath[i];
            currentPosition.y = 0.0f;

            distanceVector = currentPosition - pPlayerPosition;
            float sqrDistance = distanceVector.sqrMagnitude;

            if (sqrDistance < smallestDistance)
            {
                index = i;
                smallestDistance = sqrDistance;
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
            return geometry;
        }

        Vector3 worldPos;

        for (int i = 0; i < geometry.Length; ++i) // Fill backwards so we can reuse it in the line renderer
        {
            worldPos = GetWorldPosFromGPS(feature.geometry.coordinates[geometry.Length - (i + 1)][1], feature.geometry.coordinates[geometry.Length - (i + 1)][0]);
            geometry[i] = worldPos;
        }

        return geometry;
    }

    public Transform Player
    {
        get
        {
            return _player;
        }
    }
}