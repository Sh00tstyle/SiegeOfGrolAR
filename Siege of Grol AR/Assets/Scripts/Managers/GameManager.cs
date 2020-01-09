﻿using System;
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

    private Location _currentLocation;
    private GameObject _currentLocationObject;
    private Transform _currentLocationModelTransform;
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
        ProgressHandler.Instance.SetStoryDecision(pIsHelpingSpy);
        MenuManager.Instance.GoToMenu(MenuTypes.MAINMENU);
    }

    public void SelectFinalLetterCharacter(int pCharacterIndex)
    {
        LetterCharacter character = (LetterCharacter)pCharacterIndex;
        Debug.Log("Selected " + character);

        // Frame Citizen: Commander, Identify Spy: Drunkard
        if((ProgressHandler.Instance.IsHelpingSpy && character == LetterCharacter.Commander) || (!ProgressHandler.Instance.IsHelpingSpy && character == LetterCharacter.Drunkard))
        {
            MenuManager.Instance.ShowPopup("Great!", "I knew I could count on you!", "OK", () => {
                ProgressHandler.Instance.IncreaseStoryProgress();
                MenuManager.Instance.GoToMenu(MenuTypes.MAINMENU);
                NavigationManager.Instance.SetLineRendererVisibility(false, false);
            });
        }
        else
        {
            MenuManager.Instance.ShowPopup("Are you sure?", "It seems like you selected the wrong option, try again!", "OK", null);
        }
    }

    public void LoadStoryProgress()
    {
        _currentLocationIndex = 0;
        int currentProgressIndex = ProgressHandler.Instance.StoryProgressIndex;

        for (int i = 0; i <= currentProgressIndex; ++i) // Create the location for each progress point that was reached
            CreateLocation(i);


        switch(currentProgressIndex)
        {
            case 0: // Do nothing, just there to prevent going into "default" during the first load
                break;

            case 1: // Priest decision
                MenuManager.Instance.GoToMenu(MenuTypes.PRIESTDECISIONMENU);
                break;

            case 3: // Selection for framing/identification
                MenuManager.Instance.GoToMenu(MenuTypes.FINALLETTERMENU);
                break;

            default: // Default return to the map
                MenuManager.Instance.GoToMenu(MenuTypes.MAINMENU);
                break;
        }
            
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
        locationPos.y = 0.0f;
         
        _currentLocationObject = Instantiate(_currentLocation.locationPrefab, locationPos, Quaternion.identity);

        Transform[] childTransforms = _currentLocationObject.GetComponentsInChildren<Transform>();

        if (childTransforms.Length > 1)
            _currentLocationModelTransform = childTransforms[1];
        else
            _currentLocationModelTransform = null;
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
                    SceneHandler.Instance.LoadScene(Scenes.Dialog);
                }
            }
        }

    }

    public Transform CurrentLocationTransform
    {
        get
        {
            if (_currentLocationModelTransform != null)
                return _currentLocationModelTransform;
            
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

public enum LetterCharacter
{
    Priest, 
    Drunkard,
    Commander
}