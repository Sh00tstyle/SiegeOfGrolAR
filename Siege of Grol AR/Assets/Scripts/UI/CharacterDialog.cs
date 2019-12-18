﻿using GoogleARCore;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CharacterDialog : MonoBehaviour
{
    [SerializeField] GameObject _priest, _drunkard, _cannonCommander;
    [SerializeField] Camera _firstPersonCamera;
    [SerializeField] TextMeshProUGUI _narrationTextField, _narratorNameField;
    [SerializeField] GameObject _narrationCanvas;
    [SerializeField] Narration[] _priestNarration, _drunkardNarration, _cannonNarration;

    [Serializable]
    public struct Narration
    {
        public string Text;
        //  Later use
        //public AnimationCurve Timing;
        //public float Time, TimeAfter;
        //public AudioClip AudioClip;
    }

    private GameObject _currentObject;
    private Narration[] _currentNaration;
    private int _currentNarationIndex;

    private bool _hasPlaced;

    void Awake()
    {
        _hasPlaced = false;
        _currentNarationIndex = 0;

        Progress storyProgress = (Progress)ProgressHandler.Instance.StoryProgressIndex;
        switch (storyProgress)
        {
            case Progress.Priest:
                _currentObject = _priest;
                _currentNaration = _priestNarration;
                break;
            case Progress.Drunkard:
                _currentObject = _drunkard;
                _currentNaration = _drunkardNarration;
                break;
            case Progress.CannonCommander:
                _currentObject = _cannonCommander;
                _currentNaration = _cannonNarration;
                break;
        }

        _narratorNameField.text = storyProgress.ToString();
    }

    void Update()
    {
        if (_hasPlaced)
        {
            DetectCharacterSelect();
            return;
        }

        if (!_hasPlaced)
            PlacePreview();

    }

    void DetectCharacterSelect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.gameObject.tag == Tags.ARCharacter)
                    NextNarration();
            }
        }

    }

    void PlacePreview()
    {
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(Screen.width * 0.5f, Screen.height * 0.5f, raycastFilter, out hit)) // Middle of the screen
        {
            if ((hit.Trackable is DetectedPlane) && Vector3.Dot(_firstPersonCamera.transform.position - hit.Pose.position, hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                // Enable object
                if (!_currentObject.gameObject.activeSelf)
                    _currentObject.gameObject.SetActive(true);

                // Place object in center of the screen
                if (Input.GetMouseButtonDown(0) && !_hasPlaced)
                    PlaceCharacter(hit);
                else
                    PositionCharacter(hit);
            }
        }

    }


    private void PositionCharacter(TrackableHit pHit)
    {
        _currentObject.transform.position = pHit.Pose.position;
        _currentObject.transform.rotation = pHit.Pose.rotation;
    }


    private void PlaceCharacter(TrackableHit pHit)
    {
        // Position object
        PositionCharacter(pHit);

        // Anchor it and parent it to the anchor
        Anchor anchor = pHit.Trackable.CreateAnchor(pHit.Pose);
        _currentObject.transform.parent = anchor.transform;

        _hasPlaced = true;
        _narrationCanvas.SetActive(true);
        ChangeText();
    }

    private void ChangeText()
    {
        Narration narration = _currentNaration[_currentNarationIndex];
        _narrationTextField.text = narration.Text;
    }

    public void NextNarration()
    {
        _currentNarationIndex++;

        if (_currentNarationIndex >= _currentNaration.Length)
            SceneHandler.Instance.LoadScene(2);
        else
            ChangeText();

    }
}
