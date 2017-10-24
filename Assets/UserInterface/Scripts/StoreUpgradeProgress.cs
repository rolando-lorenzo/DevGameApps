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
	public delegate void UpgradeProgressChangeAction (int currentValue);
	public event UpgradeProgressChangeAction OnUpgradeProgressChange;
	#endregion

	#region Class accesors
	public int currentProgress{ set; get;}
	public string idUpgrade{ set; get;}
	public int limitOfUpgrades { set; get;}
	#endregion

	#region MonoBehaviour overrides
	void Start(){
		
		currentProgress = PlayerPrefs.GetInt ("StoreUpgrade"+idUpgrade,0); //limit 5
		if(currentProgress > limitOfUpgrades){
			currentProgress = limitOfUpgrades;
		}
		ChangeImgsColor ();
		if (OnUpgradeProgressChange != null)
			OnUpgradeProgressChange (currentProgress);
	}
	#endregion

	#region Class implementation
	public bool IncrementProgress(){

		bool wasIncremented = false;
		++currentProgress;
		if (currentProgress <= limitOfUpgrades) {
			ChangeImgsColor ();
			wasIncremented = true;
		} else {
			currentProgress = limitOfUpgrades;
		}
		PlayerPrefs.SetInt ("StoreUpgrade"+idUpgrade,currentProgress);

		if (OnUpgradeProgressChange != null)
			OnUpgradeProgressChange (currentProgress);

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