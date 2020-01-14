using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMarkdown : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _canvas;
    [SerializeField] private MenuBehaviour _bottomMenu;
    [SerializeField] private MenuAnimation _dissmissAnimation;

    [Header("Text")]
    [SerializeField] private Text _hintText;
    [SerializeField] float _textOffset = 20f;
    [SerializeField] RectTransform _textRect;
    [SerializeField] float _fadeInTextDuration, _fadeOutTextDuration;
    [SerializeField] Ease _fadeInTextEase, _fadeOutTextEase;


    [Header("Circle")]
    [SerializeField] private RectTransform _highlightCircle;
    [SerializeField] private Button _clickRegion;
    [SerializeField] private Vector2 _punchScale;
    [SerializeField] private float _punchDuration = 1.2f;
    [SerializeField] private Ease _fadeInEase = Ease.InSine, _fadeOutEase = Ease.InSine, _moveInEase = Ease.InSine, _moveOutEase = Ease.InSine;
    [SerializeField] private float _fadeInDuration = 1, _fadeOutDuration = 2, _moveInDuration = 6, _moveOutDuration;


    private string _locationText
    {
        get
        {
            string characterName = ((Progress)ProgressHandler.Instance.StoryProgressIndex).ToString();
            return string.Format("You are tasked to visit the {0} at the {1}", characterName, GameManager.Instance.CurrentLocation.locationName);
        }
    }

    private Tween _activeTween;

    private Vector2 _offset;
    private float _defaultScale;
    private bool _tutorialRunning;

    void Awake()
    {
        _defaultScale = _highlightCircle.localScale.x;
    }

    public void ReplayLastTutorial()
    {
        if (!_tutorialRunning)
            UpdatePosition(GameManager.Instance.CurrentLocationTransform);
    }

    private Vector2 GetCanvasPosition(Vector3 pWorldPosition)
    {
        Vector2 viewportPosition = CameraManager.Instance.MainCamera.WorldToViewportPoint(pWorldPosition);
        return new Vector2(
            ((viewportPosition.x * _canvas.sizeDelta.x) - (_canvas.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * _canvas.sizeDelta.y) - (_canvas.sizeDelta.y * 0.5f)));
    }

    private float GetCameraDistanceToObject(Vector3 pObject)
    {
        return Vector3.Distance(pObject, CameraManager.Instance.MainCamera.gameObject.transform.position);
    }

    public void UpdatePosition(Transform pLocation, bool pBounce = true, bool pBounceInfinite = true, int pBounceLoops = 0, float pPunchDelay = 2)
    {
        // If bottom menu is activated, dismiss it.
        if (_bottomMenu != null && _bottomMenu.gameObject.activeSelf)
            _bottomMenu.HideMenu(_dissmissAnimation);

        // Now navigate from player to the location
        CameraManager.Instance.SwitchFocusObject(pLocation, 3, 5, 3, _fadeInEase).OnComplete(() =>
        {
            // Tell the player to go there
            //Translate Vector3 WorldPosition to Vector2 UI Space
            Vector2 CanvasPosition = GetCanvasPosition(pLocation.position);

            // Enable gameObjects
            _highlightCircle.gameObject.SetActive(true);
            _textRect.gameObject.SetActive(true);

            _tutorialRunning = true;

            // Fade in the canvas, if not faded in yet
            if (_canvasGroup.alpha < 1)
                _activeTween = FadeCircle(true);

            // If text is not transparent yet, fade it out first
            if (_hintText.color.a > 0)
                _activeTween = FadeText(false, Vector2.zero, 0, null);

            // Get scale
            float distance = _defaultScale / GetCameraDistanceToObject(pLocation.position) + 0.2f;

            FocusCircle(true, CanvasPosition, distance).OnComplete(() =>
            {
                //Move Text to location and add text
                _activeTween = FadeText(true, CanvasPosition, distance, _locationText);
                // Bounce if neccesary
                if (pBounce) _activeTween = Punch(_highlightCircle, pBounceInfinite, pBounceLoops).SetDelay(pPunchDelay);

                //Enable user input
                _clickRegion.interactable = true;
            });
        });
    }




    private Tween FadeText(bool pFadeIn, Vector2 pPosition, float pDistance, string pText = null)
    {
        if (pText != null)
            _textRect.localPosition = new Vector2(pPosition.x, (pPosition.y - _highlightCircle.sizeDelta.y / 2 - _textOffset) * pDistance);

        if (pFadeIn && pText != null)
            _hintText.text = pText;

        return _hintText.DOFade(
            pFadeIn ? 1 : 0,
            pFadeIn ? _fadeInTextDuration : _fadeOutTextDuration)
        .SetEase(pFadeIn ? _fadeInTextEase : _fadeOutTextEase);

    }

    private Sequence FocusCircle(bool pFocusIn, Vector2 pCanvasPosition, float pScale)
    {
        Sequence focusSequence = DOTween.Sequence();
        focusSequence.Insert(0, _highlightCircle.DOAnchorPos(pCanvasPosition, pFocusIn ? _moveInDuration : _moveOutDuration)
            .SetEase(pFocusIn ? _moveInEase : _moveOutEase));
        focusSequence.Insert(0, _highlightCircle.DOScale(pScale, pFocusIn ? _moveInDuration : _moveOutDuration)
            .SetEase(pFocusIn ? _moveInEase : _moveOutEase));
        return focusSequence;
    }

    public void CompleteTutorial()
    {
        _activeTween.Kill();

        // Disable user input for the click region
        _clickRegion.interactable = false;

        FadeText(false, Vector3.zero, 0, null);
        FocusCircle(false, Vector2.zero, _defaultScale).OnComplete(() =>
        {
            // Disable tutorial gameObjects
            _highlightCircle.gameObject.SetActive(false);
            _textRect.gameObject.SetActive(false);

            _tutorialRunning = false;

            // Move back to the player
            CameraManager.Instance.SwitchFocusObject(NavigationManager.Instance.PlayerTransform, 5, 5, 3, _fadeInEase);
        });
    }

    private Tween Punch(RectTransform pRect, bool pBounceInfinite, int pBounceLoops)
    {
        return pRect.DOPunchScale(_punchScale, _punchDuration, 0, 0).SetLoops(pBounceInfinite ? -1 : pBounceLoops);
    }

    private Tween FadeCircle(bool pFadeIn)
    {
        return _canvasGroup.DOFade(pFadeIn ? 1 : 0, pFadeIn ? _fadeInDuration : _fadeOutDuration)
        .SetEase(pFadeIn ? _fadeInEase : _fadeOutEase);
    }

}
