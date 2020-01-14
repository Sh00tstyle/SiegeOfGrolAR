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
    [SerializeField] private AudioSource _audioComponent;
    [SerializeField] Narration[] _priestNarration, _drunkardNarration, _cannonNarration;
    

    [Serializable]
    public struct Narration
    {
        public string Text;
        //  Later use
        //public AnimationCurve Timing;
        //public float Time, TimeAfter;
        public AudioClip AudioClip;
    }

    private GameObject _currentObject;
    private Narration[] _currentNaration;
    private int _currentNarrationIndex;

    private bool _hasPlaced;

    private Coroutine _finishRoutine;
    private Progress _storyProgress;

    void Awake()
    {
        _hasPlaced = false;
        _currentNarrationIndex = 0;

        _storyProgress = (Progress)ProgressHandler.Instance.StoryProgressIndex;
        switch (_storyProgress)
        {
            case Progress.Priest:
                AudioManager.Instance.Play("PriestBG");
                _currentObject = _priest;
                _currentNaration = _priestNarration;
                break;
            case Progress.Drunkard:
                AudioManager.Instance.Play("DrunkardBG");
                _currentObject = _drunkard;
                _currentNaration = _drunkardNarration;
                break;
            case Progress.CannonCommander:
                AudioManager.Instance.Play("CommanderBG");
                _currentObject = _cannonCommander;
                _currentNaration = _cannonNarration;
                break;
        }

        _narratorNameField.text = _storyProgress.ToString();
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
        Narration narration = _currentNaration[_currentNarrationIndex];
        _narrationTextField.text = narration.Text;
    }

    public void NextNarration()
    {
        _currentNarrationIndex++;

        if (_currentNarrationIndex >= _currentNaration.Length && _finishRoutine == null)
            _finishRoutine = StartCoroutine(FinishDialog());
        else if (_currentNarrationIndex < _currentNaration.Length)
        {
            ChangeText();
            _audioComponent.clip = _currentNaration[_currentNarrationIndex].AudioClip;
            _audioComponent.Play();
            AudioManager.Instance.StopPlaying("PriestBG");
            AudioManager.Instance.StopPlaying("DrunkardBG");
            AudioManager.Instance.StopPlaying("CommanderBG");
        }
    }

    private IEnumerator FinishDialog()
    {
        switch (_storyProgress)
        {
            case Progress.Priest:
                yield return StartCoroutine(FinalizePriest());              
                break;

            case Progress.Drunkard:
                yield return StartCoroutine(FinalizeDrunkard());               
                break;

            case Progress.CannonCommander:
                yield return StartCoroutine(FinalizeCannonCommander());
                break;

            default:
                Debug.LogError("No dialog should be needed for this location/progress");
                break;
        }
    }

    private IEnumerator FinalizePriest()
    {
        AnimationPlayer animationPlayer = Camera.main.GetComponentInChildren<AnimationPlayer>();

        if (animationPlayer == null)
        {
            Debug.LogError("CharacterDialog::Unable to get the animation player so the animation will not be played");
            _finishRoutine = null;
            yield break;
        }

        _narrationCanvas.SetActive(false);

        yield return StartCoroutine(animationPlayer.PlayAnimationClipRoutine()); // Wait until the video has finished playing
        Debug.Log("Finished playing animation, loading the next scene...");

        _finishRoutine = null;
        ProgressHandler.Instance.IncreaseStoryProgress();
        SceneHandler.Instance.LoadScene(Scenes.Map); // Load into the map and show the decision screen
        AudioManager.Instance.Play("GameBG");
    }

    private IEnumerator FinalizeDrunkard()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        _finishRoutine = null;
        SceneHandler.Instance.LoadScene(Scenes.DrunkardInteraction);
        AudioManager.Instance.Play("");
    }

    private IEnumerator FinalizeCannonCommander()
    {
        AnimationPlayer animationPlayer = Camera.main.GetComponentInChildren<AnimationPlayer>();

        if (animationPlayer == null)
        {
            Debug.LogError("CharacterDialog::Unable to get the animation player so the animation will not be played");
            _finishRoutine = null;
            yield break;
        }

        _narrationCanvas.SetActive(false);

        yield return StartCoroutine(animationPlayer.PlayAnimationClipRoutine()); // Wait until the video has finished playing
        Debug.Log("Finished playing animation, loading the next scene...");

        _finishRoutine = null;
        SceneHandler.Instance.LoadScene(Scenes.CannonInteraction);
        AudioManager.Instance.Play("");
    }
}
