using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManagement : MonoBehaviour {
    
    private static AdsManagement _instance;
    //if Unity XCodeGAME IDSTORE GAME IDSTORE GAME NAME
    private string gameIdApple = "1564713";
    //elif UNITY_ANDROID
    private string gameIdAndroid = "1564712";
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

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
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

        if (Advertisement.isSupported)
        {
            Advertisement.Initialize(gameIdApple, true);
            InitializeAds = true;
        }
        else
        {
            InitializeAds = false;
        }

        Debug.Log(InitializeAds);
    } 

    public void ShowVideo()
    {
        if (Advertisement.IsReady("rewardedVideo"))
        {
            ShowOptions options = new ShowOptions();
            options.resultCallback = HandleShowResult;
            Advertisement.Show("rewardedVideo", options);
        }
        else
        {
            Debug.Log("Dont load Video");
        }    
    }

    private void HandleShowResult(ShowResult result)
    {
        Debug.Log(result);
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("Video completed - Offer a reward to the player");
                CoinLive = "1";
                break;
            case ShowResult.Skipped:
                Debug.LogWarning("Video was skipped - Do NOT reward the player");
                CoinLive = "0";
                break;
            case ShowResult.Failed:
                Debug.LogError("Video failed to show");
                CoinLive = "0";
                break;
        }
    }
}
