using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageManager : MonoBehaviour {

	#region Class members
	public bool developmentMode;
	public SystemLanguage deviceLanguage;

	public List<SystemLanguage> idioms = new List<SystemLanguage> ();
	#endregion

	#region Class accesors
	public static LanguageManager ins {
		get { return FindObjectOfType<LanguageManager> (); }
	}

	public int languageCount {
		get { return idioms.Count; }
	}

	public int currentLanguage { get; set; }
	#endregion

	#region MonoBehaviour overrides
	private void Awake () {
		if (!developmentMode)
			deviceLanguage = Application.systemLanguage;

		for (int i = 0; i < idioms.Count; i++) {
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