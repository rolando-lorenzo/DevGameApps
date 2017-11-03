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
    public int minSessionCount;
    public float delayAfterLaunchInHours;
    public float postponeInHours;

    //internal varibles
    private DateTime dateTimeNow;
    private bool rateStateGlobal { get; set; }
    private bool postPoneTime { get; set; }
    private int globalCount { get; set; }

    [Header("Rate Panel")]
    public GameObject panelRate;
    public GameObject panelDialogMessage;
    public Sprite imgDialogMessageINFO;
    public Transform mainCointener;

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
        delayAfterLaunchInHours = delayAfterLaunchInHours > 1 ? 1 : delayAfterLaunchInHours;
        postponeInHours = postponeInHours > 24 ? 24 : postponeInHours;
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
        dateTimeNow = DateTime.Now;
        //string nowDateAux = dateNow.Date.ToLongDateString() + " " + dateNow.ToLongTimeString();
        //get time postpone 
        globalCount = GameItemsManager.GetValueById(GameItemsManager.Item.globalCountRate);
        //globalCount = globalCount > 0 && ratestate != false ? globalCount : minSessionCount;
        globalCount = globalCount > 0 ? globalCount : minSessionCount;
        int postPoneTimeAux = GameItemsManager.GetValueById(GameItemsManager.Item.postPoneTime);
        postPoneTime = postPoneTimeAux == 0 ? false : true;

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

    public void ShowAPIRater()
    {
        try
        {

            if (CanShowRateIt())
            {
                RateAppShow();
            }
        }
        catch (System.Exception)
        {
            Debug.Log("it can't open the Rate Dialog.");
        }
    }

    private bool CanShowRateIt()
    {
        Debug.Log("count Global: "+ globalCount);
        if (globalCount > 0)
        {
            if (TimeVerify())
            {
                if (internetConnection && CheckNetworkAvailability())
                {
                    Debug.Log("intert");
                    return true;
                }
                else
                {
                    Debug.Log("No intert");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        
        return false;
    }

    private bool TimeVerify()
    {
        //if it rate was postponed 
        Debug.Log("postPone:" + postPoneTime);
        if (postPoneTime)
        {
            //return TimeBetweenPostponeAndLaunch();
            if (TimeBetweenPostponeAndLaunch())
            {
                Debug.Log("here Sofia");
                return true;
            }
            else if (TimeBetweenLaunchAndNow())
            {

                System.Random gen = new System.Random();
                int prob = gen.Next(100);

                return prob <= 50;
            }
            {
                return false;
            }
        }
        else
        {
            TimeBetweenLaunchAndNow();
        }

        return false;
    }

    private bool TimeBetweenPostponeAndLaunch()
    {
        int baseHour = (int)postponeInHours / 1;
        int baseMinutes = (int)((postponeInHours - baseHour) * 60);

        Debug.Log("DelayAfterPostPone Hour:" + baseHour);
        Debug.Log("DelayAfterPostPone Minutes:" + baseMinutes);

        string postponeDateTime = GameItemsManager.GetValueStringById(GameItemsManager.Item.dateTimePostponeExecution);
        DateTime dateTimePostpone = DateTime.Parse(postponeDateTime);
        DateTime AuxTime = DateTime.Now;
        TimeSpan subtime = AuxTime.Subtract(dateTimePostpone);

        Debug.Log("PostPone Sub:" + subtime.Days+ " - " + subtime.Hours + " - " + subtime.Minutes);

        if (subtime.Days == 0)
        {
            if (baseHour <= subtime.Hours)
            {
                if (baseMinutes <= subtime.Minutes)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private bool TimeBetweenLaunchAndNow()
    {
        int baseHour = (int)delayAfterLaunchInHours / 1;
        int baseMinutes = (int)((delayAfterLaunchInHours - baseHour) * 60);

        Debug.Log("DelayAfterLunch Hour:" + baseHour);
        Debug.Log("DelayAfterLunch Minutes:" + baseMinutes);

        DateTime AuxTime = DateTime.Now;
        TimeSpan subtime = AuxTime.Subtract(dateTimeNow);

        Debug.Log("Launch Sub:" + subtime.Days + " - " + subtime.Hours + " - " + subtime.Minutes);
        //it verify if the gamer played to more at time launch 

        if (subtime.Days == 0)
        {
            if (baseHour <= subtime.Hours)
            {
                if (baseMinutes <= subtime.Minutes)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

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
        Debug.Log("here susana");
        GameObject panel = Instantiate (panelRate) as GameObject;
        ControllerRate controller = panel.GetComponent<ControllerRate>();
        controller.globalCount = globalCount;
        Debug.Log(GetRateUrl());
        controller.rateUrls = GetRateUrl();
        controller.lateButtonText.text = txtBtnLateRate;
        controller.rateButtonText.text = txtBtnRate;
        controller.titleRate.text = txtTitleRate;
        controller.messageRate.text = txtMessageRate;

        panel.transform.SetParent(mainCointener, false);
        controller.RateAppShow();

    }

    private void HandlerOnRateApp(int paws)
    {
        Debug.Log("Paws rate: "+paws);

        /*GameObject mostrarMsg = Instantiate(panelDialogMessage) as GameObject;
        DialogMessage popupMsg = mostrarMsg.GetComponent<DialogMessage>();
        popupMsg.txtMessage.text = "Temporal";
        popupMsg.txtTitle.text = "Title Temporal";
        popupMsg.imgStatus.overrideSprite = imgDialogMessageINFO;
        mostrarMsg.transform.SetParent(mainCointener, false);
        popupMsg.OpenDialogmessage();*/
    }

    #endregion

    #region Interface implementation
    #endregion
}
