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

	public GameObject panelSetting;
    public GameObject panelConditions;
    public GameObject buttonConditions;
    public GameObject textConditions;
    public GameObject btnCloseConditions;

    //Sound Manager
    public GameObject musicSoundManager;
    private bool stateMusic { get; set; }

    #endregion

    #region Class accesors
    #endregion

    #region MonoBehaviour overrides

    void Awake()
    {
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
        panelSetting.SetActive(false);

        Button btnconditions = buttonConditions.GetComponent<Button>();
        btnconditions.onClick.AddListener(ViewConditions);

        Button btncloseconditions = btnCloseConditions.GetComponent<Button>();
        btncloseconditions.onClick.AddListener(ViewSetting);

        textConditions.GetComponent<Text>().text = conditionstext;
        //end conditions

        Button btnsetting = buttonSetting.GetComponent<Button>();
        btnsetting.onClick.AddListener(OpenDialogBoxSetting);

		Button btnstore = buttonStore.GetComponent<Button>();
		btnstore.onClick.AddListener(GoStoreScene);

        Button btnclosedialog = buttonCloseDialog.GetComponent<Button>();
        btnclosedialog.onClick.AddListener(CloseDialogBoxSetting);

		Button btnplay = buttonPlay.GetComponent<Button>();
		btnplay.onClick.AddListener(GoWorldScene);

		logoGameAnim.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
        StartCoroutine(MoveInButtonsMenu());

        PlayConstant constant = new PlayConstant();
        int auxStateMusic = PlayerPrefs.GetInt(key: constant.gameMusic, defaultValue: 1);
        GenerateStateSoundManger(auxStateMusic);
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

	void GoStoreScene(){
		SceneManager.LoadScene("StoreScene");
	}

    public void OpenDialogBoxSetting()
    {
        Debug.Log("Modal Box Setting Open!");
        panelSetting.SetActive(true);
        dialogBoxSettingAnim.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
    }

    void CloseDialogBoxSetting()
    {
        Debug.Log("Modal Box Setting Close!");
        dialogBoxSettingAnim.MoveOut(GUIAnimSystem.eGUIMove.SelfAndChildren);
        panelSetting.SetActive(false);
        panelConditions.SetActive(false);
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

    private void GenerateStateSoundManger(int state)
    {
        if(state == 1)
        {
            stateMusic = true;
        }
        else
        {
            stateMusic = false;
        }
        AudioSource music = musicSoundManager.GetComponent<AudioSource>();
        music.mute = !stateMusic;
        //toggleMusic.startToggleState(stateMusic);
    }

    private void handlerToggleStateMusic(bool stateMsc)
    {
        Debug.Log("Music: " + stateMusic);
        stateMusic = stateMsc;
        PlayConstant constant = new PlayConstant();
        SavePlayerPrefs(constant.gameMusic, stateMsc);

        AudioSource music = musicSoundManager.GetComponent<AudioSource>();
        music.mute = !stateMusic;
    }

    private void SavePlayerPrefs(string constant, bool stateValue)
    {
        int auxStateValue = 0;
        if (stateValue)
        {
            auxStateValue = 1;
        }
        else
        {
            auxStateValue = 0;
        }


        PlayerPrefs.SetInt(constant, auxStateValue);
        PlayerPrefs.Save();
    }

    private void handlerToggleStateFx(bool stateFx)
    {
        Debug.Log("FX: " + stateFx);
    }

	void GoWorldScene()
	{
		SceneManager.LoadScene("WorldScene");
	}

    #endregion

    #region Interface implementation
    #endregion
}
