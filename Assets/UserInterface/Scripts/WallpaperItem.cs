using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallpaperItem : MonoBehaviour, IStorePurchase
{

    #region Class members
    //Event rate
    public delegate void EventWallpaperBuy(WallpaperItem itemWallpaper);
    public event EventWallpaperBuy OnWallpaperBuy;

    //wallpaper
    private GameObject panelWall;
    private ControllerWallpaper controller;
    public GameObject gameObjectControllerWallpaper;
    [HideInInspector]
    public GameItemsManager.Wallpaper idWallpaper;
    [HideInInspector]
    public string nameFile;
    public string idStoreGooglePlay { get; set; }
    public bool isAvailableInStore { get; set; }
    [HideInInspector]
    public Sprite spriteWallpaper;

    #endregion

    #region MonoBehaviour overrides
    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        
    }
    private void Awake()
    {
        
    }

    private void Start()
    {
        
    }
    #endregion

    #region Class implementation

    public void ShowDilogWallpaper()
    {
        Canvas main = GameObject.FindObjectOfType<Canvas>();
        Transform mainCointener = main.GetComponent<Transform>();

        panelWall = Instantiate(gameObjectControllerWallpaper) as GameObject;
        controller = panelWall.GetComponent<ControllerWallpaper>();
        controller.OnWallpaperBuy += HandlerControllerWallpaperBuy;
        panelWall.transform.SetParent(mainCointener, false);
        controller.ShowWallpaper();
    }

    private void HandlerControllerWallpaperBuy(ControllerWallpaper itemWallpaper, bool isError)
    {
        controller.OnWallpaperBuy -= HandlerControllerWallpaperBuy;
        if (isError)
        {
            Debug.Log("event controler: " + idWallpaper.ToString());
            if (OnWallpaperBuy != null)
                OnWallpaperBuy(this);
        }
        
    }

    public void VerifyUnlockandLockWallpaper()
    {
        Debug.Log("estas aqui");
        LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation();
        if (!GameItemsManager.isLockedWallpaper(idWallpaper))
        {
           /* Debug.Log("esta bloqueado");
            //Transform buttonSellCharacter = btnBuyProduct.transform;
            //buttonSellCharacter.gameObject.SetActive(false);

            bu .onClick.AddListener(() => PlayWithCharacter(this));
            imageUnlock.SetActive(true);
            btnBuyProduct.GetComponentInChildren<Text>().text = lm.GetString("button_text_play");
            return;*/
        }
        else
        {
            //controller.SetActive(false);
        }
    }

    #endregion
}
