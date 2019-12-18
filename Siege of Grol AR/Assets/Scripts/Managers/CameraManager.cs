using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField]
    private Transform _orbitTarget;

    [SerializeField]
    private float _distance = 5.0f; 

    [SerializeField]
    private float _xSpeed = 120.0f;

    [SerializeField]
    private float _ySpeed = 120.0f;

    [SerializeField]
    private float _yMinLimit = -20f;

    [SerializeField]
    private float _yMaxLimit = 80f;

    [SerializeField]
    private float _minDistance = 2.0f;

    [SerializeField]
    private float _maxDistance = 15.0f;

    private Camera _mainCamera;

    private float _rotationYAxis;
    private float _rotationXAxis;

    private Sequence _objectFocusSequence;

    private float _previousTouchDistance;

    private void Awake()
    {
        if(_orbitTarget == null)
        {
            Debug.LogError("CameraManager::The orbit target was null, make sure it is assigned in the inspector of " + name);
            return;
        }
        
        UpdateCameraOrientation(_orbitTarget.position);

        _previousTouchDistance = 0;
    }

    private void LateUpdate()
    {
        if (_orbitTarget == null)
            return;

        HandleCameraInput();
    }

    public Sequence SwitchFocusObject(Transform pFocusObject, float pDuration, float pZoomInDistance, float pZoomOutDistance, Ease pEase)
    {
        Sequence focusSwitchSequence = DOTween.Sequence();

        // Append the zoom out tweening animation
        Vector3 zoomOutPos = GetCameraPosForTarget(_orbitTarget.position, pZoomOutDistance);
        focusSwitchSequence.Append(_mainCamera.transform.DOMove(zoomOutPos, pDuration / 3.0f).SetEase(pEase));

        // Append the movement tweening animation
        Vector3 moveTargetPos = GetCameraPosForTarget(pFocusObject.position, pZoomOutDistance);
        focusSwitchSequence.Append(_mainCamera.transform.DOMove(moveTargetPos, pDuration / 3.0f).SetEase(pEase));

        // Append the zoom in tweening animation
        Vector3 zoomInTargetPos = GetCameraPosForTarget(pFocusObject.position, pZoomInDistance);
        focusSwitchSequence.Append(_mainCamera.transform.DOMove(zoomInTargetPos, pDuration / 3.0f).SetEase(pEase));

        _distance = pZoomInDistance;
        _orbitTarget = pFocusObject;

        _objectFocusSequence = focusSwitchSequence;
        return focusSwitchSequence;
    }

    private void HandleCameraInput()
    {
        if (_objectFocusSequence != null && _objectFocusSequence.active)
            return;

#if UNITY_EDITOR
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        
        if(!Mathf.Approximately(scrollDelta, 0.0f))
            _distance -= scrollDelta * 2.0f;

#else
        if(Input.touchCount >= 2) // For detecting a pinch gesture we need two or more fingers (but the first two count)
        {
            Touch firstTouch = Input.GetTouch(0);
            Touch secondTouch = Input.GetTouch(1);

            Vector2 distanceVector = firstTouch.position - secondTouch.position;

            if (Mathf.Approximately(_previousTouchDistance, 0.0f))
                _previousTouchDistance = distanceVector.magnitude;

            float difference = _previousTouchDistance - distanceVector.magnitude;

            if (!Mathf.Approximately(difference, 0.0f)) // Ignore differences that are (almost) zero
            _distance += difference * 0.005f;

            _previousTouchDistance = distanceVector.magnitude;
        }
        else 
        {
            _previousTouchDistance = 0.0f;
        }
#endif

        _distance = ClampDistance(_distance);

        RenderSettings.fogDensity = _distance / _maxDistance * 0.075f;

        UpdateCameraOrientation(_orbitTarget.position); // This is probably irrelevant on mobile
    }

    private void UpdateCameraOrientation(Vector3 pOrbitTargetPos)
    {
        bool allowInput = false;

#if UNITY_EDITOR
        allowInput = Input.GetMouseButton(0) && !JoystickHandler.IsUsingJoystick;
#else
        allowInput = Input.touchCount == 1 && !JoystickHandler.IsUsingJoystick;
#endif

        if (_mainCamera == null)
        {
            // Lazy initialization
            _mainCamera = Camera.main;

            _rotationXAxis = 180.0f;
            _rotationYAxis = _yMaxLimit;
        }
        else if(allowInput)
        {
#if UNITY_EDITOR
            // Not sure if this works on mobile
            _rotationXAxis += _xSpeed * Input.GetAxis("Mouse X") * _distance * 0.02f;
            _rotationYAxis -= _ySpeed * Input.GetAxis("Mouse Y") * 0.02f;

#else
            // Move the camera
            Touch firstTouch = Input.GetTouch(0);

            _rotationXAxis += _xSpeed * firstTouch.deltaPosition.x * _distance * 0.005f;
            _rotationYAxis -= _ySpeed * firstTouch.deltaPosition.y * 0.005f;
#endif
        }

        _rotationYAxis = Util.ClampAngle(_rotationYAxis, _yMinLimit, _yMaxLimit);

        Quaternion rotation = Quaternion.Euler(_rotationYAxis, _rotationXAxis, 0.0f);
        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -_distance) + pOrbitTargetPos;

        _mainCamera.transform.rotation = rotation;
        _mainCamera.transform.position = position;
    }

    private Vector3 GetCameraPosForTarget(Vector3 pTargetPos, float pDistance)
    {
        Quaternion rotation = Quaternion.Euler(_rotationYAxis, _rotationXAxis, 0.0f);
        Vector3 position = rotation * new Vector3(0.0f, 0.0f, - pDistance) + pTargetPos;

        return position;
    }

    private float ClampDistance(float pDistance) {
        return Mathf.Clamp(pDistance, _minDistance, _maxDistance);
    }

    public Camera MainCamera
    {
        get
        {
            return _mainCamera;
        }
    }

    public float RotationYAxis
    {
        get
        {
            return _rotationYAxis;
        }
    }

    public float RotationXAxis
    {
        get
        {
            return _rotationXAxis;
        }
    }
}
