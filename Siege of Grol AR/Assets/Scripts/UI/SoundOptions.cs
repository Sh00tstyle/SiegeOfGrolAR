using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundOptions : MonoBehaviour
{

    [SerializeField] private Slider _ambientSlider, _musicSlider, _sfxSlider;
    [SerializeField] private AudioMixer _masterMixer;

    private void Awake()
    {
        _ambientSlider.value = GetSavedValue(SoundChannel.AMBIENT);
        _musicSlider.value = GetSavedValue(SoundChannel.MUSIC);
        _sfxSlider.value = GetSavedValue(SoundChannel.SFX);   
    }

    private void OnEnable()
    {
        _ambientSlider.onValueChanged.AddListener(delegate { ChangeValue(SoundChannel.AMBIENT, _ambientSlider.value); });
        _musicSlider.onValueChanged.AddListener(delegate { ChangeValue(SoundChannel.MUSIC, _musicSlider.value); });
        _sfxSlider.onValueChanged.AddListener(delegate { ChangeValue(SoundChannel.SFX, _sfxSlider.value); });
    }

    private void OnDisable()
    {
        _ambientSlider.onValueChanged.RemoveAllListeners();
        _musicSlider.onValueChanged.RemoveAllListeners();
        _sfxSlider.onValueChanged.RemoveAllListeners();
    }

    float GetSavedValue(SoundChannel pGroup)
    {
        string channel = pGroup.ToString();
        if (PlayerPrefs.HasKey(channel))
            return PlayerPrefs.GetFloat(channel);
        else
            return 1;
    }


    public void ChangeValue(SoundChannel pGroup, float pValue)
    {
        string channel = pGroup.ToString();

        _masterMixer.SetFloat(channel, pValue);
        PlayerPrefs.SetFloat(channel, pValue);
    }
}
