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
    private Transform _currentLocationTransform;
    private Narrator _currentLocationNarrator;
    private Interaction _currentInteraction;


    private int _currentLocationIndex;

    protected override void Initialize()
    {
        _currentLocationIndex = 0;

        _currentLocation = _locationDatabase.locations[_currentLocationIndex];
        CreateNewLocation();
    }

    public void SetStoryDecision(bool isHelpingSpy)
    {
        _isHelpingSpy = isHelpingSpy;

        // Could notify other entities at this point, but not sure if it is needed?
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

    private void CreateNewLocation()
    {
        GameObject instantiatedPrefab;

        instantiatedPrefab = Instantiate(_currentLocation.locationPrefab);
        _currentLocationTransform = instantiatedPrefab.transform;
        _currentLocationNarrator = instantiatedPrefab.GetComponent<Narrator>();

        if (_currentLocationIndex == 0)
            return; // this is the first location, the story decision will take place here

        if(_isHelpingSpy)
        {
            instantiatedPrefab = Instantiate(_currentLocation.helpInteractionPrefab, _currentLocationTransform);
            _currentInteraction = instantiatedPrefab.GetComponent<Interaction>();
        }
        else
        {
            instantiatedPrefab = Instantiate(_currentLocation.sabotageInteractionPrefab, _currentLocationTransform);
            _currentInteraction = instantiatedPrefab.GetComponent<Interaction>();
        }

        // DEBUG: Activate to see if it works
        _currentInteraction.Activate();
    }
}
