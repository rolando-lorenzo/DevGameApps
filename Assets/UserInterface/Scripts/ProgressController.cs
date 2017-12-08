using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressController : MonoBehaviour {
    #region Class members
    public enum Simulation
    {
        TRUE,
        FALSE
    }

    [Header("Game Mode")]
    public GameItemsManager.GameMode gameMode;
    public Simulation simulation;

    [Header("Setting Controller")]
    public GameObject dialogMessagePop;
    public Sprite spriteAlert;
    public Sprite[] spriteWorlds;
    private Transform mainContainer;

    
    #endregion

    #region MonoBehaviour overrides

    private void OnEnable()
    {
        ProgressWorldLevel.OnProgressCompleteWorld += HandlerProgressCompleteWorld;
    }
    // Use this for initialization
    void Start () {
        if(gameMode == GameItemsManager.GameMode.DEBUG)
            StartCoroutine(VerifyEvents());
	}

    // Update is called once per frame
    void Update () {
		
	}

    private void OnDisable()
    {
        ProgressWorldLevel.OnProgressCompleteWorld -= HandlerProgressCompleteWorld;
    }

    #endregion

    #region Class implementation
    private IEnumerator VerifyEvents()
    {
        yield return new WaitForSeconds(2.0f);
        if(simulation == Simulation.FALSE)
        {
           // ProgressWorldLevel.SimulationEventFalse();
        }
        else
        {
            //ProgressWorldLevel.SimulationEventTrue();
        }
    }
    private void HandlerProgressCompleteWorld(string infoText, bool isAlertProgress)
    {
        LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation();
        GameObject mostrarMsg = Instantiate(dialogMessagePop) as GameObject;
        DialogMessage popupMsg = mostrarMsg.GetComponent<DialogMessage>();
        if(isAlertProgress == true)
        {
            popupMsg.txtMessage.text = lm.GetString(infoText);
           /* switch (worldsNames)
            {
                case ProgressWorldLevel.WorldsNames.Train:
                    popupMsg.imgStatus.overrideSprite = spriteWorlds[0];
                    break;
                case ProgressWorldLevel.WorldsNames.Zoo:
                    popupMsg.imgStatus.overrideSprite = spriteWorlds[1];
                    break;
                case ProgressWorldLevel.WorldsNames.Mansion:
                    popupMsg.imgStatus.overrideSprite = spriteWorlds[2];
                    break;
            }*/
            
            var imgStatusTransform = popupMsg.imgStatus.transform as RectTransform;
            imgStatusTransform.sizeDelta = new Vector2(300f, 300f);
        }
        else
        {
            //popupMsg.txtMessage.text = string.Format(lm.GetString(infoText), yinYangs.ToString());
            popupMsg.imgStatus.overrideSprite = spriteAlert;
        }
        
        popupMsg.imgStatus.gameObject.SetActive(true);
        Canvas main = GameObject.FindObjectOfType<Canvas>();
        mainContainer = main.GetComponent<Transform>();
        mostrarMsg.transform.SetParent(mainContainer, false);
        popupMsg.OpenDialogmessage();
    }
    #endregion
}
