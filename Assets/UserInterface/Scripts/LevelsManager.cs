using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

[System.Serializable]
public class Level {
	public int idLevel;
	public Sprite imgLevel;
	public string nameLevel;
	public string nameScene;
	public bool isLocked;
}

[System.Serializable]
public class World {
	public string id;
	public string nameWorld;
	public List<Level> levels;
}

public class LevelsManager : MonoBehaviour {

	#region Class members
	public GameObject panelWorldUSA;
	public GameObject panelWorldMexico;
	public GameObject panelWorldAfrica;
	public GameObject panelWorldChina;
	public List<World> worlds;
	private LevelItem[] btnsLevels;
	private GUIAnim[] btnsLevelsAnim;
	private GameObject currentPanel;
	private Button btnBack;
	private Text txtHeaderWorld;
	#endregion


	#region Class accesors
	private string currentWorld { set; get; }
	private int maxLevels { set; get; }

	#endregion

	#region MonoBehaviour overrides
	void Awake () {
		currentWorld = GameItemsManager.GetValueStringById (GameItemsManager.Item.WorldName);

        maxLevels = ProgressWorldLevel.GetLevelWorl( ProgressWorldLevel.GetWorldsEnum(currentWorld));

		switch (ProgressWorldLevel.GetWorldsEnum(currentWorld)) {
			case ProgressWorldLevel.WorldsNames.Circus:
				currentPanel = panelWorldUSA;
				break;
			case ProgressWorldLevel.WorldsNames.Train:
				currentPanel = panelWorldMexico;
				break;
			case ProgressWorldLevel.WorldsNames.Zoo:
				currentPanel = panelWorldAfrica;
				break;
			case ProgressWorldLevel.WorldsNames.Manssion:
				currentPanel = panelWorldChina;
				break;
		}

		currentPanel.SetActive (true);
		btnsLevels = currentPanel.GetComponentsInChildren<LevelItem> ();
		btnsLevelsAnim = currentPanel.GetComponentsInChildren<GUIAnim> ();
		Transform btnTrsform = currentPanel.transform.Find ("ButtonBack");
		if (btnTrsform != null) {
			btnBack = btnTrsform.gameObject.GetComponent<Button> ();
			btnBack.onClick.AddListener (GoWorldScene);
		}
		

		if (enabled) {
			GUIAnimSystem.Instance.m_AutoAnimation = false;
		}
	}

	void Start () {
		PopulateLevels ();
		StartCoroutine (AnimMoveInBnts ());
	}
	#endregion


	#region Class implementation
	/// <summary>
	/// Populates the levels.
	/// </summary>
	public void PopulateLevels () {
		Debug.Log ("Llenando niveles");
		foreach (LevelItem level in btnsLevels) {
			foreach (World world in worlds) {
				if (string.Equals (world.nameWorld, currentWorld)) {
					// Debug.Log(world.nameWorld);
					foreach (Level lb in world.levels) {
						if (lb.idLevel == level.id) {
							//Debug.Log("idLevel: "+lb.idLevel);
							level.btnGoLevel.image.overrideSprite = lb.imgLevel;
							level.nameLevel.text = lb.idLevel.ToString ();
							level.nameScene = lb.nameScene;
							if (maxLevels >= level.id) {
								level.btnGoLevel.interactable = true;
							}
							else {
								level.btnGoLevel.interactable = false;
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Animations the bnts levels.
	/// </summary>
	public void AnimBntsLevels () {
		foreach (GUIAnim guiAnim in btnsLevelsAnim) {
			guiAnim.MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);
		}
	}

	/// <summary>
	/// Coroutine Animations the move in bnts.
	/// </summary>
	/// <returns>The move in bnts.</returns>
	private IEnumerator AnimMoveInBnts () {
		yield return new WaitForSeconds (0f);
		AnimBntsLevels ();
	}


	void GoWorldScene () {
		
		Debug.Log ("Cargando WorldScene!...");
		SceneManager.LoadScene ("WorldScene");
	}
	#endregion

	#region Interface implementation
	#endregion
}