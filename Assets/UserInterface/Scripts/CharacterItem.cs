using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Spine.Unity;

public class CharacterItem : MonoBehaviour, IStorePurchase {

	#region Class members
	[Header ("Setting Character")]
    public GameItemsManager.Character idCharacter;
	public SkeletonAnimation skeletonAnimation;
	public SkeletonRenderer skeletonRenderer;
	public SkeletonDataAsset skeletonDataAsset;
	public string skinName;
	public GameObject character;
	public Button btnBuyProduct;
    public Button buttonBuyWallpaper;
	public Text nameCharacter;
	public GameObject imageUnlock;

    [HideInInspector]
	public string descCharacter;
	public delegate void ItemPurchasedAction (CharacterItem itemPurchased);
	public event ItemPurchasedAction OnItemPurchased;
    #endregion

    #region Class accesors
    public bool isPurchased { set; get; }
	public string idStoreGooglePlay { get; set; }
	public bool isAvailableInStore { get; set; }


    //wallpaper
    public WallpaperItem wallpaperItem;
    #endregion

    #region MonoBehaviour overrides
    private void Awake () {
        btnBuyProduct.onClick.AddListener (() => BuyCharacter (this));
        buttonBuyWallpaper.onClick.AddListener(() => BuyWallpaper());
    }


    private void Start () {
        if (skeletonDataAsset != null){
            skeletonRenderer.skeletonDataAsset = skeletonDataAsset;
            skeletonRenderer.initialSkinName = skinName;
            skeletonRenderer.Initialize(false);
            skeletonAnimation.state.SetAnimation(0, "Idle", true);
        }
	}
	#endregion

	#region Super class overrides
	public override string ToString () {
		return base.ToString () + ": Personaje: " + idCharacter.ToString ();
	}
	#endregion

	#region Class implementation
	public void BuyCharacter (CharacterItem character) {
		if (OnItemPurchased != null)
			OnItemPurchased (character);
	}

	public void VerifyUnlockandLockCharacter () {
        LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation();
        if (!GameItemsManager.isLockedCharacter (idCharacter)) {
			Debug.Log ("esta bloqueado");
			//Transform buttonSellCharacter = btnBuyProduct.transform;
			//buttonSellCharacter.gameObject.SetActive(false);

			btnBuyProduct.onClick.AddListener (() => PlayWithCharacter (this));
			imageUnlock.SetActive (true);
            btnBuyProduct.GetComponentInChildren<Text> ().text = lm.GetString("button_text_play");
			return;
		}
		else {
			imageUnlock.SetActive (false);
		}
	}

	public void PlayWithCharacter (CharacterItem character) {
        GameItemsManager.SetLastChooseCharacter (character.idCharacter);
		SceneManager.LoadScene ("WorldScene");

	}


	public void UpdateTextTranslation () {
		LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation ();
		if (nameCharacter != null) {
			nameCharacter.text = lm.GetString (nameCharacter.text);
		}
		if (descCharacter != null) {
			descCharacter = lm.GetString (descCharacter);
		}
		if (btnBuyProduct != null) {
			btnBuyProduct.GetComponentInChildren<Text> ().text = lm.GetString ("text_btn_buy");
		}

	}

    private void BuyWallpaper()
    {
        wallpaperItem.ShowDilogWallpaper();
    }
    
    #endregion

}