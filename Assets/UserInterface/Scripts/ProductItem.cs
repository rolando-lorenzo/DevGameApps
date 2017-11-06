using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class ProductItem : MonoBehaviour, IStorePurchase {

	#region Class members
    public GameItemsManager.StoreProduct idProductItem;
	public Button btnProduct;
	public Image imgProduct;
	public Text nameProduct;
	public Text descProduct;
	public Text priceProduct;
	public Button btnBuy;

	//Configuracion slide
	public float slideTime;
	public float minY, maxY;
	protected internal RectTransform rectTranform;
	#endregion

	#region Class accesors
	public float valCurrency{ get; set;}
	public bool isOpen { get; set; }
	public string idStoreGooglePlay{ get; set;}
	public bool isAvailableInStore{ get; set;}
	#endregion

	#region MonoBehaviour overrides
	void Awake () {
		InitComponets ();
	}
	#endregion


	#region Class implementation
	public void InitComponets(){
		rectTranform = GetComponent<RectTransform> ();
		btnProduct = GetComponent<Button> ();
		btnProduct.onClick.AddListener (OpenCloseButton);
	}


	public void OpenCloseButton () {
		if (!isOpen) {
			Open ();
			StoreManager.instance.CloseAllButtons (this);
		} else {
			Close ();
		}
			
	}

	protected internal void Open () {
		isOpen = true;
		Tween.FloatTween (gameObject, "ScaleButton", 0, 1, slideTime, (tween) => {
			rectTranform.sizeDelta = new Vector2 (rectTranform.sizeDelta.x, Mathf.Lerp (minY, maxY, tween.Progress));
		});
	}

	protected internal void Close () {
		isOpen = false;
		Tween.FloatTween (gameObject, "ScaleButton", 0, 1, slideTime, (tween) => {
			rectTranform.sizeDelta = new Vector2 (rectTranform.sizeDelta.x, Mathf.Lerp (maxY, minY, tween.Progress));
		});
	}

    public void UpdateTextTranslation()
    {
        LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation();
        if (nameProduct != null)
        {
            nameProduct.text = lm.GetString(nameProduct.text);
        }
        if (descProduct != null)
        {
            descProduct.text = lm.GetString(descProduct.text);
        }
        if(btnBuy != null){
            btnBuy.GetComponentInChildren<Text>().text = lm.GetString("text_btn_buy");
        }
    }

	public override string ToString()
	{
        return base.ToString() + ": Item: " + idProductItem.ToString() +", Name: "+nameProduct.text +", Price: "+priceProduct.text+", ID Google: "+idStoreGooglePlay;
	}
	#endregion

}