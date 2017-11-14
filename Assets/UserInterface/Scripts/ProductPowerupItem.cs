using UnityEngine;

public class ProductPowerupItem : ProductItem {

	#region Class members
    public GameItemsManager.StorePower idProductPower;

	public delegate void ProductItemPurchasedAction (ProductPowerupItem productItemPurchased);
	public event ProductItemPurchasedAction OnProductPowerUpItemPurchased;
	private SpinnerStorePowerups spinnerPowerUp;
	public int numProductsToBuy { get; set;}
	#endregion


	#region MonoBehaviour overrides
	void Awake () {
		numProductsToBuy = 1;
		base.InitComponets ();
		spinnerPowerUp = GetComponentInChildren<SpinnerStorePowerups> ();
		btnBuy.onClick.AddListener (() => BuyProductItem(this));
	}

	void OnEnable(){
		spinnerPowerUp.OnSpinnerPowerupChange += HandleChangeSpinnerValue;
	}

	void OnDisable(){
		spinnerPowerUp.OnSpinnerPowerupChange -= HandleChangeSpinnerValue;
	}
	#endregion


	#region Class implementation
	public void BuyProductItem(ProductPowerupItem pItem){
		//Debug.Log ("Comprando PowerUp..."+pItem.idProductItem+" "+pItem.nameProduct.text);
		if (OnProductPowerUpItemPurchased != null)
			OnProductPowerUpItemPurchased (pItem);

	}

	private void HandleChangeSpinnerValue(int value){
		numProductsToBuy = value;
		value *= (int)valCurrency;
		priceProduct.text = MenuUtils.FormatPawprintsProducts (value);
	}

    public override string ToString()
    {
        return base.ToString() + ", Num Products To Buy: " + numProductsToBuy.ToString();
    }
		
	#endregion
}