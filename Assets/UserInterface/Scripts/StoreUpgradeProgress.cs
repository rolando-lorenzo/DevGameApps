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
    public GameItemsManager.StoreProduct idUpgrade{ set; get;}
	public int limitOfUpgrades { set; get;}
	#endregion

	#region MonoBehaviour overrides
	void Start(){

        currentProgress = GameItemsManager.GetUpgradeValue(idUpgrade); //limit 5
		if(currentProgress > limitOfUpgrades){
			currentProgress = limitOfUpgrades;
		}
		ChangeImgsColor ();
		if (OnUpgradeProgressChange != null)
			OnUpgradeProgressChange (currentProgress);
	}
	#endregion

	#region Class implementation
	/// <summary>
	/// Increments the progress.
	/// </summary>
	/// <returns><c>true</c>, if progress was incremented, <c>false</c> otherwise.</returns>
	public bool IsIncrementableProgress(){

		bool wasIncremented = false;
		++currentProgress;
		if (currentProgress <= limitOfUpgrades) {
			wasIncremented = true;
		} else {
			currentProgress = limitOfUpgrades;
		}

		return wasIncremented;
	}

	/// <summary>
	/// Changes the color of the imgs progresss
	/// </summary>
	public void ChangeImgsColor(){
        for (int i = 0; i < GameItemsManager.GetUpgradeValue(idUpgrade); i++) {
			Image currImg = imgsProgress [i];
			currImg.color = Color.blue;
		}
	}
	#endregion

}