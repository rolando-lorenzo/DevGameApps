using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ProductItem : MonoBehaviour {

	#region Class members
	public string idProductItem;
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
	public bool isOpen { get; set; }
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
	#endregion

}