using EasyMobile;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ControllerWallpaper : MonoBehaviour {

    #region Class members
    public delegate void EventWallpaperBuy(WallpaperItem itemWallpaper);
    public static event EventWallpaperBuy OnWallpaperBuy;

    public delegate void EventWallpaperMenssage(string title, string message, DialogMessage.typeMessage typeMessage);
    public static event EventWallpaperMenssage OnWallpaperMessage;


    //Setting Rate
    [Header("Setting")]
    public Image imageWallpaper;
    public GameObject buttonWallpaperBuy;
    public GameObject buttonWallpaperShare;
    public Button buttonCloseWallpaperBuy;
    [Header("Setting Anim")]
    public GameObject gameObjectDialogWallpaper;
    public GameObject panelWallpaper;
    public GUIAnim animDialogWallpaper;
    [HideInInspector]
    public WallpaperItem objWallpaperItem { get; set; }
    //public GUIAnim animWallpaper;
    #endregion

    #region Class accesors
    #endregion

    #region MonoBehaviour overrides
    private void OnEnable()
    {
        StoreManager.OnStoreWallpaperUnlockButton += HandlerStoreWallpaperUnlockButton;
        StoreManager.OnWallpaperIsSelected += HandlerWallpaperIsSelected;
    }

    private void Start()
    {
        //PlayerPrefs.DeleteAll();
        //buttonWallpaperShare.SetActive(false);
        gameObjectDialogWallpaper.SetActive(false);
        buttonCloseWallpaperBuy.onClick.AddListener(() => CloseWallpaperBuy());
        buttonWallpaperBuy.GetComponent<Button>().onClick.AddListener(() => BuyWallpaper());
        buttonWallpaperShare.GetComponent<Button>().onClick.AddListener(() => ShareWallpaper());


    }

    private void Awake()
    {
       
    }

    private void OnDisable()
    {
        StoreManager.OnStoreWallpaperUnlockButton -= HandlerStoreWallpaperUnlockButton;
        StoreManager.OnWallpaperIsSelected -= HandlerWallpaperIsSelected;
    }
    #endregion

    #region Super class overrides
    #endregion

    #region Class implementation
    private void HandlerWallpaperIsSelected(WallpaperItem itemWallpaper)
    {
        Debug.Log("am here");
        objWallpaperItem = itemWallpaper;
        imageWallpaper.overrideSprite = objWallpaperItem.spriteWallpaper;
        if (objWallpaperItem.islocked == false)
        {
            VerifyWallpaperItemLock();
        }
        else
        {
            VerifyWallpaperItemUnLock();
        }
        ShowWallpaper();
    }

    public void ShowWallpaper()
    {        
        gameObjectDialogWallpaper.SetActive(true);
        StartCoroutine(ShowPanelWallpaperEnumerator());        
    }

    private IEnumerator ShowPanelWallpaperEnumerator()
    {
        yield return new WaitForSeconds(.0f);
        panelWallpaper.SetActive(true);
        animDialogWallpaper.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
        MenuUtils.CanvasSortingOrderShow();
    }

    private void CloseWallpaperBuy()
    {
        animDialogWallpaper.MoveOut(GUIAnimSystem.eGUIMove.SelfAndChildren);
        StartCoroutine(ClosePanelWallpaper());
    }

    private IEnumerator ClosePanelWallpaper()
    {
        yield return new WaitForSeconds(1f);
        if (panelWallpaper != null)
        {
            gameObjectDialogWallpaper.SetActive(false);
            panelWallpaper.SetActive(false);
            MenuUtils.CanvasSortingOrderHiden();
            //GameObject.Destroy(gameObjectDialogWallpaper);
        }
    }

    private void BuyWallpaper()
    {
        Debug.Log("estas Aqui");
        //Debug.Log(OnWallpaperBuy);
        if (OnWallpaperBuy != null)
            OnWallpaperBuy(objWallpaperItem);
    }

    public void VerifyWallpaperItemLock()
    {
        buttonWallpaperShare.SetActive(true);
        buttonWallpaperBuy.SetActive(false);
    }

    public void VerifyWallpaperItemUnLock()
    {
        buttonWallpaperShare.SetActive(false);
        buttonWallpaperBuy.SetActive(true);
    }

    private void ShareWallpaper()
    {
        try
        {
            //string folderLocation = "/mnt/sdcard/DCIM/Camera/";
            //string folderLocationTwo = "/mnt/sdcard/DCIM/Camera/BJWT";

            String nameImg = objWallpaperItem.idStoreGooglePlay.Replace("com.EstacionPi.BJWTFoundation", "");
            Debug.Log("Obteniendo img de carpeta Resources...ID:"+nameImg);
            var texture = Resources.Load<Texture2D>(nameImg);
            Debug.Log("Guardando en dispoitivo Ruta:"+Application.persistentDataPath+" Imagen:"+nameImg + ".png");
            string filepath = System.IO.Path.Combine(Application.persistentDataPath, nameImg+".png");
            File.WriteAllBytes(filepath,texture.EncodeToPNG());
            Debug.Log("Guardando exitosamente!!" );

            MobileNativeShare.ShareImage(filepath, "Wallpaper " + objWallpaperItem.nameFileImage + " !!", "BJWT");

            /*if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Move(filepath, folderLocation);
                //System.IO.File.Copy(filepath, folderLocationTwo);
            }*/
        }
        catch(Exception e)
        {
            if (OnWallpaperMessage != null)
                OnWallpaperMessage("msg_store_title_popup", "msg_err_fails_share_wallpaper", DialogMessage.typeMessage.ERROR);
           // CloseWallpaperBuy();
            Debug.Log(e);
        }
    }

    private void HandlerStoreWallpaperUnlockButton()
    {
        VerifyWallpaperItemLock();
    }
    #endregion

    #region Interface implementation
    #endregion
}
