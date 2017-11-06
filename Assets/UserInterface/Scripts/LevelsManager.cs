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
	private string currentWorld { set; get; }
    private int maxLevels { set; get; }

    #endregion

    #region MonoBehaviour overrides
    void Awake () {
        currentWorld = GameItemsManager.GetValueStringById(GameItemsManager.Item.WorldName);

        char[] split = { '/', '-' };
        string[] progresslevels = GameItemsManager.GetValueStringById(GameItemsManager.Item.GameProgressLevel).Split(split);

        for (int i = 0; i < progresslevels.Length; i++)
        {
            if (progresslevels[i] == currentWorld)
            {
                maxLevels = Int32.Parse(progresslevels[i + 1]);
            }
        }

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
							level.imgLockLevel.GetComponent<Image>().sprite = lb.imgLockLevel;
							level.nameLevel.text = lb.nameLevel;
							level.nameScene = lb.nameScene;
                            if (maxLevels >= level.id)
                            {
								level.imgLockLevel.gameObject.SetActive(false);
                                //level.btnGoLevel.interactable = true;
                            }
                            else
                            {
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