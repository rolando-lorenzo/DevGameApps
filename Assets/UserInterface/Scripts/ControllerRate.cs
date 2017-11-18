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

    public void UpdateTextTranslation()
    {
        LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation();
        if (titleRate != null)
        {
            titleRate.text = lm.GetString(titleRate.text);
        }
        if (messageRate != null)
        {
            messageRate.text = lm.GetString(messageRate.text);
        }
        if (rateButton != null)
        {
            rateButton.GetComponentInChildren<Text>().text = lm.GetString(rateButtonText.text);
        }

        if (laterButton != null)
        {
            laterButton.GetComponentInChildren<Text>().text = lm.GetString(lateButtonText.text);
        }
    }

    private void LaterRateAppValue()
    {
        globalCount--;
        globalCount = globalCount < 1 ? 1 : globalCount;

        Debug.Log(globalCount);

        GameItemsManager.SetValueById(GameItemsManager.Item.globalCountRate, globalCount);

        CloseDialogRateAndPanel();
    }

    private void RateAppValue()
    {
        if(OnRateApp != null)
            OnRateApp(10);
        
        globalCount = 1;
        GameItemsManager.SetValueById(GameItemsManager.Item.globalCountRate, globalCount);
        GameItemsManager.SetValueById(GameItemsManager.Item.rateState, 1);
        //Debug.Log(rateUrls);
        Application.OpenURL(rateUrls);

        CloseDialogRateAndPanel();
    }

    #endregion

    #region Interface implementation
    #endregion
}
