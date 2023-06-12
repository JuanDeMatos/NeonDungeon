using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider master, music, effects;
    [SerializeField] TMP_Dropdown quality;

    void Start()
    {
        Quality();
        Sound();
    }

    private void Quality()
    {
        int quality = PlayerPrefs.GetInt("QualityIndex", 2);

        this.quality.value = quality;

        QualitySettings.SetQualityLevel(quality,true);
    }

    private void Sound()
    {
        float master = PlayerPrefs.GetFloat("MasterVolume", 1);
        float effects = PlayerPrefs.GetFloat("EffectsVolume", 1);
        float music = PlayerPrefs.GetFloat("MusicVolume", 0.16f);

        this.master.value = master;
        this.effects.value = effects;
        this.music.value = music;

        mixer.SetFloat("MasterVolume", Mathf.Log10(master) * 20);
        mixer.SetFloat("EffectsVolume", Mathf.Log10(effects) * 20);
        mixer.SetFloat("MusicVolume", Mathf.Log10(music) * 20);
    }

    public void Quality (int index)
    {
        PlayerPrefs.SetInt("QualityIndex", index);
        QualitySettings.SetQualityLevel(index,true);
    }

    public void MasterVolume (float value)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);

        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }

    public void EffectsVolume(float value)
    {
        mixer.SetFloat("EffectsVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("EffectsVolume", value);
        PlayerPrefs.Save();
    }

    public void MusicVolume(float value)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }
}
