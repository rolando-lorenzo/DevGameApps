using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Level {
	public int idLevel;
	public Sprite imgLevel;
	public string nameLevel;
	public Sprite imgLockLevel;
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
	public string currentWorld { set; get; }

	#endregion

	#region MonoBehaviour overrides
	void Awake () {
        PlayConstant constant = new PlayConstant();
		currentWorld = PlayerPrefs.GetString (constant.worldName, "Circus");

        char[] split = { '/', '-' };
        string progrs = PlayerPrefs.GetString("progressLevels");
        Debug.Log(progrs);
        string[] progresslevels = PlayerPrefs.GetString("progressLevels").Split(split);


        switch (currentWorld) {
			case "Circus":
				currentPanel = panelWorldUSA;
				break;
			case "Train":
				currentPanel = panelWorldMexico;
				break;
			case "Zoo":
				currentPanel = panelWorldAfrica;
				break;
			case "Mansion":
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
		Transform txtHeaderTrsform = currentPanel.transform.Find ("TextHeaderWorld");
		if (txtHeaderTrsform != null) {
			txtHeaderWorld = txtHeaderTrsform.gameObject.GetComponent<Text> ();
			txtHeaderWorld.text = currentWorld;
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
							level.imgLockLevel.sprite = lb.imgLockLevel;
							level.nameLevel.text = lb.nameLevel;
							level.nameScene = lb.nameScene;
                            
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
		if (currentPanel != null) {
			currentPanel.SetActive (false);
		}

		Debug.Log ("Cargando WorldScene!...");
		SceneManager.LoadScene ("WorldScene");
	}
	#endregion

	#region Interface implementation
	#endregion
}