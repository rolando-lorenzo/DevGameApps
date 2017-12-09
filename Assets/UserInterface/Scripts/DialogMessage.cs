using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogMessage : MonoBehaviour {

    #region Class members
    //Consifguracion boton 
    public GameObject panelDialog;
    public GameObject panelPrefabsDialog;
    public Text txtMessage;
    public Text txtTitle;
    public Image imgDialog;
    public Button btnCloseDialog;
    public Image imgStatus;
    public bool isCharacterOrPowerUp { get; set; }
    public enum typeMessage
	{
		ERROR,
		WARNING,
		INFO
	}
    public GUIAnim gUIAnimDialog;

    #endregion

    #region MonoBehaviour overrides
    void Awake()
    {
        btnCloseDialog.GetComponent<Button>();
        btnCloseDialog.onClick.AddListener(CloseDialogMessage);
    }

    private void Start()
    {
       /// StartCoroutine(DebugOpenDilog());
    }

    private IEnumerator DebugOpenDilog()
    {
        yield return new WaitForSeconds(1.0f);
        OpenDialogmessage();
    }
    #endregion


    #region Class implementation
    public void OpenDialogmessage()
    {
        if (panelPrefabsDialog != null)
        {
            panelPrefabsDialog.SetActive(true);
        }
        gUIAnimDialog.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
        StartCoroutine(WaitCanvas());
        
    }

    private IEnumerator WaitCanvas()
    {
        yield return new WaitForSeconds(.5f);
        MenuUtils.CanvasSortingOrderShow();
    }

    private void CloseDialogMessage()
    {
        gUIAnimDialog.MoveOut(GUIAnimSystem.eGUIMove.SelfAndChildren);
        StartCoroutine(InactivePanel());
    }

    private IEnumerator InactivePanel()
    {
        
        yield return new WaitForSeconds(1.0f);
        if(isCharacterOrPowerUp == true)
        {
            MenuUtils.CanvasSortingOrderHiden();
        }
        
        if (panelPrefabsDialog != null)
        {
            panelPrefabsDialog.SetActive(false);
			GameObject.Destroy (panelDialog);
            
        }
    }

    public void UpdateTextTranslation()
    {
        LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation();
        if (txtMessage != null)
        {
            txtMessage.text = lm.GetString(txtMessage.text);
        }
        if (txtTitle != null)
        {
            txtTitle.text = lm.GetString(txtTitle.text);
        }
    }
    #endregion

}
