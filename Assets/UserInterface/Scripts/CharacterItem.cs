using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterItem : MonoBehaviour {

	#region Class members
	public int idCharacter;
	public GameObject character;
	public Button btnBuyProduct;
	public Text nameCharacter;
	public string descCharacter;
	public delegate void ItemPurchasedAction (CharacterItem itemPurchased);
	public event ItemPurchasedAction OnItemPurchased;
	#endregion

	#region Class accesors
	public bool isPurchased { set; get; }
	#endregion

	#region MonoBehaviour overrides
	private void Awake () {
		btnBuyProduct = character.GetComponentInChildren<Button> ();
		nameCharacter = character.GetComponentInChildren<Text> ();
		btnBuyProduct.onClick.AddListener (() => BuyCharacter(this));
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
		Debug.Log ("Comprando personaje... "+character.nameCharacter.text);
		if (OnItemPurchased != null)
			OnItemPurchased (character);
	}

	#endregion

	#region Interface implementation
	#endregion
}