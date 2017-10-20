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

    public delegate void EventInitializeAds(bool stateAds);
    public event EventInitializeAds OnInitializeAds;

    public delegate void EventCoinLive(int lives);
    public event EventCoinLive OnCoinLive;

    public delegate void EventMessageAds(string messageAds, bool typeAds);
    public event EventMessageAds OnMessageAds;

    public delegate void EventNewLevelUnlocked(int newLevel);
    public event EventNewLevelUnlocked OnNewLevelUnlocked;

    public bool InitializeAds { get; set; }
    public string CoinLive { get; set; }

    public static AdsManagement Instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject gameObject = new GameObject("AdsManage");
                gameObject.AddComponent<AdsManagement>();
            }

            return _instance;

        }
    }

    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
            gameIdGlobal = gameIdAndroid;
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            gameIdGlobal = gameIdApple;
    }

    private void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        _instance = this;
    }

    public void InitAds()
    {
        if (Advertisement.isSupported)
        {
            Advertisement.Initialize(gameIdAndroid, true);
            if (OnInitializeAds != null)
                OnInitializeAds(true);

            InitializeAds = true;
        }
        else
        {
            if (OnInitializeAds != null)
                OnInitializeAds(false);
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
            if (OnInitializeAds != null)
                OnInitializeAds(false);
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

    public void CompleteLevel(string nameWorld, int nameLevel)
    {
        char[] split = { '/', '-' };
        PlayConstant constant = new PlayConstant();
        string[] progresslevels = PlayerPrefs.GetString(constant.gameProgressLevel).Split(split);

        int maxlevel = 0;

        for (int i = 0; i < progresslevels.Length; i++)
        {

            //Debug.Log(levelname);
            if (progresslevels[i] == nameWorld)
            {
                maxlevel = Int32.Parse(progresslevels[i + 1]);
                if (nameLevel + 1 <= 5)
                {
                    int newlevel = nameLevel + 1;
                    if (newlevel >= maxlevel)
                    {
                        progresslevels[i + 1] = "" + newlevel;
                    }                   
                }
                else
                {
                    progresslevels[i + 1] = "5";
                }

            }
            //Debug.Log(progresslevels[i]);
        }

        int levelunlock = nameLevel + 1;
        if (OnNewLevelUnlocked != null)
            OnNewLevelUnlocked(levelunlock);

        CreateAndSaveLevel(progresslevels);
    }

    private void CreateAndSaveLevel(string[] progresslevels)
    {
        string cadena = "";
        for (int i = 0; i < progresslevels.Length; i += 2)
        {
            if ((i + 1) < progresslevels.Length)
            {
                cadena += progresslevels[i] + "-" + progresslevels[i + 1] + "/";
            }

        }
        Debug.Log(cadena);
        PlayConstant constant = new PlayConstant();
        PlayerPrefs.SetString(constant.gameProgressLevel, cadena);
    }
}
