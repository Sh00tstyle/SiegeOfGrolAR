using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipeDetection : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    UnityEvent _swipeLeft = new UnityEvent();
    UnityEvent _swipeUp = new UnityEvent();
    UnityEvent _swipeDown = new UnityEvent();
    UnityEvent _swipeRight = new UnityEvent();
    int _eventCount;

    public void OnBeginDrag(PointerEventData pEventData)
    {
    }


    public void OnDrag(PointerEventData pEventData)
    {
    }

    public void AddListener(Direction pDirection, UnityAction pAction)
    {
        Debug.Log("Add Listener");
        _GetSwipeEventFromDirection(pDirection).AddListener(pAction);
        _eventCount++;
       
    }

    UnityEvent _GetSwipeEventFromDirection(Direction pDirection)
    {
        UnityEvent swipeEvent = null;
        switch (pDirection)
        {
            case Direction.UP:
                swipeEvent = _swipeUp;
                break;
            case Direction.DOWN:
                swipeEvent = _swipeDown;
                break;
            case Direction.RIGHT:
                swipeEvent = _swipeUp;
                break;
            case Direction.LEFT:
                swipeEvent = _swipeLeft;
                break;
        }
        return swipeEvent;
    }

    public void OnEndDrag(PointerEventData pEventData)
    {
        if (_eventCount <= 0)
            return;

        _GetSwipeEventFromDirection(_GetSwipeDirection((pEventData.position - pEventData.pressPosition).normalized)).Invoke();
        Debug.Log(_GetSwipeDirection((pEventData.position - pEventData.pressPosition).normalized));
    }


    Direction _GetSwipeDirection(Vector3 pSwipeVector)
    {
        float positiveX = Mathf.Abs(pSwipeVector.x);
        float positiveY = Mathf.Abs(pSwipeVector.y);
        Direction draggedDir;
        if (positiveX > positiveY)
        {
            draggedDir = (pSwipeVector.x > 0) ? Direction.RIGHT : Direction.LEFT;
        }
        else
        {
            draggedDir = (pSwipeVector.y > 0) ? Direction.UP : Direction.DOWN;
        }
        return draggedDir;
    }


}
