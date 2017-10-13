using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductPowerupItem : ProductItem {

	#region Class members
	public delegate void ProductItemPurchasedAction (ProductPowerupItem productItemPurchased);
	public event ProductItemPurchasedAction OnProductPowerUpItemPurchased;
	#endregion


	#region MonoBehaviour overrides
	void Awake () {
		base.InitComponets ();
		btnBuy.onClick.AddListener (() => BuyProductItem(this));
	}
	#endregion


	#region Class implementation
	public void BuyProductItem(ProductPowerupItem pItem){
		Debug.Log ("Comprando PowerUp..."+pItem.idProductItem+" "+pItem.nameProduct.text);
		if (OnProductPowerUpItemPurchased != null)
			OnProductPowerUpItemPurchased (pItem);

	}
	#endregion
}