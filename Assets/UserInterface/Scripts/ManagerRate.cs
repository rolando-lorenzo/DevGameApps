using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class ManagerRate : MonoBehaviour {

    #region Class members
    /// <summary>
    /// com.BJWT.maze.thing
    /// markert://details?id=
    /// 1185855724
    /// https://itunes.apple.com/app/id/
    /// </summary>
    //Apple data
    [Header("Rate Url")]
    public string appIdApple;
    public string appurlRateApple;

    //Android data
    public string appIdAndroid;
    public string appurlRateAndroid;

    //Setting Rate

    [Header("Rate Count Setting")]
    public bool internetConnection;
    public int LimitRandom;
    public int minSessionCount;

    //internal varibles
    private DateTime dateTimeNow;
    private bool rateStateGlobal { get; set; }
    private bool postPoneTime { get; set; }
    private int globalCount { get; set; }

    [Header("Rate Panel")]
    public GameObject panelRate;
    public GameObject panelDialogMessage;
    public Sprite imgDialogMessageINFO;
    private Transform mainCointener;

    [Header("Text Panel")]
    public string txtTitleRate;
    public string txtMessageRate;
    public string txtBtnRate;
    public string txtBtnLateRate;

    //instance
    public static ManagerRate instance;
    #endregion

    #region Class accesors
    #endregion

    #region MonoBehaviour overrides
    private void OnEnable()
    {
        ControllerRate.OnRateApp += HandlerOnRateApp;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartRate();
    }

    private void OnDisable()
    {
        ControllerRate.OnRateApp -= HandlerOnRateApp;
    }

    private void OnDestroy()
    {
        //GameItemsManager.SetValueStringById(GameItemsManager.Item.dateTimePostponeExecution, postponeDateTime);   
    }
    #endregion

    #region Super class overrides
    #endregion

    #region Class implementation
    public void StartRate()
    {
        globalCount = GameItemsManager.GetValueById(GameItemsManager.Item.globalCountRate);
        //globalCount = globalCount > 0 && ratestate != false ? globalCount : minSessionCount;
        globalCount = globalCount > 0 ? globalCount : minSessionCount;
        int rateStateGlobalAux = GameItemsManager.GetValueById(GameItemsManager.Item.rateState);
        rateStateGlobal = rateStateGlobalAux == 0 ? false : true;

        Debug.Log(postPoneTime);       
    }

    public string GetRateUrl()
    {
        string rateUrl = "";
        if (Application.platform == RuntimePlatform.Android)
            rateUrl = appurlRateAndroid + appIdAndroid;
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            rateUrl = appurlRateApple + appIdApple;
        else
            rateUrl = appurlRateAndroid + appIdAndroid;

        return rateUrl;
    }

    public void ShowAPIRaterRandom()
    {
        Canvas main = GameObject.FindObjectOfType<Canvas>();
        mainCointener = main.GetComponent<Transform>();
        DontDestroyOnLoad(gameObject);
        try
        {

            if (CanShowRateItRandom())
            {
                RateAppShow();
            }
        }
        catch (System.Exception)
        {
            Debug.Log("it can't open the Rate Dialog.");
        }
    }

    private bool CanShowRateItRandom()
    {
        Debug.Log("state: "+rateStateGlobal);
        if (rateStateGlobal == false) {
            if (globalCount > 0)
            {
                LimitRandom = LimitRandom > 3 ? LimitRandom : 3;
                System.Random gen = new System.Random();
                int prob = gen.Next(LimitRandom);
                //int search = (int)LimitRandom / 2;
                int search = gen.Next(LimitRandom);
                Debug.Log("random" + prob);
                Debug.Log("seacrh" + search);
                if (prob == search)
                {
                    if (internetConnection && CheckNetworkAvailability())
                    {
                        Debug.Log("internet");
                        return true;
                    }
                    else
                    {
                        Debug.Log("No internet");
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            return false;
        }

        return false;
    }

    //verify Network
    private bool CheckNetworkAvailability()
    {
        string HtmlText = GetHtmlFromUri("http://google.com");
        if (HtmlText == "")
        {
            //No connection
            Debug.Log("Connection Error");
            return false;
        }
        else if (!HtmlText.Contains("schema.org/WebPage"))
        {
            //Redirecting since the beginning of googles html contains that 
            //phrase and it was not found
            Debug.Log("Connection false");
            return false;
        }
        else
        {
            //success
            Debug.Log("Connection Success");
            return true;
        }
    }

    public string GetHtmlFromUri(string resource)
    {
        string html = string.Empty;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
        try
        {
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                if (isSuccess)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        //We are limiting the array to 80 so we don't have
                        //to parse the entire html document feel free to 
                        //adjust (probably stay under 300)
                        char[] cs = new char[80];
                        reader.Read(cs, 0, cs.Length);
                        foreach (char ch in cs)
                        {
                            html += ch;
                        }
                    }
                }
            }
        }
        catch
        {
            return "";
        }

        return html;
    }

    private void RateAppShow()
    {
        GameObject panel = Instantiate (panelRate) as GameObject;
        ControllerRate controller = panel.GetComponent<ControllerRate>();
        controller.globalCount = globalCount;
        //Debug.Log(GetRateUrl());/
        controller.rateUrls = GetRateUrl();
        controller.lateButtonText.text = txtBtnLateRate;
        controller.rateButtonText.text = txtBtnRate;
        controller.titleRate.text = txtTitleRate;
        controller.messageRate.text = txtMessageRate;
        controller.UpdateTextTranslation();
        panel.transform.SetParent(mainCointener, false);
        controller.RateAppShow();

    }

    private void HandlerOnRateApp(int paws)
    {
        GameItemsManager.addValueById(GameItemsManager.Item.NumPawprints,paws);
        LanguagesManager lm =MenuUtils.BuildLeanguageManagerTraslation();
        Debug.Log("Paws rate: "+paws);
        GameObject mostrarMsg = Instantiate(panelDialogMessage) as GameObject;
        DialogMessage popupMsg = mostrarMsg.GetComponent<DialogMessage>();
        popupMsg.txtMessage.text = string.Format(lm.GetString("msg_info_reward_facebook"),paws);
        popupMsg.txtTitle.text = lm.GetString("msg_info_tnks_review");
        popupMsg.imgStatus.overrideSprite = imgDialogMessageINFO;
        mostrarMsg.transform.SetParent(mainCointener, false);
        popupMsg.OpenDialogmessage();
    }

    #endregion

    #region Interface implementation
    #endregion
}
