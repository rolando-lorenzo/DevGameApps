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
    public GameItemsManager.StorePower idUpgrade{ set; get;}
	public int limitOfUpgrades { set; get;}
	#endregion

	#region MonoBehaviour overrides
	void Start(){

        //currentProgress = GameItemsManager.GetPowerUpgradeLevel(idUpgrade); //limit 5
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
        int progress = GameItemsManager.GetPowerUpgradeLevel(idUpgrade) - 1;
        bool wasIncremented = false;
        if(progress < limitOfUpgrades){
            ++currentProgress;
            if (currentProgress <= limitOfUpgrades)
            {
                wasIncremented = true;
            }
            else
            {
                currentProgress = limitOfUpgrades;
            }
        }
		return wasIncremented;
	}

	/// <summary>
	/// Changes the color of the imgs progresss
	/// </summary>
	public void ChangeImgsColor(){
        int progress = GameItemsManager.GetPowerUpgradeLevel(idUpgrade)-1;
        //Debug.Log(GameItemsManager.GetPowerUpgradeLevel(idUpgrade));
        for (int i = 0; i < progress; i++) {
			Image currImg = imgsProgress [i];
			currImg.color = Color.blue;
		}
	}
	#endregion

}