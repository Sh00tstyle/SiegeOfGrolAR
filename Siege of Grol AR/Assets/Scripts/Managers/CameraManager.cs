using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraManager : Singleton<CameraManager>
{
    public Transform debugTransform;

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

    private Camera _mainCamera;

    private float _rotationYAxis;
    private float _rotationXAxis;

    private Sequence _objectFocusSequence;

    private bool _debugToggle = true;

    private void Awake()
    {
        if(_orbitTarget == null)
        {
            Debug.LogError("CameraManager::The orbit target was null, make sure it is assigned in the inspector of " + name);
            return;
        }
        
        UpdateCameraOrientation(_orbitTarget.position);
    }

    private void LateUpdate()
    {
        if (_orbitTarget == null)
            return;

        HandleCameraInput();

        // DEBUG
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(_debugToggle)
            {
                _debugToggle = false;
                _objectFocusSequence = SwitchFocusObject(debugTransform, 1.5f, 3.0f, 10.0f, Ease.Linear);
            }
            else
            {
                _debugToggle = true;
                _objectFocusSequence = SwitchFocusObject(NavigationManager.Instance.Player, 1.5f, 3.0f, 10.0f, Ease.Linear);
            }
        }
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

        return focusSwitchSequence;
    }

    private void HandleCameraInput()
    {
        if (_objectFocusSequence != null && _objectFocusSequence.active)
            return;

        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");

        if(scrollDelta > 0.0f) // Scroll up
            _distance -= 0.5f;
        else if (scrollDelta < 0.0f) // Scroll down
            _distance += 0.5f;

        if ((Input.GetMouseButton(0) || Input.touchCount >= 2) && !JoystickHandler.IsUsingJoystick)
            UpdateCameraOrientation(_orbitTarget.position);
        else
            UpdateCameraOrientation(_orbitTarget.position, false); // This is probably irrelevant on mobile
    }

    private void UpdateCameraOrientation(Vector3 pOrbitTargetPos, bool allowInput = true)
    {
        if(_mainCamera == null)
        {
            // Lazy initialization
            _mainCamera = Camera.main;

            _rotationXAxis = 0.0f;
            _rotationYAxis = _yMaxLimit;
        }
        else if(allowInput)
        {
            // Not sure if this works on mobile
            _rotationXAxis += _xSpeed * Input.GetAxis("Mouse X") * _distance * 0.02f;
            _rotationYAxis -= _ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
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
