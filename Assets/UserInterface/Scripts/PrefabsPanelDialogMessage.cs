using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrefabsPanelDialogMessage : MonoBehaviour {

    #region Class members
    //Consifguracion boton 
    public GameObject panelPrefabsDialog;
    public Text txtMessage;
    public Text txtTitle;
    public Image imgDialog;
    public Button btnCloseDialog;
    private GUIAnim animImageDialog;

    #endregion

    #region Class accesors

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

    #region Super class overrides
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
        Debug.Log("Desactivando StorePopup..");
        yield return new WaitForSeconds(2.0f);
        if (panelPrefabsDialog != null)
        {
			GameObject.Destroy (panelPrefabsDialog);
            
        }
    }
    #endregion

    #region Interface implementation
    #endregion
}
