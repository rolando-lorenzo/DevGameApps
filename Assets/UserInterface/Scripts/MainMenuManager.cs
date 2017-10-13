using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

    #region Class members
    // GUIAnimFREE objects of top and bottom 
    public GUIAnim logoGameAnim;

    //Buttons of menu
	public GUIAnim btnPlayAnim;
	public GUIAnim btnSettingAnim;
	public GUIAnim btnStoreAnim;

    //Button of Facebook and list Score
	public GUIAnim btnFacebookAnim;
	public GUIAnim btnListScoreAnim;

    //Text Conditions
    public string conditionstext;

    //DilogBox
	public GUIAnim dialogBoxSettingAnim;
    //Button Actions
    public Button buttonPlay;
    public Button buttonSetting;
    public Button buttonCloseDialog;
	public Button buttonStore;
	public Button buttonFacebook;
	public Button buttonListScore;

	private GameObject panelSetting;
	private GameObject panelConditions;
	private GameObject buttonConditions;
	private GameObject textConditions;
	private GameObject btnCloseConditions;

    #endregion

    #region Class accesors
    #endregion

    #region MonoBehaviour overrides
    void Awake()
    {
        panelSetting = GameObject.Find("PanelSetting");
        panelConditions = GameObject.Find("PanelConditions");
        buttonConditions = GameObject.Find("ButtonConditions");
        textConditions = GameObject.Find("TextConditions");
        btnCloseConditions = GameObject.Find("BtnCloseConditions");

        if (enabled)
        {
            // Set GUIAnimSystemFREE.Instance.m_AutoAnimation to false in Awake() will let you control all GUI Animator elements in the scene via scripts.
            GUIAnimSystem.Instance.m_AutoAnimation = false;
        }
    }

    void Start()
    {
        //conditions
        panelConditions.SetActive(false);

        Button btnconditions = buttonConditions.GetComponent<Button>();
        btnconditions.onClick.AddListener(ViewConditions);

        Button btncloseconditions = btnCloseConditions.GetComponent<Button>();
        btncloseconditions.onClick.AddListener(ViewSetting);

        textConditions.GetComponent<Text>().text = conditionstext;
        //end conditions

        Button btnplay = buttonPlay.GetComponent<Button>();
        btnplay.onClick.AddListener(GoWorldScene);

        Button btnsetting = buttonSetting.GetComponent<Button>();
        btnsetting.onClick.AddListener(OpenDialogBoxSetting);

		Button btnstore = buttonStore.GetComponent<Button>();
		btnstore.onClick.AddListener(GoStoreScene);

        Button btnclosedialog = buttonCloseDialog.GetComponent<Button>();
        btnclosedialog.onClick.AddListener(CloseDialogBoxSetting);

		logoGameAnim.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
        StartCoroutine(MoveInButtonsMenu());
    }

    void Update()
    {

    }
    #endregion

    #region Super class overrides
    #endregion

    #region Class implementation
    private IEnumerator MoveInButtonsMenu()
    {
        //Wait 2frame
        yield return new WaitForSeconds(1.0f);

        // MoveIn all buttons
        btnPlayAnim.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
        btnSettingAnim.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
        btnStoreAnim.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);

        //start rutine of effect btn facebook
        StartCoroutine(MoveInButtonsSecondMenu());

    }

    private IEnumerator MoveInButtonsSecondMenu()
    {
        //Wait 2frame
        yield return new WaitForSeconds(1.0f);
        // MoveIn all buttons
        btnFacebookAnim.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
        btnListScoreAnim.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);

    }


    void GoWorldScene()
    {
        SceneManager.LoadScene("WorldScene");
    }

	void GoStoreScene(){
		SceneManager.LoadScene("StoreScene");
	}

    void OpenDialogBoxSetting()
    {
        Debug.Log("Modal Box Setting Open!");
        dialogBoxSettingAnim.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
    }

    void CloseDialogBoxSetting()
    {
        Debug.Log("Modal Box Setting Close!");
        dialogBoxSettingAnim.MoveOut(GUIAnimSystem.eGUIMove.SelfAndChildren);
    }

    private void ViewConditions()
    {
        panelConditions.SetActive(true);
        panelSetting.SetActive(false);
    }

    private void ViewSetting()
    {
        panelSetting.SetActive(true);
        panelConditions.SetActive(false);
        
    }
    #endregion

    #region Interface implementation
    #endregion
}
