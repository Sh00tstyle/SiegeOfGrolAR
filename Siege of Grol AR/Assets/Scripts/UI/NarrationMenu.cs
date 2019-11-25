using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NarrationMenu : MonoBehaviour
{
    [Serializable]
    public struct Narration
    {
        public string Text;
        public AnimationCurve Timing;
        public float Time, TimeAfter;
        public AudioClip AudioClip;
    }

    [SerializeField]
    private bool _waitForInput = true;

    [SerializeField]
    private Text _narratorField;

    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private Image _nextButton, _narrationProgress;

    [SerializeField]
    private Button _textButton;

    [SerializeField]
    private Narration[] _narration;

    [SerializeField]
    private float _fadingTime, _nextButtonFadeIn, _nextButtonFadeOut;

    private Color _originalColor;
    private int _currentNarrationIndex;
    private Tween _textTween, _buttonTween;
    
    private void Awake()
    {
        _originalColor = _narratorField.color;
        NextNarration();
    }

    public void NextNarration()
    {

        if (_currentNarrationIndex >= _narration.Length - 1)
            _currentNarrationIndex = 0;
        else
            _currentNarrationIndex++;


        // Kill the button tween, and set it to dissapear
        _buttonTween.Kill();
        FadeNextButton(false);

        // Allow the text to be clickable again
        _textButton.interactable = true;

        // Get next narration and reset all contents
        Narration narration = _narration[_currentNarrationIndex];
        _narratorField.text = "";
        _narratorField.color = _originalColor;

        // Start animating the new loaded text
        _textTween = _narratorField.DOText(narration.Text, narration.Time, true).SetEase(narration.Timing);

        // Start playing the audioClip
        StartCoroutine(StartAudioClip(narration.AudioClip, callBack =>
        {
            // If audio is complete, let the nextButton appear
            if (callBack)
                FadeNextButton(true);
        }));
    }

    public void ShowText()
    {
        //if (!_textTween.IsPlaying())
        //    return;

        _textTween.Kill();
        _narratorField.text = _narration[_currentNarrationIndex].Text;

        if (!_waitForInput)
            NextNarration();
        else
            _textButton.interactable = false;
    }

    private IEnumerator StartAudioClip(AudioClip pAudioClip, System.Action<bool> pCallback)
    {
        _audioSource.clip = pAudioClip;
        _audioSource.Play();

        while (_audioSource.isPlaying)
        {
            _narrationProgress.fillAmount = _audioSource.time / pAudioClip.length;
            yield return null;
        }

        _narrationProgress.fillAmount = 1;
        pCallback(true);
    }

    private void FadeNextButton(bool pActive)
    {
        // Fade in/out the button
        _buttonTween = _nextButton.DOFade(pActive ? 1 : 0, pActive ? _nextButtonFadeIn : _nextButtonFadeOut);

        // If it's fading in, have the button popup after a delay
        if (pActive)
            _buttonTween = _nextButton.transform.DOScale(1.32f, 1f).SetLoops(-1).SetDelay(8);
    }
}
