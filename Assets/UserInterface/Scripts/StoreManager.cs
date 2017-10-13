using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PankioAssets;
using UnityEngine.SceneManagement;

public class ProductStore {
	public string idProduct;
	public Sprite imgProducto;
	public string nameProduct;
	public string priceProduct;
	public string descProduct;
	public string btnBuyText;
	public bool isActive;
}

[System.Serializable]
public class PackagesStore : ProductStore { }

[System.Serializable]
public class PowerUpsStore : ProductStore { }

[System.Serializable]
public class UpgradesStore : ProductStore {
}

[System.Serializable]
public class CharacterStore{
	public int idCharacter;
	public Sprite imgCharacter;
	public Sprite imgBuyCharacter;
	public string storeProductId;
	public string nameCharacter;
	public string descriptionCharacter;
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
	public Transform mainContainer;
	public List<PackagesStore> packagesStore;
	public List<PowerUpsStore> powerUpsStore;
	public List<UpgradesStore> upgradesStore;
	public List<CharacterStore> characters;

	public static StoreManager instance;
	private List<ProductItem> btnsSlide;
	private CharacterItem[] charactersTemplate;
	private InfiniteScroll infiniteScrollCharacters;
	private Text descCharacter;
	#endregion

	#region Class accesors
	public int numeroHuellas{ get; set;}
	#endregion

	#region MonoBehaviour overrides
	void OnEnable()
	{
		infiniteScrollCharacters = charactersPanel.GetComponentInChildren<InfiniteScroll> ();
		infiniteScrollCharacters.OnItemChanged += HandleCurrentCharacter;
		foreach (CharacterItem ch in charactersTemplate) {
			ch.OnItemPurchased += HandleCharacterSuccessPurchased;
		}
		foreach (ProductItem pi in btnsSlide) {
			if (pi is ProductPackagesItem){
				((ProductPackagesItem)pi).OnProductPackageItemPurchased += HandleProductSuccessPurchased;
			}

			if (pi is ProductPowerupItem){
				((ProductPowerupItem)pi).OnProductPowerUpItemPurchased += HandleProductSuccessPurchased;
			}

			if (pi is ProductUpgradeItem){
				((ProductUpgradeItem)pi).OnProductUpgradeItemPurchased += HandleProductSuccessPurchased;
				((ProductUpgradeItem)pi).OnProductUpgradeBuyLimitMax   += HandleProductLimitBuyReached;
			}
		}
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
		PopulatePackages (packagesStore);
		PopulatePowerUps (powerUpsStore);
		PopulateUpgrades (upgradesStore);
		PopulateCharacters ();
		numeroHuellas = 100;
	}

	void Start(){
		
		ShowCountHuellas ();
		StartCoroutine (AnimPanelsPackages ());
	}

	void OnDisable()
	{
		if (infiniteScrollCharacters != null) {
			infiniteScrollCharacters.OnItemChanged -= HandleCurrentCharacter;
		}
		foreach (CharacterItem ch in charactersTemplate) {
			if(ch != null)
				ch.OnItemPurchased -= HandleCharacterSuccessPurchased;
		}

		foreach (ProductItem pi in btnsSlide) {
			if (pi is ProductPackagesItem){
				((ProductPackagesItem)pi).OnProductPackageItemPurchased -= HandleProductSuccessPurchased;
			}

			if (pi is ProductPowerupItem){
				((ProductPowerupItem)pi).OnProductPowerUpItemPurchased -= HandleProductSuccessPurchased;
			}

			if (pi is ProductUpgradeItem){
				((ProductUpgradeItem)pi).OnProductUpgradeItemPurchased -= HandleProductSuccessPurchased;
				((ProductUpgradeItem)pi).OnProductUpgradeBuyLimitMax   -= HandleProductLimitBuyReached;
			}
		}

	}
	#endregion

	#region Super class overrides
	#endregion

	#region Class implementation
	private void PopulatePackages(List<PackagesStore> products){
		
		foreach(var product in products){
			GameObject nuevoProducto = Instantiate (btnProductPackage) as GameObject;
			ProductItem ProductItem = nuevoProducto.GetComponent<ProductItem> ();
			ProductItem.idProductItem = product.idProduct;
			ProductItem.imgProduct.sprite = product.imgProducto;
			ProductItem.nameProduct.text = product.nameProduct;
			ProductItem.priceProduct.text = product.priceProduct;
			ProductItem.descProduct.text = product.descProduct;
			ProductItem.btnBuy.GetComponentInChildren<Text> ().text = product.btnBuyText;
			//ProductStore currentProd = product;
			//ProductItem.btnBuy.onClick.AddListener(() => BuyProduct(currentProd));
			nuevoProducto.transform.SetParent (containerPaquetes,false);
			btnsSlide.Add (ProductItem);
		}
	}

	public void PopulateCharacters(){
		foreach (CharacterStore character in characters) {
			foreach (CharacterItem characterTemplate in charactersTemplate) {
				if(character.idCharacter == characterTemplate.idCharacter){
					characterTemplate.nameCharacter.text = character.nameCharacter;
					characterTemplate.btnBuyProduct.image.overrideSprite = character.imgBuyCharacter;
					characterTemplate.descCharacter = character.descriptionCharacter;
					Image currentImg = characterTemplate.character.GetComponent<Image> ();
					currentImg.overrideSprite = character.imgCharacter;
				}	
			}
		}
		if (characters.Count > 0) {
			descCharacter.text = characters [0].descriptionCharacter;
		}

	}

	private void PopulatePowerUps(List<PowerUpsStore> products){
		
		foreach(var product in products){
			GameObject nuevoProducto = Instantiate (btnProductPowerup) as GameObject;
			ProductItem ProductItem = nuevoProducto.GetComponent<ProductItem> ();
			ProductItem.idProductItem = product.idProduct;
			ProductItem.imgProduct.sprite = product.imgProducto;
			ProductItem.nameProduct.text = product.nameProduct;
			ProductItem.priceProduct.text = product.priceProduct;
			ProductItem.descProduct.text = product.descProduct;
			ProductItem.btnBuy.GetComponentInChildren<Text> ().text = product.btnBuyText;
			//ProductStore currentProd = product;
			//ProductItem.btnBuy.onClick.AddListener(() => BuyProduct(currentProd));
			nuevoProducto.transform.SetParent (containerPowerUps,false);
			btnsSlide.Add (ProductItem);
		}
	}

	private void PopulateUpgrades(List<UpgradesStore> products){

		foreach(var product in products){
			GameObject nuevoProducto = Instantiate (btnProductUpgrade) as GameObject;
			ProductUpgradeItem ProductItem = nuevoProducto.GetComponent<ProductUpgradeItem> ();

			ProductItem.idProductItem = product.idProduct;
			ProductItem.imgProduct.sprite = product.imgProducto;
			ProductItem.nameProduct.text = product.nameProduct;
			ProductItem.priceProduct.text = product.priceProduct;
			ProductItem.descProduct.text = product.descProduct;
			ProductItem.btnBuy.GetComponentInChildren<Text> ().text = product.btnBuyText;
			//ProductStore currentProd = product;
			//ProductItem.btnBuy.onClick.AddListener(() => BuyProduct(currentProd));
			nuevoProducto.transform.SetParent (containerUpgrades,false);
			ProductItem.SetChildId (product.idProduct);
			btnsSlide.Add (ProductItem);
		}
	}


	public void BuyProduct(ProductStore p){
		
		Debug.Log ("Comprando un producto..."+p.nameProduct);
	}

	public void CloseAllButtons (ProductItem btnCurrent) {
		Debug.Log ("cerrando todos botones...");
		foreach (ProductItem btn in btnsSlide) {
			if (btn.isOpen && btn != btnCurrent) {
				Debug.Log ("entrando if cerrar");
				btn.Close ();
			}
		}
			
	}

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

	public void ShowCountHuellas(){
		Text contHuellasText = contadorHuellas.GetComponentInChildren<Text> ();
		contHuellasText.text = numeroHuellas.ToString();
	}

	private void HandleCurrentCharacter(ScrollItem previousItem, ScrollItem currentItem, int itemIndex){
		if(currentItem != null){
			CharacterItem c = currentItem.gameObject.GetComponentInChildren<CharacterItem> ();
			descCharacter.text = c.descCharacter;
		}
	}

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

	private void HandleCharacterSuccessPurchased(CharacterItem chItem){
		Debug.Log ("Manager event success purchased: "+chItem.nameCharacter.text);

		GameObject mostrarMsg = Instantiate (popupBuy) as GameObject;
		PupUpBuyProduct popupMsg = mostrarMsg.GetComponent<PupUpBuyProduct> ();
		popupMsg.message.text = "Felicidades !! Has comprado a "+chItem.nameCharacter.text;
		if (searchImgCharacter (chItem.idCharacter) != null) {
			popupMsg.imgProductPurchased.overrideSprite = searchImgCharacter(chItem.idCharacter).imgCharacter;
		}
		mostrarMsg.transform.SetParent (mainContainer,false);
		popupMsg.OpenPupup ();

	}

	private void HandleProductSuccessPurchased(ProductItem pi){
		Debug.Log ("---- Manager event success purchased ---");
		if (pi is ProductPackagesItem){
			ProductPackagesItem currentBought = ((ProductPackagesItem)pi);
			Debug.Log ("[StoreManager] Paquete comprado"+currentBought.idProductItem +" "+ currentBought.nameProduct.text);
		}

		if (pi is ProductPowerupItem){
			ProductPowerupItem currentBought = ((ProductPowerupItem)pi);
			Debug.Log ("[StoreManager] Powerup comprado"+currentBought.idProductItem +" "+ currentBought.nameProduct.text);
		}

		if (pi is ProductUpgradeItem){
			ProductUpgradeItem currentBought = ((ProductUpgradeItem)pi);
			Debug.Log ("[StoreManager] Upgrade comprado"+currentBought.idProductItem +" "+ currentBought.nameProduct.text);
		}

		BuildPopupPurchasedProduct (pi);
	}

	private void HandleProductLimitBuyReached(string msg){
		Debug.Log ("[StoreManager] Event limit buy max reached: "+msg);
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

	private void ToggleActivePackagesPanels(){
		packagesPanel.SetActive(!packagesPanel.activeSelf);
		powerUpsPanel.SetActive(!powerUpsPanel.activeSelf);
		upgradesPanel.SetActive(!upgradesPanel.activeSelf);
	}

	private IEnumerator AnimPanelsPackages() {
		yield return new WaitForSeconds (0.5f);
		packagesPanelAnim.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
		powerUpsPanelAnim.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
		upgradesPanelAnim.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);

	}
	#endregion

}