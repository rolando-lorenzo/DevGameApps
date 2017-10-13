using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageItem : MonoBehaviour {

	#region Class members
	private Text text;
	public List<string> textLanguages = new List<string> ();
	#endregion

	#region Class accesors
	#endregion

	#region MonoBehaviour overrides
	private void Start () {
		text = GetComponent<Text> ();
		text.text = textLanguages[LanguageManager.ins.currentLanguage];
	}
	#endregion

	#region Super class overrides
	#endregion

	#region Class implementation
	#endregion

	#region Interface implementation
	#endregion
}