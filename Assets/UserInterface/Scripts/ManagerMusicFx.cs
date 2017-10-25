using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerMusicFx : MonoBehaviour {

    #region Class members
    public Slider musicSlider;
    public GameObject musicGameObjectOn;
    public GameObject musicGameObjectOff;
    private Image musicImage;

    public Slider fxSlider;
    public GameObject fxGameObjectOn;
    public GameObject fxGameObjectOff;
    private Image fxImage;
    private bool stateFx { get; set; }

    public Sprite[] musicHornSprite;
    private bool stateMusic { get; set; }


    private float maxSlider {get; set;}
    #endregion

    #region Class accesors
    #endregion

    #region MonoBehaviour overrides
    void Awake()
    {
        stateMusic = ExtensionMethods.GetBool("MusicState");
        stateFx = ExtensionMethods.GetBool("FXState");

        verifyStateButtonMusicFx(musicSlider, stateMusic, musicGameObjectOn, musicGameObjectOff, "music");
        verifyStateButtonMusicFx(fxSlider, stateFx, fxGameObjectOn, fxGameObjectOff, "fx");

        maxSlider = musicSlider.maxValue;
    }


    void Start()
    {
        Button musicButtonOn = musicGameObjectOn.GetComponent<Button>();
        musicButtonOn.onClick.AddListener(() => SetMusicStatus());

        Button fxButtonOn = fxGameObjectOn.GetComponent<Button>();
        fxButtonOn.onClick.AddListener(() => SetFxStatus());

        Button musicButtonOff = musicGameObjectOff.GetComponent<Button>();
        musicButtonOff.onClick.AddListener(() => SetMusicStatus());

        Button fxButtonOff = fxGameObjectOff.GetComponent<Button>();
        fxButtonOff.onClick.AddListener(() => SetFxStatus());

        musicImage = musicGameObjectOn.GetComponent<Image>();
        fxImage = fxGameObjectOn.GetComponent<Image>();
        //set Value to Music Slider and Fx Slider
        musicSlider.value = SoundManager.ins.GetMusicSaveVolume();
        fxSlider.value = SoundManager.ins.GetFXSaveVolume();
    }

    void Update()
    {
        UpadateImageMusic(musicSlider, musicImage, "music");
        UpadateImageMusic(fxSlider, fxImage, "fx");
    }
    #endregion

    #region Super class overrides
    #endregion

    #region Class implementation

    private void UpadateImageMusic(Slider slider, Image image, string nameAudio)
    {
        try
        {
            float stepImage = maxSlider / musicHornSprite.Length;
            for (int i = 0; i < musicHornSprite.Length; i++)
            {
                float baseComparation = i * stepImage;
                if (slider.value >= baseComparation)
                {
                    if (i==0 && slider.value == 0)
                    {
                        image.sprite = musicHornSprite[0];
                    }
                    else if(i != 0)
                    {
                        image.sprite = musicHornSprite[i];
                    }

                    if(nameAudio == "music")
                    {
                        SoundManager.ins.SetMusicVolume(slider.value);
                    }
                    else
                    {
                        SoundManager.ins.SetFXVolume(slider.value);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
        
    }

    private void verifyStateButtonMusicFx(Slider slider, bool state, GameObject onGO, GameObject offGO , string name)
    {
        slider.interactable = !state;
        onGO.SetActive(!state);
        offGO.SetActive(state);

        if (state == false)
        {
            UpadateImageMusic(slider, onGO.GetComponent<Image>(), name);
        }
    }

    private void SetFxStatus()
    {
        bool statefx = (stateFx == true) ? false : true;
        stateFx = statefx;
        SoundManager.ins.SetFXState(statefx);
        verifyStateButtonMusicFx(fxSlider, stateFx, fxGameObjectOn, fxGameObjectOff, "fx");
    }

    private void SetMusicStatus()
    {
        bool statemusic = (stateMusic == true) ? false : true;
        stateMusic = statemusic;
        SoundManager.ins.SetMusicState(statemusic);
        verifyStateButtonMusicFx(musicSlider, stateMusic, musicGameObjectOn, musicGameObjectOff, "music");
      
    }
    #endregion

    #region Interface implementation
    #endregion
}
