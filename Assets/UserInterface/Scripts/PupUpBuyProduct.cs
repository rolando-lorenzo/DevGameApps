using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PupUpBuyProduct : MonoBehaviour {

	#region Class members

	public GameObject popupSelfBuy;
	public Text message ;
	public Image imgProductPurchased;
	public Button btnClosePopup;
	private GUIAnim animPopup;

	#endregion

	#region Class accesors
	public bool isError{ set; get;}
	#endregion

	#region MonoBehaviour overrides
	void Awake () {
		animPopup = popupSelfBuy.GetComponent<GUIAnim> ();
		btnClosePopup.onClick.AddListener (() => ClosePupup());
	}
	void Start(){
	}
	#endregion

	#region Super class overrides
	public override string ToString()
	{
		return base.ToString() + ": " + message.text.ToString();
	}
	#endregion

	#region Class implementation
	public void OpenPupup(){
		if (popupSelfBuy != null) {
			popupSelfBuy.SetActive (true);
		}
		animPopup.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
	}

	public void ClosePupup(){
		animPopup.MoveOut(GUIAnimSystem.eGUIMove.SelfAndChildren);
		StartCoroutine (InactivePopup());
	}

	private IEnumerator InactivePopup()
	{
		Debug.Log("Desactivando StorePopup..");
		yield return new WaitForSeconds(2.0f);
		if (popupSelfBuy != null) {
			popupSelfBuy.SetActive (false);
			GameObject.Destroy (popupSelfBuy);
		}
	}

    public void UpdateTextTranslation()
    {
        LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation();
        if (message != null)
        {
            message.text = lm.GetString(message.text);
        }
    }
	#endregion


}