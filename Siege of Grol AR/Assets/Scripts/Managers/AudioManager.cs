using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{

    public Sound[] sounds;

    void Awake()
    {
        SetDontDestroyOnLoad();

        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start()
    {
        Play("StartTheme"); // This is only called in the main scene during the first startup
    }

    public void Play (string name)
    {
        Debug.Log("Playing sound " + name);

        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }          
        s.source.Play();
    }

    public void StopPlaying(string sound)
    {
        Debug.Log("Stopping sound " + name);

        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        //s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        //s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

        s.source.Stop();
    }
}
