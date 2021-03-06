﻿using System;
using System.Collections;
using System.Collections.Generic;
using GoogleARCore.Examples.ObjectManipulation;
using GoogleARCore;
using UnityEngine;

public class CrosshairManipulator : Manipulator
{
    [SerializeField]
    private Camera _firstPersonCamera;

    [SerializeField]
    private GameObject _pawnPrefab;

    [SerializeField]
    private GameObject _manipulatorPrefab;

    [SerializeField]
    private LineRenderer _manipulationLineRenderer;

    private Transform _manipulationAnchor;
    private Transform _currentManipulationTransform;

    public Transform Part1;
    public Transform Part2;
    public Transform Part3;
    public Transform Part4;
    public Transform Part5;
    public Transform Part6;

    private GameObject[] ManiCount;
    private int HowManyMani;
    private int ManiMax = 6;

    private bool _hasPlacedPawn;

    private void Awake()
    {
        // Setup manipulation transform
        _manipulationAnchor = new GameObject("Manipulation Anchor").transform;

        _manipulationAnchor.position = _firstPersonCamera.transform.position + _firstPersonCamera.transform.forward * 5.0f;
        _manipulationAnchor.parent = _firstPersonCamera.transform;

        _hasPlacedPawn = false;
    }

    protected override void Update()
    {
        base.Update();

        UpdateManipulatingObject();
    }

    public void StartManipulatingObject()
    {
        // Try to manipulate an object first
        ManipulateExistingObject();
    }

    public void UpdateManipulatingObject()
    {
        // Update the line renderer
        if (_currentManipulationTransform != null)
            UpdateLineRendererPositions();
    }

    public void StopManipulatingObject()
    {
        _manipulationLineRenderer.enabled = false;

        if (_currentManipulationTransform != null)
            ResetObjectManipulation();
        else if(!_hasPlacedPawn)
            CreateNewObject(); // No manipulation was happening, so try to spawn an object
    }

    private void ManipulateExistingObject()
    {
        RaycastHit hit;
        Ray ray = _firstPersonCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f)); // Middle of the screen

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == Tags.ARPawn)
            {
                _currentManipulationTransform = hit.transform.parent; // Parent is the Manipulator, Parents Parent is the Anchor

                _manipulationAnchor.position = hit.point;
                _currentManipulationTransform.parent = _manipulationAnchor; // Anchor it to the transform attached to the camera to move it around

                _manipulationLineRenderer.enabled = true;
                _manipulationLineRenderer.positionCount = 2; // For now, only show the start and end linesegments

                UpdateLineRendererPositions();
            }
        }
    }

    private void ResetObjectManipulation()
    {
        // Release the grabbed object's anchor
        _currentManipulationTransform.parent = null;
        _currentManipulationTransform = null;

        _manipulationLineRenderer.enabled = false;
    }

    private void UpdateLineRendererPositions()
    {
        Vector3 cameraPos = _firstPersonCamera.transform.position;
        cameraPos -= _firstPersonCamera.transform.up * 0.1f;

        _manipulationLineRenderer.SetPosition(0, cameraPos); // The ray should come out at the bottom of the screen
        _manipulationLineRenderer.SetPosition(1, _currentManipulationTransform.position);
    }

    private void CreateNewObject()
    {
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(Screen.width * 0.5f, Screen.height * 0.5f, raycastFilter, out hit)) // Middle of the screen
        {
            // Use hit pose and camera pose to check if hittest is from the
            // back of the plane, if it is, no need to create the anchor.
            if ((hit.Trackable is DetectedPlane) && Vector3.Dot(_firstPersonCamera.transform.position - hit.Pose.position, hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                // Instantiate game object at the hit pose.
                //GameObject gameObject = Instantiate(_pawnPrefab, hit.Pose.position, hit.Pose.rotation);
                if (!_pawnPrefab.activeInHierarchy)
                {
                    _pawnPrefab.transform.position = hit.Pose.position;
                    _pawnPrefab.transform.rotation = hit.Pose.rotation;
                }
                _pawnPrefab.SetActive(true);
                
                addManipulator(Part1, hit);
                addManipulator(Part2, hit);
                addManipulator(Part3, hit);
                addManipulator(Part4, hit);
                addManipulator(Part5, hit);
                addManipulator(Part6, hit);

                // Create an anchor to allow ARCore to track the hitpoint as understanding of
                // the physical world evolves.
                Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);

                // Make manipulator a child of the anchor.
                _pawnPrefab.transform.parent = anchor.transform;

                _hasPlacedPawn = true;
            }
        }
    }

    private void addManipulator (Transform pGameObject, TrackableHit pHit)
    {
        ManiCount = GameObject.FindGameObjectsWithTag("Manipulator");
        HowManyMani = ManiCount.Length;
        // Instantiate manipulator.
        if (HowManyMani < ManiMax)
        {
            GameObject manipulator = Instantiate(_manipulatorPrefab, pGameObject.position, pGameObject.rotation);
            //_manipulatorPrefab.SetActive(true);
            //_manipulatorPrefab.transform.position = pgameObject.position;
            //_manipulatorPrefab.transform.rotation = pgameObject.rotation;

            // Make game object a child of the manipulator.
            pGameObject.transform.parent = manipulator.transform;

            // Create an anchor to allow ARCore to track the hitpoint as understanding of
            // the physical world evolves.
            Anchor anchor = pHit.Trackable.CreateAnchor(pHit.Pose);

            // Make manipulator a child of the anchor.
            manipulator.transform.parent = anchor.transform;
        }
        
    }

    public void ContinueStory()
    {
        StartCoroutine(FinalizeCannonInteraction());
    }

    IEnumerator FinalizeCannonInteraction()
    {
        AudioManager.Instance.Play("CannonFinal");

        float timer = 23.0f;

        while (timer > 0.0f) // DEBUG
        {
            if (Input.touchCount >= 3)
                break;

            timer -= Time.deltaTime;
            yield return null;
        }

        //yield return new WaitForSeconds(23.0f); // Maybe not do this?

        ProgressHandler.Instance.IncreaseStoryProgress();
        AudioManager.Instance.StopPlaying("CannonTheme");
        AudioManager.Instance.Play("GameBG");
        SceneHandler.Instance.LoadScene(Scenes.Map);
    }
}