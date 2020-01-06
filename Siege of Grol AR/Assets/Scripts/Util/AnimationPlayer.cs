using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AnimationPlayer : MonoBehaviour
{
    [SerializeField]
    private VideoClip _animationClip;

    [SerializeField]
    private GameObject _background;

    [SerializeField]
    private GameObject _arCanvas;

    private VideoPlayer _videoPlayer;
    private Coroutine _preparationRoutine;

    private bool _shouldSkipVideo;
    private float _skipTimer;

    private void Awake()
    {
        Camera mainCamera = Camera.main;

        _videoPlayer = mainCamera.gameObject.AddComponent<VideoPlayer>();
        transform.parent = mainCamera.transform; // Parent to the camera for easier (but not ideal) access from other scripts

        InitializeVideoPlayer();
    }

    private void Update()
    {
        CheckForSkip();
    }

    public void PlayAnimation()
    {
        StartCoroutine(PlayAnimationClipRoutine());
    }

    public IEnumerator PlayAnimationClipRoutine()
    {
        _videoPlayer.enabled = true;

        if (!_videoPlayer.isPrepared)
        {
            Debug.LogError("AnimationPlayer::Unable to play animation clip " + _animationClip.name + ", the clip has not been prepared yet!");
            yield break;
        }

        if(_videoPlayer.isPlaying)
        {
            Debug.LogError("AnimationPlayer::Unable to play animation clip " + _animationClip.name + ", a clip is already playing on the VideoPlayer");
            yield break;
        }

        _background.SetActive(true);
        _arCanvas.SetActive(false);
        _videoPlayer.Play();

        while(_videoPlayer.isPlaying)
        {
            if (_shouldSkipVideo)
            {
                _videoPlayer.Stop();
                break;
            }
            
            yield return null;
        }

        _background.SetActive(false);
        _arCanvas.SetActive(true);
        _videoPlayer.enabled = false;
    }

    private void InitializeVideoPlayer()
    {
        if(_videoPlayer == null)
        {
            Debug.LogError("AnimationPlayer::Unable to initialize video player, it was null! Make sure there is a Main camera active in the scene.");
            return;
        }

        _videoPlayer.playOnAwake = false;
        _videoPlayer.clip = _animationClip;

        _videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;
        _videoPlayer.targetCameraAlpha = 1.0f;
        _videoPlayer.isLooping = false;

        _shouldSkipVideo = false;

        PrepareAnimationClip();
    }

    private void PrepareAnimationClip()
    {
        // Returns true, if the video preparation could be started, otherwise returns false
        if (_preparationRoutine != null)
            return;

        _preparationRoutine = StartCoroutine(PrepareAnimationClipRoutine());
    }

    private IEnumerator PrepareAnimationClipRoutine()
    {
        _videoPlayer.Prepare();

        while (!_videoPlayer.isPrepared)
            yield return null;

        Debug.Log("Successfully prepared clip " + _animationClip.name);
        _preparationRoutine = null;
    }

    private void CheckForSkip()
    {
        if (!_videoPlayer.isPlaying)
            return;

        if (Input.GetMouseButton(0) && !_shouldSkipVideo)
        {
            _skipTimer += Time.deltaTime;

            if (_skipTimer >= 3.0f)
                _shouldSkipVideo = true;
        }
        else if (Input.GetMouseButtonUp(0) && !_shouldSkipVideo)
        {
            _skipTimer = 0.0f;
        }
    }
}
