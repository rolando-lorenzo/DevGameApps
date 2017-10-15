using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagerFacebook : MonoBehaviour {

    private static ManagerFacebook _instance;
    #region Class members
    public Button btnStartFacebook;
    public Image imageProfilePicture;
    public Text textUserName;

    public Button btnCloseDialog;
    public GameObject panelFacebook;
    public GUIAnim animDialogFacebook;

    public Button btnPlay;
    private bool flagFacebook = false;
    public GameObject panelDialogMessage;

    public Button btnShareFacebook;
    public Button btnInviteFacebook;
    public Button btnShareScoreScreenFacebook;

    public Transform mainCointener;
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

        btnPlay.onClick.AddListener(GoWorldScene);
        panelFacebook.SetActive(false);
    }

    private void OnDisable()
    {
        FacebookDelegate.Instance.OnLoginFacebookStatus -= HandleLoginFacebookStatus;
        FacebookDelegate.Instance.OnMessageFacebookProgress -= HandleMessegeFacebookProgress;
        FacebookDelegate.Instance.OnProfileUsernameFacebook -= HandleProfileUsernameFacebook;
        FacebookDelegate.Instance.OnProfilePictureFacebook -= HandleProfilePictureFacebook;
    }
    #endregion

    #region Super class overrides

    #endregion

    #region Class implementation
    private void StarLoginFacebook()
    {
        flagFacebook = true;
        FacebookDelegate.Instance.LoginUserFacebook();
    }

    private void HandleLoginFacebookStatus(bool statusLogin)
    {
        Debug.Log("start Here Facebook");
        if (statusLogin)
        {
            flagFacebook = true;
            panelFacebook.SetActive(true);
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

    private void HandleMessegeFacebookProgress(string message, bool status)
    {
        GameObject dialogaux = Instantiate(panelDialogMessage) as GameObject;
        PrefabsPanelDialogMessage dialog =  dialogaux.GetComponent<PrefabsPanelDialogMessage>();
        dialog.txtTitle.text = "Facebook";
        dialog.txtMessage.text = message;
        dialogaux.transform.SetParent(mainCointener, false);
        dialog.OpenDialogmessage();
        Debug.Log(message  + "-"+ status);
    }

    private void HandheldPawsFacebookReward(int paws)
    {
        //get rewards in number of paws
        Debug.Log("Num paws:" + paws);
    }

    private void closeFadePanelFacebook()
    {
        panelFacebook.SetActive(false);
        flagFacebook = false;
    }

    private void GoWorldScene()
    {
        if (flagFacebook == false)
        {
            SceneManager.LoadScene("WorldScene");
        }

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

    #endregion

    #region Interface implementation
    #endregion
}
