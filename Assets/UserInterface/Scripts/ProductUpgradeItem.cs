using System;
using System.Collections.Generic;
using System.Linq;
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
    public List<Array> listPrice { get; set; }
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
        Debug.Log("PowerUpgradeLevel "+currentVal + " id" + idProductPower);
        //float val = valCurrency*(currentVal);
        priceProduct.text = GetPriceUpgrade(currentVal);
        storeUpProg.ChangeImgsColor();
	}

    private string GetPriceUpgrade(int idStatePowerUpgrade)
    {
        string price = "";
        foreach(string[] itemproduct in listPrice)
        {
            if (itemproduct[0].Contains(idStatePowerUpgrade.ToString()))
            {
                Debug.Log("Cotains Id in UpgradeStore: " + itemproduct[0] + " - id: " + idStatePowerUpgrade);
                price = itemproduct[1];
                return price;
            }
        }

        if (string.IsNullOrEmpty(price))
        {
            Array last = listPrice.Last();
            price = last.GetValue(1).ToString();
            Debug.Log("Cotains Id in UpgradeStoreLAST: " + last.GetValue(1).ToString() + " - id: " + idStatePowerUpgrade);
        }

        return price;
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