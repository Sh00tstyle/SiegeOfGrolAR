using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMarkdown : MonoBehaviour
{
    [SerializeField] private float _scale;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _gameObject;
    [SerializeField] private RectTransform _highlightCircle, _canvas;


    private Vector2 _offset;
    private Vector2 _scaleVector;

    void Awake()
    {
        _scaleVector = new Vector2(_scale, _scale); 
        _offset = new Vector2((float)_canvas.sizeDelta.x / 2f, (float)_canvas.sizeDelta.y / 2f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            UpdatePosition(_gameObject.transform.position, 1, 12);
    }

    private Vector2 GetCanvasPosition(Vector3 pWorldPosition)
    {
        Vector2 viewportPosition = _camera.WorldToViewportPoint(pWorldPosition);
        return new Vector2(viewportPosition.x * _canvas.sizeDelta.x, viewportPosition.y * _canvas.sizeDelta.y);
    }

    public void UpdatePosition(Vector3 pWorldPosition, float pScale, float pDuration)
    {
        Vector2 canvasPosition = GetCanvasPosition(pWorldPosition) - _offset;

        // Scale the circle
        _highlightCircle.DOScale(pScale, pDuration);
        _highlightCircle.DOAnchorPos(canvasPosition, pDuration);
        //_circle.DOAnchorPos(canvasPosition, 1).OnComplete(() =>
        //{
        //    _highlightRect.DOPunchScale(_scaleVector, 1, 1, 1).SetLoops(-1);
        //    _circle.DOPunchScale(_scaleVector, 1, 1, 1).SetLoops(-1);
        //});
        //_highlightRect.DOAnchorPos(canvasPosition, 1);
    }


    private void PunchScaleHighlight()
    {

    }

}
