using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof (LanguageItem))]
public class LanguageItemEditor : Editor {

	#region Class members
	private LanguageItem script;
	#endregion

	#region Class accesors
	#endregion

	#region MonoBehaviour overrides
	private void OnEnable () {
		script = target as LanguageItem;

		if (script.textLanguages.Count < LanguageManager.ins.languageCount) {
			while (script.textLanguages.Count < LanguageManager.ins.languageCount)
				script.textLanguages.Add ("");
		}

		else {
			while (script.textLanguages.Count > LanguageManager.ins.languageCount)
				script.textLanguages.RemoveAt (script.textLanguages.Count - 1);
		}
	}

	public override void OnInspectorGUI () {
		script = (LanguageItem) target;

		EditorGUI.BeginChangeCheck ();
		for (int i = 0; i < script.textLanguages.Count; i++) {
			EditorStyles.textArea.wordWrap = true;
			EditorGUILayout.LabelField (LanguageManager.ins.idioms[i].ToString ());
			script.textLanguages[i] = EditorGUILayout.TextArea (script.textLanguages[i]);
		}

		if (EditorGUI.EndChangeCheck ())
			EditorUtility.SetDirty (script);
	}
	#endregion

	#region Super class overrides
	#endregion

	#region Class implementation
	#endregion

	#region Interface implementation
	#endregion
}