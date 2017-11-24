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
    public Color unlockColor;
    public Sprite[] unlockButtonSprite; 


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
        progressLevels = GameItemsManager.GetValueStringById(GameItemsManager.Item.GameProgressLevel, ProgressWorldLevel.GetLevelWorlBase());
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
        VerifyLevel();
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
		switch (ProgressWorldLevel.GetWorldsEnum(world)) {
			case ProgressWorldLevel.WorldsNames.Circus:
				//name the world in Xml
				nameWorldText = ProgressWorldLevel.WorldsNames.Circus.ToString();
				break;
			case ProgressWorldLevel.WorldsNames.Train:
				nameWorldText = ProgressWorldLevel.WorldsNames.Train.ToString();
				break;
			case ProgressWorldLevel.WorldsNames.Zoo:
				nameWorldText = ProgressWorldLevel.WorldsNames.Zoo.ToString();
				break;
			case ProgressWorldLevel.WorldsNames.Manssion:
				nameWorldText = ProgressWorldLevel.WorldsNames.Manssion.ToString();
				break;
		}

        GameItemsManager.SetValueStringById(GameItemsManager.Item.WorldName, nameWorldText);
        GameItemsManager.SetValueStringById(GameItemsManager.Item.GameProgressLevel, progressLevels);
        string nameScene = "LevelScene";
        SceneManager.LoadScene(sceneName: nameScene);
    }

    private void VerifyLevel()
    {
        ProgressWorldLevel.WorldsNames[] world = { ProgressWorldLevel.WorldsNames.Circus, ProgressWorldLevel.WorldsNames.Train, ProgressWorldLevel.WorldsNames.Zoo, ProgressWorldLevel.WorldsNames.Manssion };
        for (int i = 0; i < world.Length; i++)
        {
            int maxlevel = ProgressWorldLevel.GetLevelWorl(world[i]);
            //Debug.Log(world[i] + " " + maxlevel);
            if (maxlevel == 0)
            {
                InteractableWorlds(world[i]);
            }
        }
    }

    private void InteractableWorlds(ProgressWorldLevel.WorldsNames world)
    {
        switch (world)
        {
            case ProgressWorldLevel.WorldsNames.Train:
                SetDataButton(buttonGoLevelTrain.GetComponent<Button>(), unlockButtonSprite[0]);
                break;
            case ProgressWorldLevel.WorldsNames.Zoo:
                SetDataButton(buttonGoLevelZoo.GetComponent<Button>(), unlockButtonSprite[1]);
                break;
            case ProgressWorldLevel.WorldsNames.Manssion:
                SetDataButton(buttonGoLevelMansion.GetComponent<Button>(), unlockButtonSprite[2]);
                break;
        }
    }

    private void SetDataButton(Button buttonWorld, Sprite spriteWorld)
    {
        Button buttonAux;
        Image imageAux;
        ColorBlock colorBlockButton;

        buttonAux = buttonWorld.GetComponent<Button>();
        imageAux = buttonWorld.GetComponent<Image>();
        imageAux.overrideSprite = spriteWorld;
        buttonAux.interactable = false;
        colorBlockButton = buttonAux.colors;
        colorBlockButton.disabledColor = unlockColor;
        buttonAux.colors = colorBlockButton;
    }

    #endregion

    #region Interface implementation
    #endregion
}
