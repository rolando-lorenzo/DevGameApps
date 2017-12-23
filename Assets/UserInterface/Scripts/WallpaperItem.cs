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
    public GameItemsManager.Wallpaper idWallpaper { get; set; }
    [HideInInspector]
    public string nameFileImage { get; set; }
    public string idStoreGooglePlay { get; set; }
    public bool isAvailableInStore { get; set; }
    public bool islocked { get; set; }
    public string priceInStore { get; set; }
    [HideInInspector]
    public Sprite spriteWallpaper { get; set; }

    #endregion

    #region MonoBehaviour overrides
    private void Awake()
    {
        
    }

    private void Start()
    {
       // StartCoroutine(VerifyEvents());
    }

    
    #endregion

    #region Class implementation
    private IEnumerator VerifyEvents()
    {
        yield return new WaitForSeconds(1.5f);
        Debug.Log("wall verify events");
        Debug.Log(OnWallpaperBuyPurchased);
        Debug.Log(OnWallpaperMessageCharacter);
    }

    public void ShowDilogWallpaper(WallpaperItem item)
    {
         
        Debug.Log(idWallpaper.ToString());
        if (spriteWallpaper != null)
        {
            
            if (isAvailableInStore)
            {
                Debug.Log("this is a event");
                Debug.Log(OnWallpaperBuyPurchased);
                if (OnWallpaperBuyPurchased != null)
                    OnWallpaperBuyPurchased(this);
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
