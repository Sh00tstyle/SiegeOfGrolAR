using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Carousel : MonoBehaviour
{

    [Header("Text buttons")]
    [SerializeField]
    bool _NextToStart = false;
    [SerializeField] Text _nextText;
    [SerializeField] Text _previousText;

    [Header("Carousel and items")]
    [SerializeField] CanvasScaler _scaling;
    [SerializeField] RectTransform _container;
    [SerializeField] Image[] _carouselPanels;

    [Header("Circle and/or Avatar indicators")]
    [SerializeField] Image[] _circleIndicators;
    [SerializeField] Image[] _inBetweens;
    [SerializeField] Image[] _avatars;
    [SerializeField] float _maxSize, _minSize, _minAlpha, _maxAlpha, _speed;

    [Header("Swiping")]
    [SerializeField] SwipeDetection _swipeDetection;
    [SerializeField] float _swipeSpeed, _buttonFadeSpeed;
    [SerializeField] Ease _ease;

    [Header("Target menu")]
    [SerializeField] MenuAnimation[] _menuAnimations;

    int _currentIndex = 0;
    float _canvasWidth;
    Tween _activeTween;



    void Awake()
    {
        // Store width for animating
        _canvasWidth = _scaling.referenceResolution.x;



        // Change buttons and indicators
        ChangeCircleIndicator();
        ChangeButtonsText();
        ChangeAvatarIndicators();

        // Add listeners for right and left swipe
        _swipeDetection.AddListener(Direction.LEFT, () => ChangePosition(true));
        _swipeDetection.AddListener(Direction.RIGHT, () => ChangePosition(false));
    }

    public void ChangePosition(bool pSwipeLeft)
    {
        if (!pSwipeLeft && _currentIndex <= 0)
            return;
        else if (pSwipeLeft && _currentIndex >= _carouselPanels.Length - 1)
            return;

        // Cancel current animations
        _activeTween.Kill();

        // Iterate index
        _currentIndex = pSwipeLeft ? _currentIndex = _currentIndex + 1 : _currentIndex = _currentIndex - 1;

        // Change buttons and indicators
        ChangeCircleIndicator();
        ChangeButtonsText();


        // Tween
        _activeTween = _container.transform.DOLocalMoveX(_currentIndex * -_canvasWidth, _swipeSpeed).SetEase(_ease)
            .OnStart(() =>
            {   // Enable the canvas we want to see          
                _carouselPanels[_currentIndex].gameObject.SetActive(true);
            }
            ).OnComplete(() =>
            {   //Hide previous image
                _carouselPanels[pSwipeLeft ? _currentIndex - 1 : _currentIndex + 1].gameObject.SetActive(false);
                ChangeAvatarIndicators();
            });
    }

    void ChangeAvatarIndicators()
    {
        if (_inBetweens == null)
            return;

        Image inBetweenImage;
        if (_currentIndex == 0 || _currentIndex == 1)
            inBetweenImage = _inBetweens[0];
        else
            inBetweenImage = _inBetweens[1];

        inBetweenImage.DOFade(0.7f, 0.1f).OnComplete(() =>
        {
            inBetweenImage.sprite = _avatars[_currentIndex == 0 ? 1 : 0].sprite;
        });
        inBetweenImage.DOFade(1, 0.1f).SetDelay(0.1f);
    }


    void ChangeButtonsText()
    {
        // Remove previous in case we're at the first page, otherwise show it if not active
        if (_currentIndex <= 0)
            _previousText.DOFade(0, _buttonFadeSpeed).OnComplete(() => _previousText.gameObject.SetActive(false));
        else if (!_previousText.gameObject.activeSelf)
        {
            _previousText.gameObject.SetActive(true);
            _previousText.DOFade(1, _buttonFadeSpeed);
        }

        // In case we are at the last page, change 'Next' to 'Start' otherwise to 'Next' 

        if (_NextToStart)
        {
            if (_currentIndex >= _carouselPanels.Length - 1)
                _nextText.text = "Start";
            else if (_nextText.text == "Start")
                _nextText.text = "Next";
        }
        else
        {
            if (_currentIndex >= _carouselPanels.Length - 1)
                _nextText.DOFade(0, _buttonFadeSpeed).OnComplete(() => _nextText.gameObject.SetActive(false));
            else if (!_nextText.gameObject.activeSelf)
            {
                _nextText.gameObject.SetActive(true);
                _nextText.DOFade(1, _buttonFadeSpeed);
            }
        }
    }


    void ChangeCircleIndicator()
    {
        for (int i = 0; i < _circleIndicators.Length; ++i)
        {
            _circleIndicators[i].rectTransform.DOScale(i == _currentIndex ? _maxSize : _minSize, _speed);
            _circleIndicators[i].DOFade(i == _currentIndex ? _maxAlpha : _minAlpha, _speed);
        }
    }

    // Custom for introduction
    public void GoToMenu()
    {
        if (_currentIndex == _carouselPanels.Length - 1)
            for (int i = 0; i < _menuAnimations.Length; i++)
                MenuManager.Instance.GoToMenu(_menuAnimations[i].menu, _menuAnimations[i]);


    }

}
