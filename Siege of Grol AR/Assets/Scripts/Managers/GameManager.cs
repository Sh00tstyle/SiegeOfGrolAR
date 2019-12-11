using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private LocationDatabase _locationDatabase;

    [SerializeField]
    private float _minInteractionDistance = 1.0f;

    private bool _isHelpingSpy;

    private Location _currentLocation;
    private Transform _currentLocationTransform;
    private int _currentLocationIndex;

    private Narrator _currentNarrator;
    private Interaction _currentInteraction;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        _currentLocationIndex = 0;

        _currentLocation = _locationDatabase.locations[_currentLocationIndex];
        CreateNewLocation();
    }

    private void Update()
    {
        HandleLocationInput();
    }

    public void SetStoryDecision(bool pIsHelpingSpy)
    {
        _isHelpingSpy = pIsHelpingSpy;

        // Could notify other entities at this point, but not sure if it is needed since it is probably set by an interaction/behaviour
    }

    public void SetProgress(int pProgressIndex)
    {
        throw new NotImplementedException();
    }

    public void NextLocation()
    {
        ++_currentLocationIndex;

        if(_currentLocationIndex >= _locationDatabase.locations.Length)
        {
            _currentLocation = null; // no more locations left
            return;
        }

        _currentLocation = _locationDatabase.locations[_currentLocationIndex];
        CreateNewLocation();
    }

    public void StartInteraction()
    {
        if (_currentInteraction != null)
            _currentInteraction.Activate();
    }

    private void CreateNewLocation()
    {
        Vector3 locationPos = NavigationManager.Instance.GetWorldPosFromGPS(_currentLocation.latitude, _currentLocation.longitude);
        _currentLocationTransform = Instantiate(_currentLocation.locationPrefab, locationPos, Quaternion.identity).transform;

        if (_isHelpingSpy && _currentLocation.helpInteractionPrefab != null)
        {
            Instantiate(_currentLocation.helpInteractionPrefab, _currentLocationTransform);
        }
        else if(!_isHelpingSpy && _currentLocation.sabotageInteractionPrefab != null)
        {
            Instantiate(_currentLocation.sabotageInteractionPrefab, _currentLocationTransform);
        }
    }

    private void HandleLocationInput()
    {
        if(_currentLocationTransform != null)
        {
            Vector3 distanceVector = NavigationManager.Instance.PlayerTransform.position - _currentLocationTransform.position;

            if (distanceVector.magnitude > _minInteractionDistance) // Do not allow interaction input if the player is not nearby
                return;
        }

        if (Input.GetMouseButtonDown(0)) // Detect the location on mouse button press / touch
        {
            RaycastHit hit;
            Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == Tags.Location)
                {
                    Debug.Log("Clicked location");

                    // Store the currently selected narration and interaction
                    _currentNarrator = hit.transform.GetComponent<Narrator>();
                    _currentInteraction = _currentNarrator.GetComponentInChildren<Interaction>();

                    if (_currentNarrator != null)
                        _currentNarrator.OpenMenu();
                }
            }
        }
    }

    public bool IsHelpingSpy
    {
        get
        {
            return _isHelpingSpy;
        }
    }

    public Transform CurrentLocationTransform
    {
        get
        {
            return _currentLocationTransform;
        }
    }

    public Location CurrentLocation
    {
        get
        {
            return _currentLocation;
        }
    }

    public Narrator CurrentNarrator
    {
        get
        {
            return _currentNarrator;
        }
    }

    public Interaction CurrentInteraction
    {
        get { 
            return _currentInteraction; 
        }
    }
}
