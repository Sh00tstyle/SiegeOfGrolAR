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
        _ambientSlider.value = GetSavedValue(SoundGroup.AMBIENT);
        _musicSlider.value = GetSavedValue(SoundGroup.MUSIC);
        _sfxSlider.value = GetSavedValue(SoundGroup.SFX);   
    }

    private void OnEnable()
    {
        _ambientSlider.onValueChanged.AddListener(delegate { ChangeValue(SoundGroup.AMBIENT, _ambientSlider.value); });
        _musicSlider.onValueChanged.AddListener(delegate { ChangeValue(SoundGroup.MUSIC, _musicSlider.value); });
        _sfxSlider.onValueChanged.AddListener(delegate { ChangeValue(SoundGroup.SFX, _sfxSlider.value); });
    }

    private void OnDisable()
    {
        _ambientSlider.onValueChanged.RemoveAllListeners();
        _musicSlider.onValueChanged.RemoveAllListeners();
        _sfxSlider.onValueChanged.RemoveAllListeners();
    }

    float GetSavedValue(SoundGroup pGroup)
    {
        string channel = pGroup.ToString();
        if (PlayerPrefs.HasKey(channel))
            return PlayerPrefs.GetFloat(channel);
        else
            return 1;
    }


    public void ChangeValue(SoundGroup pGroup, float pValue)
    {
        string channel = pGroup.ToString();

        _masterMixer.SetFloat(channel, pValue);
        PlayerPrefs.SetFloat(channel, pValue);
    }
}
