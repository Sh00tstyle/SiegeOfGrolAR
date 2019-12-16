using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    public string playerName;

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
        QualitySettings.vSyncCount = 0;;
    }

    private void Start()
    {
        LoadStoryProgress();
    }

    private void Update()
    {
        HandleLocationInput();
    }

    public void SetStoryDecision(bool pIsHelpingSpy)
    {
        _isHelpingSpy = pIsHelpingSpy;
    }

    public void LoadStoryProgress()
    {
        _currentLocationIndex = 0;
        int currentProgressIndex = ProgressHandler.Instance.StoryProgress;

        for (int i = 0; i <= currentProgressIndex; ++i) // Create the location for each progress point that was reached
            CreateLocation(i);

        if (currentProgressIndex > 0)
            MenuManager.Instance.GoToMenu(MenuTypes.MAINMENU);
    }

    public void NextStorySegment()
    {
        if (IncreaseLocationIndex()) // Create the next location if there is one remaining
        {
            CreateLocation(_currentLocationIndex);
        }

        ProgressHandler.Instance.IncreaseStoryProgress(); // Update the story progress as well
    }

    public void StartInteraction()
    {
        if (_currentInteraction != null)
            _currentInteraction.Activate();
    }

    private bool IncreaseLocationIndex()
    {
        if (_currentLocationIndex >= _locationDatabase.locations.Length - 1)
            return false; // No more locations left

        ++_currentLocationIndex;
        return true;
    }

    private void CreateLocation(int pIndex)
    {
        if (pIndex >= _locationDatabase.locations.Length)
            return;

        _currentLocation = _locationDatabase.locations[pIndex]; // Store the latest created location as current one

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
        get 
        { 
            return _currentInteraction; 
        }
    }
}
