using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickHandler : MonoBehaviour
{
    public static bool IsUsingJoystick = false;

    [SerializeField]
    private float _maxRadius = 150.0f;

    private GraphicRaycaster _graphicRaycaster;
    private PointerEventData _pointerEventData;
    private List<RaycastResult> _raycastResults;

    private Vector3 _startPosition;

    private void Awake()
    {
        _graphicRaycaster = GetComponentInParent<GraphicRaycaster>();
        _raycastResults = new List<RaycastResult>();

        if(_graphicRaycaster == null)
        {
            Debug.LogError("Unable to retrieve the graphic raycaster from the parent GameObject " + name);
        }

        _startPosition = transform.position;

        IsUsingJoystick = false;
    }

    private void Update()
    {
        HandleJoystickInput();
        EvaluateJoystickInput();
    }

    private void HandleJoystickInput()
    {
        if (Input.GetMouseButtonDown(0) && !IsUsingJoystick)
        {
            _raycastResults.Clear();

            _pointerEventData = new PointerEventData(EventSystem.current); //probably does not have to re-created every time
            _pointerEventData.position = Input.mousePosition;

            _graphicRaycaster.Raycast(_pointerEventData, _raycastResults);

            RaycastResult currentResult;

            for(int i = 0; i < _raycastResults.Count; ++i)
            {
                currentResult = _raycastResults[i];

                if(currentResult.gameObject.tag == Tags.Joystick)
                {
                    IsUsingJoystick = true;
                    return;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0) && IsUsingJoystick)
        {
            IsUsingJoystick = false;
            transform.position = _startPosition;
        }
    }

    private void EvaluateJoystickInput()
    {
        if (!IsUsingJoystick) return;

        Vector3 deltaMovement = Input.mousePosition - _startPosition;

        if(deltaMovement.magnitude > _maxRadius) // Clamp the vector
        {
            deltaMovement.Normalize();
            deltaMovement *= _maxRadius;
        }

        Vector3 targetPosition = _startPosition + deltaMovement;

        transform.position = targetPosition;

        GPSManager.Instance.MoveJoystickPlayer(deltaMovement);
    }
}
