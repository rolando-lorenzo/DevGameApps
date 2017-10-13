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


	//Button Actions
	public Button buttonBack;
	private GameObject buttonGoIn;

	private GameObject buttonGoLevelCircus;
	private GameObject buttonGoLevelTrain;
	private GameObject buttonGoLevelZoo;
	private GameObject buttonGoLevelMansion;

	private GameObject nameWorld;

	//variables
	private string nameworldtext { get; set; }
	#endregion

	#region Class accesors
	#endregion

	#region MonoBehaviour overrides
	void Awake () {
		if (enabled) {
			GUIAnimSystem.Instance.m_AutoAnimation = false;
		}

		//find button by name of the button
		buttonGoLevelCircus = GameObject.Find ("ButtonBulletCircus");
		buttonGoLevelTrain = GameObject.Find ("ButtonBulletTrain");
		buttonGoLevelZoo = GameObject.Find ("ButtonBulletZoo");
		buttonGoLevelMansion = GameObject.Find ("ButtonBulletMansion");
		buttonGoIn = GameObject.Find ("ButtonGoIn");
		nameWorld = GameObject.Find ("TextWorld");
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
		//btnNameWorld.MoveIn(GUIAnimSystem.eGUIMove.SelfAndChildren);
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
				this.nameworldtext = "Circus";
				break;
			case 2:
				this.nameworldtext = "Train";
				break;
			case 3:
				this.nameworldtext = "Zoo";
				break;
			case 4:
				this.nameworldtext = "Mansion";
				break;
		}
		nameworld.text = this.nameworldtext;
	}

	void GoLevelScene () {
		if (!String.IsNullOrEmpty (this.nameworldtext)) {
			Debug.Log ("Go Level Scene!");
			PlayerPrefs.SetString ("nameWorld", this.nameworldtext);
			//string namescene = "LevelScene" + this.nameworld;
			string namescene = "LevelScene";
			SceneManager.LoadScene (sceneName: namescene);
		}
		Debug.Log ("Dont select Level!");

	}

	#endregion

	#region Interface implementation
	#endregion
}
