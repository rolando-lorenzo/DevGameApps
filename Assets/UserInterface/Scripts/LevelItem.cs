using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LevelItem : MonoBehaviour, ILevelButton {

	#region Class members
	public int id;
	public Button btnGoLevel;
	public Text nameLevel;
    [HideInInspector]
	public string nameScene;
	private RectTransform rectTranform;
	#endregion

	#region Class accesors
	public bool isLocked { get; set; }
	#endregion

	#region MonoBehaviour overrides
	void Awake () {
		rectTranform = GetComponent<RectTransform> ();
		btnGoLevel = GetComponent<Button> ();
		btnGoLevel.onClick.AddListener (() => GoToLevel (nameScene));
	}
	#endregion

	#region Super class overrides
	public override string ToString () {
		return base.ToString () + ": " + id.ToString () + " " + nameScene;
	}
	#endregion


	#region Interface implementation
	public void GoToLevel (string level) {
		Debug.Log ("Cargando escena..." + level);
		SceneManager.LoadScene (level);
	}


	#endregion
}