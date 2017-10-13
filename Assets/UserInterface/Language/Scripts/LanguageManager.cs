using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageManager : MonoBehaviour {

	#region Class members
	public SystemLanguage[] idioms;
	private SystemLanguage deviceLanguage;
	#endregion

	#region Class accesors
	public static LanguageManager ins {
		get { return FindObjectOfType<LanguageManager> (); }
	}

	public int languageCount {
		get { return idioms.Length; }
	}

	public int currentLanguage { get; set; }
	#endregion

	#region MonoBehaviour overrides
	private void Awake () {
		deviceLanguage = Application.systemLanguage;

		for (int i = 0; i < idioms.Length; i++) {
			if (idioms[i].ToString () == deviceLanguage.ToString ()) {
				currentLanguage = i;
				break;
			}
		}
	}
	#endregion

	#region Super class overrides
	#endregion

	#region Class implementation
	#endregion

	#region Interface implementation
	#endregion
}