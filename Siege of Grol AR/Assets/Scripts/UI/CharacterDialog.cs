using GoogleARCore;
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

    private bool _hasPlaced, _inNarration;
    void Awake()
    {
        Progress storyProgress = (Progress)ProgressHandler.Instance.StoryProgress;
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

        _narratorNameField.text = GameManager.Instance.CurrentLocation.characterName;
    }

    void Update()
    {
        if (_inNarration)
            return;

        if (!_hasPlaced)
            PlacePreview();
        else
            DetectCharacterSelect();
    }

    void DetectCharacterSelect()
    {
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (hit.transform.gameObject.tag == Tags.ARCharacter)
                    {
                        _inNarration = true;
                        _narrationCanvas.gameObject.SetActive(true);
                        ChangeText();
                    }
                }
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
    }

    private void ChangeText()
    {
        Narration narration = _currentNaration[_currentNarationIndex];
        _narrationTextField.text = narration.Text;
    }

    public void NextNarration()
    {
        _currentNarationIndex++;
        if (_currentNarationIndex > _currentNaration.Length)
            Debug.Log("Change scene");
        //SceneHandler.Instance.LoadScene(index);
        else
            ChangeText();

    }
}
