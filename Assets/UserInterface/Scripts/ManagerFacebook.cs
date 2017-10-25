using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagerFacebook : MonoBehaviour {

    
    #region Class members
    public Button btnStartFacebook;
    public Image imageProfilePicture;
    public Text textUserName;

    public Button btnCloseDialog;
    public GUIAnim animDialogFacebook;

    public GameObject panelDialogMessage;

    public Button btnShareFacebook;
    public Button btnInviteFacebook;
    public Button btnShareScoreScreenFacebook;
	public Button btnSignOut;

	public Sprite imgDialogMessageINFO;
	public Sprite imgDialogMessageWARN;
	public Sprite imgDialogMessageERR;

    public Transform mainCointener;

	private static ManagerFacebook _instance;
    #endregion

    #region Class accesors

    #endregion

    #region MonoBehaviour overrides
    private void OnEnable()
    {
        FacebookDelegate.Instance.OnLoginFacebookStatus += HandleLoginFacebookStatus;
        FacebookDelegate.Instance.OnMessageFacebookProgress += HandleMessegeFacebookProgress;
        FacebookDelegate.Instance.OnProfileUsernameFacebook += HandleProfileUsernameFacebook;
        FacebookDelegate.Instance.OnProfilePictureFacebook += HandleProfilePictureFacebook;
        FacebookDelegate.Instance.OnPawsFacebookReward += HandheldPawsFacebookReward;
    }

    private void Awake()
    {
        _instance = this;
        FacebookDelegate.Instance.InitFB();
    }

    private void Start()
    {
        btnStartFacebook.GetComponent<Button>();
        btnStartFacebook.onClick.AddListener(StarLoginFacebook);

        btnCloseDialog.GetComponent<Button>();
        btnCloseDialog.onClick.AddListener(closeFadePanelFacebook);

        btnShareFacebook.onClick.AddListener(ShareFacebook);
        btnShareScoreScreenFacebook.onClick.AddListener(ShareScoreScreenFacebook);
        btnInviteFacebook.onClick.AddListener(InviteFacebook);

		btnSignOut.onClick.AddListener (SignOut);


    }

    private void OnDisable()
    {
        FacebookDelegate.Instance.OnLoginFacebookStatus -= HandleLoginFacebookStatus;
        FacebookDelegate.Instance.OnMessageFacebookProgress -= HandleMessegeFacebookProgress;
        FacebookDelegate.Instance.OnProfileUsernameFacebook -= HandleProfileUsernameFacebook;
        FacebookDelegate.Instance.OnProfilePictureFacebook -= HandleProfilePictureFacebook;
		FacebookDelegate.Instance.OnPawsFacebookReward -= HandheldPawsFacebookReward;
    }
    #endregion

    

    #region Class implementation
    private void StarLoginFacebook()
    {
        FacebookDelegate.Instance.LoginUserFacebook();
    }

    private void HandleLoginFacebookStatus(bool statusLogin)
    {
        Debug.Log("start Here Facebook");
        if (statusLogin)
        {
            animDialogFacebook.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
        }
    }

    private void HandleProfilePictureFacebook(Sprite picture)
    {
        imageProfilePicture.GetComponent<Image>();
        imageProfilePicture.sprite = picture;
    }

    private void HandleProfileUsernameFacebook(string name)
    {
        textUserName.text = name;
    }

    private void HandleMessegeFacebookProgress(string message, bool isError)
    {
		if (isError) {
			BuildDialogMessage ("Facebook", message, DialogMessage.typeMessage.ERROR);
		} else {
			BuildDialogMessage ("Facebook",message,DialogMessage.typeMessage.INFO);
		}
    }

    private void HandheldPawsFacebookReward(int paws)
    {
		GameItemsManager.addValueById (GameItemsManager.Item.numPawprints,paws);
        //get rewards in number of paws
        Debug.Log("Num huellas de regalo:" + paws);
	}

    private void closeFadePanelFacebook()
    {
        animDialogFacebook.MoveOut(GUIAnimSystem.eGUIMove.SelfAndChildren);
    }

 

    private void InviteFacebook()
    {
        FacebookDelegate.Instance.InviteFriendsFacebook();
    }

    private void ShareScoreScreenFacebook()
    {
        FacebookDelegate.Instance.ShareScoreScreenFacebook();
    }

    private void ShareFacebook()
    {
        FacebookDelegate.Instance.ShareFacebook();
    }

	private void SignOut(){
		FacebookDelegate.Instance.LogOut ();
		closeFadePanelFacebook ();
	}

	/// <summary>
	/// Builds the dialog message.
	/// </summary>
	/// <param name="title">Title.</param>
	/// <param name="msg">Message.</param>
	/// <param name="type">Type.(ERROR, INFO, WARN)</param>
	private void BuildDialogMessage(string title, string msg, DialogMessage.typeMessage type){
		GameObject mostrarMsg = Instantiate (panelDialogMessage) as GameObject;
		DialogMessage popupMsg = mostrarMsg.GetComponent<DialogMessage> ();
		popupMsg.txtMessage.text= msg;
		popupMsg.txtTitle.text = title;
		switch (type) {
		case DialogMessage.typeMessage.ERROR:
			popupMsg.imgStatus.overrideSprite = imgDialogMessageERR;
			break;
		case DialogMessage.typeMessage.INFO:
			popupMsg.imgStatus.overrideSprite = imgDialogMessageINFO;
			break;
		case DialogMessage.typeMessage.WARNING:
			popupMsg.imgStatus.overrideSprite = imgDialogMessageWARN;
			break;
		}
		mostrarMsg.transform.SetParent (mainCointener,false);
		popupMsg.OpenDialogmessage();
	}
    #endregion

    #region Interface implementation
    #endregion
}
