using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManagement : MonoBehaviour {
    
    private static AdsManagement _instance;
    //if Unity XCodeGAME IDSTORE GAME IDSTORE GAME NAME
    private string gameIdApple = "1573415";
    //elif UNITY_ANDROID
	private string gameIdAndroid = "1573414";

    private string gameIdGlobal { get; set; }

    public delegate void EventCoinLive(int lives);
    public static event EventCoinLive OnCoinLive;

    public delegate void EventMessageAds(string messageAds, bool typeAds);
    public static event EventMessageAds OnMessageAds;

    public delegate void EventNewLevelUnlocked(int newLevel);
    public static event EventNewLevelUnlocked OnNewLevelUnlocked;

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

        Debug.Log(OnMessageAds);
        if (OnMessageAds != null)//No se puede cargar el vídeo.
            OnMessageAds("Mata a SUSANA diosito video", true);
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
