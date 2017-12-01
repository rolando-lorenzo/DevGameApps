﻿using EasyMobile;
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
    }

    private void Start()
    {
        //buttonWallpaperShare.SetActive(false);
       // gameObjectDialogWallpaper.SetActive(false);
        buttonCloseWallpaperBuy.onClick.AddListener(() => CloseWallpaperBuy());
        buttonWallpaperBuy.GetComponent<Button>().onClick.AddListener(() => BuyWallpaper());
        buttonWallpaperShare.GetComponent<Button>().onClick.AddListener(() => ShareWallpaper());
    }

    private void Awake()
    {
    }

    private void OnDisable()
    {
        StoreManager.OnStoreWallpaperUnlockButton += HandlerStoreWallpaperUnlockButton;
    }
    #endregion

    #region Super class overrides
    #endregion

    #region Class implementation
    public void ShowWallpaper()
    {
        imageWallpaper.overrideSprite = objWallpaperItem.spriteWallpaper;
        if (objWallpaperItem.islocked == false)
        {
            VerifyWallpaperItemLock();
        }
        else
        {
            VerifyWallpaperItemUnLock();
        }

        //gameObjectDialogWallpaper.SetActive(true);
        StartCoroutine(ShowPanelWallpaperEnumerator());        
    }

    private IEnumerator ShowPanelWallpaperEnumerator()
    {
        yield return new WaitForSeconds(.0f);
        panelWallpaper.SetActive(true);
        animDialogWallpaper.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
        MenuUtils.CanvasSortingOrder();
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
            //gameObjectDialogWallpaper.SetActive(false);
            panelWallpaper.SetActive(false);
            MenuUtils.CanvasSortingOrder();
            GameObject.Destroy(gameObjectDialogWallpaper);
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
            Debug.Log("Obteniendo img de carpeta Resources...");
            var texture = Resources.Load<Texture2D>(objWallpaperItem.idStoreGooglePlay);
            string filepath = System.IO.Path.Combine(Application.persistentDataPath, objWallpaperItem.idStoreGooglePlay+".png");
            File.WriteAllBytes(filepath,texture.EncodeToPNG());
            Debug.Log("Img de logro1 Guardada exitosamente !!");

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
