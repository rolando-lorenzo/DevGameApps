﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogMessage : MonoBehaviour {

    #region Class members
    //Consifguracion boton 
    public GameObject panelPrefabsDialog;
    public Text txtMessage;
    public Text txtTitle;
    public Image imgDialog;
    public Button btnCloseDialog;
	public Image imgStatus;
	public enum typeMessage
	{
		ERROR,
		WARNING,
		INFO
	}
    private GUIAnim animImageDialog;

    #endregion

    #region MonoBehaviour overrides
    void Awake()
    {
        animImageDialog = imgDialog.GetComponent<GUIAnim>();
        btnCloseDialog.GetComponent<Button>();
        btnCloseDialog.onClick.AddListener(CloseDialogMessage);
    }

    private void Start()
    {       
    }
    #endregion


    #region Class implementation
    public void OpenDialogmessage()
    {
        if (panelPrefabsDialog != null)
        {
            panelPrefabsDialog.SetActive(true);
        }
        animImageDialog.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
    }

    private void CloseDialogMessage()
    {
        animImageDialog.MoveOut(GUIAnimSystem.eGUIMove.SelfAndChildren);
        StartCoroutine(InactivePanel());
    }

    private IEnumerator InactivePanel()
    {
        yield return new WaitForSeconds(2.0f);
        if (panelPrefabsDialog != null)
        {
            panelPrefabsDialog.SetActive(false);
			GameObject.Destroy (panelPrefabsDialog);
        }
    }
    #endregion

}
