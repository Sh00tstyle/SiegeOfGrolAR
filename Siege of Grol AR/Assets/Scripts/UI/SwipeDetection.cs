using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipeDetection : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private UnityEvent _swipeLeft = new UnityEvent();
    private UnityEvent _swipeUp = new UnityEvent();
    private UnityEvent _swipeDown = new UnityEvent();
    private UnityEvent _swipeRight = new UnityEvent();
    private int _eventCount;

    private void Start()
    {
        if (_eventCount <= 0)
            enabled = false;
    }

    public void OnBeginDrag(PointerEventData pEventData)
    {

    }

    public void OnDrag(PointerEventData pEventData)
    {

    }

    public void OnEndDrag(PointerEventData pEventData)
    {
        GetSwipeEventFromDirection(GetSwipeDirection((pEventData.position - pEventData.pressPosition).normalized)).Invoke();
    }

    public void AddListener(Direction pDirection, UnityAction pAction)
    {
        GetSwipeEventFromDirection(pDirection).AddListener(pAction);
        _eventCount++;
    }

    private UnityEvent GetSwipeEventFromDirection(Direction pDirection)
    {
        switch (pDirection)
        {
            case Direction.UP:
                return _swipeUp;

            case Direction.DOWN:
                return _swipeDown;

            case Direction.RIGHT:
                return _swipeUp;

            case Direction.LEFT:
                return _swipeLeft;

            default:
                Debug.LogError("SwipeDetection::The given direction " + pDirection + " could not be identified!");
                return null;
        }

    }

    private Direction GetSwipeDirection(Vector3 pSwipeVector)
    {
        float positiveX = Mathf.Abs(pSwipeVector.x);
        float positiveY = Mathf.Abs(pSwipeVector.y);

        if (positiveX > positiveY)
        {
            return (pSwipeVector.x > 0) ? Direction.RIGHT : Direction.LEFT;
        }
        else
        {
            return (pSwipeVector.y > 0) ? Direction.UP : Direction.DOWN;
        }

    }

}
