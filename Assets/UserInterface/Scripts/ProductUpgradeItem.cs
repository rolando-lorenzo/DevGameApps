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
	#endregion

	#region MonoBehaviour overrides
	void Awake () {
		
		base.InitComponets ();
		storeUpProg = progressBar.GetComponent<StoreUpgradeProgress> ();
		btnBuy.onClick.AddListener (() => BuyProductItem(this));
	}
	#endregion


	#region Class implementation
	public void BuyProductItem(ProductUpgradeItem pItem){
		if (pItem.storeUpProg.IncrementProgress ()) {
			Debug.Log ("Comprando item Upgrade..." + pItem.idProductItem + " " + pItem.nameProduct.text);
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


	public void SetChildId(string id){
		storeUpProg.idUpgrade = id;
	}
	#endregion

}