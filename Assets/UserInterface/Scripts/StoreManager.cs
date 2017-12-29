using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PankioAssets;
using UnityEngine.SceneManagement;
using Spine.Unity;
using System;
using UnityEngine.Purchasing;

public class ProductStore {

	public Sprite imgProducto;
	public string nameProduct;
	public float priceProduct;
	public string descProduct;
	public string idInStoreGooglePlay;
	public bool isAvailableInStore;
}

[System.Serializable]
public class PackagesStore : ProductStore {
	public GameItemsManager.StoreProduct idProduct;
}

[System.Serializable]
public class PowerUpsStore : ProductStore {
	public GameItemsManager.StorePower idProductPower;
}

[System.Serializable]
public class UpgradesStore : ProductStore {
	public GameItemsManager.StorePower idProductPower;
	public List<string> levelsUpgradesIdGooglePlay;
}

[System.Serializable]
public class WallpaperStore
{
    public GameItemsManager.Wallpaper idWallpaperProduct;
    public Sprite wallpaperCharacter;
    public string idStoreGooglePlay;
    public bool isAvailableInStore;
}

[System.Serializable]
public class CharacterStore {
    public GameItemsManager.Character idCharacter;
	public SkeletonDataAsset skeletonDataAsset;
	public string skinName;
	public Sprite imgBuyCharacter;
	public string nameCharacter;
	public string descriptionCharacter;
	public string idInStoreGooglePlay;
	public bool isAvailableInStore;
    public bool isLockedCharacter;
    public WallpaperStore wallpaper;
    
}

public class StoreManager : MonoBehaviour, IStoreListener
{

    #region Class members
    public GameItemsManager.GameMode gameMode;
	public GameObject btnProductPackage;
	public GameObject btnProductPowerup;
	public GameObject btnProductUpgrade;
	public Button btnCharacters;
	public Button buttonPowerups;
	public Button btnInviteFriends;
	public Button btnBackScene;
	public GameObject charactersPanel;
	public GameObject packagesPanel;
	public GameObject backgroundCharacters;
	public GameObject backgroundPackages;
	public GameObject headerProducts;
	public Transform containerPaquetes;
	public GameObject popupBuy;
	public GameObject dialogMessage;
    public GameObject dialogMessageWallpaper;
    public Sprite imgDialogMessageINFO;
	public Sprite imgDialogMessageWARN;
	public Sprite imgDialogMessageERR;
	public Transform mainContainer;
	public Text charcaterDescription;
	public Text charcaterName;
	public List<PackagesStore> packagesStore;
	public List<PowerUpsStore> powerUpsStore;
	public List<UpgradesStore> upgradesStore;
	public List<CharacterStore> characters;

	public static StoreManager instance;
	private List<ProductItem> btnsSlide;
	private List<IStorePurchase> itemsAvailableInStore;
	private List<IStorePurchase> itemsNotAvailableStore;
	private CharacterItem[] charactersTemplate;
	private InfiniteScroll infiniteScrollCharacters;

	private const int CHARACTER_LOCK = 1;
	private const int CHARACTER_UNLOCK = 2;

	private GUIAnim backgroundCharactersAnim;
	private GUIAnim backgroundPackagesAnim;

    //bool of state WallpaperDilalog and Panel
    private bool stateDialogWallpaper { get; set; }
    private bool statePanelCharacter { get; set; }
    //event wallpaper 
    public delegate void EventStoreWallpaperUnlockButton();
    public static event EventStoreWallpaperUnlockButton OnStoreWallpaperUnlockButton;
    public delegate void EventWallpaperIsSelected(WallpaperItem wallpaperItem, bool isInitializedIAP);
    public static event EventWallpaperIsSelected OnWallpaperIsSelected;
    //IAP Variables

    private IStorePurchase productItemBuy { get; set; }

    private static IStoreController storeController;
    private static IExtensionProvider storeExtensionProvider;

    public static string kProductIDConsumable = "consumable";
    public static string kProductIDNonConsumable = "nonconsumable";
    public static string kProductIDSubscription = "subscription";

    List<Array> listProduct;
    Product product { set; get; }
    // Apple App Store-specific product identifier for the subscription product.
    private static string kProductNameAppleSubscription = "com.unity3d.subscription.new";
    // Google Play Store-specific product identifier subscription product.
    private static string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";
    #endregion

    #region Class accesors
    #endregion

    #region MonoBehaviour overrides
    void OnEnable () {
		//Eventos internos
		infiniteScrollCharacters = charactersPanel.GetComponentInChildren<InfiniteScroll> ();
		infiniteScrollCharacters.OnItemChanged += HandleCurrentCharacter;
		foreach (CharacterItem ch in charactersTemplate) {
			ch.OnItemPurchased += HandleCharacterToWillPurchase;
            ch.wallpaperItem.OnWallpaperBuyPurchased += HandleWallpaperPurchase;
            ch.wallpaperItem.OnWallpaperMessageCharacter += HandleWallpaperToMessage;
        }
		foreach (ProductItem pi in btnsSlide) {
			if (pi is ProductPackagesItem) {
				((ProductPackagesItem) pi).OnProductPackageItemPurchased += HandleProductToWillPurchased;
			}

			if (pi is ProductPowerupItem) {
				((ProductPowerupItem) pi).OnProductPowerUpItemPurchased += HandleProductToWillPurchased;
			}

			if (pi is ProductUpgradeItem) {
				((ProductUpgradeItem) pi).OnProductUpgradeItemPurchased += HandleProductToWillPurchased;
				((ProductUpgradeItem) pi).OnProductUpgradeBuyLimitMax += HandleProductLimitBuyReached;
			}
		}

		//Eventos del Service IAP
		/*IAPManager.OnIAPInitialized += HandleIncializationIAP;
		IAPManager.OnIAPMessageProgress += HandleIAPEvents;
		IAPManager.OnIAPSuccessPurchasedInStore += HandleSuccessPurchasedInStore;*/

        ControllerWallpaper.OnWallpaperBuy += HandleWallpaperToWillPurchase;
        ControllerWallpaper.OnWallpaperMessage += HandleWallpaperToMessage;
    }

	void Awake () {
        statePanelCharacter = false;
        stateDialogWallpaper = false;
		instance = this;
		btnCharacters.onClick.AddListener (ShowCharactersPanel);
		btnInviteFriends.onClick.AddListener (InviteFriends);
		btnBackScene.onClick.AddListener (GoMainMenu);
		buttonPowerups.onClick.AddListener (ShowPowerupsPanel);
		charactersTemplate = charactersPanel.GetComponentsInChildren<CharacterItem> ();


		if (enabled) {
			GUIAnimSystem.Instance.m_AutoAnimation = false;
		}

		btnsSlide = new List<ProductItem> ();
		itemsAvailableInStore = new List<IStorePurchase> ();
		itemsNotAvailableStore = new List<IStorePurchase> ();
        listProduct = new List<Array>();

        PopulatePackages(packagesStore);
        PopulatePowerUps(powerUpsStore);
        PopulateUpgrades(upgradesStore);
        PopulateCharacters();
       

        //If game mode is equals to DEBUG not initializes IAP api
        if(gameMode == GameItemsManager.GameMode.RELEASE){
            InitializePurchasing(itemsAvailableInStore);
        }
		

		backgroundCharactersAnim = backgroundCharacters.GetComponent<GUIAnim> ();
		backgroundPackagesAnim = backgroundPackages.GetComponent<GUIAnim> ();

		backgroundCharactersAnim.m_MoveIn.Enable = true;
		backgroundCharactersAnim.m_MoveIn.MoveFrom = GUIAnim.ePosMove.RightScreenEdge;
		backgroundCharactersAnim.m_MoveOut.Enable = true;
		backgroundCharactersAnim.m_MoveOut.MoveTo = GUIAnim.ePosMove.RightScreenEdge;

		backgroundPackagesAnim.m_MoveIn.Enable = true;
		backgroundPackagesAnim.m_MoveIn.MoveFrom = GUIAnim.ePosMove.LeftScreenEdge;
		backgroundPackagesAnim.m_MoveOut.Enable = true;
		backgroundPackagesAnim.m_MoveOut.MoveTo = GUIAnim.ePosMove.LeftScreenEdge;
	}

    void Start()
    {
        AnimPanelIn(backgroundPackagesAnim);
        MenuUtils.CanvasSortingOrderShow();
    }

    void OnDisable () {
		if (infiniteScrollCharacters != null) {
			infiniteScrollCharacters.OnItemChanged -= HandleCurrentCharacter;
		}

		foreach (CharacterItem ch in charactersTemplate) {
			if (ch != null)
				ch.OnItemPurchased -= HandleCharacterToWillPurchase;
                ch.wallpaperItem.OnWallpaperMessageCharacter += HandleWallpaperToMessage;
                ch.wallpaperItem.OnWallpaperBuyPurchased += HandleWallpaperPurchase;
        }

		foreach (ProductItem pi in btnsSlide) {
			if (pi is ProductPackagesItem) {
				((ProductPackagesItem) pi).OnProductPackageItemPurchased -= HandleProductToWillPurchased;
			}

			if (pi is ProductPowerupItem) {
				((ProductPowerupItem) pi).OnProductPowerUpItemPurchased -= HandleProductToWillPurchased;
			}

			if (pi is ProductUpgradeItem) {
				((ProductUpgradeItem) pi).OnProductUpgradeItemPurchased -= HandleProductToWillPurchased;
				((ProductUpgradeItem) pi).OnProductUpgradeBuyLimitMax -= HandleProductLimitBuyReached;
			}
		}

		//Eventos del Service IAP
		/*IAPManager.OnIAPInitialized -= HandleIncializationIAP;
		IAPManager.OnIAPMessageProgress -= HandleIAPEvents;
		IAPManager.OnIAPSuccessPurchasedInStore -= HandleSuccessPurchasedInStore;*/

        ControllerWallpaper.OnWallpaperBuy -= HandleWallpaperToWillPurchase;
        ControllerWallpaper.OnWallpaperMessage -= HandleWallpaperToMessage;
    }
    #endregion

    #region Super class overrides
    #endregion

    #region Class implementation
    private string GetPriceProductStore( string idStoreProduct)
    {
        string price = "";
        foreach (string[] item in listProduct)
        {
            Debug.Log("IN LIST: " + item[0]+" - "+ idStoreProduct);
            if (string.Equals(item[0], idStoreProduct, StringComparison.InvariantCulture))
            {
                return item[1];
            }
        }
        
        return price;
    }

    private List<Array> GetPriceProductUpgradeStore(List<string> idStoreProducts)
    {
        List<Array> price = new List<Array>();
        foreach (string[] item in listProduct)
        {
            foreach(string idStoreUpgrade in idStoreProducts)
            {                
                if (string.Equals(item[0], idStoreUpgrade, StringComparison.InvariantCulture))
                {
                    Debug.Log("IN LIST Upgrade: " + item[0] + " - " + idStoreUpgrade);
                    price.Add(item);
                }
            }            
        }

        return price;
    }

    private void ReplacePricePopulatePackages(List<PackagesStore> products)
    {
        //replace price
        Component[] component = GameObject.Find("ContentPanelPaquetes").GetComponentsInChildren<ProductPackagesItem>();
        foreach(ProductPackagesItem itempack in component)
        {
              string id = itempack.idStoreGooglePlay;

            itempack.priceProduct.text = GetPriceProductStore(id);
        }
    }

    private void ReplacePricePopulateUpgrades(List<UpgradesStore> products)
    {
        Component[] component = GameObject.Find("ContentPanelPaquetes").GetComponentsInChildren<ProductUpgradeItem>();
        foreach (ProductUpgradeItem itemupgrade in component)
        {
            List<string> listUpgrade = new List<string>();
            listUpgrade = itemupgrade.levelsUpgradesIdGooglePlay;
            itemupgrade.listPrice = GetPriceProductUpgradeStore(listUpgrade);
            itemupgrade.UpdatePriceAndProgress();
        }
    }

    /// <summary>
    /// Populates the packages panel.
    /// </summary>
    /// <param name="products">Products.</param>
    private void PopulatePackages (List<PackagesStore> products) {

		LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation ();

		//creates header of current elemnts
		BuildHeaderProducts (lm.GetString ("text_header_product_packages"));

		//creates body of products
		foreach (var product in products) {
			GameObject nuevoProducto = Instantiate (btnProductPackage) as GameObject;
			ProductPackagesItem ProductItem = nuevoProducto.GetComponent<ProductPackagesItem> ();
			ProductItem.idProductItem = product.idProduct;
			ProductItem.imgProduct.sprite = product.imgProducto;
			ProductItem.nameProduct.text = product.nameProduct;
			ProductItem.priceProduct.text = MenuUtils.FormatPriceProducts (product.priceProduct);
			ProductItem.valCurrency = product.priceProduct;
			ProductItem.descProduct.text = product.descProduct;

            #if UNITY_IOS
            ProductItem.idStoreGooglePlay = "com.EstacionPi.BJWTFoundation."+product.idInStoreGooglePlay;
            #else
            ProductItem.idStoreGooglePlay = product.idInStoreGooglePlay;
            #endif

            ProductItem.isAvailableInStore = product.isAvailableInStore;
            nuevoProducto.name = product.idInStoreGooglePlay;
            nuevoProducto.transform.SetParent (containerPaquetes, false);

			if (product.isAvailableInStore) {
				// it could be purchased only in Online Store
				itemsAvailableInStore.Add (ProductItem);

			}
			else {
				// it could be purchased only with items like Hulleas, yinyangs ....
				itemsNotAvailableStore.Add (ProductItem);
			}
			ProductItem.UpdateTextTranslation ();
			btnsSlide.Add (ProductItem);
		}
	}

	/// <summary>
	/// Populates the characters panel.
	/// </summary>
	public void PopulateCharacters () {

		LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation ();

		foreach (CharacterStore character in characters) {
			foreach (CharacterItem characterTemplate in charactersTemplate) {
				if (character.idCharacter == characterTemplate.idCharacter) {
					characterTemplate.nameCharacter.text = character.nameCharacter;
					characterTemplate.btnBuyProduct.image.overrideSprite = character.imgBuyCharacter;
					characterTemplate.descCharacter = character.descriptionCharacter;

                    if(!character.isLockedCharacter){
                        
                        GameItemsManager.SetUnlockCharacter(characterTemplate.idCharacter);
                    }
                    #if UNITY_IOS
                    characterTemplate.idStoreGooglePlay = "com.EstacionPi.BJWTFoundation."+character.idInStoreGooglePlay;
                    #else
                    characterTemplate.idStoreGooglePlay = character.idInStoreGooglePlay;
                    #endif

                    #if UNITY_IOS
                    characterTemplate.wallpaperItem.idStoreGooglePlay = "com.EstacionPi.BJWTFoundation."+character.wallpaper.idStoreGooglePlay;
                    #else
                    characterTemplate.wallpaperItem.idStoreGooglePlay = character.wallpaper.idStoreGooglePlay;
                    #endif

                    characterTemplate.name = character.idInStoreGooglePlay;
                    characterTemplate.isAvailableInStore = character.isAvailableInStore;

					if (character.skeletonDataAsset != null)
						characterTemplate.skeletonDataAsset = character.skeletonDataAsset;
					if (character.skinName != null)
						characterTemplate.skinName = character.skinName;

					if (character.isAvailableInStore) {
						// it could be purchased only in Online Store
						itemsAvailableInStore.Add (characterTemplate);
					}
					else {
						// it could be purchased only with items like Hulleas, yinyangs ....
						itemsNotAvailableStore.Add (characterTemplate);
					}

                    characterTemplate.wallpaperItem.idWallpaper = character.wallpaper.idWallpaperProduct;
                    characterTemplate.wallpaperItem.spriteWallpaper = character.wallpaper.wallpaperCharacter;
                    characterTemplate.wallpaperItem.idStoreGooglePlay = character.wallpaper.idStoreGooglePlay;
                    characterTemplate.wallpaperItem.nameFileImage = character.nameCharacter + ".png";
                    characterTemplate.wallpaperItem.isAvailableInStore = character.wallpaper.isAvailableInStore;


                    if (character.wallpaper.isAvailableInStore)
                    {
                        // it could be purchased only in Online Store
                        itemsAvailableInStore.Add(characterTemplate.wallpaperItem);
                    }
                    else
                    {
                        // it could be purchased only with items like Hulleas, yinyangs ....
                        itemsNotAvailableStore.Add(characterTemplate.wallpaperItem);
                    }

                    characterTemplate.wallpaperItem.VerifyUnlockandLockWallpaper();

					characterTemplate.UpdateTextTranslation ();       //update current translations of character's elements  
					characterTemplate.VerifyUnlockandLockCharacter (); //To image locked and unlocked
				}
			}
		}
		if (characters.Count > 0) {
            string idStore = "";
            #if UNITY_IOS
               idStore = "com.EstacionPi.BJWTFoundation."+characters[0].idInStoreGooglePlay;
            #else
                idStore = characters[0].idInStoreGooglePlay;
            #endif

            if (IsInitialized())
            {
                //charcaterPriceStore.text = GetPriceProductStore(idStore);
            }

            charcaterDescription.text = lm.GetString (characters[0].descriptionCharacter);
			charcaterName.text = lm.GetString (characters[0].nameCharacter);
		}

	}

	/// <summary>
	/// Populates the power ups panel.
	/// </summary>
	/// <param name="products">Products.</param>
	private void PopulatePowerUps (List<PowerUpsStore> products) {

		LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation ();

		//creates header of current elemnts
		BuildHeaderProducts (lm.GetString ("text_header_product_powerups"));

		//creates body of products
		foreach (var product in products) {
			GameObject nuevoProducto = Instantiate (btnProductPowerup) as GameObject;
			ProductPowerupItem ProductItem = nuevoProducto.GetComponent<ProductPowerupItem> ();
			ProductItem.idProductPower = product.idProductPower;
			ProductItem.imgProduct.sprite = product.imgProducto;
			ProductItem.nameProduct.text = product.nameProduct;
			ProductItem.priceProduct.text = MenuUtils.FormatPawprintsProducts (product.priceProduct);
			ProductItem.valCurrency = product.priceProduct;
			ProductItem.descProduct.text = product.descProduct;
			

            #if UNITY_IOS
            ProductItem.idStoreGooglePlay = "com.EstacionPi.BJWTFoundation."+product.idInStoreGooglePlay;
            #else
            ProductItem.idStoreGooglePlay = product.idInStoreGooglePlay;
            #endif

			ProductItem.isAvailableInStore = product.isAvailableInStore;
            //nuevoProducto.name = product.idInStoreGooglePlay;
			nuevoProducto.transform.SetParent (containerPaquetes, false);

			if (product.isAvailableInStore) {
				// it could be purchased only in Online Store
				itemsAvailableInStore.Add (ProductItem);
			}
			else {
				// it could be purchased only with items like Hulleas, yinyangs ....
				itemsNotAvailableStore.Add (ProductItem);
			}
			ProductItem.UpdateTextTranslation ();
			btnsSlide.Add (ProductItem);
		}
	}

	/// <summary>
	/// Populates the upgrades panel.
	/// </summary>
	/// <param name="products">Products.</param>
	private void PopulateUpgrades (List<UpgradesStore> products) {

		LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation ();

		//creates header of current elemnts
		BuildHeaderProducts (lm.GetString ("text_header_product_upgrades"));

		//creates body of products
		foreach (var product in products) {
			GameObject nuevoProducto = Instantiate (btnProductUpgrade) as GameObject;
			ProductUpgradeItem ProductItem = nuevoProducto.GetComponent<ProductUpgradeItem> ();

			ProductItem.idProductPower = product.idProductPower;
			ProductItem.imgProduct.sprite = product.imgProducto;
			ProductItem.nameProduct.text = product.nameProduct;
			ProductItem.priceProduct.text = MenuUtils.FormatPriceProducts (product.priceProduct);
			ProductItem.valCurrency = product.priceProduct;
			ProductItem.descProduct.text = product.descProduct;
			ProductItem.idStoreGooglePlay = product.idInStoreGooglePlay;
			ProductItem.isAvailableInStore = product.isAvailableInStore;


            #if UNITY_IOS
                List<string> listAux = new List<string>();
                foreach(string valIds in product.levelsUpgradesIdGooglePlay){
                    listAux.Add("com.EstacionPi.BJWTFoundation." +valIds);
                }
                ProductItem.levelsUpgradesIdGooglePlay = listAux;
            #else
                ProductItem.levelsUpgradesIdGooglePlay = product.levelsUpgradesIdGooglePlay;
            #endif

			nuevoProducto.transform.SetParent (containerPaquetes, false);
            ProductItem.name = ProductItem.levelsUpgradesIdGooglePlay[0].Replace("1","");

            ProductItem.SetChildId (product.idProductPower);
			if (product.isAvailableInStore) {
				// it could be purchased only in Online Store
				itemsAvailableInStore.Add (ProductItem);
			}
			else {
				// it could be purchased only with items like Hulleas, yinyangs ....
				itemsNotAvailableStore.Add (ProductItem);
			}
			ProductItem.UpdateTextTranslation ();
			btnsSlide.Add (ProductItem);
		}
	}


	private void BuildHeaderProducts (string text) {

		GameObject header = Instantiate (headerProducts) as GameObject;
		header.GetComponentInChildren<Text> ().text = text;
		header.transform.SetParent (containerPaquetes, false);
	}

	/// <summary>
	/// Closes all buttons.
	/// </summary>
	/// <param name="btnCurrent">Button current.</param>
	public void CloseAllButtons (ProductItem btnCurrent) {
		foreach (ProductItem btn in btnsSlide) {
			if (btn.isOpen && btn != btnCurrent) {
				btn.Close ();
			}
		}

	}

	/// <summary>
	/// Shows the characters panel.
	/// </summary>
	public void ShowCharactersPanel () {
        statePanelCharacter = true;
		float xposition = backgroundCharactersAnim.transform.localPosition.x;
        AnimPanelOut(backgroundPackagesAnim, btnCharacters.gameObject);
        StartCoroutine (WaitForIn (backgroundCharactersAnim, buttonPowerups.gameObject));
        StartCoroutine(WaitCharacterShow());
	}

    /// <summary>
    /// Shows the powerups panel.
    /// </summary>
    public void ShowPowerupsPanel () {
        statePanelCharacter = false;
        float xposition = backgroundPackagesAnim.transform.localPosition.x;
        AnimPanelOut (backgroundCharactersAnim, buttonPowerups.gameObject);
        StartCoroutine (WaitForIn (backgroundPackagesAnim,btnCharacters.gameObject));
        StartCoroutine(WaitCharacterHiden());
    }

    private IEnumerator WaitCharacterHiden()
    {
        yield return new WaitForSeconds(.5f);
        Debug.Log("fhide Character");
        MenuUtils.CanvasSortingOrderShow();
    }

    private IEnumerator WaitCharacterShow()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("show Character");
        MenuUtils.CanvasSortingOrderHiden();
    }

	public void GoMainMenu () {
		SceneManager.LoadScene ("MainScene");
	}

	public void InviteFriends () {
		Debug.Log ("Inivitando Amigos !!");
        FacebookDelegate.Instance.ShareScreenFacebook();
	}

	/// <summary>
	/// Handles OnChangeCharcater event  on infinite-scroll.
	/// </summary>
	/// <param name="previousItem">Previous item.</param>
	/// <param name="currentItem">Current item.</param>
	/// <param name="itemIndex">Item index.</param>
	private void HandleCurrentCharacter (ScrollItem previousItem, ScrollItem currentItem, int itemIndex) {
		if (currentItem != null) {
			CharacterItem c = currentItem.gameObject.GetComponentInChildren<CharacterItem> ();
            string idStore = "";
            idStore = c.idStoreGooglePlay;
            if (IsInitialized())
            {
                //charcaterPriceStore.text = GetPriceProductStore(idStore);
            }
            charcaterDescription.text = c.descCharacter;
			charcaterName.text = c.nameCharacter.text;
		}
	}
    
	/// <summary>
	/// Handles the character success purchased.
	/// </summary>
	/// <param name="chItem">Character item.</param>
	private void HandleCharacterToWillPurchase (CharacterItem chItem) {
		Debug.Log ("Intentando comprar... : " + chItem.nameCharacter.text);
		
		//If character was purchased avoid buy again
        if (!GameItemsManager.isLockedCharacter (chItem.idCharacter)) {
			BuildDialogMessage ("msg_store_title_popup", "msg_err_purchase_again", DialogMessage.typeMessage.ERROR, true);
			return;
		}
		//proceeds to buy on store online
		if (chItem.isAvailableInStore) {
			Debug.Log ("Comprando en IAP...");

            if (gameMode == GameItemsManager.GameMode.RELEASE){
                BuyInStore(chItem);
            } 
            else { //Simulates success purchase, DEBUG mode
                Debug.Log("Modo debug activado, se simula compra exitosa online.");
                HandleSuccessPurchasedInStore("msg_info_success_purchased",chItem);
            }
			
		}
	}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="wallItem">WallpaperItem</param>
    private void HandleWallpaperToWillPurchase (WallpaperItem wallItem)
    {
        Debug.Log("dentro Wallhandler");
        Debug.Log(wallItem.idWallpaper.ToString());

        //If wallpaper was purchased avoid buy again
        if (!GameItemsManager.isLockedWallpaper(wallItem.idWallpaper))
        {
            BuildDialogMessage("msg_store_title_popup", "msg_err_purchase_again", DialogMessage.typeMessage.ERROR, true);
            return;
        }

        //proceeds to buy on store online
        if (wallItem.isAvailableInStore)
        {
            Debug.Log("Comprando en IAP...");
            stateDialogWallpaper = true;

            if (gameMode == GameItemsManager.GameMode.RELEASE)
            {
                BuyInStore(wallItem);
            }
            else
            { //Simulates success purchase, DEBUG mode
                Debug.Log("Modo debug activado, se simula compra exitosa online.");
                HandleSuccessPurchasedInStore("msg_info_success_purchased", wallItem);
            }
        }
    }

    /// <summary>
    /// Checks the num items available to play.
    /// </summary>
    /// <returns><c>true</c>, if items available to play was checked, <c>false</c> otherwise.</returns>
    /// <param name="numHuellas">Number huellas.</param>
    /// <param name="numYingYangs">Number ying yangs.</param>
    private bool checkItemsAvailableToPlay (int numHuellas, int numYingYangs) {
		if (numHuellas <= GameItemsManager.GetValueById (GameItemsManager.Item.NumPawprints)
			&& numYingYangs <= GameItemsManager.GetValueById (GameItemsManager.Item.NumYinYangs)) {
			return true;
		}
		else {
			return false;
		}
	}

	/// <summary>
	/// Handles the product success purchased. (Packages, PowerUps, Upgrades)
	/// </summary>
	/// <param name="pi">Product Item</param>
	private void HandleProductToWillPurchased (ProductItem pi) {
		Debug.Log ("---- Manager event success purchased ---");

		//Va a la tienda de Google play
		if (pi is ProductPackagesItem) {
			ProductPackagesItem currentBought = ((ProductPackagesItem) pi);
            Debug.Log ("[StoreManager] Iniciando compra... ID: " + currentBought.idStoreGooglePlay + " " + currentBought.nameProduct.text);

			if (currentBought.isAvailableInStore) {
                if(gameMode == GameItemsManager.GameMode.RELEASE){
                    BuyInStore(pi);
                } 
                else { // If game configured DEBUG mode simulates successul purchase 
                    Debug.Log("Modo debug activado, se simula compra exitosa online.");
                    HandleSuccessPurchasedInStore("msg_info_success_purchased",pi);
                }
				
			}
		}


		//Utiliza las Huellas disponibles
		if (pi is ProductPowerupItem) {
			ProductPowerupItem currentBought = ((ProductPowerupItem) pi);
			Debug.Log ("[StoreManager] Powerup comprado" + currentBought.idProductPower + " " + currentBought.nameProduct.text +" "+currentBought.numProductsToBuy);

			int globalCostPowerUp = ((int) currentBought.valCurrency * currentBought.numProductsToBuy);

			if (globalCostPowerUp <= GameItemsManager.GetValueById (GameItemsManager.Item.NumPawprints)) {

				//subtracts pawprints
				substractElements (globalCostPowerUp, 0, 0, 0);
				Debug.Log ("Compra exitosa !! Ahora ya cuentas con el PowerUp " + currentBought.ToString ());
                //Save new PowerUp
                Debug.Log(currentBought.idProductPower);
                GameItemsManager.AddPowerCount (currentBought.idProductPower, currentBought.numProductsToBuy);
                Debug.Log ("Se ha guardado "+currentBought.numProductsToBuy+" nuevos power ups .. " + currentBought.idProductPower.ToString () + ", Ahora ya cuentas con " + GameItemsManager.GetPowerCount (currentBought.idProductPower));

				BuildDialogMessage ("msg_store_title_popup", "msg_info_success_purchased", DialogMessage.typeMessage.INFO, false);

			}
			else {
				BuildDialogMessage ("msg_store_title_popup", "msg_err_insuficient_resources", DialogMessage.typeMessage.ERROR, false);
			}
		}

		//Va a la tienda de Google play
		if (pi is ProductUpgradeItem) {
			ProductUpgradeItem currentBought = ((ProductUpgradeItem) pi);
            Debug.Log ("[StoreManager] Upgrade a comprar... ID: " + currentBought.idStoreGooglePlay);
			if (currentBought.isAvailableInStore) {
                if (gameMode == GameItemsManager.GameMode.RELEASE)
                {
                    BuyInStore(pi);
                }
                else { // If game configured DEBUG mode simulates successul purchase 
                    Debug.Log("Modo debug activado, se simula compra exitosa online.");
                    HandleSuccessPurchasedInStore("msg_info_success_purchased", pi);
                }
			}
		}

	}

	/// <summary>
	/// Handles the incialization IA.
	/// </summary>
	/// <param name="isError">If set to <c>true</c> is error.</param>
	private void HandleIncializationIAP (bool isError) {
		if (isError) {
			Debug.Log ("[StoreManager] Ocurrio un error al inicializar api de IAP");
			BuildDialogMessage ("msg_store_title_popup", "msg_err_init_api_iap", DialogMessage.typeMessage.ERROR,false);
        }
        else
        {
            Debug.Log("Handle IAP Inizialization LIST");
            foreach (var item in storeController.products.all)
            {
                if (item.availableToPurchase)
                {
                    string[] productStore = {
                        item.definition.storeSpecificId,
                        item.metadata.localizedPriceString,
                        item.definition.id,
                        item.metadata.localizedPrice.ToString(),
                        item.metadata.localizedTitle,
                        item.metadata.localizedDescription,
                        item.metadata.isoCurrencyCode,
                        item.transactionID,
                        item.receipt
                    };

                    listProduct.Add(productStore);

                    //Debug.Log(string.Join(" - ", productStore));
                }
            }
            ReplacePricePopulatePackages(packagesStore);
            ReplacePricePopulateUpgrades(upgradesStore);
        }

        
	}

	/// <summary>
	/// Handles the IAP events showing dialog message popup.
	/// </summary>
	/// <param name="messageResult">Message result.</param>
	/// <param name="isError">If set to <c>true</c> is error.</param>
	private void HandleIAPEvents (string messageResult, bool isError) {
		string titlePopup = "msg_store_title_popup";
        Debug.Log("Handler IAP Info");
        if (isError) {
            Debug.Log("stateDialogWallpaper" + stateDialogWallpaper);
            if(stateDialogWallpaper == true)
            {
                BuildDialogMessageWallpaper(titlePopup, messageResult, DialogMessage.typeMessage.ERROR);
                stateDialogWallpaper = false;
            }
            else{
                Debug.Log("statePanelCharacter"+statePanelCharacter);
                if (statePanelCharacter == true)
                {
                    BuildDialogMessage(titlePopup, messageResult, DialogMessage.typeMessage.ERROR, true);
                }
                else
                {
                    BuildDialogMessage(titlePopup, messageResult, DialogMessage.typeMessage.ERROR, false);
                }
            }
           
		}
		else {
			BuildDialogMessage (titlePopup, messageResult, DialogMessage.typeMessage.INFO, backgroundCharacters.activeSelf);
		}

	}

	/// <summary>
	/// Handles the purchased in store online (ProductItem or CharacterItem).
	/// </summary>
	/// <param name="messageResult">Message result.</param>
	/// <param name="item">Item.</param>
	private void HandleSuccessPurchasedInStore (string messageResult, IStorePurchase ite) {
		Debug.Log ("Menssage from store: " + messageResult);

		//If was successful purchased a product from IAP
		if (ite is ProductItem) {
			ProductItem currentProduct = ((ProductItem) ite);

			//If it's some Package.
			if (currentProduct is ProductPackagesItem) {
				ProductPackagesItem currentPck = ((ProductPackagesItem) currentProduct);
				Debug.Log ("Paquete pagado-> " + currentPck.ToString ());

				switch (currentPck.idProductItem) {

					//if was purchased package 1
					case GameItemsManager.StoreProduct.Package1:
						//200 huellas + 2 multiplicadores + 2 chuletas
						addElements (200, 2, 2, 0);
						break;

					//if was purchased package 2
					case GameItemsManager.StoreProduct.Package2:
						//500 huellas + 5 multiplicadores + 3 chuletas + 2 magnetos
						addElements (500, 5, 3, 2);
						break;

					//if was purchased package 3
					case GameItemsManager.StoreProduct.Package3:
						//1000 huellas + 10 multiplicadores + 5 chuletas + 5 magnetos
						addElements (1000, 10, 5, 5);
						break;

					//if was purchased package 4
					case GameItemsManager.StoreProduct.Package4:
						//15,000 huellas + 10 multiplicadores + 10 chuletas + 10 magnetos
						addElements (15000, 10, 10, 10);
						break;
				}
			}

			//If it's some Upgrade.
			if (currentProduct is ProductUpgradeItem) {
				ProductUpgradeItem currentUpg = ((ProductUpgradeItem) currentProduct);
				Debug.Log ("Upgrade pagado-> " + currentUpg);
				//increments in 1 after purchased current upgrade.
				GameItemsManager.AddUpgradeValue (currentUpg.idProductPower, 1);
				//Debug.Log ("Se guarda y actualiza el valor del upgrade " + currentUpg.idProductItem.ToString () + ", ahora tiene un valor: " + GameItemsManager.GetPowerUpgradeLevel (currentUpg.idProductItem));
				currentUpg.UpdatePriceAndProgress ();

			}

			BuildPopupPurchasedProduct (currentProduct);
		}

		//If was successful purchased a character from IAP
		if (ite is CharacterItem) {
			CharacterItem currentCharacter = ((CharacterItem) ite);

			//Unlock character
            GameItemsManager.SetUnlockCharacter (currentCharacter.idCharacter);
            if (!GameItemsManager.isLockedCharacter (currentCharacter.idCharacter)) {
                Debug.Log ("Se ha desbloqueado a " + currentCharacter.idCharacter.ToString ());
				currentCharacter.VerifyUnlockandLockCharacter ();
			}
			else {
				BuildDialogMessage ("msg_store_title_popup", "msg_err_fails_unlock_character", DialogMessage.typeMessage.ERROR,true);
                Debug.Log ("No se pudo desbloquear a " + currentCharacter.idCharacter.ToString ());
			}
			BuildPopupPurchasedCharacter (currentCharacter);
		}

        //If was successful purchased a wallpaper from IAP
        if (ite is WallpaperItem)
        {
            WallpaperItem currentWallpaper = ((WallpaperItem)ite);

            //Unlock character
            GameItemsManager.SetUnlockWallpaper(currentWallpaper.idWallpaper);
            if (!GameItemsManager.isLockedWallpaper(currentWallpaper.idWallpaper))
            {
                Debug.Log("Se ha desbloqueado a " + currentWallpaper.idWallpaper.ToString());
                currentWallpaper.VerifyUnlockandLockWallpaper();
                if (OnStoreWallpaperUnlockButton != null)
                    OnStoreWallpaperUnlockButton();
            }
            else
            {
                //cambiar textos
                BuildDialogMessageWallpaper("msg_store_title_popup", "msg_err_fails_unlock_wallpaper", DialogMessage.typeMessage.ERROR);
                Debug.Log("No se pudo desbloquear a " + currentWallpaper.idWallpaper.ToString());
            }
            BuildPopupPurchasedWallpaper(currentWallpaper);
        }


    }

	/// <summary>
	/// Handles the product limit buy reached.
	/// </summary>
	/// <param name="msg">Message.</param>
	private void HandleProductLimitBuyReached (string msg) {
		BuildDialogMessage ("msg_store_title_popup", msg, DialogMessage.typeMessage.ERROR,false);
	}

    private CharacterStore SearchImgCharacter (GameItemsManager.Character id) {
		CharacterStore characterAux = null;
		foreach (CharacterStore character in characters) {
			if (character.idCharacter == id) {
				characterAux = character;
			}
		}
		return characterAux;
	}

	/// <summary>
	/// Searchs the image product.
	/// </summary>
	/// <returns>The image product.</returns>
    /// <param name="id">Identifier.</SetPowerCountparam>
	private ProductStore SearchImgProduct (ProductItem item) {
		ProductStore prodAux = null;
        if (item is ProductPowerupItem) {
            ProductPowerupItem productPackI = ((ProductPowerupItem)item);
            foreach (PowerUpsStore prod in powerUpsStore) {
                if (prod.idProductPower == productPackI.idProductPower) {
					prodAux = prod;
                    break;
				}
			}
        } else if (item is ProductPackagesItem)
        {
            ProductPackagesItem productPackI = ((ProductPackagesItem)item);
            foreach (PackagesStore prod in packagesStore)
            {
                if (prod.idProduct == productPackI.idProductItem)
                {
                    prodAux = prod;
                    break;
                }
            }
        }
		else if (item is ProductUpgradeItem) {
			ProductUpgradeItem productUpgradeI = ((ProductUpgradeItem) item);
            foreach (UpgradesStore prod in upgradesStore) {
                if (prod.idProductPower == productUpgradeI.idProductPower) {
					prodAux = prod;
                    break;
				}
			}
		}  
        return prodAux;
	}


	private void AnimPanelIn (GUIAnim obj) {
        obj.gameObject.SetActive(true);
		obj.MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);
	}

    private void AnimPanelOut (GUIAnim obj,GameObject buttonObj) {
		obj.MoveOut (GUIAnimSystem.eGUIMove.SelfAndChildren);
        StartCoroutine(WaitForHide(obj.gameObject, buttonObj));
	}

    private IEnumerator WaitForIn (GUIAnim obj, GameObject buttonOfObject) {
        obj.gameObject.SetActive(true);
        buttonOfObject.SetActive(true);
		yield return new WaitForSeconds (1.0f);
		obj.MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);
	}

    private IEnumerator WaitForHide(GameObject obj, GameObject buttonObj)
    {
        yield return new WaitForSeconds(1.0f);
        obj.SetActive(false);
        buttonObj.SetActive(false);
    }


	/// <summary>
	/// Adds the elements.
	/// </summary>
	/// <param name="numHuellasPurchased">Number huellas purchased.</param>
	/// <param name="numMultiplicadoresPurchased">Number multiplicadores purchased.</param>
	/// <param name="numChuletasPurchased">Number chuletas purchased.</param>
	/// <param name="numMagnetosPurchased">Number magnetos purchased.</param>
	private void addElements (int numHuellasPurchased,
							 int numMultiplicadoresPurchased,
							 int numChuletasPurchased,
							 int numMagnetosPurchased,
							 int numYingYangs = 0) {
        Debug.Log("ADDS");

		GameItemsManager.addValueById (GameItemsManager.Item.NumPawprints, numHuellasPurchased);
		GameItemsManager.addValueById (GameItemsManager.Item.NumMultipliers, numMultiplicadoresPurchased);
        GameItemsManager.AddPowerCount(GameItemsManager.StorePower.PawPrintMultiplier, numMultiplicadoresPurchased);
		GameItemsManager.addValueById (GameItemsManager.Item.NumPorkchop, numChuletasPurchased);
        GameItemsManager.AddPowerCount(GameItemsManager.StorePower.PorkChop, numChuletasPurchased);
		GameItemsManager.addValueById (GameItemsManager.Item.NumMagnets, numMagnetosPurchased);
        GameItemsManager.AddPowerCount(GameItemsManager.StorePower.Magnet,numMagnetosPurchased);
		GameItemsManager.addValueById (GameItemsManager.Item.NumYinYangs, numYingYangs);

    }

	/// <summary>
	/// Substracts the elements.
	/// </summary>
	/// <param name="numHuellasPurchased">Number huellas purchased.</param>
	/// <param name="numMultiplicadoresPurchased">Number multiplicadores purchased.</param>
	/// <param name="numChuletasPurchased">Number chuletas purchased.</param>
	/// <param name="numMagnetosPurchased">Number magnetos purchased.</param>
	private void substractElements (int numHuellasPurchased,
		int numMultiplicadoresPurchased,
		int numChuletasPurchased,
		int numMagnetosPurchased,
		int numYingYangs = 0) {

        Debug.Log("SUBSTRAct");

        GameItemsManager.subtractValueById (GameItemsManager.Item.NumPawprints, numHuellasPurchased);
		GameItemsManager.subtractValueById (GameItemsManager.Item.NumMultipliers, numMultiplicadoresPurchased);
		GameItemsManager.subtractValueById (GameItemsManager.Item.NumPorkchop, numChuletasPurchased);
		GameItemsManager.subtractValueById (GameItemsManager.Item.NumMagnets, numMagnetosPurchased);
		GameItemsManager.subtractValueById (GameItemsManager.Item.NumYinYangs, numYingYangs);
		
	}

    private void HandleWallpaperToMessage(string title, string message, DialogMessage.typeMessage typeMessage)
    {
        
        if (typeMessage == DialogMessage.typeMessage.ERROR)
        {
            BuildDialogMessageWallpaper(title, message, typeMessage);
        }
        else
        {
            BuildDialogMessage(title, message, typeMessage, true);
        }
    }

    /// <summary>
    /// Builds the dialog message.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="msg">Message.</param>
    /// <param name="type">Type.(ERROR, INFO, WARN)</param>
    private void BuildDialogMessage (string title, string msg, DialogMessage.typeMessage type, bool isCharacterOrPowerUp) {
		GameObject mostrarMsg = Instantiate (dialogMessage) as GameObject;
		DialogMessage popupMsg = mostrarMsg.GetComponent<DialogMessage> ();
		popupMsg.txtMessage.text = msg;
		popupMsg.txtTitle.text = title;
		switch (type) {
			case DialogMessage.typeMessage.ERROR:
				popupMsg.imgStatus.overrideSprite = imgDialogMessageERR;
				break;
			case DialogMessage.typeMessage.INFO:
				popupMsg.imgStatus.overrideSprite = imgDialogMessageINFO;
				break;
			case DialogMessage.typeMessage.WARNING:
				popupMsg.imgStatus.overrideSprite = imgDialogMessageWARN;
				break;
		}
		mostrarMsg.transform.SetParent (mainContainer, false);
        //false puwerUp true Character
        popupMsg.isCharacterOrPowerUp = isCharacterOrPowerUp;
		popupMsg.UpdateTextTranslation ();
		popupMsg.OpenDialogmessage ();
	}

	/// <summary>
	/// Builds the popup purchased ProductItem.
	/// </summary>
	/// <param name="pi">ProductItem</param>
	private void BuildPopupPurchasedProduct (ProductItem pi) {
		LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation ();
		GameObject mostrarMsg = Instantiate (dialogMessage) as GameObject;
		DialogMessage popupMsg = mostrarMsg.GetComponent<DialogMessage> ();
		popupMsg.txtMessage.text = string.Format (lm.GetString ("msg_info_success_store_purchased"), pi.nameProduct.text);
        popupMsg.imgStatus.gameObject.SetActive(true);
        if (SearchImgProduct (pi) != null) {
			popupMsg.imgStatus.overrideSprite = SearchImgProduct (pi).imgProducto;
            var imgStatusTransform = popupMsg.imgStatus.transform as RectTransform;
            imgStatusTransform.sizeDelta = new Vector2(450f, 450f);
        }

		mostrarMsg.transform.SetParent (mainContainer, false);
        popupMsg.isCharacterOrPowerUp = false;
		popupMsg.OpenDialogmessage();
	}


	/// <summary>
	/// Builds the popup purchased CharacterItem.
	/// </summary>
	/// <param name="pi">ProductItem</param>
	private void BuildPopupPurchasedCharacter (CharacterItem pi) {
		LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation ();
		GameObject mostrarMsg = Instantiate (dialogMessage) as GameObject;
		DialogMessage popupMsg = mostrarMsg.GetComponent<DialogMessage> ();
		popupMsg.txtMessage.text = string.Format (lm.GetString ("msg_info_success_store_purchased"), pi.nameCharacter.text);

        popupMsg.imgStatus.overrideSprite = imgDialogMessageINFO;

        mostrarMsg.transform.SetParent (mainContainer, false);
        popupMsg.isCharacterOrPowerUp = true;
		popupMsg.OpenDialogmessage ();
	}

    /// <summary>
	/// Builds the popup purchased CharacterItem.
	/// </summary>
	/// <param name="pi">ProductItem</param>
	private void BuildPopupPurchasedWallpaper(WallpaperItem pi)
    {
        LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation();
        GameObject mostrarMsg = Instantiate(dialogMessageWallpaper) as GameObject;
        DialogMessageWallpaper popupMsg = mostrarMsg.GetComponent<DialogMessageWallpaper>();
        popupMsg.txtMessage.text = string.Format(lm.GetString("msg_info_success_store_purchased"), pi.idWallpaper.ToString());

        popupMsg.imgStatus.overrideSprite = imgDialogMessageINFO;
        mostrarMsg.transform.SetParent(mainContainer, false);
        popupMsg.OpenDialogmessage();
    }

    /// <summary>
    /// Builds the dialog message.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="msg">Message.</param>
    /// <param name="type">Type.(ERROR, INFO, WARN)</param>
    private void BuildDialogMessageWallpaper(string title, string msg, DialogMessage.typeMessage type)
    {
        GameObject mostrarMsg = Instantiate(dialogMessageWallpaper) as GameObject;
        DialogMessageWallpaper popupMsg = mostrarMsg.GetComponent<DialogMessageWallpaper>();
        popupMsg.txtMessage.text = msg;
        popupMsg.txtTitle.text = title;
        switch (type)
        {
            case DialogMessage.typeMessage.ERROR:
                popupMsg.imgStatus.overrideSprite = imgDialogMessageERR;
                break;
            case DialogMessage.typeMessage.INFO:
                popupMsg.imgStatus.overrideSprite = imgDialogMessageINFO;
                break;
            case DialogMessage.typeMessage.WARNING:
                popupMsg.imgStatus.overrideSprite = imgDialogMessageWARN;
                break;
        }
        mostrarMsg.transform.SetParent(mainContainer, false);
        popupMsg.UpdateTextTranslation();
        popupMsg.OpenDialogmessage();
    }

    private void HandleWallpaperPurchase(WallpaperItem itemWallpaper)
    {
        Debug.Log("Iam Here");
        Debug.Log(OnWallpaperIsSelected);
        Debug.Log(OnStoreWallpaperUnlockButton);
        string idStore = "";
        #if UNITY_IOS
            idStore = "com.EstacionPi.BJWTFoundation."+itemWallpaper.idStoreGooglePlay;
        #else
            idStore = itemWallpaper.idStoreGooglePlay;
        #endif

        itemWallpaper.priceInStore = GetPriceProductStore(idStore);
        if (OnWallpaperIsSelected != null)
            OnWallpaperIsSelected(itemWallpaper, IsInitialized());
    }

    #endregion

    #region IAP Purchase

    public void InitializePurchasing(List<IStorePurchase> listItemProducts)
    {
        Debug.Log("[IAPManager] Initing Unity IAP");
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            Debug.Log("[IAPManager] Inicializado IAP...");
            HandleIncializationIAP(false);
            return;
        }
        Debug.Log("[IAPManager] Inicializando IAP...");
        // Create a builder, first passing in a suite of Unity provided stores.
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());



        foreach (IStorePurchase item in listItemProducts)
        {
            if (item is ProductItem)
            {
                ProductItem currentProduct = ((ProductItem)item);


                if (currentProduct is ProductUpgradeItem)
                { //If is Upgrade
                    ProductUpgradeItem currentUp = ((ProductUpgradeItem)currentProduct);
                    if (currentUp.levelsUpgradesIdGooglePlay != null
                        && currentUp.levelsUpgradesIdGooglePlay.Count > 0)
                    {
                        foreach (string lvs in currentUp.levelsUpgradesIdGooglePlay)
                        {
                            Debug.Log("Agregando item(ProductUpgradeItem) para el store: " + lvs);
                            builder.AddProduct(lvs, ProductType.Consumable);
                        }
                    }
                }
                else
                { //If is PowerUp or Package
                    Debug.Log("Agregando item(ProductItem) para el store: " + currentProduct.idStoreGooglePlay);
                    builder.AddProduct(currentProduct.idStoreGooglePlay, ProductType.Consumable);
                }
            }

            if (item is CharacterItem)
            {
                CharacterItem currentProduct = ((CharacterItem)item);
                Debug.Log("Agregando item(CharacterItem) para el store: " + currentProduct.idStoreGooglePlay);
                builder.AddProduct(currentProduct.idStoreGooglePlay, ProductType.Consumable);
            }

            if (item is WallpaperItem)
            {
                WallpaperItem currentProduct = ((WallpaperItem)item);
                Debug.Log("Agregando item(WallpaperItem) para el store: " + currentProduct.idStoreGooglePlay);
                builder.AddProduct(currentProduct.idStoreGooglePlay, ProductType.Consumable);
            }
        }

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
        Debug.Log("[IAPManager] IAP inicializado !!!");
    }


    private bool IsInitialized()
    {
        return storeController != null && storeExtensionProvider != null;
    }

    public void BuyInStore(IStorePurchase productItemObject)
    {
        productItemBuy = productItemObject;
        BuyProductID(productItemObject);
    }

    private void BuyProductID(IStorePurchase productItem)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = null;
            if (productItem is ProductItem)
            {
                ProductItem currentProduct = ((ProductItem)productItem);
                product = storeController.products.WithID(id: currentProduct.idStoreGooglePlay);
            }
            else if (productItem is CharacterItem)
            {
                CharacterItem currentCharacter = ((CharacterItem)productItem);
                product = storeController.products.WithID(id: currentCharacter.idStoreGooglePlay);
            }
            else if (productItem is WallpaperItem)
            {
                WallpaperItem currentCharacter = ((WallpaperItem)productItem);
                product = storeController.products.WithID(id: currentCharacter.idStoreGooglePlay);
            }
            else
            {
                HandleIAPEvents("msg_err_produc_not_avaliable", true);
            }

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                storeController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                HandleIAPEvents("msg_err_produc_not_avaliable", true);
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            HandleIAPEvents("msg_err_init_api_iap", true);
        }
    }


    /// <summary>
    /// This will be called when Unity IAP has finished initialising.
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        storeController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        storeExtensionProvider = extensions;

        extensions.GetExtension<IAppleExtensions>().RegisterPurchaseDeferredListener(OnDeferred);

        //Debug.Log("Available items:");
        HandleIncializationIAP(false);
    }

    /// <summary>
    /// iOS Specific.
    /// This is called as part of Apple's 'Ask to buy' functionality,
    /// when a purchase is requested by a minor and referred to a parent
    /// for approval.
    ///
    /// When the purchase is approved or rejected, the normal purchase events
    /// will fire.
    /// </summary>
    /// <param name="item">Item.</param>
    private void OnDeferred(Product item)
    {
        Debug.Log("Purchase deferred: " + item.definition.id);
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("Billing failed to initialize!");
        switch (error)
        {
            case InitializationFailureReason.AppNotKnown:
                Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
                break;
            case InitializationFailureReason.PurchasingUnavailable:
                // Ask the user if billing is disabled in device settings.
                Debug.Log("Billing disabled!");
                break;
            case InitializationFailureReason.NoProductsAvailable:
                // Developer configuration error; check product metadata.
                Debug.Log("No products available for purchase!");
                break;
        }

        string msg = "msg_err_init_api_iap";
        HandleIAPEvents(msg, true);
    }



    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs result)
    {
        if (String.Equals(result.purchasedProduct.definition.id, productItemBuy.idStoreGooglePlay, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", result.purchasedProduct.definition.id));

            string nameItemPurchased = "";
            if (productItemBuy is ProductItem)
            {
                ProductItem currentProduct = ((ProductItem)productItemBuy);
                nameItemPurchased = currentProduct.nameProduct.text;
            }

            if (productItemBuy is CharacterItem)
            {
                CharacterItem currentCharacter = ((CharacterItem)productItemBuy);
                nameItemPurchased = currentCharacter.nameCharacter.text;
            }

            if (productItemBuy is WallpaperItem)
            {
                WallpaperItem currentWallpaper = ((WallpaperItem)productItemBuy);
                nameItemPurchased = currentWallpaper.name;
            }

            string msj = "Compra exitosa !! Has adquirido " + nameItemPurchased + ". Felicidades !!";
            Debug.Log(msj);
            HandleSuccessPurchasedInStore("msg_info_success_purchased", productItemBuy);

            /*string msj = "msg_err_purchased_error";
            HandleIAPEvents(msj, true);*/
        }
        else
        {
            string msj = "msg_err_purchased_error";
            HandleIAPEvents(msj, true);
        }

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log("Purchase failed: " + product.definition.id);
        Debug.Log(failureReason);
        string msj = "msg_err_purchased_error";
        HandleIAPEvents(msj, true);
    }
    #endregion
}