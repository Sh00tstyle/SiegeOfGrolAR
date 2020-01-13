using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InteractionButton : MonoBehaviour
{
    public UnityEvent onButtonPressStart = new UnityEvent();
    public UnityEvent onButtonPressEnd = new UnityEvent();

    private GraphicRaycaster _graphicRaycaster;

    private PointerEventData _pointerEventData;
    private List<RaycastResult> _raycastResults;

    private bool _isPressingButton;

    private void Awake()
    {
        _graphicRaycaster = GetComponentInParent<GraphicRaycaster>();

        if(_graphicRaycaster == null)
        {
            Debug.LogError("InteractionButton::Graphic Raycaster in " + name + " was null");
        }

        _isPressingButton = false;
    }

    private void Update()
    {
        DetectButtonInput();
    }

    private void OnButtonPressStart()
    {
        if (onButtonPressStart != null)
            onButtonPressStart.Invoke();
    }

    private void OnButtonPressEnd()
    {
        if (onButtonPressEnd != null)
            onButtonPressEnd.Invoke();
    }

    private void DetectButtonInput()
    {
        if (Input.GetMouseButtonDown(0) && !_isPressingButton)
        {
            if (_raycastResults == null)
                _raycastResults = new List<RaycastResult>();

            _raycastResults.Clear();

            _pointerEventData = new PointerEventData(EventSystem.current); //probably does not have to re-created every time
            _pointerEventData.position = Input.mousePosition;

            _graphicRaycaster.Raycast(_pointerEventData, _raycastResults);

            RaycastResult currentResult;

            for (int i = 0; i < _raycastResults.Count; ++i)
            {
                currentResult = _raycastResults[i];

                if (currentResult.gameObject.tag == Tags.InteractButton)
                {
                    _isPressingButton = true;
                    OnButtonPressStart();
                    return;
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && _isPressingButton)
        {
            _isPressingButton = false;
            OnButtonPressEnd();
        }
    }

    public void ContinueStory()
    {
        ProgressHandler.Instance.IncreaseStoryProgress();
        SceneHandler.Instance.LoadScene(Scenes.Map);
    }
}
