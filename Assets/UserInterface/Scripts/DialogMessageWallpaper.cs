using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogMessageWallpaper : MonoBehaviour {

    #region Class members
    //Consifguracion boton 
    public GameObject panelDialog;
    public GameObject panelPrefabsDialog;
    public Text txtMessage;
    public Text txtTitle;
    public Image imgDialog;
    public Button btnCloseDialog;
    public Image imgStatus;
    public GUIAnim gUIAnimDialog;
    public enum typeMessage
    {
        ERROR,
        WARNING,
        INFO
    }

    #endregion

    #region MonoBehaviour overrides
    void Awake()
    {
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
        //MenuUtils.CanvasSortingOrder();
        gUIAnimDialog.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);

    }

    private void CloseDialogMessage()
    {
        gUIAnimDialog.MoveOut(GUIAnimSystem.eGUIMove.SelfAndChildren);
        StartCoroutine(InactivePanel());
    }

    private IEnumerator InactivePanel()
    {
        yield return new WaitForSeconds(2.0f);
        if (panelPrefabsDialog != null)
        {
            panelPrefabsDialog.SetActive(false);
           // MenuUtils.CanvasSortingOrder();
            GameObject.Destroy(panelDialog);
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
