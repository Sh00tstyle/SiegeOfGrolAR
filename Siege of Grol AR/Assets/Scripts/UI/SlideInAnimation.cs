using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideInAnimation : MonoBehaviour
{
    [SerializeField] RectTransform[] _objects;
    [SerializeField] float _speed = 0.4f, _delay;
    [SerializeField] Direction _direction;

    private Sequence _sequence;

    void OnEnable()
    {
      _sequence = DOTween.Sequence();
        _sequence.AppendInterval(0.4f);
        _sequence.SetDelay(_delay);
        for (int i = 0; i < _objects.Length; ++i)
        {
            // Move the avatars up
            _objects[i].anchoredPosition = GetDirection(_direction) * 1000;

            // Add them to the sequence
            _sequence.Append(_objects[i].DOAnchorPos(Vector2.zero, _speed).SetEase(Ease.InOutSine));
        }
    }

    private void OnDisable()
    {
        _sequence.Kill();
    }

    private Vector2 GetDirection(Direction pDirection)
    {
        switch (pDirection)
        {
            case Direction.UP:
                return Vector2.up;
            case Direction.DOWN:
                return Vector2.down;
            case Direction.RIGHT:
                return Vector2.right;
            case Direction.LEFT:
                return Vector2.left;
            default:
                return Vector2.up;
        }
    }
}
