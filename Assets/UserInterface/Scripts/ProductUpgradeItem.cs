using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductUpgradeItem : ProductItem {

	#region Class members

	public GameObject progressBar;
	private StoreUpgradeProgress storeUpProg;
	public delegate void ProductItemPurchasedAction (ProductUpgradeItem productItemPurchased);
	public event ProductItemPurchasedAction OnProductUpgradeItemPurchased;
	public delegate void ProductItemBuyLimitAction (string msg);
	public event ProductItemBuyLimitAction OnProductUpgradeBuyLimitMax;


	public List<string> levelsUpgradesIdGooglePlay { get; set; }
	#endregion

	#region MonoBehaviour overrides
	void Awake () {
		
		base.InitComponets ();
		storeUpProg = progressBar.GetComponent<StoreUpgradeProgress> ();
		btnBuy.onClick.AddListener (() => BuyProductItem(this));
	}

	void OnEnable(){
		storeUpProg.OnUpgradeProgressChange += HandleUpgradeprogressChange;
	}

	void OnDisable(){
		storeUpProg.OnUpgradeProgressChange -= HandleUpgradeprogressChange;
	}
	#endregion


	#region Class implementation
	public void BuyProductItem(ProductUpgradeItem pItem){
		if (pItem.storeUpProg.IncrementProgress ()) {
			Debug.Log ("Current idInGooglePlay... " + levelsUpgradesIdGooglePlay [(pItem.storeUpProg.currentProgress-1)]);
			pItem.idStoreGooglePlay = levelsUpgradesIdGooglePlay [(pItem.storeUpProg.currentProgress-1)];
			if (OnProductUpgradeItemPurchased != null) {
				OnProductUpgradeItemPurchased (pItem);
			}
				
		} else {
			Debug.Log ("Se ha alcanzado el limite de compra permitido." + pItem.nameProduct.text);
			if (OnProductUpgradeBuyLimitMax != null) {
				OnProductUpgradeBuyLimitMax ("Se ha alcanzado el limite de compra permitido para " + pItem.nameProduct.text);
			}
		}

	}
		
	private void HandleUpgradeprogressChange(int value){
		float val = valCurrency*(value+1);
		priceProduct.text = MenuUtils.FormatPriceProducts (val);
	}

	public void SetChildId(string id){
		storeUpProg.idUpgrade = id;
		if (levelsUpgradesIdGooglePlay != null && levelsUpgradesIdGooglePlay.Count > 0) {
			storeUpProg.limitOfUpgrades = levelsUpgradesIdGooglePlay.Count;
		}
	}
	#endregion

}