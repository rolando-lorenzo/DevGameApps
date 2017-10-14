using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class WorldManager : MonoBehaviour {

    #region Class members
    public GUIAnim imgWorld;
    public GUIAnim btnBulletCircus;
    public GUIAnim btnBulletTrain;
    public GUIAnim btnBulletZoo;
    public GUIAnim btnBulletMansion;

    //Buttons of conform Scene
    public GUIAnim btnBack;
    public GUIAnim btnGoIn;
    public GUIAnim btnNameWorld;
    public GUIAnim nameSingGUIAnim;


    //Button Actions
    public Button buttonBack;
    public GameObject buttonGoIn;

    public GameObject buttonGoLevelCircus;
    public GameObject buttonGoLevelTrain;
    public GameObject buttonGoLevelZoo;
    public GameObject buttonGoLevelMansion;

    public GameObject nameWorld;

    //variables
    private string nameworldtext { get; set; }
    private string progressLevels { get; set; }
	#endregion

	#region Class accesors
	#endregion

	#region MonoBehaviour overrides
	void Awake () {
		if (enabled) {
			GUIAnimSystem.Instance.m_AutoAnimation = false;
		}
        nameworldtext = "Circus";
        PlayConstant constant = new PlayConstant();
        progressLevels = PlayerPrefs.GetString(constant.gameProgressLevel, "Circus-1/Train-1/Zoo-1/Mansion-1");
    }

	void Start () {
		//Button of Back
		Button btnback = buttonBack.GetComponent<Button> ();
		btnback.onClick.AddListener (GoMainScene);

		//Button of Go In
		Button btngoin = buttonGoIn.GetComponent<Button> ();
		btngoin.onClick.AddListener (GoLevelScene);

		//Buttons Of levels
		Button btnlevelusa = buttonGoLevelCircus.GetComponent<Button> ();
		btnlevelusa.onClick.AddListener (() => ChangeNameWorld (1));

		Button btnlevelmex = buttonGoLevelTrain.GetComponent<Button> ();
		btnlevelmex.onClick.AddListener (() => ChangeNameWorld (2));

		Button btnlevelafr = buttonGoLevelZoo.GetComponent<Button> ();
		btnlevelafr.onClick.AddListener (() => ChangeNameWorld (3));

		Button btnlevelch = buttonGoLevelMansion.GetComponent<Button> ();
		btnlevelch.onClick.AddListener (() => ChangeNameWorld (4));


		imgWorld.MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);
		btnBack.MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);
		btnGoIn.MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);
		btnNameWorld.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
        nameSingGUIAnim.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
		StartCoroutine (MoveInBulletsWorld ());
	}


	void Update () {

	}
	#endregion

	#region Super class overrides
	#endregion

	#region Class implementation
	private IEnumerator MoveInBulletsWorld () {
		//Wait 2frame
		yield return new WaitForSeconds (1.0f);

		// MoveIn all buttons
		btnBulletCircus.MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);
		btnBulletTrain.MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);
		btnBulletZoo.MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);
		btnBulletMansion.MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);


	}


	void GoMainScene () {
		Debug.Log ("Go Main Scene!");
		SceneManager.LoadScene ("MainScene");
	}

	private void ChangeNameWorld (int world) {
		Text nameworld = nameWorld.GetComponent<Text> ();
		switch (world) {
			case 1:
				//name the world in Xml
				nameworldtext = "Circus";
				break;
			case 2:
				nameworldtext = "Train";
				break;
			case 3:
				nameworldtext = "Zoo";
				break;
			case 4:
				nameworldtext = "Mansion";
				break;
		}
		nameworld.text = nameworldtext;
	}

	void GoLevelScene () {
		if (!String.IsNullOrEmpty (nameworldtext)) {
            PlayConstant constant = new PlayConstant();
            string nameWorld = constant.worldName;
			PlayerPrefs.SetString (nameWorld, nameworldtext);
            PlayerPrefs.SetString(constant.gameProgressLevel, progressLevels);
			//string namescene = "LevelScene" + this.nameworld;
			string namescene = "LevelScene";
            PlayerPrefs.Save();
			SceneManager.LoadScene (sceneName: namescene);
		}
		Debug.Log ("Dont select Level!");

	}

	#endregion

	#region Interface implementation
	#endregion
}
