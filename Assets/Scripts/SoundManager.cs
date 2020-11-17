using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Audio players components.
    public AudioSource MusicSource;
    public AudioSource SFXSource;

    // Singleton instance.
    public static SoundManager Instance = null;

    // Initialize the singleton instance.
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }
   

    // Play a single clip through the music source. (ŞİMDİLİK GEREK YOK, TEK KLİP VAR)
    public void PlayMusic(AudioClip clip)
    {
        MusicSource.clip = clip;
        MusicSource.Play();
    }

    // Play a single clip through the sound effects source.
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.clip = clip;
        SFXSource.Play();
    }

    public void ToggleMusic()
    {
        if (PlayerPrefs.GetInt("MusicPref", 1) == 1)
        {
            PlayerPrefs.SetInt("MusicPref", 0);
            MusicSource.Stop();
        }
        else
        {
            PlayerPrefs.SetInt("MusicPref", 1);
            MusicSource.Play();
        }
    }

    public void ToggleSFX()
    {
        if (PlayerPrefs.GetInt("SFXPref", 1) == 1)
        {
            PlayerPrefs.SetInt("SFXPref", 0);
            SFXSource.volume = 0f;
        }
        else
        {
            PlayerPrefs.SetInt("SFXPref", 1);
            SFXSource.volume = 1f;
        }
    }


}
