using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NarrationMenu : MonoBehaviour
{

    [SerializeField]
    bool _waitForInput = true;

    [SerializeField]
    Text _narratorField;

    [SerializeField]
    AudioSource _audioSource;

    [SerializeField]
    Image _nextButton, _narrationProgress;

    [SerializeField]
    Button _textButton;

    [SerializeField]
    Narration[] _narration;

    [SerializeField]
    float _fadingTime, _nextButtonFadeIn, _nextButtonFadeOut;

    Color _originalColor;
    int _currentNarrationIndex;
    Tween _textTween, _buttonTween;

    [System.Serializable]
    public struct Narration
    {
        public string Text;
        public AnimationCurve Timing;
        public float Time, TimeAfter;
        public AudioClip AudioClip;
    }

    void Awake()
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

    IEnumerator StartAudioClip(AudioClip pAudioClip, System.Action<bool> pCallback)
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


    void FadeNextButton(bool pActive)
    {
        // Fade in/out the button
        _buttonTween = _nextButton.DOFade(pActive ? 1 : 0, pActive ? _nextButtonFadeIn : _nextButtonFadeOut);


        // If it's fading in, have the button popup after a delay
        if (pActive)
            _buttonTween = _nextButton.transform.DOScale(1.32f, 1f).SetLoops(-1).SetDelay(8);

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
}
