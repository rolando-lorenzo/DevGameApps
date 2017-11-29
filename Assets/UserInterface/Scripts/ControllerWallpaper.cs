using EasyMobile;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ControllerWallpaper : MonoBehaviour {

    #region Class members
    //Event rate
    public delegate void EventWallpaperBuy(WallpaperItem itemWallpaper);
    public event EventWallpaperBuy OnWallpaperBuy;
   

    //Setting Rate
    [Header("Setting")]
    public Image imageWallpaper;
    public Button buttonWallpaperBuy;
    public Button buttonCloseWallpaperBuy;
    [Header("Setting Anim")]
    public GameObject gameObjectDialogWallpaper;
    public GameObject panelWallpaper;
    public GUIAnim animDialogWallpaper;
    [HideInInspector]
    public WallpaperItem objWallpaperItem { get; set; }
    //public GUIAnim animWallpaper;
    public static ControllerWallpaper instance;
    #endregion

    #region Class accesors
    #endregion

    #region MonoBehaviour overrides

    private void Start()
    {
        gameObjectDialogWallpaper.SetActive(false);
        buttonCloseWallpaperBuy.onClick.AddListener(() => CloseWallpaperBuy());
        buttonWallpaperBuy.onClick.AddListener(() => BuyWallpaper());
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    #region Super class overrides
    #endregion

    #region Class implementation
    public void ShowWallpaper()
    {
        if (objWallpaperItem != null)
        {
            imageWallpaper.overrideSprite = objWallpaperItem.spriteWallpaper;
            if(objWallpaperItem.islocked == false)
                VerifyWallpaperItemLock();
        }


        gameObjectDialogWallpaper.SetActive(true);
        StartCoroutine(ShowPanelWallpaperEnumerator());
    }

    private IEnumerator ShowPanelWallpaperEnumerator()
    {
        yield return new WaitForSeconds(.5f);
        panelWallpaper.SetActive(true);
        yield return new WaitForSeconds(.5f);
        animDialogWallpaper.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
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
        buttonWallpaperBuy.onClick.AddListener(() => ShareWallpaper());
        buttonWallpaperBuy.GetComponent<Text>().text = "Share Image";
    }

    private void ShareWallpaper()
    {
        string folderLocation = "/mnt/sdcard/DCIM/Camera/";
        string folderLocationTwo = "/mnt/sdcard/DCIM/Camera/BJWT";
        //string myScreenshotLocation = myFolderLocation + filename;
        byte[] imageData = File.ReadAllBytes(objWallpaperItem.pathFileImage);

        string filepath = System.IO.Path.Combine(Application.persistentDataPath, objWallpaperItem.nameFileImage + ".png");
        File.WriteAllBytes(filepath, imageData);

        if (System.IO.File.Exists(filepath))
        {
            System.IO.File.Copy(filepath, folderLocation);
            System.IO.File.Copy(filepath, folderLocationTwo);
        }

        MobileNativeShare.ShareImage(filepath, "Wallpaper "+objWallpaperItem.nameFileImage+" !!", "BJWT");

    }
    #endregion

    #region Interface implementation
    #endregion
}
