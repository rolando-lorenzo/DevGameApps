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
    public delegate void EventWallpaperMessage(string title, string message, DialogMessage.typeMessage typeMessage);
    public event EventWallpaperMessage OnWallpaperMessageCharacter;

    [HideInInspector]
    public GameItemsManager.Wallpaper idWallpaper;
    [HideInInspector]
    public string nameFileImage { get; set; }
    public string idStoreGooglePlay { get; set; }
    public bool isAvailableInStore { get; set; }
    public bool islocked { get; set; }
    [HideInInspector]
    public Sprite spriteWallpaper;
    public GameObject dialogWallpaper;

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

        Debug.Log(idWallpaper.ToString());
        if (spriteWallpaper != null)
        {
            
            if (isAvailableInStore)
            {
                Debug.Log("dentro show");
                Canvas main = GameObject.FindObjectOfType<Canvas>();
                Transform mainCointener = main.GetComponent<Transform>();
                //Debug.Log("open Modal");
                //Debug.Log(idWallpaper);
                GameObject Parent = Instantiate(dialogWallpaper) as GameObject;
                ControllerWallpaper cwall = Parent.GetComponent<ControllerWallpaper>();
                cwall.objWallpaperItem = item;
                Parent.transform.SetParent(mainCointener, false);
                cwall.ShowWallpaper();
            }
            else
            {
                Debug.Log("dentro if4");
                if (OnWallpaperMessageCharacter != null)
                    OnWallpaperMessageCharacter("msg_store_title_popup", "msg_err_avalible_wallpaper", DialogMessage.typeMessage.WARNING);
            }

        }
        else
        {
            Debug.Log("dentro if 5");
            if (OnWallpaperMessageCharacter != null)
                OnWallpaperMessageCharacter("msg_store_title_popup", "msg_err_avalible_wallpaper", DialogMessage.typeMessage.WARNING);
        }
        
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
