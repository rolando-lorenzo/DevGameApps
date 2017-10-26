using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManagement : MonoBehaviour {
    
    private static AdsManagement _instance;
    //if Unity XCodeGAME IDSTORE GAME IDSTORE GAME NAME
    private string gameIdApple = "1564713";
    //elif UNITY_ANDROID
    private string gameIdAndroid = "1564712";

    private string gameIdGlobal { get; set; }

    public delegate void EventCoinLive(int lives);
    public static event EventCoinLive OnCoinLive;

    public delegate void EventMessageAds(string messageAds, bool typeAds);
    public static event EventMessageAds OnMessageAds;

    public bool InitializeAds { get; set; }

    public static AdsManagement Instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject gameObject = new GameObject("ManageAds");
                gameObject.AddComponent<AdsManagement>();
            }

            return _instance;

        }
    }

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
            gameIdGlobal = gameIdAndroid;
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            gameIdGlobal = gameIdApple;
    }

    void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        _instance = this;
    }

    public void InitAds()
    {
        if (Advertisement.isSupported)
        {
            Advertisement.Initialize(gameIdAndroid, true);
            InitializeAds = true;
        }
        else
        {
            InitializeAds = false;
        }
    } 

    public void ShowVideo()
    {
        if (InitializeAds == true)
        {
            if (Advertisement.IsReady("rewardedVideo"))
            {
                ShowOptions options = new ShowOptions();
                options.resultCallback = HandleShowResult;
                Advertisement.Show("rewardedVideo", options);
            }
            else
            {
                if (OnMessageAds != null)//No se puede cargar el vídeo.
                    OnMessageAds("Unable to load video", false);
            }
        }
        else
        {
            InitializeAds = false;
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        Debug.Log(result);
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("Video completed - Offer a reward to the player");
                if (OnCoinLive != null)
                    OnCoinLive(1);
                break;
            case ShowResult.Skipped:
                Debug.LogWarning("Video was skipped - Do NOT reward the player");
                if (OnCoinLive != null)
                    OnCoinLive(0);
                break;
            case ShowResult.Failed:
                if (OnCoinLive != null)
                    OnCoinLive(0);
                break;
        }
    }
}
