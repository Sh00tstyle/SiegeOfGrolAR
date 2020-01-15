using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ProgressIndication : MonoBehaviour
{
    [SerializeField] private Image[] _circles;
    [SerializeField] private TextMeshProUGUI[] _text;

    [SerializeField] private float _minAlpha;

    private void Awake()
    {
        for (int i = 0; i < _circles.Length; i++)
        {
            bool isAchieved = ProgressHandler.Instance.StoryProgressIndex >= i;
            _circles[i].DOFade(isAchieved ? 1 : _minAlpha, 0);
            _text[i].alpha = isAchieved ? 1 : _minAlpha;
            _circles[i].transform.localScale = Vector3.one * (isAchieved ? 1 : 0.9f);
        }

    }
}
