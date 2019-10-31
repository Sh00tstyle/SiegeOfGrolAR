using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SwipeDetection : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [System.Serializable]
    public class SwipeEvent : UnityEvent { }



    [SerializeField]
    SwipeEvent _rightSwipe, _leftSwipe, _upSwipe, _downSwipe;




    // Unneccesary?
    public void OnBeginDrag(PointerEventData pEventData)
    {
    }

    // Unneccesary?
    public void OnDrag(PointerEventData pEventData)
    {
    }


    public void OnEndDrag(PointerEventData pEventData)
    {
        Direction direction = _GetSwipeDirection((pEventData.position - pEventData.pressPosition).normalized);
        switch (direction)
        {
            case Direction.UP:
                _upSwipe.Invoke();
                break;
            case Direction.DOWN:
                _downSwipe.Invoke();
                break;
            case Direction.RIGHT:
                _rightSwipe.Invoke();
                break;
            case Direction.LEFT:
                _leftSwipe.Invoke();
                break;
        }

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
