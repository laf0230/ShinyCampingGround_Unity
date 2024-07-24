using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    Day
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;


    public AudioClip dayMusic;
    public AudioClip enterMusic;
    AudioSource backgroundAudioSource;
    AudioSource enterAudioSource;

    // Music은 UIManager에서 변경됨

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(string name)
    {
        Sound sound = Array.Find(musicSounds, x => x.name == name);

        if (sound == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            musicSource.clip = sound.sound;
            musicSource.Play();
        }
    }

    public void PlaySFXMusic(string name)
    {
        Sound sound = Array.Find(sfxSounds, x => x.name == name);

        if (sound == null)
        {
            Debug.Log("SFX Sound Not Found");
        } else
        {
            sfxSource.clip = sound.sound;
            sfxSource.Play();
        }
    }
}
