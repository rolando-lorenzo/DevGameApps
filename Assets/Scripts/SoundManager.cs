using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour {

	#region Class members
	public static SoundManager ins;
	public AudioSource fxSource, musicSource;
	public float fadeTime = 2;
	#endregion

	#region Class accesors
	#endregion

	#region MonoBehaviour overrides
	private void Awake () {
		if (ins == null) {
			ins = this;
		}
		else {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);
	}

	private void Start () {
		musicSource.volume = PlayerPrefs.GetFloat ("volumeLevelMusic", 1);
		fxSource.volume = PlayerPrefs.GetFloat ("fxLevelVolume", 1);
//		musicSource.mute = ExtensionMethods.GetBool ("MusicState", false);
//		fxSource.mute = ExtensionMethods.GetBool ("FXState", false);
	}

	#endregion

	#region Super class overrides
	#endregion

	#region Class implementation


	public void PlayMusic (AudioClip clip, float volumeScale) {
		if (musicSource.clip != null) {
			if (musicSource.clip.name == clip.name)
				return;
		}

		if (musicSource.clip == null)
			AnimateMusicFadeIn (clip, volumeScale);
		else
			AnimateMusicFadeOut (clip, volumeScale);
	}

	public void PlaySound (AudioClip clip, float volumeScale = 1) {
		fxSource.PlayOneShot (clip, volumeScale);
	}

	private void AnimateMusicFadeIn (AudioClip clip, float volumeScale) {
		musicSource.clip = clip;
		musicSource.volume = 0;
		musicSource.Play ();
		Tween.FloatTween (gameObject, "FadeIn", 0, GetMusicVolumeScale () * volumeScale, fadeTime, (tween) => {
			musicSource.volume = tween.Value;
		});
	}

	private void AnimateMusicFadeOut (AudioClip clip, float volumeScale) {
		Tween.FloatTween (gameObject, "FadeOut", GetMusicVolumeScale () * volumeScale, 0, fadeTime, (tween) => {
			musicSource.volume = tween.Value;
		}, (tween) => {
			AnimateMusicFadeIn (clip, volumeScale);
		});
	}

	public void SetMusicVolume (float volume) {
		musicSource.volume = volume;
		PlayerPrefs.SetFloat ("volumeLevelMusic", volume);
	}

	public void SetFXVolume (float volume) {
		fxSource.volume = volume;
		PlayerPrefs.SetFloat ("volumeLevelFX", volume);
	}

	public float GetMusicVolumeScale () {
		return PlayerPrefs.GetFloat ("volumeLevelMusic", 1);
	}

	public float GetFXSaveVolume () {
		return PlayerPrefs.GetFloat ("volumeLevelFX", 1);
	}

	public void SetMusicState (bool state) {
		musicSource.mute = state;
		//ExtensionMethods.SetBool ("MusicState", state);
	}

	public void SetFXState (bool state) {
		fxSource.mute = state;
		//ExtensionMethods.SetBool ("FXState", state);
	}
	#endregion

	#region Interface implementation
	#endregion
}
