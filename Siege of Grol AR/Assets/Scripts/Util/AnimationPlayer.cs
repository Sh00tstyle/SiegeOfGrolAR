using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AnimationPlayer : MonoBehaviour
{
    [SerializeField]
    private VideoClip _animationClip;

    private VideoPlayer _videoPlayer;
    private Coroutine _preparationRoutine;

    private void Awake()
    {
        Camera mainCamera = Camera.main;

        _videoPlayer = mainCamera.gameObject.AddComponent<VideoPlayer>();
        transform.parent = mainCamera.transform; // Parent to the camera for easier (but not ideal) access from other scripts

        InitializeVideoPlayer();
    }

    public void PlayAnimation()
    {
        StartCoroutine(PlayAnimationClipRoutine());
    }

    public IEnumerator PlayAnimationClipRoutine()
    {
        if(!_videoPlayer.isPrepared)
        {
            Debug.LogError("AnimationPlayer::Unable to play animation clip " + _animationClip.name + ", the clip has not been prepared yet!");
            yield break;
        }

        if(_videoPlayer.isPlaying)
        {
            Debug.LogError("AnimationPlayer::Unable to play animation clip " + _animationClip.name + ", a clip is already playing on the VideoPlayer");
            yield break;
        }

        _videoPlayer.Play();

        while(_videoPlayer.isPlaying)
        {
            yield return null;
        }
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

    private void DisableVideoPlayer()
    {
        _videoPlayer.enabled = false;
    }
}
