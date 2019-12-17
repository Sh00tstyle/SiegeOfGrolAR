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
    private GameObject _currentLocationObject;
    private int _currentLocationIndex;

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
        int currentProgressIndex = ProgressHandler.Instance.StoryProgressIndex;

        for (int i = 0; i <= currentProgressIndex; ++i) // Create the location for each progress point that was reached
            CreateLocation(i);

        if (currentProgressIndex > 0)
            MenuManager.Instance.GoToMenu(MenuTypes.MAINMENU);
    }

    public void NextStorySegment()
    {
        // Only needed if the progress is increased outside of an AR interaction

        if (IncreaseLocationIndex()) // Create the next location if there is one remaining
        {
            CreateLocation(_currentLocationIndex);
        }

        ProgressHandler.Instance.IncreaseStoryProgress(); // Update the story progress as well
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
        locationPos.y = 0.1f;

        _currentLocationObject = Instantiate(_currentLocation.locationPrefab, locationPos, Quaternion.identity);
    }

    private void HandleLocationInput()
    {
        if(_currentLocationObject != null)
        {
            Vector3 distanceVector = NavigationManager.Instance.PlayerTransform.position - _currentLocationObject.transform.position;

            if (distanceVector.magnitude > _minInteractionDistance) // Do not allow interaction input if the player is not nearby
                return;
        }

        if (Input.GetMouseButtonDown(0)) // Detect the location on mouse button press / touch
        {
            RaycastHit hit;
            Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == Tags.Location && hit.transform.gameObject == _currentLocationObject)
                {
                    SceneHandler.Instance.LoadScene(1); // Dialog scene
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
            return _currentLocationObject.transform;
        }
    }

    public Location CurrentLocation
    {
        get
        {
            return _currentLocation;
        }
    }
}
