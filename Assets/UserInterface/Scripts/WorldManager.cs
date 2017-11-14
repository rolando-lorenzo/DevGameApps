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

    public GameObject buttonGoLevelCircus;
    public GameObject buttonGoLevelTrain;
    public GameObject buttonGoLevelZoo;
    public GameObject buttonGoLevelMansion;

    //variables
    private string nameWorldText { get; set; }
    private string progressLevels { get; set; }
	#endregion

	#region Class accesors
	#endregion

	#region MonoBehaviour overrides
	void Awake () {
		if (enabled) {
			GUIAnimSystem.Instance.m_AutoAnimation = false;
		}

        progressLevels = GameItemsManager.GetValueStringById(GameItemsManager.Item.GameProgressLevel, "Circus-5/Train-5/Zoo-5/Mansion-5");
    }

	void Start () {
        Debug.Log("Ultimo charcater "+GameItemsManager.GetLastChooseCharacter().ToString());

		//Button of Back
		Button btnback = buttonBack.GetComponent<Button> ();
		btnback.onClick.AddListener (GoMainScene);

		//Buttons Of Go in levels
		Button btnlevelusa = buttonGoLevelCircus.GetComponent<Button> ();
		btnlevelusa.onClick.AddListener (() => GoLevelScene(1));

		Button btnlevelmex = buttonGoLevelTrain.GetComponent<Button> ();
		btnlevelmex.onClick.AddListener (() => GoLevelScene(2));

		Button btnlevelafr = buttonGoLevelZoo.GetComponent<Button> ();
		btnlevelafr.onClick.AddListener (() => GoLevelScene(3));

		Button btnlevelch = buttonGoLevelMansion.GetComponent<Button> ();
		btnlevelch.onClick.AddListener (() => GoLevelScene(4));


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

	private void GoLevelScene(int world) {
		switch (world) {
			case 1:
				//name the world in Xml
				nameWorldText = "Circus";
				break;
			case 2:
				nameWorldText = "Train";
				break;
			case 3:
				nameWorldText = "Zoo";
				break;
			case 4:
				nameWorldText = "Mansion";
				break;
		}

        GameItemsManager.SetValueStringById(GameItemsManager.Item.WorldName, nameWorldText);
        GameItemsManager.SetValueStringById(GameItemsManager.Item.GameProgressLevel, progressLevels);
        //string namescene = "LevelScene" + this.nameworld;
        string nameScene = "LevelScene";
        SceneManager.LoadScene(sceneName: nameScene);
    }

	#endregion

	#region Interface implementation
	#endregion
}
