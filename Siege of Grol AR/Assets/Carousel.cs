using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Carousel : MonoBehaviour
{
    [SerializeField] Image[] _canvasses;
    [SerializeField] SwipeDetection _swipeDetection;

    // 0 = center, -1 = left, 1 = right and so on.
    int _currentIndex = 0;

    void Awake()
    {
        // Add listeners for right and left swipe
        _swipeDetection.AddListener(Direction.LEFT, () => ChangePosition(true));
        _swipeDetection.AddListener(Direction.RIGHT, () => ChangePosition(false));
    }
    public void ChangePosition(bool left)
    {

        _currentIndex = left ? _currentIndex = _currentIndex - 1 : _currentIndex = _currentIndex + 1;
        Debug.Log(_currentIndex);

    }

    void MoveCanvasses(bool left)
    {

    }
}
