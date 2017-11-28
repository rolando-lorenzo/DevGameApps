using System.Collections.Generic;
using UnityEngine;

public class ProductUpgradeItem : ProductItem {

	#region Class members
    public GameItemsManager.StorePower idProductPower;

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
    void Start(){
        //Update price and color with saved value of progress
        UpdatePriceAndProgress();
    }
	
	#endregion


	#region Class implementation
	public void BuyProductItem(ProductUpgradeItem pItem){

        if (pItem.storeUpProg.IsIncrementableProgress ()) {
			Debug.Log ("Current idInGooglePlay... " + levelsUpgradesIdGooglePlay [(pItem.storeUpProg.currentProgress-1)]);
			pItem.idStoreGooglePlay = levelsUpgradesIdGooglePlay [(pItem.storeUpProg.currentProgress-1)];
			if (OnProductUpgradeItemPurchased != null) {
				OnProductUpgradeItemPurchased (pItem);
			}
				
		} else {
			Debug.Log ("Se ha alcanzado el limite de compra permitido." + pItem.nameProduct.text);
			if (OnProductUpgradeBuyLimitMax != null) {
                OnProductUpgradeBuyLimitMax ("msg_err_purchase_limit_reached");
			}
		}

	}
		
	public void UpdatePriceAndProgress(){
        int currentVal = GameItemsManager.GetPowerUpgradeLevel(idProductPower);
        Debug.Log("PowerUpgradeLevel "+currentVal);
        float val = 0;
        if(currentVal == 1){
            val = 9; //valor en tienda del primer upgrade
        } else {
            val = 19; //valor en tienda del segundo upgrade
        } 
        //float val = valCurrency*(currentVal);
		priceProduct.text = MenuUtils.FormatPriceProducts (val);
        storeUpProg.ChangeImgsColor();
	}

    public void SetChildId(GameItemsManager.StorePower id){
		storeUpProg.idUpgrade = id;
		if (levelsUpgradesIdGooglePlay != null && levelsUpgradesIdGooglePlay.Count > 0) {
			storeUpProg.limitOfUpgrades = levelsUpgradesIdGooglePlay.Count;
		}
	}


    public override string ToString()
    {
        return base.ToString();
    }
	#endregion

}