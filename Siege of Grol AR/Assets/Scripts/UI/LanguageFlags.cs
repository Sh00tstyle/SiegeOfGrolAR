using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageFlags : MonoBehaviour
{
    [SerializeField] Image[] _languageFlags;
    [SerializeField] float _flagFadeDuration, _minimumOpacity, _maxScale;

    Tween[] _activeTweens = new Tween[3];
    int _currentIndex = 4;


    void Awake()
    {
        ChangeLanguage(0);
    }
    public void ChangeLanguage(int index)
    {
        if (_currentIndex == index) return;
        _currentIndex = index;

        for (int i = 0; i < _languageFlags.Length; ++i)
        {
            _activeTweens[i] = _languageFlags[i].DOFade(i == index ? 1 : _minimumOpacity, _flagFadeDuration).SetEase(Ease.InOutSine);
            _activeTweens[i] = _languageFlags[i].rectTransform.DOScale(i == index ? _maxScale : 1, _flagFadeDuration).SetEase(Ease.InOutSine);
        }
          

        // Change settings in GameManager?
        // GameManager.Instance.Save(x);
    }
}
