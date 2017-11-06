using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using PankioAssets;
using System;
using UnityEngine.SceneManagement;


public class ProductStore {
    public GameItemsManager.StoreProduct idProduct;
	public Sprite imgProducto;
	public string nameProduct;
	public float priceProduct;
	public string descProduct;
	public string idInStoreGooglePlay;
	public bool isAvailableInStore;
}

[System.Serializable]
public class PackagesStore : ProductStore { 
}

[System.Serializable]
public class PowerUpsStore : ProductStore { 
}

[System.Serializable]
public class UpgradesStore : ProductStore {
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
    public Button buttonPowerups;
	public Button btnInviteFriends;
	public Button btnBackScene;
    public GameObject charactersPanel;
	public GameObject packagesPanel;
    public GameObject backgroundCharacters;
    public GameObject backgroundPackages;
    public GameObject contadorHuellas;
    public GameObject headerProducts;
	public Transform containerPaquetes;
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
    private bool IsMovePanelFromRightPackages = true;
    private bool IsMovePanelFromRightCharacters = true;

	private const int CHARACTER_LOCK = 1;
	private const int CHARACTER_UNLOCK = 2;

    private GUIAnim backgroundCharactersAnim;
    private GUIAnim backgroundPackagesAnim;
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
		btnBackScene.onClick.AddListener (GoMainMenu);
        buttonPowerups.onClick.AddListener(ShowPowerupsPanel);
		charactersTemplate = charactersPanel.GetComponentsInChildren<CharacterItem> ();

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

        backgroundCharactersAnim = backgroundCharacters.GetComponent<GUIAnim>();
        backgroundPackagesAnim = backgroundPackages.GetComponent<GUIAnim>();

        backgroundCharactersAnim.m_MoveIn.Enable = true;
        backgroundCharactersAnim.m_MoveIn.MoveFrom = GUIAnim.ePosMove.RightScreenEdge;
        backgroundCharactersAnim.m_MoveOut.Enable = true;
        backgroundCharactersAnim.m_MoveOut.MoveTo = GUIAnim.ePosMove.LeftScreenEdge;

        backgroundPackagesAnim.m_MoveIn.Enable = true;
        backgroundPackagesAnim.m_MoveIn.MoveFrom = GUIAnim.ePosMove.LeftScreenEdge;
        backgroundPackagesAnim.m_MoveOut.Enable = true;
        backgroundPackagesAnim.m_MoveOut.MoveTo = GUIAnim.ePosMove.RightScreenEdge;
	}

	void Start(){
        AnimPanelIn(backgroundPackagesAnim,IsMovePanelFromRightPackages);
		ShowELementsInScreen ();
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

        LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation();

        //creates header of current elemnts
        BuildHeaderProducts(lm.GetString("text_header_product_packages"));

        //creates body of products
		foreach(var product in products){
			GameObject nuevoProducto = Instantiate (btnProductPackage) as GameObject;
			ProductItem ProductItem = nuevoProducto.GetComponent<ProductItem> ();
			ProductItem.idProductItem = product.idProduct;
			ProductItem.imgProduct.sprite = product.imgProducto;
			ProductItem.nameProduct.text = product.nameProduct;
			ProductItem.priceProduct.text = MenuUtils.FormatPriceProducts(product.priceProduct);
			ProductItem.valCurrency = product.priceProduct;
			ProductItem.descProduct.text = product.descProduct;
			ProductItem.idStoreGooglePlay = product.idInStoreGooglePlay;
			ProductItem.isAvailableInStore = product.isAvailableInStore;
			nuevoProducto.transform.SetParent (containerPaquetes,false);

			if (product.isAvailableInStore) {
				// it could be purchased only in Online Store
				itemsAvailableInStore.Add (ProductItem); 

			} else { 
				// it could be purchased only with items like Hulleas, yinyangs ....
				itemsNotAvailableStore.Add (ProductItem); 
			}
            ProductItem.UpdateTextTranslation();
			btnsSlide.Add (ProductItem);
		}
	}

	/// <summary>
	/// Populates the characters panel.
	/// </summary>
	public void PopulateCharacters(){

        LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation();

		foreach (CharacterStore character in characters) {
			foreach (CharacterItem characterTemplate in charactersTemplate) {
				if(character.idCharacter == characterTemplate.idCharacter){
					characterTemplate.nameCharacter.text = character.nameCharacter;
					characterTemplate.btnBuyProduct.image.overrideSprite = character.imgBuyCharacter;
					characterTemplate.descCharacter = character.descriptionCharacter;
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
                    characterTemplate.UpdateTextTranslation();
				}	
			}
		}
		if (characters.Count > 0) {
            descCharacter.text = lm.GetString(characters[0].descriptionCharacter);
		}

	}

	/// <summary>
	/// Populates the power ups panel.
	/// </summary>
	/// <param name="products">Products.</param>
	private void PopulatePowerUps(List<PowerUpsStore> products){
		
        LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation();

        //creates header of current elemnts
        BuildHeaderProducts(lm.GetString("text_header_product_powerups"));

        //creates body of products
		foreach(var product in products){
			GameObject nuevoProducto = Instantiate (btnProductPowerup) as GameObject;
			ProductItem ProductItem = nuevoProducto.GetComponent<ProductItem> ();
			ProductItem.idProductItem = product.idProduct;
			ProductItem.imgProduct.sprite = product.imgProducto;
			ProductItem.nameProduct.text = product.nameProduct;
			ProductItem.priceProduct.text = MenuUtils.FormatPawprintsProducts(product.priceProduct);
			ProductItem.valCurrency = product.priceProduct;
			ProductItem.descProduct.text = product.descProduct;
			ProductItem.idStoreGooglePlay = product.idInStoreGooglePlay;
			ProductItem.isAvailableInStore = product.isAvailableInStore;
            nuevoProducto.transform.SetParent (containerPaquetes,false);
			if(product.isAvailableInStore){
				// it could be purchased only in Online Store
				itemsAvailableInStore.Add (ProductItem);
			} else { 
				// it could be purchased only with items like Hulleas, yinyangs ....
				itemsNotAvailableStore.Add (ProductItem); 
			}
            ProductItem.UpdateTextTranslation();
			btnsSlide.Add (ProductItem);
		}
	}

	/// <summary>
	/// Populates the upgrades panel.
	/// </summary>
	/// <param name="products">Products.</param>
	private void PopulateUpgrades(List<UpgradesStore> products){

        LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation();

        //creates header of current elemnts
        BuildHeaderProducts(lm.GetString("text_header_product_upgrades"));

        //creates body of products
		foreach(var product in products){
			GameObject nuevoProducto = Instantiate (btnProductUpgrade) as GameObject;
			ProductUpgradeItem ProductItem = nuevoProducto.GetComponent<ProductUpgradeItem> ();

			ProductItem.idProductItem = product.idProduct;
			ProductItem.imgProduct.sprite = product.imgProducto;
			ProductItem.nameProduct.text = product.nameProduct;
			ProductItem.priceProduct.text = MenuUtils.FormatPriceProducts(product.priceProduct);
			ProductItem.valCurrency = product.priceProduct;
			ProductItem.descProduct.text = product.descProduct;
			ProductItem.idStoreGooglePlay = product.idInStoreGooglePlay;
			ProductItem.isAvailableInStore = product.isAvailableInStore;
			ProductItem.levelsUpgradesIdGooglePlay = product.levelsUpgradesIdGooglePlay;
            nuevoProducto.transform.SetParent (containerPaquetes,false);
			ProductItem.SetChildId (product.idProduct);
			if(product.isAvailableInStore){
				// it could be purchased only in Online Store
				itemsAvailableInStore.Add (ProductItem);
			} else { 
				// it could be purchased only with items like Hulleas, yinyangs ....
				itemsNotAvailableStore.Add (ProductItem); 
			}
            ProductItem.UpdateTextTranslation();
			btnsSlide.Add (ProductItem);
		}
	}


    private void BuildHeaderProducts(string text){

        GameObject header = Instantiate(headerProducts) as GameObject;
        header.GetComponentInChildren<Text>().text = text;
        header.transform.SetParent(containerPaquetes, false);
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
	public void ShowCharactersPanel(){
        AnimPanelOut(backgroundPackagesAnim,IsMovePanelFromRightCharacters);
        StartCoroutine(WaitForIn(backgroundCharactersAnim,IsMovePanelFromRightCharacters));

        IsMovePanelFromRightCharacters = !IsMovePanelFromRightCharacters;
	}

    /// <summary>
    /// Shows the powerups panel.
    /// </summary>
    public void ShowPowerupsPanel(){
        AnimPanelOut(backgroundCharactersAnim,IsMovePanelFromRightPackages);
        StartCoroutine(WaitForIn(backgroundPackagesAnim,IsMovePanelFromRightPackages));

        IsMovePanelFromRightPackages = !IsMovePanelFromRightPackages;
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
		txtElemts.Append("Huellas: "+ GameItemsManager.GetValueById(GameItemsManager.Item.NumPawprints).ToString());
		txtElemts.Append(", Multiplicadores: "+ GameItemsManager.GetValueById(GameItemsManager.Item.NumMultipliers).ToString());
		txtElemts.Append(", Chuletas: "+ GameItemsManager.GetValueById(GameItemsManager.Item.NumMultipliers).ToString());
		txtElemts.Append(", Magnetos: "+  GameItemsManager.GetValueById(GameItemsManager.Item.NumMagnets).ToString());
		txtElemts.Append(", YinYangs: "+ GameItemsManager.GetValueById(GameItemsManager.Item.NumYinYangs).ToString());
		Debug.Log (txtElemts);
		Text contHuellasText = contadorHuellas.GetComponentInChildren<Text> ();
		contHuellasText.text = txtElemts.ToString();
	}

	/// <summary>
	/// Handles OnChangeCharcater event  on infinite-scroll.
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
        GameItemsManager.Character en = (GameItemsManager.Character)Enum.Parse(typeof(GameItemsManager.Character), chItem.idStoreGooglePlay);

        //If character was purchased avoid buy again
        if(!GameItemsManager.isLockedCharacter(en)){
            BuildDialogMessage("msg_store_title_popup", "msg_err_purchase_again", DialogMessage.typeMessage.ERROR);
            return;
        }
        //proceeds to buy on store online
		if (chItem.isAvailableInStore) {
			Debug.Log ("Comprando en IAP...");
			iapManager.BuyInStore (chItem);
		} else {
			//Caso de que se compren personajes con huellas yong yangs etc. aqui se maneja el pago.
		}
	}

	/// <summary>
	/// Checks the num items available to play.
	/// </summary>
	/// <returns><c>true</c>, if items available to play was checked, <c>false</c> otherwise.</returns>
	/// <param name="numHuellas">Number huellas.</param>
	/// <param name="numYingYangs">Number ying yangs.</param>
	private bool checkItemsAvailableToPlay(int numHuellas, int numYingYangs){
		if (numHuellas <= GameItemsManager.GetValueById (GameItemsManager.Item.NumPawprints)
		    && numYingYangs <= GameItemsManager.GetValueById (GameItemsManager.Item.NumYinYangs)) {
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

			if (globalCostPowerUp <= GameItemsManager.GetValueById (GameItemsManager.Item.NumPawprints)) {
				
                //subtracts pawprints
				substractElements (globalCostPowerUp, 0, 0, 0);
                Debug.Log("Compra exitosa !! Ahora ya cuentas con el PowerUp " + currentBought.ToString());

                //Save new PowerUp
                GameItemsManager.SetPowerUpValue(currentBought.idProductItem, 1);
                Debug.Log("Se ha guardado 1 nuevo power up .. "+ currentBought.idProductItem.ToString() +", Ahora ya cuentas con "+GameItemsManager.GetPowerUpValue(currentBought.idProductItem));

                BuildDialogMessage ("msg_store_title_popup","msg_info_success_purchased",DialogMessage.typeMessage.INFO);

			} else {
                BuildDialogMessage ("msg_store_title_popup","msg_err_insuficient_resources",DialogMessage.typeMessage.ERROR);
			}
			Debug.Log (contadorHuellas.GetComponentInChildren<Text> ().text);
		}

        //Va a la tienda de Google play
		if (pi is ProductUpgradeItem){
			ProductUpgradeItem currentBought = ((ProductUpgradeItem)pi);
			Debug.Log ("[StoreManager] Upgrade a comprar..."+currentBought);
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
            BuildDialogMessage ("msg_store_title_popup","msg_err_init_api_iap",DialogMessage.typeMessage.ERROR);
		}

	}

	/// <summary>
	/// Handles the IAP events showing dialog message popup.
	/// </summary>
	/// <param name="messageResult">Message result.</param>
	/// <param name="isError">If set to <c>true</c> is error.</param>
	private void HandleIAPEvents(string messageResult, bool isError){
        string titlePopup = "msg_store_title_popup";
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
        Debug.Log("Menssage from store: "+messageResult);
		
        //If was successful purchased a product from IAP
        if (ite is ProductItem) {
			ProductItem currentProduct = ((ProductItem)ite);

            //If it's some Package.
			if (currentProduct is ProductPackagesItem) {
				ProductPackagesItem currentPck= ((ProductPackagesItem)currentProduct);
				Debug.Log ("Paquete pagado-> "+currentPck.ToString());
				
                switch(currentPck.idProductItem){

                    //if was purchased package 1
                    case GameItemsManager.StoreProduct.Package1:
                        //200 huellas + 2 multiplicadores + 2 chuletas
                        addElements(200, 2, 2, 0);
                        break;

                    //if was purchased package 2
                    case GameItemsManager.StoreProduct.Package2:
                        //500 huellas + 5 multiplicadores + 3 chuletas + 2 magnetos
                        addElements(500, 5, 3, 2);
                        break;
                    
                    //if was purchased package 3
                    case GameItemsManager.StoreProduct.Package3:
                        //1000 huellas + 10 multiplicadores + 5 chuletas + 5 magnetos
                        addElements(1000, 10, 5, 5);
                        break;

                    //if was purchased package 4
                    case GameItemsManager.StoreProduct.Package4:
                        //15,000 huellas + 10 multiplicadores + 10 chuletas + 10 magnetos
                        addElements(15000, 10, 10, 10);
                        break;
                }
			}

            //If it's some Upgrade.
            if (currentProduct is ProductUpgradeItem){
                ProductUpgradeItem currentUpg = ((ProductUpgradeItem)currentProduct);
                Debug.Log("Upgrade pagado-> " + currentUpg);
                //increments in 1 after purchased current upgrade.
                GameItemsManager.SetUpgradeValue(currentUpg.idProductItem,1);
                Debug.Log("Se guarda y actualiza el valor del upgrade "+currentUpg.idProductItem.ToString() + ", ahora tiene un valor: "+GameItemsManager.GetUpgradeValue(currentUpg.idProductItem));
                currentUpg.UpdatePriceAndProgress();

            }

            BuildPopupPurchasedProduct (currentProduct);
		} 

        //If was successful purchased a character from IAP
		if (ite is CharacterItem) {
			CharacterItem currentCharacter = ((CharacterItem)ite);

            GameItemsManager.Character ch = (GameItemsManager.Character)Enum.Parse(typeof(GameItemsManager.Character), currentCharacter.idStoreGooglePlay);
            //Unlock character
            GameItemsManager.SetUnlockCharacter(ch);
            if(!GameItemsManager.isLockedCharacter(ch)){
                Debug.Log("Se ha desbloqueado a "+ch.ToString());
            } else{
                BuildDialogMessage("msg_store_title_popup", "msg_err_fails_unlock_character", DialogMessage.typeMessage.ERROR);
                Debug.Log("No se pudo desbloquear a "+ch.ToString());
            }
			BuildPopupPurchasedCharacter (currentCharacter);
		}


	}

	/// <summary>
	/// Handles the product limit buy reached.
	/// </summary>
	/// <param name="msg">Message.</param>
	private void HandleProductLimitBuyReached(string msg){
        BuildDialogMessage ("msg_store_title_popup", msg, DialogMessage.typeMessage.ERROR);
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
    private ProductStore searchImgProduct(GameItemsManager.StoreProduct id){
		ProductStore prodAux = null;
		foreach (ProductStore prod in packagesStore) {
			if (prod.idProduct == id) {
				prodAux = prod;
				return prodAux;
			}
		}
		foreach (ProductStore prod in powerUpsStore) {
			if (prod.idProduct == id) {
				prodAux = prod;
				return prodAux;
			}
		}
		foreach (ProductStore prod in upgradesStore) {
			if (prod.idProduct == id) {
				prodAux = prod;
				return prodAux;
			}
		}
		return prodAux;
	}


    private void AnimPanelIn(GUIAnim obj, bool isMovePanelFromRight){
     
        obj.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
    }

    private void AnimPanelOut(GUIAnim obj, bool isMovePanelFromRight)
    {
        obj.MoveOut(GUIAnimSystem.eGUIMove.SelfAndChildren);
    }

    private IEnumerator WaitForIn(GUIAnim obj, bool isMovePanelFromRight){
        yield return new WaitForSeconds(1.0f);
        obj.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
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


		GameItemsManager.addValueById (GameItemsManager.Item.NumPawprints,numHuellasPurchased);
		GameItemsManager.addValueById (GameItemsManager.Item.NumMultipliers,numMultiplicadoresPurchased);
		GameItemsManager.addValueById (GameItemsManager.Item.NumPorkchop,numChuletasPurchased);
		GameItemsManager.addValueById (GameItemsManager.Item.NumMagnets,numMagnetosPurchased);
		GameItemsManager.addValueById (GameItemsManager.Item.NumYinYangs,numYingYangs);
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

		GameItemsManager.subtractValueById (GameItemsManager.Item.NumPawprints,numHuellasPurchased);
		GameItemsManager.subtractValueById (GameItemsManager.Item.NumMultipliers,numMultiplicadoresPurchased);
		GameItemsManager.subtractValueById (GameItemsManager.Item.NumPorkchop,numChuletasPurchased);
		GameItemsManager.subtractValueById (GameItemsManager.Item.NumMagnets,numMagnetosPurchased);
		GameItemsManager.subtractValueById (GameItemsManager.Item.NumYinYangs,numYingYangs);
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
        popupMsg.UpdateTextTranslation();
		popupMsg.OpenDialogmessage();
	}

	/// <summary>
	/// Builds the popup purchased ProductItem.
	/// </summary>
	/// <param name="pi">ProductItem</param>
	private void BuildPopupPurchasedProduct(ProductItem pi){
        LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation();
		GameObject mostrarMsg = Instantiate (popupBuy) as GameObject;
		PupUpBuyProduct popupMsg = mostrarMsg.GetComponent<PupUpBuyProduct> ();
        popupMsg.message.text = string.Format(lm.GetString("msg_info_success_store_purchased"), pi.nameProduct.text);
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
        LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation();
		GameObject mostrarMsg = Instantiate (popupBuy) as GameObject;
		PupUpBuyProduct popupMsg = mostrarMsg.GetComponent<PupUpBuyProduct> ();
        popupMsg.message.text = string.Format(lm.GetString("msg_info_success_store_purchased"),pi.nameCharacter.text);
		if (searchImgCharacter (pi.idCharacter) != null) {
			popupMsg.imgProductPurchased.overrideSprite = searchImgCharacter(pi.idCharacter).imgCharacter;
		}
		mostrarMsg.transform.SetParent (mainContainer,false);
		popupMsg.OpenPupup ();
	}
	#endregion

}