using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class CharacterItem : MonoBehaviour, IStorePurchase {

	#region Class members
	public int idCharacter;
	public GameObject character;
	public Button btnBuyProduct;
	public Text nameCharacter;
	public string descCharacter;
	public delegate void ItemPurchasedAction (CharacterItem itemPurchased);
	public event ItemPurchasedAction OnItemPurchased;
	public const int CHARACTER_LOCK = 1;
	public const int CHARACTER_UNLOCK = 2;
	#endregion

	#region Class accesors
	public bool isPurchased { set; get; }
	public string idStoreGooglePlay{ get; set;} 
	public bool isAvailableInStore{ get; set;}
	#endregion

	#region MonoBehaviour overrides
	private void Awake () {
		btnBuyProduct = character.GetComponentInChildren<Button> ();
		nameCharacter = character.GetComponentInChildren<Text> ();
		btnBuyProduct.onClick.AddListener (() => BuyCharacter(this));
		InactivateButtonBuyIfUnlocked ();

	}
	#endregion

	#region Super class overrides
	public override string ToString()
	{
		return base.ToString() + ": Personaje: " + idCharacter.ToString();
	}
	#endregion

	#region Class implementation
	public void BuyCharacter(CharacterItem character){
		if (OnItemPurchased != null)
			OnItemPurchased (character);
	}

	public void InactivateButtonBuyIfUnlocked(){
		try{
			GameItemsManager.Item itCharacter = (GameItemsManager.Item) Enum.Parse(typeof(GameItemsManager.Item), idStoreGooglePlay); 
			Debug.Log("Status character "+itCharacter+" "+GameItemsManager.GetValueById(itCharacter));
			if(GameItemsManager.GetValueById(itCharacter) == CHARACTER_UNLOCK){
				btnBuyProduct.gameObject.SetActive(false);
			}
		}catch(ArgumentException){
			Debug.Log ("Error al convertir enum.");
		}
	}

	#endregion

}