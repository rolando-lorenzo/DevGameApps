using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerSound : MonoBehaviour {

    #region Class members
    public GameObject musicSoundManager;
    private bool stateMusic { get; set; }

    #endregion

    #region Class accesors
    #endregion

    #region MonoBehaviour overrides

    void Start()
    {
        PlayConstant constant = new PlayConstant();
        int auxStateMusic = PlayerPrefs.GetInt(key: constant.gameMusic, defaultValue: 1);
        GenerateStateSoundManger(auxStateMusic);
    }

    void Update()
    {

    }
    #endregion

    #region Super class overrides
    #endregion

    #region Class implementation

    private void GenerateStateSoundManger(int state)
    {
        if (state == 1)
        {
            stateMusic = true;
        }
        else
        {
            stateMusic = false;
        }
        AudioSource music = musicSoundManager.GetComponent<AudioSource>();
        music.mute = !stateMusic;
    }
    #endregion

    #region Interface implementation
    #endregion
}
