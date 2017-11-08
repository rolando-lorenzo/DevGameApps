using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class CharacterItem : MonoBehaviour, IStorePurchase {

	#region Class members
    [Header("Setting Character")]
	public int idCharacter;
	public GameObject character;
	public Button btnBuyProduct;
	public Text nameCharacter;
    public GameObject ImageUnlock;

    [HideInInspector]
	public string descCharacter;
	public delegate void ItemPurchasedAction (CharacterItem itemPurchased);
	public event ItemPurchasedAction OnItemPurchased;
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

    public void VerifyUnlockandLockCharacter()
    {
        GameItemsManager.Character en = (GameItemsManager.Character)Enum.Parse(typeof(GameItemsManager.Character), idStoreGooglePlay);
        if (!GameItemsManager.isLockedCharacter(en))
        {
            Debug.Log("esta bloqueado");
            Transform buttonSellCharacter = btnBuyProduct.transform;
            buttonSellCharacter.gameObject.SetActive(false);
            ImageUnlock.SetActive(true);
            return;
        }
        else
        {
            ImageUnlock.SetActive(false);
        }
    }


    public void UpdateTextTranslation()
    {
        LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation();
        if (nameCharacter != null)
        {
            nameCharacter.text = lm.GetString(nameCharacter.text);
        }
        if (descCharacter != null)
        {
            descCharacter = lm.GetString(descCharacter);
        }
        if(btnBuyProduct != null){
            btnBuyProduct.GetComponentInChildren<Text>().text = lm.GetString("text_btn_buy");
        }

    }

	#endregion

}