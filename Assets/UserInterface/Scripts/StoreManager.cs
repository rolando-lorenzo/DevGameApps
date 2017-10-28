using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using PankioAssets;
using System;
using UnityEngine.SceneManagement;


public class ProductStore {
	public string idProduct;
	public Sprite imgProducto;
	public string nameProduct;
	public float priceProduct;
	public string descProduct;
	public string btnBuyText;
	public string idInStoreGooglePlay;
	public bool isAvailableInStore;
}

[System.Serializable]
public class PackagesStore : ProductStore { 
}

[System.Serializable]
public class PowerUpsStore : ProductStore { 
	public int costInHuellas;
}

[System.Serializable]
public class UpgradesStore : ProductStore {
	public int costInHuellas;
	public List<string> levelsUpgradesIdGooglePlay;
}

[System.Serializable]
public class CharacterStore{
	public int idCharacter;
	public Sprite imgCharacter;
	public Sprite imgBuyCharacter;
	public string nameCharacter;
	public string descriptionCharacter;
	public string idInStoreGooglePlay;
	public bool isAvailableInStore;
}

public class StoreManager : MonoBehaviour {

	#region Class members
	public GameObject btnProductPackage;
	public GameObject btnProductPowerup;
	public GameObject btnProductUpgrade;
	public Button btnCharacters;
	public Button btnInviteFriends;
	public Button btnCloseCharactersPanel;
	public Button btnBackScene;
	public GameObject packagesPanel;
	public GameObject powerUpsPanel;
	public GameObject upgradesPanel;
	public GameObject contadorHuellas;

	private GUIAnim packagesPanelAnim;
	private GUIAnim powerUpsPanelAnim;
	private GUIAnim upgradesPanelAnim;

	public GameObject charactersPanel;
	public Transform containerPaquetes;
	public Transform containerPowerUps;
	public Transform containerUpgrades;
	public GameObject popupBuy;
	public GameObject dialogMessage;
	public Sprite imgDialogMessageINFO;
	public Sprite imgDialogMessageWARN;
	public Sprite imgDialogMessageERR;
	public Transform mainContainer;
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
	private Text descCharacter;
	private IAPManager iapManager;

	private const int CHARACTER_LOCK = 1;
	private const int CHARACTER_UNLOCK = 2;
    /// <summary>
    /// manager of languages
    /// </summary>
    /// 

	#endregion

	#region Class accesors
	#endregion

	#region MonoBehaviour overrides
	void OnEnable()
	{
        //Eventos internos
        infiniteScrollCharacters = charactersPanel.GetComponentInChildren<InfiniteScroll> ();
		infiniteScrollCharacters.OnItemChanged += HandleCurrentCharacter;
		foreach (CharacterItem ch in charactersTemplate) {
			ch.OnItemPurchased += HandleCharacterToWillPurchase;
		}
		foreach (ProductItem pi in btnsSlide) {
			if (pi is ProductPackagesItem){
				((ProductPackagesItem)pi).OnProductPackageItemPurchased += HandleProductToWillPurchased;
			}

			if (pi is ProductPowerupItem){
				((ProductPowerupItem)pi).OnProductPowerUpItemPurchased += HandleProductToWillPurchased;
			}

			if (pi is ProductUpgradeItem){
				((ProductUpgradeItem)pi).OnProductUpgradeItemPurchased += HandleProductToWillPurchased;
				((ProductUpgradeItem)pi).OnProductUpgradeBuyLimitMax   += HandleProductLimitBuyReached;
			}
		}

		//Eventos del Service IAP
		iapManager.OnIAPInitialized += HandleIncializationIAP;
		iapManager.OnIAPMessageProgress += HandleIAPEvents;
		iapManager.OnIAPSuccessPurchasedInStore += HandleSuccessPurchasedInStore;
	}

	void Awake () {
		instance = this;
		btnCharacters.onClick.AddListener (ShowCharactersPanel);
		btnInviteFriends.onClick.AddListener (InviteFriends);
		btnCloseCharactersPanel.onClick.AddListener (HideCharactersPanel);
		btnBackScene.onClick.AddListener (GoMainMenu);
		charactersTemplate = charactersPanel.GetComponentsInChildren<CharacterItem> ();

		packagesPanelAnim = packagesPanel.GetComponent<GUIAnim>();
		powerUpsPanelAnim = powerUpsPanel.GetComponent<GUIAnim>();
		upgradesPanelAnim = upgradesPanel.GetComponent<GUIAnim>();

		Transform txtHeaderTrsform = charactersPanel.transform.Find ("TextDescCharacters");
		if (txtHeaderTrsform != null) {
			descCharacter = txtHeaderTrsform.gameObject.GetComponent<Text> ();
		}
		if (enabled)
		{
			GUIAnimSystem.Instance.m_AutoAnimation = false;
		}
		btnsSlide = new List<ProductItem> ();
		itemsAvailableInStore  = new List<IStorePurchase> ();
		itemsNotAvailableStore  = new List<IStorePurchase> ();
		PopulatePackages (packagesStore);
		PopulatePowerUps (powerUpsStore);
		PopulateUpgrades (upgradesStore);
		PopulateCharacters ();
		iapManager = IAPManager.Instance;
		iapManager.InitializePurchasing (itemsAvailableInStore);


	}

	void Start(){
        ShowELementsInScreen ();
		StartCoroutine (AnimPanelsPackages ());
	}

	void OnDisable()
	{
		if (infiniteScrollCharacters != null) {
			infiniteScrollCharacters.OnItemChanged -= HandleCurrentCharacter;
		}
		foreach (CharacterItem ch in charactersTemplate) {
			if(ch != null)
				ch.OnItemPurchased -= HandleCharacterToWillPurchase;
		}

		foreach (ProductItem pi in btnsSlide) {
			if (pi is ProductPackagesItem){
				((ProductPackagesItem)pi).OnProductPackageItemPurchased -= HandleProductToWillPurchased;
			}

			if (pi is ProductPowerupItem){
				((ProductPowerupItem)pi).OnProductPowerUpItemPurchased -= HandleProductToWillPurchased;
			}

			if (pi is ProductUpgradeItem){
				((ProductUpgradeItem)pi).OnProductUpgradeItemPurchased -= HandleProductToWillPurchased;
				((ProductUpgradeItem)pi).OnProductUpgradeBuyLimitMax   -= HandleProductLimitBuyReached;
			}
		}

		//Eventos del Service IAP
		iapManager.OnIAPInitialized -= HandleIncializationIAP;
		iapManager.OnIAPMessageProgress -= HandleIAPEvents;
		iapManager.OnIAPSuccessPurchasedInStore -= HandleSuccessPurchasedInStore;
	}
	#endregion

	#region Super class overrides
	#endregion

	#region Class implementation

	/// <summary>
	/// Populates the packages panel.
	/// </summary>
	/// <param name="products">Products.</param>
	private void PopulatePackages(List<PackagesStore> products){
		LanguagesManager managerGlobal= LanguagesManager.Load();
        string deviceLanguage = Application.systemLanguage.ToString();

        if (managerGlobal.VerifyLanguage(deviceLanguage))
        {
            Language languageEnum = managerGlobal.GetLanguageEnum(deviceLanguage);
            managerGlobal.SetLanguage(languageEnum);
        }
        else
        {
            managerGlobal.SetLanguage(Language.Default);
        }

        foreach (var product in products){
			GameObject nuevoProducto = Instantiate (btnProductPackage) as GameObject;
			ProductItem ProductItem = nuevoProducto.GetComponent<ProductItem> ();
			ProductItem.idProductItem = product.idProduct;
			ProductItem.imgProduct.sprite = product.imgProducto;

            String nameProduct = managerGlobal.GetString(product.nameProduct);

			ProductItem.nameProduct.text = nameProduct;
			ProductItem.priceProduct.text = MenuUtils.FormatPriceProducts(product.priceProduct);
			ProductItem.valCurrency = product.priceProduct;

            String descProduct = managerGlobal.GetString(product.descProduct);

			ProductItem.descProduct.text = descProduct;
			ProductItem.idStoreGooglePlay = product.idInStoreGooglePlay;
			ProductItem.isAvailableInStore = product.isAvailableInStore;

            String btnBuyText = managerGlobal.GetString(product.btnBuyText);

            ProductItem.btnBuy.GetComponentInChildren<Text> ().text = btnBuyText;
			nuevoProducto.transform.SetParent (containerPaquetes,false);

			if (product.isAvailableInStore) {
				// it could be purchased only in Online Store
				itemsAvailableInStore.Add (ProductItem); 

			} else { 
				// it could be purchased only with items like Hulleas, yinyangs ....
				itemsNotAvailableStore.Add (ProductItem); 
			}
			btnsSlide.Add (ProductItem);
		}
	}

	/// <summary>
	/// Populates the characters panel.
	/// </summary>
	public void PopulateCharacters(){
        LanguagesManager managerGlobal = LanguagesManager.Load();
        string deviceLanguage = Application.systemLanguage.ToString();

        if (managerGlobal.VerifyLanguage(deviceLanguage))
        {
            Language languageEnum = managerGlobal.GetLanguageEnum(deviceLanguage);
            managerGlobal.SetLanguage(languageEnum);
        }
        else
        {
            managerGlobal.SetLanguage(Language.Default);
        }
        foreach (CharacterStore character in characters) {
			foreach (CharacterItem characterTemplate in charactersTemplate) {
				if(character.idCharacter == characterTemplate.idCharacter){

                    string nameCharacter = managerGlobal.GetString(character.nameCharacter);

                    characterTemplate.nameCharacter.text = nameCharacter;
					characterTemplate.btnBuyProduct.image.overrideSprite = character.imgBuyCharacter;

                    string descriptionCharacter = managerGlobal.GetString(character.descriptionCharacter);
                    string btnText = managerGlobal.GetString("btnglobal");

                    characterTemplate.descCharacter = descriptionCharacter;
                    characterTemplate.btnBuyProduct.GetComponentInChildren<Text>().text = btnText;
					characterTemplate.idStoreGooglePlay = character.idInStoreGooglePlay;
					characterTemplate.isAvailableInStore = character.isAvailableInStore;
					Image currentImg = characterTemplate.character.GetComponent<Image> ();
					currentImg.overrideSprite = character.imgCharacter;
					if(character.isAvailableInStore){
						// it could be purchased only in Online Store
						itemsAvailableInStore.Add (characterTemplate);
					} else { 
						// it could be purchased only with items like Hulleas, yinyangs ....
						itemsNotAvailableStore.Add (characterTemplate); 
					}
				}	
			}
		}
		if (characters.Count > 0) {
			descCharacter.text = characters [0].descriptionCharacter;
		}

	}

	/// <summary>
	/// Populates the power ups panel.
	/// </summary>
	/// <param name="products">Products.</param>
	private void PopulatePowerUps(List<PowerUpsStore> products){
        LanguagesManager managerGlobal = LanguagesManager.Load();
        string deviceLanguage = Application.systemLanguage.ToString();

        if (managerGlobal.VerifyLanguage(deviceLanguage))
        {
            Language languageEnum = managerGlobal.GetLanguageEnum(deviceLanguage);
            managerGlobal.SetLanguage(languageEnum);
        }
        else
        {
            managerGlobal.SetLanguage(Language.Default);
        }
        foreach (var product in products){
			GameObject nuevoProducto = Instantiate (btnProductPowerup) as GameObject;
			ProductItem ProductItem = nuevoProducto.GetComponent<ProductItem> ();
			ProductItem.idProductItem = product.idProduct;
			ProductItem.imgProduct.sprite = product.imgProducto;

            String nameProduct = managerGlobal.GetString(product.nameProduct);

            ProductItem.nameProduct.text = nameProduct;
			ProductItem.priceProduct.text = MenuUtils.FormatPawprintsProducts(product.priceProduct);
			ProductItem.valCurrency = product.priceProduct;

            String descProduct = managerGlobal.GetString(product.descProduct);

            ProductItem.descProduct.text = descProduct;
			ProductItem.costInHuellas = product.costInHuellas;
			ProductItem.idStoreGooglePlay = product.idInStoreGooglePlay;
			ProductItem.isAvailableInStore = product.isAvailableInStore;

            String btnBuyText = managerGlobal.GetString(product.btnBuyText);

            ProductItem.btnBuy.GetComponentInChildren<Text> ().text = btnBuyText;
			nuevoProducto.transform.SetParent (containerPowerUps,false);
			if(product.isAvailableInStore){
				// it could be purchased only in Online Store
				itemsAvailableInStore.Add (ProductItem);
			} else { 
				// it could be purchased only with items like Hulleas, yinyangs ....
				itemsNotAvailableStore.Add (ProductItem); 
			}
			btnsSlide.Add (ProductItem);
		}
	}

	/// <summary>
	/// Populates the upgrades panel.
	/// </summary>
	/// <param name="products">Products.</param>
	private void PopulateUpgrades(List<UpgradesStore> products){
        LanguagesManager managerGlobal = LanguagesManager.Load();
        string deviceLanguage = Application.systemLanguage.ToString();

        if (managerGlobal.VerifyLanguage(deviceLanguage))
        {
            Language languageEnum = managerGlobal.GetLanguageEnum(deviceLanguage);
            managerGlobal.SetLanguage(languageEnum);
        }
        else
        {
            managerGlobal.SetLanguage(Language.Default);
        }

        foreach (var product in products){
			GameObject nuevoProducto = Instantiate (btnProductUpgrade) as GameObject;
			ProductUpgradeItem ProductItem = nuevoProducto.GetComponent<ProductUpgradeItem> ();

			ProductItem.idProductItem = product.idProduct;
			ProductItem.imgProduct.sprite = product.imgProducto;

            string nameProduct = managerGlobal.GetString(product.nameProduct);

            ProductItem.nameProduct.text = nameProduct;
			ProductItem.priceProduct.text = MenuUtils.FormatPriceProducts(product.priceProduct);
			ProductItem.valCurrency = product.priceProduct;

            string descProduct = managerGlobal.GetString(product.descProduct);

            ProductItem.descProduct.text = descProduct;
			ProductItem.costInHuellas = product.costInHuellas;
			ProductItem.idStoreGooglePlay = product.idInStoreGooglePlay;
			ProductItem.isAvailableInStore = product.isAvailableInStore;
			ProductItem.levelsUpgradesIdGooglePlay = product.levelsUpgradesIdGooglePlay;

            string btnBuyText  = managerGlobal.GetString(product.btnBuyText);

            ProductItem.btnBuy.GetComponentInChildren<Text> ().text = btnBuyText;
			nuevoProducto.transform.SetParent (containerUpgrades,false);
			ProductItem.SetChildId (product.idProduct);
			if(product.isAvailableInStore){
				// it could be purchased only in Online Store
				itemsAvailableInStore.Add (ProductItem);
			} else { 
				// it could be purchased only with items like Hulleas, yinyangs ....
				itemsNotAvailableStore.Add (ProductItem); 
			}
			btnsSlide.Add (ProductItem);
		}
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
	/// Shows the characters (Turn TRUE SetActive)panel.
	/// </summary>
	public void ShowCharactersPanel(){
		ToggleActivePackagesPanels ();
		charactersPanel.SetActive (true);
		GUIAnim guiAnim = charactersPanel.GetComponent<GUIAnim> ();
		guiAnim.MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);
	}

	public void HideCharactersPanel(){
		ToggleActivePackagesPanels ();
		GUIAnim guiAnim = charactersPanel.GetComponent<GUIAnim> ();
		guiAnim.MoveOut (GUIAnimSystem.eGUIMove.SelfAndChildren);
	}

	public void GoMainMenu(){
		SceneManager.LoadScene("MainScene");
	}

	public void InviteFriends(){
		Debug.Log ("Inivitando Amigos !!");
	}

	/// <summary>
	/// Update Text Elements in screen.
	/// </summary>
	public void ShowELementsInScreen(){
		StringBuilder txtElemts = new StringBuilder();
		txtElemts.Append("Huellas: "+ GameItemsManager.GetValueById(GameItemsManager.Item.numPawprints).ToString());
		txtElemts.Append(", Multiplicadores: "+ GameItemsManager.GetValueById(GameItemsManager.Item.numMultipliers).ToString());
		txtElemts.Append(", Chuletas: "+ GameItemsManager.GetValueById(GameItemsManager.Item.numMultipliers).ToString());
		txtElemts.Append(", Magnetos: "+  GameItemsManager.GetValueById(GameItemsManager.Item.numMagnets).ToString());
		txtElemts.Append(", YinYangs: "+ GameItemsManager.GetValueById(GameItemsManager.Item.numYinYangs).ToString());
		Debug.Log (txtElemts);
		Text contHuellasText = contadorHuellas.GetComponentInChildren<Text> ();
		contHuellasText.text = txtElemts.ToString();
	}

	/// <summary>
	/// Handles the current character.
	/// </summary>
	/// <param name="previousItem">Previous item.</param>
	/// <param name="currentItem">Current item.</param>
	/// <param name="itemIndex">Item index.</param>
	private void HandleCurrentCharacter(ScrollItem previousItem, ScrollItem currentItem, int itemIndex){
		if(currentItem != null){
			CharacterItem c = currentItem.gameObject.GetComponentInChildren<CharacterItem> ();
			descCharacter.text = c.descCharacter;
		}
	}



	/// <summary>
	/// Handles the character success purchased.
	/// </summary>
	/// <param name="chItem">Character item.</param>
	private void HandleCharacterToWillPurchase(CharacterItem chItem){
		Debug.Log ("Intentando comprar... : "+chItem.nameCharacter.text);
		if (chItem.isAvailableInStore) {
			Debug.Log ("Comprando en IAP...");
			iapManager.BuyInStore (chItem);
		} else {
			Debug.Log ("Comprando con Items (Huellas, YingYangs, etc...)");
			//Unlock character
			GameItemsManager.Item en = (GameItemsManager.Item)Enum.Parse(typeof(GameItemsManager.Item),chItem.idStoreGooglePlay);
			Debug.Log (en);
			if (string.Equals (chItem.idStoreGooglePlay, GameItemsManager.Item.personajeJaguarMexicano.ToString ())) {

				//case Jaguar Mexicano (Needs 25000 huellas, 5 YingYangs)
				if(checkItemsAvailableToPlay(25000,5)){
					GameItemsManager.SetValueById (GameItemsManager.Item.personajeJaguarMexicano, CHARACTER_UNLOCK);
					chItem.InactivateButtonBuyIfUnlocked ();
					Debug.Log ("Se ha desbloqueado a "
						+GameItemsManager.Item.personajeJaguarMexicano.ToString()+" a valor: "
						+GameItemsManager.GetValueById(GameItemsManager.Item.personajeJaguarMexicano));
					BuildPopupPurchasedCharacter (chItem);
				} else {
					BuildDialogMessage ("Message from Store","Nesecitas 25000 huellas, 5 YingYangs ! Para desbloquear este personaje.",DialogMessage.typeMessage.ERROR);
				}

			} else if (string.Equals (chItem.idStoreGooglePlay, GameItemsManager.Item.personajeTigreSable.ToString ())) {

				//case Tigre dientes de sable (Needs 50000 huellas, 15 YingYangs)
				if(checkItemsAvailableToPlay(50000,15)){
					GameItemsManager.SetValueById (GameItemsManager.Item.personajeTigreSable, CHARACTER_UNLOCK);
					chItem.InactivateButtonBuyIfUnlocked ();
					Debug.Log ("Se ha desbloqueado a "
						+GameItemsManager.Item.personajeTigreSable.ToString()+" a valor: "
						+GameItemsManager.GetValueById(GameItemsManager.Item.personajeTigreSable));
					BuildPopupPurchasedCharacter (chItem);
					
				} else {
					BuildDialogMessage ("Message from Store","Nesecitas 50000 huellas, 15 YingYangs ! Para desbloquear este personaje.",DialogMessage.typeMessage.ERROR);
				}

			} else if (string.Equals (chItem.idStoreGooglePlay, GameItemsManager.Item.personajeGatoMontes.ToString ())) {

				//case Gato Montes (Needs 25000 huellas, 10 YingYangs)
				if (checkItemsAvailableToPlay (25000, 10)) {
					GameItemsManager.SetValueById (GameItemsManager.Item.personajeGatoMontes, CHARACTER_UNLOCK);
					chItem.InactivateButtonBuyIfUnlocked ();
					Debug.Log ("Se ha desbloqueado a "
						+ GameItemsManager.Item.personajeGatoMontes.ToString () + " a valor: "
						+ GameItemsManager.GetValueById (GameItemsManager.Item.personajeGatoMontes));
					BuildPopupPurchasedCharacter (chItem);
				} else {
					BuildDialogMessage ("Message from Store","Nesecitas 25000 huellas, 10 YingYangs ! Para desbloquear este personaje.",DialogMessage.typeMessage.ERROR);
				}

			} else {
				BuildDialogMessage ("Message from Store","Personaje no considerado para Online Store !",DialogMessage.typeMessage.ERROR);
			}
		}
	}

	/// <summary>
	/// Checks the num items available to play.
	/// </summary>
	/// <returns><c>true</c>, if items available to play was checked, <c>false</c> otherwise.</returns>
	/// <param name="numHuellas">Number huellas.</param>
	/// <param name="numYingYangs">Number ying yangs.</param>
	private bool checkItemsAvailableToPlay(int numHuellas, int numYingYangs){
		if (numHuellas <= GameItemsManager.GetValueById (GameItemsManager.Item.numPawprints)
		    && numYingYangs <= GameItemsManager.GetValueById (GameItemsManager.Item.numYinYangs)) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// Handles the product success purchased. (Packages, PowerUps, Upgrades)
	/// </summary>
	/// <param name="pi">Product Item</param>
	private void HandleProductToWillPurchased(ProductItem pi){
		Debug.Log ("---- Manager event success purchased ---");

		//Va a la tienda de Google play
		if (pi is ProductPackagesItem){
			ProductPackagesItem currentBought = ((ProductPackagesItem)pi);
			Debug.Log ("[StoreManager] Iniciando compra... "+currentBought.idProductItem +" "+ currentBought.nameProduct.text);
		
			if (currentBought.isAvailableInStore) {
				iapManager.BuyInStore (pi);
			}
		}

		//Utiliza las Huellas disponibles
		if (pi is ProductPowerupItem){
			ProductPowerupItem currentBought = ((ProductPowerupItem)pi);
			Debug.Log ("[StoreManager] Powerup comprado"+currentBought.idProductItem +" "+ currentBought.nameProduct.text);

			int globalCostPowerUp = ((int)currentBought.valCurrency * currentBought.numProductsToBuy);

			if (globalCostPowerUp <= GameItemsManager.GetValueById (GameItemsManager.Item.numPawprints)) {
				//subtracts pawprints
				substractElements (globalCostPowerUp, 0, 0, 0);
				BuildDialogMessage ("Message from Store","Compra exitosa !! Ahora ya cuentas con el PowerUp "+currentBought.nameProduct.text,DialogMessage.typeMessage.INFO);
			} else {
				BuildDialogMessage ("Message from Store","No cuentas con Huellas suficientes para este PowerUp",DialogMessage.typeMessage.ERROR);
			}
			Debug.Log (contadorHuellas.GetComponentInChildren<Text> ().text);
		}

		if (pi is ProductUpgradeItem){
			ProductUpgradeItem currentBought = ((ProductUpgradeItem)pi);
			Debug.Log ("[StoreManager] Upgrade comprado"+currentBought.idProductItem +" "+ currentBought.nameProduct.text);
			if (currentBought.isAvailableInStore) {
				iapManager.BuyInStore (pi);
			}
		}

	}

	/// <summary>
	/// Handles the incialization IA.
	/// </summary>
	/// <param name="isError">If set to <c>true</c> is error.</param>
	private void HandleIncializationIAP (bool isError){
		if (isError) {
			Debug.Log ("[StoreManager] Ocurrio un error al inicializar api de IAP");
			BuildDialogMessage ("Message from Store","Ocurrio un error al inicializar api de IAP",DialogMessage.typeMessage.ERROR);
		}

	}

	/// <summary>
	/// Handles the IAP events showing dialog message popup.
	/// </summary>
	/// <param name="messageResult">Message result.</param>
	/// <param name="isError">If set to <c>true</c> is error.</param>
	private void HandleIAPEvents(string messageResult, bool isError){
		string titlePopup = "Message from Store";
		if (isError) {
			BuildDialogMessage (titlePopup, messageResult, DialogMessage.typeMessage.ERROR);
		} else {
			BuildDialogMessage (titlePopup,messageResult,DialogMessage.typeMessage.INFO);
		}

	}

	/// <summary>
	/// Handles the purchased in store online (ProductItem or CharacterItem).
	/// </summary>
	/// <param name="messageResult">Message result.</param>
	/// <param name="item">Item.</param>
	private void HandleSuccessPurchasedInStore(string messageResult, IStorePurchase ite){
		string titlePopup = "Message from Store";

		if (ite is ProductItem) {
			//Acciones en la compra exitosa desde IAP
			ProductItem currentProduct = ((ProductItem)ite);
			if (currentProduct is ProductPackagesItem) {
				ProductPackagesItem currentPck= ((ProductPackagesItem)currentProduct);
				Debug.Log ("Paquete pagado-> "+currentPck.ToString());
				//if was purchased package 1
				if (string.Equals(currentPck.idProductItem,"Pck1")) {
					//200 huellas + 2 multiplicadores + 2 chuletas
					addElements (200,2,2,0);
				}

				//if was purchased package 2
				if (string.Equals(currentPck.idProductItem,"Pck2")) {
					//500 huellas + 5 multiplicadores + 3 chuletas + 2 magnetos
					addElements (500,5,3,2);
				}

				//if was purchased package 3
				if (string.Equals(currentPck.idProductItem,"Pck3")) {
					//1000 huellas + 10 multiplicadores + 5 chuletas + 5 magnetos
					addElements (1000,10,5,5);
				}

				//if was purchased package 4
				if (string.Equals(currentPck.idProductItem,"Pck4")) {
					//15,000 huellas + 10 multiplicadores + 10 chuletas + 10 magnetos
					addElements (15000,10,10,10);
				}
			}
			BuildDialogMessage (titlePopup,messageResult,DialogMessage.typeMessage.INFO);
		} 

		if (ite is CharacterItem) {
			//Acciones en la compra exitosa de carateres desde IAP
			CharacterItem currentCharacter = ((CharacterItem)ite);
			//Unlock character
			if(string.Equals(currentCharacter.idStoreGooglePlay,
				GameItemsManager.Item.personajePantera.ToString())){
				
				GameItemsManager.SetValueById (GameItemsManager.Item.personajePantera, CHARACTER_UNLOCK);
				Debug.Log ("Se ha desbloqueado a "
					+GameItemsManager.Item.personajePantera.ToString()+" a valor: "
					+GameItemsManager.GetValueById(GameItemsManager.Item.personajePantera));
			}
			if(string.Equals(currentCharacter.idStoreGooglePlay,
				GameItemsManager.Item.personajePuma.ToString())){

				GameItemsManager.SetValueById (GameItemsManager.Item.personajePuma, CHARACTER_UNLOCK);
				Debug.Log ("Se ha desbloqueado a "
					+GameItemsManager.Item.personajePuma.ToString()+" a valor: "
					+GameItemsManager.GetValueById(GameItemsManager.Item.personajePuma));
			}

			BuildPopupPurchasedCharacter (currentCharacter);
		}


	}

	/// <summary>
	/// Handles the product limit buy reached.
	/// </summary>
	/// <param name="msg">Message.</param>
	private void HandleProductLimitBuyReached(string msg){
		BuildDialogMessage ("Message from Store", msg, DialogMessage.typeMessage.ERROR);
	}

	private CharacterStore searchImgCharacter(int id){
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
	/// <param name="id">Identifier.</param>
	private ProductStore searchImgProduct(string id){
		ProductStore prodAux = null;
		foreach (ProductStore prod in packagesStore) {
			if (string.Equals(prod.idProduct,id)) {
				prodAux = prod;
				return prodAux;
			}
		}
		foreach (ProductStore prod in powerUpsStore) {
			if (string.Equals(prod.idProduct,id)) {
				prodAux = prod;
				return prodAux;
			}
		}
		foreach (ProductStore prod in upgradesStore) {
			if (string.Equals(prod.idProduct,id)) {
				prodAux = prod;
				return prodAux;
			}
		}
		return prodAux;
	}

	/// <summary>
	/// Toggles the show active products panels.
	/// </summary>
	private void ToggleActivePackagesPanels(){
		packagesPanel.SetActive(!packagesPanel.activeSelf);
		powerUpsPanel.SetActive(!powerUpsPanel.activeSelf);
		upgradesPanel.SetActive(!upgradesPanel.activeSelf);
	}

	/// <summary>
	/// Animations the panels products.
	/// </summary>
	/// <returns>The panels packages.</returns>
	private IEnumerator AnimPanelsPackages() {
		yield return new WaitForSeconds (0.5f);
		packagesPanelAnim.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
		powerUpsPanelAnim.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
		upgradesPanelAnim.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);

	}

	/// <summary>
	/// Adds the elements.
	/// </summary>
	/// <param name="numHuellasPurchased">Number huellas purchased.</param>
	/// <param name="numMultiplicadoresPurchased">Number multiplicadores purchased.</param>
	/// <param name="numChuletasPurchased">Number chuletas purchased.</param>
	/// <param name="numMagnetosPurchased">Number magnetos purchased.</param>
	private void addElements(int numHuellasPurchased,
							 int numMultiplicadoresPurchased,
							 int numChuletasPurchased,
							 int numMagnetosPurchased,
							 int numYingYangs = 0){


		GameItemsManager.addValueById (GameItemsManager.Item.numPawprints,numHuellasPurchased);
		GameItemsManager.addValueById (GameItemsManager.Item.numMultipliers,numMultiplicadoresPurchased);
		GameItemsManager.addValueById (GameItemsManager.Item.numPorkchop,numChuletasPurchased);
		GameItemsManager.addValueById (GameItemsManager.Item.numMagnets,numMagnetosPurchased);
		GameItemsManager.addValueById (GameItemsManager.Item.numYinYangs,numYingYangs);
		ShowELementsInScreen ();
	}

	/// <summary>
	/// Substracts the elements.
	/// </summary>
	/// <param name="numHuellasPurchased">Number huellas purchased.</param>
	/// <param name="numMultiplicadoresPurchased">Number multiplicadores purchased.</param>
	/// <param name="numChuletasPurchased">Number chuletas purchased.</param>
	/// <param name="numMagnetosPurchased">Number magnetos purchased.</param>
	private void substractElements(int numHuellasPurchased,
		int numMultiplicadoresPurchased,
		int numChuletasPurchased,
		int numMagnetosPurchased,
		int numYingYangs = 0){

		GameItemsManager.subtractValueById (GameItemsManager.Item.numPawprints,numHuellasPurchased);
		GameItemsManager.subtractValueById (GameItemsManager.Item.numMultipliers,numMultiplicadoresPurchased);
		GameItemsManager.subtractValueById (GameItemsManager.Item.numPorkchop,numChuletasPurchased);
		GameItemsManager.subtractValueById (GameItemsManager.Item.numMagnets,numMagnetosPurchased);
		GameItemsManager.subtractValueById (GameItemsManager.Item.numYinYangs,numYingYangs);
		ShowELementsInScreen ();
	}

	/// <summary>
	/// Builds the dialog message.
	/// </summary>
	/// <param name="title">Title.</param>
	/// <param name="msg">Message.</param>
	/// <param name="type">Type.(ERROR, INFO, WARN)</param>
	private void BuildDialogMessage(string title, string msg, DialogMessage.typeMessage type){
		GameObject mostrarMsg = Instantiate (dialogMessage) as GameObject;
		DialogMessage popupMsg = mostrarMsg.GetComponent<DialogMessage> ();
		popupMsg.txtMessage.text= msg;
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
		mostrarMsg.transform.SetParent (mainContainer,false);
		popupMsg.OpenDialogmessage();
	}

	/// <summary>
	/// Builds the popup purchased ProductItem.
	/// </summary>
	/// <param name="pi">ProductItem</param>
	private void BuildPopupPurchasedProduct(ProductItem pi){
		GameObject mostrarMsg = Instantiate (popupBuy) as GameObject;
		PupUpBuyProduct popupMsg = mostrarMsg.GetComponent<PupUpBuyProduct> ();
		popupMsg.message.text = "Felicidades !! Has comprado a "+pi.nameProduct.text;
		if (searchImgProduct (pi.idProductItem) != null) {
			popupMsg.imgProductPurchased.overrideSprite = searchImgProduct(pi.idProductItem).imgProducto;
		}
		mostrarMsg.transform.SetParent (mainContainer,false);
		popupMsg.OpenPupup ();
	}


	/// <summary>
	/// Builds the popup purchased CharacterItem.
	/// </summary>
	/// <param name="pi">ProductItem</param>
	private void BuildPopupPurchasedCharacter(CharacterItem pi){
		GameObject mostrarMsg = Instantiate (popupBuy) as GameObject;
		PupUpBuyProduct popupMsg = mostrarMsg.GetComponent<PupUpBuyProduct> ();
		popupMsg.message.text = "Felicidades !! Has comprado a "+pi.nameCharacter.text;
		if (searchImgCharacter (pi.idCharacter) != null) {
			popupMsg.imgProductPurchased.overrideSprite = searchImgCharacter(pi.idCharacter).imgCharacter;
		}
		mostrarMsg.transform.SetParent (mainContainer,false);
		popupMsg.OpenPupup ();
	}
	#endregion

}