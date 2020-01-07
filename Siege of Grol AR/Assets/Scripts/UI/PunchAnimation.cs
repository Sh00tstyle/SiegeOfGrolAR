using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PunchAnimation : MonoBehaviour
{
    [SerializeField] RectTransform _imageRect;
    [SerializeField] float _scale, _delay;

    private void Awake()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.SetDelay(_delay);
        sequence.Append(_imageRect.DOPunchScale(new Vector3(1, 1, 1) * _scale, 1, 0, 0.2f));
        sequence.SetLoops(-1);
    }
}
