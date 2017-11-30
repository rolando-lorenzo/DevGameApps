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
    public event EventWallpaperBuy OnWallpaperBuy;
   

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
    public static ControllerWallpaper instance;
    #endregion

    #region Class accesors
    #endregion

    #region MonoBehaviour overrides

    private void Start()
    {
        buttonWallpaperShare.SetActive(false);
        gameObjectDialogWallpaper.SetActive(false);
        buttonCloseWallpaperBuy.onClick.AddListener(() => CloseWallpaperBuy());
        buttonWallpaperBuy.GetComponent<Button>().onClick.AddListener(() => BuyWallpaper());
        buttonWallpaperShare.GetComponent<Button>().onClick.AddListener(() => ShareWallpaper());
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
        Debug.Log(objWallpaperItem.idWallpaper.ToString());
        if (objWallpaperItem != null && objWallpaperItem.spriteWallpaper !=null)
        {
            if (objWallpaperItem.isAvailableInStore)
            {
                imageWallpaper.overrideSprite = objWallpaperItem.spriteWallpaper;
                if (objWallpaperItem.islocked == false)
                    VerifyWallpaperItemLock();
                else
                    VerifyWallpaperItemUnLock();

                gameObjectDialogWallpaper.SetActive(true);
                StartCoroutine(ShowPanelWallpaperEnumerator());
            }
            else
            {
                Debug.Log("No disponible");
            }
                
        }
        else
        {
            Debug.Log("No dispoble");
        }

        
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
            string folderLocation = "/mnt/sdcard/DCIM/Camera/";
            //string folderLocationTwo = "/mnt/sdcard/DCIM/Camera/BJWT";
            Debug.Log("Obteniendo img de carpeta Resources...");
            var texture = Resources.Load<Texture2D>(objWallpaperItem.idStoreGooglePlay);

            string filepath = System.IO.Path.Combine(Application.persistentDataPath, objWallpaperItem.nameFileImage + ".png");
            File.WriteAllBytes(filepath, texture.EncodeToPNG());
            Debug.Log("Img de logro1 Guardada exitosamente !!");

            MobileNativeShare.ShareImage(filepath, "Wallpaper " + objWallpaperItem.nameFileImage + " !!", "BJWT");

            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Move(filepath, folderLocation);
                //System.IO.File.Copy(filepath, folderLocationTwo);
            }
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }
    #endregion

    #region Interface implementation
    #endregion
}
