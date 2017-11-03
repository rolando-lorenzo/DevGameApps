using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ControllerRate : MonoBehaviour {

    #region Class members
    //Event rate
    public delegate void EventRateApp(int paws);
    public static event EventRateApp OnRateApp;
    //Setting Rate
    [Header("Panel Rate")]
    public GameObject panelRate;
    public GUIAnim dialogRate;
    public Button rateButton;
    public Button laterButton;
    public Text titleRate;
    public Text messageRate;
    public Text rateButtonText;
    public Text lateButtonText;
    public int globalCount { get; set; }
    public string rateUrls { get; set; }

    #endregion

    #region Class accesors
    #endregion

    #region MonoBehaviour overrides

    private void Start()
    {
        rateButton.onClick.AddListener(() => RateAppValue());
        laterButton.onClick.AddListener(() => LaterRateAppValue());
    }

    private void Awake()
    {
        
    }
    #endregion

    #region Super class overrides
    #endregion

    #region Class implementation

    public void RateAppShow()
    {
        panelRate.SetActive(true);
        StartCoroutine(ShowDialogRate());
    }

    private IEnumerator ShowDialogRate()
    {
        yield return new WaitForSeconds(1.0f);
        // MoveIn all buttons
        dialogRate.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
    }

    private void CloseDialogRateAndPanel()
    {
        dialogRate.MoveOut(GUIAnimSystem.eGUIMove.SelfAndChildren);
        StartCoroutine(ClosePanel());
    }

    private IEnumerator ClosePanel()
    {
        yield return new WaitForSeconds(2.0f);
        // MoveIn all buttons
        if (panelRate != null)
        {
            panelRate.SetActive(false);
            GameObject.Destroy(panelRate);
        }
       
    }

    private void LaterRateAppValue()
    {
        globalCount--;
        globalCount = globalCount < 0 ? 0 : globalCount;

        Debug.Log(globalCount);

        GameItemsManager.SetValueById(GameItemsManager.Item.globalCountRate, globalCount);
        DateTime postTime = DateTime.Now;
        string postpone = postTime.ToLongDateString() + " " + postTime.ToLongTimeString();
        Debug.Log(postpone);
        GameItemsManager.SetValueStringById(GameItemsManager.Item.dateTimePostponeExecution, postpone);
        GameItemsManager.SetValueById(GameItemsManager.Item.postPoneTime, 1);

        CloseDialogRateAndPanel();
    }

    private void RateAppValue()
    {
        if(OnRateApp != null)
            OnRateApp(10);
        //count globalCount = 0;GameItemsManager.SetValueById(GameItemsManager.Item.globalCountRate, globalCount);
        //GameItemsManager.SetValueById(GameItemsManager.Item.rateState, 0);
        //Debug.Log(rateUrls);
        if (Application.platform == RuntimePlatform.Android)
            Application.OpenURL(rateUrls);
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            //veridy in apple
            NativeReviewRequest.RequestReview();
        else
            Application.OpenURL(rateUrls);

        CloseDialogRateAndPanel();
    }

    #endregion

    #region Interface implementation
    #endregion
}
