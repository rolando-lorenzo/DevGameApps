using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallpaperItem : MonoBehaviour, IStorePurchase
{

    #region Class members
    //Event rate
    public delegate void EventWallpaperBuyPurchased(WallpaperItem itemWallpaper);
    public event EventWallpaperBuyPurchased OnWallpaperBuyPurchased;

    [HideInInspector]
    public GameItemsManager.Wallpaper idWallpaper;
    [HideInInspector]
    public string nameFileImage { get; set; }
    public string idStoreGooglePlay { get; set; }
    public bool isAvailableInStore { get; set; }
    public bool islocked { get; set; }
    [HideInInspector]
    public Sprite spriteWallpaper;

    #endregion

    #region MonoBehaviour overrides
    private void Awake()
    {
        
    }

    private void Start()
    {
        
    }
    #endregion

    #region Class implementation

    public void ShowDilogWallpaper(WallpaperItem item)
    {
        /*Canvas main = GameObject.FindObjectOfType<Canvas>();
        Transform mainCointener = main.GetComponent<Transform>();*/
        Debug.Log("open Modal");
        Debug.Log(idWallpaper);
        ControllerWallpaper.instance.objWallpaperItem = item;
        ControllerWallpaper.instance.ShowWallpaper();
    }

    public void VerifyUnlockandLockWallpaper()
    {
        Debug.Log("VerifyUnlockandLockWallpaper");
        if (!GameItemsManager.isLockedWallpaper(idWallpaper))
        {
            islocked = false;
        }
        else
        {
            islocked = true;
        }
    }

    #endregion
}
