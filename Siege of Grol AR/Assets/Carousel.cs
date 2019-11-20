using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Carousel : MonoBehaviour
{
    [SerializeField] Text _nextText, _previousText;
    [SerializeField] RectTransform _container;
    [SerializeField] Image[] _carouselPanels;
    [SerializeField] SwipeDetection _swipeDetection;
    [SerializeField] float _swipeSpeed, _buttonFadeSpeed;
    [SerializeField] Ease _ease;

    int _currentIndex = 0;
    float _canvasWidth;
    Tween _activeTween;

    void Awake()
    {
        _canvasWidth = _container.rect.width;

        // Add listeners for right and left swipe
        _swipeDetection.AddListener(Direction.LEFT, () => ChangePosition(true));
        _swipeDetection.AddListener(Direction.RIGHT, () => ChangePosition(false));
    }

    public void ChangePosition(bool left)
    {
        if (!left && _currentIndex <= 0)
            return;
        else if (left && _currentIndex >= _carouselPanels.Length - 1)
            return;

        _activeTween.Kill();

        _currentIndex = left ? _currentIndex = _currentIndex + 1 : _currentIndex = _currentIndex - 1;
        _activeTween = _container.transform.DOLocalMoveX(_currentIndex * -_canvasWidth, _swipeSpeed).SetEase(_ease)
            .OnStart(() =>
            {   // Enable the canvas we want to see          
                _carouselPanels[_currentIndex].gameObject.SetActive(true);
            }
            ).OnComplete(() =>
            {   //Hide previous image
                _carouselPanels[left ? _currentIndex - 1 : _currentIndex + 1].gameObject.SetActive(false);
                ChangeButtonsText();
            });


    }

    void ChangeButtonsText()
    {
        // Remove previous in case we're at the first page
        if (_currentIndex <= 0)
            _previousText.DOFade(0, _buttonFadeSpeed).OnComplete(() => _previousText.gameObject.SetActive(false));
        else if (!_previousText.gameObject.activeSelf)
        {
            _previousText.gameObject.SetActive(true);
            _previousText.DOFade(1, _buttonFadeSpeed);
        }

        // In case we are at the last page, change next to start
        if (_currentIndex >= _carouselPanels.Length - 1)
            _nextText.text = "Start";
        else if (_nextText.text == "Start")
            _nextText.text = "Next";


    }

}
