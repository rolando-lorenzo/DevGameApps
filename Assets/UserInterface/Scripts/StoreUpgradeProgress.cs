using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreUpgradeProgress : MonoBehaviour {

	#region Class members

	public GameObject upgradeProgress;
	public Image statusEmpty;
	public Image statusFill;
	public Image[] imgsProgress;
	#endregion

	#region Class accesors
	public int currentProgress{ set; get;}
	public string idUpgrade{ set; get;}
	#endregion

	#region MonoBehaviour overrides
	void Start(){
		currentProgress = PlayerPrefs.GetInt ("StoreUpgrade"+idUpgrade,0); //limit 5
		ChangeImgsColor ();
	}
	#endregion

	#region Class implementation
	public bool IncrementProgress(){

		bool wasIncremented = false;
		++currentProgress;
		if (currentProgress <= imgsProgress.Length) {
			ChangeImgsColor ();
			wasIncremented = true;
		} else {
			currentProgress = 5;
		}
		Debug.Log("Set Current: StoreUpgrade"+idUpgrade+ " "+ currentProgress);
		PlayerPrefs.SetInt ("StoreUpgrade"+idUpgrade,currentProgress);
		return wasIncremented;
	}

	private void ChangeImgsColor(){
		for (int i = 0; i < currentProgress; i++) {
			Image currImg = imgsProgress [i];
			currImg.color = Color.blue;
		}
	}
	#endregion

}