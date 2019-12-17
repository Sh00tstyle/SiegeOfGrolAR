using System;
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

    private GameObject[] ManiCount;
    private int HowManyMani;
    private int ManiMax = 3;

    private void Awake()
    {
        // Setup manipulation transform
        _manipulationAnchor = new GameObject("Manipulation Anchor").transform;

        _manipulationAnchor.position = _firstPersonCamera.transform.position + _firstPersonCamera.transform.forward * 5.0f;
        _manipulationAnchor.parent = _firstPersonCamera.transform;
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

        Debug.Log("Start mani");
    }

    public void UpdateManipulatingObject()
    {
        // Update the line renderer
        if (_currentManipulationTransform != null)
            UpdateLineRendererPositions();
    }

    public void StopManipulatingObject()
    {
        Debug.Log("Stop mani");
        _manipulationLineRenderer.enabled = false;

        if (_currentManipulationTransform != null)
            ResetObjectManipulation();
        else
            CreateNewObject(); // No manipulation was happening, so try to spawn an object
    }

    private void ManipulateExistingObject()
    {
        RaycastHit hit;
        Ray ray = _firstPersonCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f)); // Middle of the screen

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
        _manipulationLineRenderer.SetPosition(0, _firstPersonCamera.ScreenToWorldPoint(new Vector3(0.5f, 0.0f, 0.5f))); // The ray should come out at the bottom of the screen
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

                // Create an anchor to allow ARCore to track the hitpoint as understanding of
                // the physical world evolves.
                Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);

                // Make manipulator a child of the anchor.
                _pawnPrefab.transform.parent = anchor.transform;
            }
        }
    }

    private void addManipulator (Transform pgameObject, TrackableHit pHit)
    {
        ManiCount = GameObject.FindGameObjectsWithTag("Manipulator");
        HowManyMani = ManiCount.Length;
        // Instantiate manipulator.
        if (HowManyMani < ManiMax)
        {
            GameObject manipulator = Instantiate(_manipulatorPrefab, pgameObject.position, pgameObject.rotation);
            //_manipulatorPrefab.SetActive(true);
            //_manipulatorPrefab.transform.position = pgameObject.position;
            //_manipulatorPrefab.transform.rotation = pgameObject.rotation;

            // Make game object a child of the manipulator.
            pgameObject.transform.parent = manipulator.transform;

            // Create an anchor to allow ARCore to track the hitpoint as understanding of
            // the physical world evolves.
            Anchor anchor = pHit.Trackable.CreateAnchor(pHit.Pose);

            // Make manipulator a child of the anchor.
            manipulator.transform.parent = anchor.transform;
        }
        
    }

    /**
    protected override bool CanStartManipulationForGesture(TapGesture gesture)
    {
        if (gesture.TargetObject == null)
        {
            return true;
        }

        return false;
    }

    protected override void OnEndManipulation(TapGesture gesture)
    {
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(Screen.width * 0.5f, Screen.height * 0.5f, raycastFilter, out hit))
        {
            // Use hit pose and camera pose to check if hittest is from the
            // back of the plane, if it is, no need to create the anchor.
            if ((hit.Trackable is DetectedPlane) &&
                Vector3.Dot(_firstPersonCamera.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                // Instantiate game object at the hit pose.
                var gameObject = Instantiate(_pawnPrefab, hit.Pose.position, hit.Pose.rotation);

                // Instantiate manipulator.
                var manipulator = Instantiate(_manipulatorPrefab, hit.Pose.position, hit.Pose.rotation);

                // Make game object a child of the manipulator.
                gameObject.transform.parent = manipulator.transform;

                // Create an anchor to allow ARCore to track the hitpoint as understanding of
                // the physical world evolves.
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                // Make manipulator a child of the anchor.
                manipulator.transform.parent = anchor.transform;

                // Select the placed object.
                manipulator.GetComponent<Manipulator>().Select();
            }
        }
    }
    /**/
}