using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField]
    private Transform _mapTransform;

    private Camera _mapCamera;

    private Vector3 _prevMousePosition;

    private void Awake()
    {
        _mapCamera = Camera.main;

        _prevMousePosition = Vector3.zero;
    }

    private void Update()
    {
        if (_mapCamera == null)
            return;

        HandleCameraInput();
    }

    private void HandleCameraInput()
    {
        // Camera movement code here
    }

    public Camera MainCamera
    {
        get
        {
            return _mapCamera;
        }
    }
}
