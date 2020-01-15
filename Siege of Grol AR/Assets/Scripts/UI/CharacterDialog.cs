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
    private Animator _currentAnimator;

    private bool _hasPlaced, _hasPositioned;

    private Coroutine _finishRoutine;
    private Progress _storyProgress;

    private TrackableHit _hit;


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

        _narratorNameField.text = _storyProgress.ToString().ToUpper();
        _currentAnimator = _currentObject.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (_hasPlaced)
        {
            // DetectCharacterSelect();
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

        if (Frame.Raycast(Screen.width * 0.5f, Screen.height * 0.5f, TrackableHitFlags.PlaneWithinPolygon, out hit)) // Middle of the screen
        {
            if (hit.Pose != null)
                _hit = hit;

            if ((_hit.Trackable is DetectedPlane) && Vector3.Dot(_firstPersonCamera.transform.position - _hit.Pose.position, _hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                // Enable object
                if (!_currentObject.gameObject.activeSelf)
                    _currentObject.gameObject.SetActive(true);

                // Place object in center of the screen
                PositionCharacter(_hit);

            }
        }

        if (Input.GetMouseButton(0) && !_hasPlaced && _hasPositioned)
            PlaceCharacter(_hit);
    }

    private void PositionCharacter(TrackableHit pHit)
    {
        _currentObject.transform.position = pHit.Pose.position;
        _currentObject.transform.rotation = pHit.Pose.rotation;

        _hasPositioned = true;
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

        _audioComponent.clip = _currentNaration[_currentNarrationIndex].AudioClip;
        _audioComponent.Play();

        if (_currentAnimator != null)
            _currentAnimator.Play("Talking");
    }

    public void NextNarration()
    {
        _currentNarrationIndex++;

        if (_currentNarrationIndex >= _currentNaration.Length && _finishRoutine == null)
            _finishRoutine = StartCoroutine(FinishDialog());
        else if (_currentNarrationIndex < _currentNaration.Length)
        {
            ChangeText();
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
        AudioManager.Instance.StopPlaying("PriestBG");

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
        AudioManager.Instance.Play("GameBG");
        SceneHandler.Instance.LoadScene(Scenes.Map); // Load into the map and show the decision screen
    }

    private IEnumerator FinalizeDrunkard()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        AudioManager.Instance.StopPlaying("DrunkardBG");

        _finishRoutine = null;
        AudioManager.Instance.Play("DrunkGameBG");
        SceneHandler.Instance.LoadScene(Scenes.DrunkardInteraction);
    }

    private IEnumerator FinalizeCannonCommander()
    {
        AudioManager.Instance.StopPlaying("CommanderBG");
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
        AudioManager.Instance.Play("CannonTheme");
        SceneHandler.Instance.LoadScene(Scenes.CannonInteraction);
    }
}
