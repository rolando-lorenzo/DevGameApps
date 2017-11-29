using EasyMobile;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ControllerWallpaper : MonoBehaviour {

    #region Class members
    //Event rate
    public delegate void EventWallpaperBuy(ControllerWallpaper itemWallpaper, bool isError);
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
    public string pathImage;
    HideInInspector]
    public string fileName;
    //public GUIAnim animWallpaper;
    #endregion

    #region Class accesors
    #endregion

    #region MonoBehaviour overrides

    private void Start()
    {
        buttonCloseWallpaperBuy.onClick.AddListener(() => CloseWallpaperBuy(this));
        buttonWallpaperBuy.onClick.AddListener(() => BuyWallpaper(this));
    }

    private void Awake()
    {

    }
    #endregion

    #region Super class overrides
    #endregion

    #region Class implementation
    public void ShowWallpaper()
    { 
        panelWallpaper.SetActive(true);
        StartCoroutine(ShowPanelWallpaperEnumerator());
    }

    private IEnumerator ShowPanelWallpaperEnumerator()
    {
        
        yield return new WaitForSeconds(1f);
        animDialogWallpaper.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
    }

    private void CloseWallpaperBuy(ControllerWallpaper wallpaperItem)
    {
        animDialogWallpaper.MoveOut(GUIAnimSystem.eGUIMove.SelfAndChildren);
        if (OnWallpaperBuy != null)
            OnWallpaperBuy(wallpaperItem, false);
        StartCoroutine(ClosePanelWallpaper());
    }

    private IEnumerator ClosePanelWallpaper()
    {
        yield return new WaitForSeconds(1f);
        if (panelWallpaper != null)
        {
            panelWallpaper.SetActive(false);
            
            GameObject.Destroy(gameObjectDialogWallpaper);
        }
    }

    private void BuyWallpaper(ControllerWallpaper wallItem)
    {
        Debug.Log("estas Aqui");
        Debug.Log(OnWallpaperBuy);
        if (OnWallpaperBuy != null)
            OnWallpaperBuy(wallItem, true);
        //CloseWallpaperBuy(wallItem);
    }

    private void ShareWallpaper()
    {
        string folderLocation = "/mnt/sdcard/DCIM/Camera/";
        string folderLocationTwo = "/mnt/sdcard/DCIM/Camera/BJWT";
        //string myScreenshotLocation = myFolderLocation + filename;
        byte[] imageData = File.ReadAllBytes(pathImage);

        string filepath = System.IO.Path.Combine(Application.persistentDataPath, fileName + ".png");
        File.WriteAllBytes(filepath, imageData);

        if (System.IO.File.Exists(filepath))
        {
            System.IO.File.Copy(filepath, folderLocation);
            System.IO.File.Copy(filepath, folderLocationTwo);
        }

        MobileNativeShare.ShareImage(filepath, "Wallpaper "+fileName+" !!", "BJWT");

    }
    #endregion

    #region Interface implementation
    #endregion
}
