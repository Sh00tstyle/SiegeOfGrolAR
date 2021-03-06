﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Carousel : MonoBehaviour
{
    [SerializeField] bool _keepNextButton;
    [SerializeField] Button _nextButton, _prevButton;

    [SerializeField] CanvasScaler _scaling;
    [SerializeField] RectTransform _container;
    [SerializeField] Image[] _carouselPanels;

    [SerializeField] Image[] _circleIndicators;
    [SerializeField] RectTransform[] _avatars;
    [SerializeField] float _avatarMinSize;
    [SerializeField] float _maxSize, _minSize, _minAlpha, _maxAlpha, _speed;

    [SerializeField] SwipeDetection _swipeDetection;
    [SerializeField] float _swipeSpeed, _buttonFadeSpeed;
    [SerializeField] Ease _ease;

    [SerializeField] MenuAnimation[] _menuAnimations;

    private int _currentIndex = 0;
    private float _canvasWidth;
    private Tween _activeTween;

    private Vector2 _leftAnchorMin, _leftAnchorMax, _rightAnchorMin, _rightAnchorMax, _centerAnchorMax, _centerAnchorMin;

    void Awake()
    {
        // Store width for animating
        _canvasWidth = _scaling.referenceResolution.x;

        // Get anchors to later move the avatars from/to
        GetAnchors();

        // Change buttons and indicators
        ChangeCircleIndicator();
        ChangeButtonsText();

        // Add listeners for right and left swipe
        _swipeDetection.AddListener(Direction.LEFT, () => ChangePosition(true));
        _swipeDetection.AddListener(Direction.RIGHT, () => ChangePosition(false));
    }

    void GetAnchors()
    {
        if (_avatars == null || _avatars.Length == 0)
            return;

        _leftAnchorMin = new Vector2(0, _avatars[0].anchorMin.y);
        _leftAnchorMax = new Vector2(0, _avatars[0].anchorMax.y);

        _centerAnchorMin = _avatars[0].anchorMin;
        _centerAnchorMax = _avatars[0].anchorMax;

        _rightAnchorMin = new Vector2(1, _avatars[0].anchorMin.y);
        _rightAnchorMax = new Vector2(1, _avatars[0].anchorMax.y);
    }

    public void ForcePosition(int index)
    {
        if (index == _currentIndex)
            return;

        ChangePosition(index > _currentIndex);
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
        ChangeAvatarIndicators(pSwipeLeft);

        // Tween
        _activeTween = _container.transform.DOLocalMoveX(_currentIndex * -_canvasWidth, _swipeSpeed).SetEase(_ease)
            .OnStart(() =>
            {   // Enable the canvas we want to see          
                _carouselPanels[_currentIndex].gameObject.SetActive(true);
            }
            ).OnComplete(() =>
            {   //Hide previous image
                _carouselPanels[pSwipeLeft ? _currentIndex - 1 : _currentIndex + 1].gameObject.SetActive(false);
                ChangeAvatarIndicators(pSwipeLeft);
            });
    }

    void ChangeAvatarIndicators(bool pSwipeLeft)
    {
        if (_avatars == null || _avatars.Length == 0)
            return;

        // Set new avatar to center
        ChangeAnchor(_avatars[_currentIndex], _centerAnchorMin, _centerAnchorMax, true);

        // Needs a bit more optimisation
        _avatars[0].gameObject.SetActive(_currentIndex <= 1);
        _avatars[_avatars.Length - 1].gameObject.SetActive(_currentIndex >= 1);

        if (_currentIndex > 0)
            ChangeAnchor(_avatars[_currentIndex - 1], _leftAnchorMin, _leftAnchorMax);
        if (_currentIndex < _avatars.Length - 1)

            ChangeAnchor(_avatars[_currentIndex + 1], _rightAnchorMin, _rightAnchorMax);
    }

    void ChangeAnchor(RectTransform pRectTransform, Vector2 pAnchorMin, Vector2 pAnchorMax, bool pCenter = false)
    {

        pRectTransform.DOAnchorMin(pAnchorMin, _swipeSpeed).SetEase(_ease);
        pRectTransform.DOAnchorMax(pAnchorMax, _swipeSpeed).SetEase(_ease);

        if (pCenter)
            pRectTransform.DOAnchorPos(new Vector2(0, 0), 0).SetEase(_ease);
        else
            pRectTransform.DOSizeDelta(new Vector2(400, 0), 0).SetEase(_ease);

        pRectTransform.DOScale(pCenter ? 1 : _avatarMinSize, _swipeSpeed).SetEase(_ease);
    }


    void ChangeButtonsText()
    {
        if (_prevButton == null && _nextButton == null)
            return;

        _prevButton.gameObject.SetActive(_currentIndex > 0);
        if (!_keepNextButton)
            _nextButton.gameObject.SetActive(_currentIndex < _carouselPanels.Length - 1);
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
