using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserOptions : MonoBehaviour
{
    [SerializeField] Image[] languageFlags;
    [SerializeField] float flagFadeDuration, minimumOpacity;

    Tween[] _activeTweens = new Tween[3];
    int _currentIndex = 4;

    public void ChangeLanguage(int index)
    {
        if (_currentIndex == index) return;
        _currentIndex = index;

        for (int i = 0; i < languageFlags.Length; ++i)
            _activeTweens[i] = languageFlags[i].DOFade(i == index ? 1 : minimumOpacity, flagFadeDuration).SetEase(Ease.InOutSine);

        // Change settings in GameManager?
        // GameManager.Instance.Save(x);
    }
}
