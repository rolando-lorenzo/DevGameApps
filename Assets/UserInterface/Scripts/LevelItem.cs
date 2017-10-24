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
	public GameObject imgLockLevel;
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
		btnGoLevel.onClick.AddListener (() => GoToLevel(nameScene, id));
	}
	#endregion

	#region Super class overrides
	public override string ToString()
	{
		return base.ToString() + ": " + id.ToString()+ " " +nameScene;
	}
	#endregion


	#region Interface implementation
	public void GoToLevel(string level, int id){
		Debug.Log ("Cargando escena..."+level);
        //SceneManager.LoadScene (level);
        GameItemsManager.SetValueById(GameItemsManager.Item.levelWorld, id);
        SceneManager.LoadScene("GameSceneTest");
	}


	#endregion
}