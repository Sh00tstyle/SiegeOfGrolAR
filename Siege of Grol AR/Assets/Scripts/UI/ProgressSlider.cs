using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ProgressSlider : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Text _currentValueText;

    void Awake()
    {
        UpdateProgress();
    }
    public void UpdateProgress()
    {
        int value = (int)Random.Range(_slider.minValue, _slider.maxValue);
        _currentValueText.text = value.ToString();
        _slider.DOValue(value, _slider.maxValue - value).OnComplete(() => { UpdateProgress(); });

    }
}
