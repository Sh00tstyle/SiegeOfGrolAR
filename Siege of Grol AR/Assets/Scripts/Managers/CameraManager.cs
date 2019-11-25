using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Camera _mainCamera;

    private float _rotationYAxis;
    private float _rotationXAxis;

    private void Awake()
    {
        _mainCamera = Camera.main;

        if (_mainCamera != null)
        {
            Vector3 rotationAngles = _mainCamera.transform.eulerAngles;

            _rotationYAxis = rotationAngles.y;
            _rotationXAxis = rotationAngles.x;
        }
        else
        {
            Debug.LogError("CameraManager::The main camera was null");
        }
    }

    private void Update()
    {
        HandleCameraInput();
    }

    private void HandleCameraInput()
    {
        if (_mainCamera == null || _orbitTarget == null)
            return;

        if (Input.GetMouseButton(0))
        {
            _rotationXAxis += _xSpeed * Input.GetAxis("Mouse X") * _distance * 0.02f;
            _rotationYAxis -= _ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
            _rotationYAxis = Util.ClampAngle(_rotationYAxis, _yMinLimit, _yMaxLimit);

            Quaternion rotation = Quaternion.Euler(_rotationYAxis, _rotationXAxis, 0.0f);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -_distance) + _orbitTarget.position;

            _mainCamera.transform.rotation = rotation;
            _mainCamera.transform.position = position;
        }
    }

    public Camera MainCamera
    {
        get
        {
            return _mainCamera;
        }
    }
}
