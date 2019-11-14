using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private LocationDatabase _locationDatabase;

    private bool _isHelpingSpy;

    private Location _currentLocation;
    private int _currentLocationIndex;

    private Narrator _currentNarrator;
    private Interaction _currentInteraction;

    private void Awake()
    {
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
        Vector3 locationPos = GPSManager.Instance.GetWorldPosFromGPS(_currentLocation.latitude, _currentLocation.longitude);
        Transform locationTransform = Instantiate(_currentLocation.locationPrefab, locationPos, Quaternion.identity).transform;

        if(_isHelpingSpy && _currentLocation.helpInteractionPrefab != null)
        {
            Instantiate(_currentLocation.helpInteractionPrefab, locationTransform);
        }
        else if(!_isHelpingSpy && _currentLocation.sabotageInteractionPrefab != null)
        {
            Instantiate(_currentLocation.sabotageInteractionPrefab, locationTransform);
        }
    }

    private void HandleLocationInput()
    {
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
