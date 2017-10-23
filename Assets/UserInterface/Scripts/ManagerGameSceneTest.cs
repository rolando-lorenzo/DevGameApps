using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagerGameSceneTest : MonoBehaviour {

    #region Class members
    public Text coinOrLive;
    public GameObject buttonViewAds;
    public Button buttonAddLive;
    public Button buttonSubtractLive;
    public Button buttonCompleteLevel;

    public Button buttonBack;
    public Text nameWorldLevel;
    public Text textNewLevel;

    private int coinLive;
    private string nameWorld { get; set; }
    private int nameLevel { get; set; }
    private int maxLevel { get; set; }
    #endregion

    #region Class accesors
    #endregion

    #region MonoBehaviour overrides
    private void OnEnable()
    {
        AdsManagement.OnCoinLive += HandleCoinLive;
        AdsManagement.OnMessageAds += HandleMessageAds;
        AdsManagement.OnNewLevelUnlocked += HandleNewLevelUnlocek;
    }

    private void Awake()
    {
        AdsManagement.Instance.InitAds();

        PlayConstant constant = new PlayConstant();
        coinLive = PlayerPrefs.GetInt(constant.coinLive, 3);

        coinOrLive.text = "" + coinLive;
        VerifyCoin();

        nameWorld = PlayerPrefs.GetString(constant.worldName, "Mexico");
        nameLevel = PlayerPrefs.GetInt(constant.levelWorld, 1);

        nameWorldLevel.text = nameWorld + " " + nameLevel;         
    }

    private void Start()
    {
        Button btnviewAds = buttonViewAds.GetComponent<Button>();
        btnviewAds.onClick.AddListener(StartAdsViewVideo);

        buttonAddLive.onClick.AddListener(AddLiveAction);
        buttonSubtractLive.onClick.AddListener(SubtractLiveAction);

        buttonCompleteLevel.onClick.AddListener(CompleteLevel);
        buttonBack.onClick.AddListener(GoToLevel);
    }

    private void Update()
    {
        VerifyCoin();
    }

    private void OnDisable()
    {
        AdsManagement.OnCoinLive -= HandleCoinLive;
        AdsManagement.OnMessageAds -= HandleMessageAds;
        AdsManagement.OnNewLevelUnlocked -= HandleNewLevelUnlocek;
    }
    #endregion

    #region Super class overrides
    #endregion

    #region Interface implementation
    private void VerifyCoin()
    {
        if (coinLive == 0)
        {
            buttonViewAds.SetActive(true);
        }
        else
        {
            buttonViewAds.SetActive(false);
        }
    }

    private void CompleteLevel()
    {
        AdsManagement.Instance.CompleteLevel(nameWorld, nameLevel);
    }


    public void GoToLevel()
    {
        SceneManager.LoadScene("LevelScene");
    }

    private void SubtractLiveAction()
    {
        coinLive -= 1;
        if (coinLive < 0)
        {
            coinLive = 0;
        }
        PlayConstant constant = new PlayConstant();
        PlayerPrefs.SetInt(constant.coinLive, coinLive);
        PlayerPrefs.Save();
        coinOrLive.text = "" + coinLive;
    }

    private void AddLiveAction()
    {
        coinLive += 1;
        PlayConstant constant = new PlayConstant();
        PlayerPrefs.SetInt(constant.coinLive, coinLive);
        PlayerPrefs.Save();
        coinOrLive.text = "" + coinLive;
    }

    private void StartAdsViewVideo()
    {
        if (AdsManagement.Instance.InitializeAds)
            AdsManagement.Instance.ShowVideo();
    }

    private void HandleNewLevelUnlocek(int newLevel)
    {
        if(newLevel>0)
            textNewLevel.text = "Desbloqueo el Nivel: " + newLevel;
        else
            textNewLevel.text = "No se desbloqueo el siguiente Nivel: ";
    }

    private void HandleMessageAds(string messageAds, bool isError)
    {
        Debug.Log(messageAds);
    }

    private void HandleCoinLive(int lives)
    {
        coinLive += lives;
        PlayConstant constant = new PlayConstant();
        PlayerPrefs.SetInt(constant.coinLive, coinLive);
        PlayerPrefs.Save();
        coinOrLive.text = "" + coinLive;
    }

    #endregion
}
