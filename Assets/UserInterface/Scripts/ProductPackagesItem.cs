using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductPackagesItem : ProductItem {

	#region Class members
	public delegate void ProductItemPurchasedAction (ProductPackagesItem productItemPurchased);
	public event ProductItemPurchasedAction OnProductPackageItemPurchased;
	#endregion


	#region MonoBehaviour overrides
	void Awake () {
		base.InitComponets ();
		btnBuy.onClick.AddListener (() => BuyProductItem(this));
	}
	#endregion


	#region Class implementation
	public void BuyProductItem(ProductPackagesItem pItem){
		Debug.Log ("Comprando Paquete..."+pItem.idProductItem+" "+pItem.nameProduct.text);
		if (OnProductPackageItemPurchased != null)
			OnProductPackageItemPurchased (pItem);

	}
	#endregion

}