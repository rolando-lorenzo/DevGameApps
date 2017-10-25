using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FadeType {In, Out}

public class SoundManager: MonoBehaviour {

    #region Class members
	public static SoundManager ins;
	public AudioSource fxSource, musicSource;
    public float volumeLevelMusic, volumeLevelSound;
    public float fadeTime;
    #endregion

    #region Class accesors
    #endregion

    #region MonoBehaviour overrides
	private void Awake()
	{
        if (ins == null)
        {
            ins = this;
        }
        else if (ins !=  this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        musicSource.volume = PlayerPrefs.GetFloat("volumeLevelMusic",1);
        fxSource.volume = PlayerPrefs.GetFloat("fxLevelVolume",1);
        musicSource.mute = ExtensionMethods.GetBool("MusicState");
        fxSource.mute = ExtensionMethods.GetBool("FXState");

        Debug.Log(ExtensionMethods.GetBool("MusicState"));
        Debug.Log(ExtensionMethods.GetBool("FXState"));
    }

    #endregion

    #region Super class overrides
    #endregion

    #region Class implementation


	public void PlayMusic(AudioClip clip)
	{
        if (musicSource.clip == null)
        {
            musicSource.clip = clip;
            musicSource.volume = 0;
            musicSource.Play();
            ITween tween = Tween.FloatTween(gameObject, "FadeIn", 0, GetMusicSaveVolume(), fadeTime, (startTween) =>
            {
                musicSource.volume = startTween.Value;
            });
        }
        else
        {
            ITween tween = Tween.FloatTween(gameObject, "FadeOut", GetMusicSaveVolume(), 0,  fadeTime, (startTween) =>
            {
                musicSource.volume = startTween.Value;
            }, (t) => {
                musicSource.clip = clip;
                musicSource.volume = 0;
                musicSource.Play();
                ITween secondTween = Tween.FloatTween(gameObject, "FadeIn", 0, GetMusicSaveVolume(), fadeTime, (endTween) =>
                {
                    musicSource.volume = endTween.Value;
                });
            });
        }
    }

    public void PlaySound(AudioClip clip)
	{
		fxSource.PlayOneShot (clip);
	}

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("volumeLevelMusic", volume);
    }

    public void SetFXVolume(float volume)
    {
        fxSource.volume = volume;
        PlayerPrefs.SetFloat("volumeLevelFX", volume);
    }

    public float GetMusicSaveVolume()
    {
        return PlayerPrefs.GetFloat("volumeLevelMusic", 1);
    }

    public float GetFXSaveVolume()
    {
        return PlayerPrefs.GetFloat("volumeLevelFX", 1);
    }

    public void SetMusicState(bool state)
    {
        musicSource.mute = state;
        ExtensionMethods.SetBool("MusicState", state);
    }

    public void SetFXState(bool state)
    {
        fxSource.mute = state;
        ExtensionMethods.SetBool("FXState", state);
    }
    #endregion

}
