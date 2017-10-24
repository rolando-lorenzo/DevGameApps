using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerMusicFx : MonoBehaviour {

    #region Class members
    public Slider musicSlider;
    public Image musicImage;

    public Slider fxSlider;
    public Image fxImage;

    public Sprite[] musicHornSprite;
    private bool stateMusic { get; set; }

    private float maxSlider {get; set;}
    #endregion

    #region Class accesors
    #endregion

    #region MonoBehaviour overrides
    void Awake()
    {
        maxSlider = musicSlider.maxValue;
    }
    void Start()
    {
        /*set Value to Music Slider and Fx Slider
        musicSlider.value = 0.35;
        fxSlider.value = 0.5;*/
    }

    void Update()
    {
        UpadateImageMusic(musicSlider, musicImage);
        UpadateImageMusic(fxSlider, fxImage);
    }
    #endregion

    #region Super class overrides
    #endregion

    #region Class implementation

    private void UpadateImageMusic(Slider slider, Image image)
    {
        float stepImage = maxSlider / musicHornSprite.Length;
        for (int i = 0; i < musicHornSprite.Length; i++)
        {
            float baseComparation = i * stepImage;
            if (slider.value >= baseComparation)
            {
                image.sprite = musicHornSprite[i];
            }
        }
    }

    private void SendSount(float valueSound)
    {
        
    }
    #endregion

    #region Interface implementation
    #endregion
}
