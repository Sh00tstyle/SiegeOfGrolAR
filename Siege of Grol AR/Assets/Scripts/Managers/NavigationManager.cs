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
    const string apiKey = "5b3ce3597851110001cf6248296d7129fb1343a4bf2e43eca3d631a1";
    const string baseURI = "https://api.openrouteservice.org/v2/directions/";

    [SerializeField]
    private Transform _mapTransform;

    [SerializeField]
    private Transform _playerTransform;

    [SerializeField]
    private double _referenceLatitude = 52.042947;

    [SerializeField]
    private double _referenceLongitude = 6.616385;

    [SerializeField]
    private float _worldScale = 1.0f;

    [SerializeField] 
    private LineRenderer _remainingPathLR, _navigatedPathLR;

    [SerializeField]
    private float _maxStrayDistance = 0.5f;

    [SerializeField]
    private float _minSegmentDistance = 0.15f;

    [SerializeField]
    private float _maxPathUpdateInterval = 2.0f;

    private Vector3[] _receivedPath;

    private Coroutine _directionRoutine;
    private Coroutine _strayDetectionRoutine;
    private Coroutine _lineRendererUpdateRoutine;

    private int _currentNavigationIndex;

    private MapPoint _convertedReferencePos;

    private void Awake()
    {
        _convertedReferencePos = Util.GPS2MapPoint(_referenceLatitude, _referenceLongitude);
    }

    public void SetLineRendererVisibility(bool pRemainingLR, bool pNavigatedLR)
    {
        _remainingPathLR.enabled = pRemainingLR;
        _navigatedPathLR.enabled = pNavigatedLR;
    }

    public void RequestNewNavigationPath()
    {
        GPSLocation reconstructedPlayerLocation = GetGPSFromWorldPos(_playerTransform.position);
        GPSLocation destinationLocation = new GPSLocation(GameManager.Instance.CurrentLocation);
        
        GetDirections(reconstructedPlayerLocation, destinationLocation);
    }

    public Vector3 GetWorldPosFromGPS(double pLatitude, double pLongitude)
    {
        // Project the GPS position on a plane
        MapPoint projectedPoint = Util.GPS2MapPoint(pLatitude, pLongitude);

        // Get the position on the map by using a reference GPS position
        projectedPoint.x -= _convertedReferencePos.x;
        projectedPoint.y -= _convertedReferencePos.y;

        projectedPoint.x *= _worldScale;
        projectedPoint.y *= _worldScale * 0.6;

        // Convert the resulting position from RH to LH
        Vector3 worldPos = new Vector3((float)projectedPoint.x, 0.005f, (float)projectedPoint.y);
        worldPos.x *= -1;
        worldPos = Quaternion.Euler(0.0f, -90.0f, 0.0f) * worldPos;

        return worldPos;
    }

    public GPSLocation GetGPSFromWorldPos(Vector3 pWorldPos)
    {
        pWorldPos = Quaternion.Euler(0.0f, 90.0f, 0.0f) * pWorldPos;
        pWorldPos.x *= -1;

        MapPoint absoluteWorldPos = new MapPoint(pWorldPos.x, pWorldPos.z);

        absoluteWorldPos.x /= _worldScale;
        absoluteWorldPos.y /= _worldScale * 0.6;

        absoluteWorldPos.x += _convertedReferencePos.x;
        absoluteWorldPos.y += _convertedReferencePos.y;

        return Util.MapPoint2GPS(absoluteWorldPos.x, absoluteWorldPos.y);
    }

    private void InitializePathLineRenderers(Vector3 pDestinationPos)
    {
        // Check the closest point where the player is
        int pathIndex = GetClosestPathIndex(_playerTransform.position);

        // Add one extra index for the player's location
        Vector3[] remaining = new Vector3[_receivedPath.Length + 2]; // The path and one extra for the player and the destination
        Vector3[] navigated = new Vector3[1]; // Only one for the player

        remaining[0] = pDestinationPos;
        remaining[remaining.Length - 1] = _playerTransform.position;

        for (int i = 0; i < _receivedPath.Length; ++i)
            remaining[i + 1] = _receivedPath[i];

        navigated[0] = _playerTransform.position;

        for (int i = 0; i < remaining.Length; ++i)
            remaining[i].y = 0.01f;

        for (int i = 0; i < navigated.Length; ++i)
            navigated[i].y = 0.01f;

        // Apply arrays to line renderers
        SetPath(_remainingPathLR, remaining);
        SetPath(_navigatedPathLR, navigated);

        _currentNavigationIndex = _receivedPath.Length - 1; // Take the last position since we filled the path reversed
    }

    private void SetPath(LineRenderer pLineRenderer, Vector3[] pPath)
    {
        pLineRenderer.positionCount = pPath.Length;
        pLineRenderer.SetPositions(pPath);
    }

    private void GetDirections(GPSLocation pStart, GPSLocation pEnd)
    {
        if (_directionRoutine != null) // The routine is still running, so don't disrupt it
            return;

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
                    Debug.LogError("The features was null, make sure a valid API key is in use");
                    yield break;
                }

                if (response.features != null && response.features.Count <= 0)
                {
                    Debug.LogError("Features list was empty");
                    yield break;
                }

                _receivedPath = GeometryToVector3(response.features[0]);

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

        _directionRoutine = null;
    }

    private IEnumerator StrayDetectionRoutine()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(_maxPathUpdateInterval);

        Vector3 closestPathSegmentPos;
        Vector3 distanceVector;

        float timer = 0.0f;

        while (true)
        {
            closestPathSegmentPos = _receivedPath[GetClosestPathIndex(_playerTransform.position, _currentNavigationIndex)];
            distanceVector = _playerTransform.position - closestPathSegmentPos;

            timer += Time.deltaTime;

            if(distanceVector.magnitude >= _maxStrayDistance && timer >= _maxPathUpdateInterval)
            {
                // Player strayed outside of the max stray radius
                RequestNewNavigationPath();
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator LineRendererUpdateRoutine()
    {
        Vector3 currentSegmentPosition;
        Vector3 distanceVector;

        while (true)
        {
            // Always update the player position
            Vector3 playerPos = _playerTransform.position;
            playerPos.y = 0.01f;

            _remainingPathLR.SetPosition(_remainingPathLR.positionCount - 1, playerPos);
            _navigatedPathLR.SetPosition(0, playerPos);

            // Update the line renderer if needed
            if (_remainingPathLR.positionCount < 2)
            {
                Debug.Log("No more path segments found");
                yield break;
            }

            // Check if the line renderer segments have to be updated
            int closestPathIndex = GetClosestPathIndex(playerPos, _currentNavigationIndex);
            int difference = _currentNavigationIndex - closestPathIndex;

            currentSegmentPosition = _receivedPath[closestPathIndex]; // Take the position in front of the player (player is last)
            distanceVector = _playerTransform.position - currentSegmentPosition;

            if(difference == 0)
            {
                if (distanceVector.magnitude <= _minSegmentDistance)
                {
                    ++_navigatedPathLR.positionCount;
                    _navigatedPathLR.SetPosition(_navigatedPathLR.positionCount - 1, currentSegmentPosition);

                    --_remainingPathLR.positionCount;
                    _remainingPathLR.SetPosition(_remainingPathLR.positionCount - 1, playerPos);

                    if (_currentNavigationIndex > 0)
                        --_currentNavigationIndex;
                }
            }
            else if(difference > 0) // We navigated one or more segments further
            {
                for (int i = closestPathIndex; i < _currentNavigationIndex; ++i)
                {
                    if (i == closestPathIndex && difference == 1 && distanceVector.magnitude > _minSegmentDistance) // Skip processing the current index if the threshold was not met yet
                        continue;

                    // Make more space for another navigation segment 
                    --_remainingPathLR.positionCount;

                    ++_navigatedPathLR.positionCount;
                    _navigatedPathLR.SetPosition(_navigatedPathLR.positionCount - 1, _receivedPath[i]); // Always append the position
                }

                _remainingPathLR.SetPosition(_remainingPathLR.positionCount - 1, playerPos);
                _currentNavigationIndex = closestPathIndex;
            }

            yield return null;
        }
    }

    private int GetClosestPathIndex(Vector3 pPlayerPosition, int pMaxIndex = -1)
    {
        int index = 0;
        float smallestDistance = Mathf.Infinity;

        pPlayerPosition.y = 0.0f; // Move all positions to compare to the same Y position

        Vector3 currentPosition;
        Vector3 distanceVector;

        if (pMaxIndex < 0)
            pMaxIndex = _receivedPath.Length - 1;

        for (int i = 0; i <= pMaxIndex; ++i)
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

    private Vector3[] GeometryToVector3(Feature pFeature)
    {
        Vector3[] geometry = new Vector3[pFeature.geometry.coordinates.Count];

        if(GPSManager.Instance == null)
        {
            Debug.LogError("The GPSManager was null");
            return geometry;
        }

        Vector3 worldPos;

        for (int i = 0; i < geometry.Length; ++i) // Fill backwards so we can reuse it in the line renderer
        {
            worldPos = GetWorldPosFromGPS(pFeature.geometry.coordinates[geometry.Length - (i + 1)][1], pFeature.geometry.coordinates[geometry.Length - (i + 1)][0]); // Parse latitude before longitude for our struct
            geometry[i] = worldPos;
        }

        return geometry;
    }

    public Transform PlayerTransform
    {
        get
        {
            return _playerTransform;
        }
    }
}
