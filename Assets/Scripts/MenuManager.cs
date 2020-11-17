using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject musicOn, musicOff, sfxOn, sfxOff;

    bool doOnce = true;

    private void Start()
    {
        musicOn.SetActive((PlayerPrefs.GetInt("MusicPref", 1) == 1) ? false : true);
        musicOff.SetActive((PlayerPrefs.GetInt("MusicPref", 1) == 1) ? true : false);

        sfxOn.SetActive((PlayerPrefs.GetInt("SFXPref", 1) == 1) ? false : true);
        sfxOff.SetActive((PlayerPrefs.GetInt("SFXPref", 1) == 1) ? true : false);

        if (PlayerPrefs.GetInt("MusicPref", 1) == 1)
        {
            if (!SoundManager.Instance.MusicSource.isPlaying)
                SoundManager.Instance.MusicSource.Play();
        }
        else
            SoundManager.Instance.MusicSource.Stop();

        if (PlayerPrefs.GetInt("SFXPref", 1) == 1)
            SoundManager.Instance.SFXSource.volume = 1f;
        else
            SoundManager.Instance.SFXSource.volume = 0f;
    }

    public void ToggleMusic()
    {
        SoundManager.Instance.ToggleMusic();
    }

    public void ToggleSFX()
    {
        SoundManager.Instance.ToggleSFX();
    }

    public void LoadGame()
    {
        PlayerPrefs.SetInt("IsFreeMode", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Game");
    }
}
