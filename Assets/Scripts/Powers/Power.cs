using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Power {
	#region Class members
	public GameItemsManager.StorePower type;
	public int index;
	public bool developmentMode;
	public float time;
	public float reloadTime;
	public float progress { get; set; }
	public bool state;
	public int count;

	public List<float> timeUpgrades = new List<float> ();
	public PowerGUI gui;
	#endregion

	#region Class accesors
	#endregion

	#region Class implementation
	public Power (GameItemsManager.StorePower type, int upgradeCount) {
		this.type = type;
		timeUpgrades = new List<float> (new float[upgradeCount]);
	}

	public void SetUp (int index) {
		this.index = index;

		SetUpgrade (GameItemsManager.GetPowerUpgradeLevel (type));

		gui.button.onClick.AddListener (Use);
		if (!developmentMode)
			count = GameItemsManager.GetPowerCount (type);
		gui.UpdateCount (count);

		if (count <= 0)
			SetState (false);
	}

	public void SetUpgrade (int upgradeLevel) {
		time = timeUpgrades[upgradeLevel - 1];
	}

	public void SetState (bool value) {
		if (count <= 0 && value)
			return;

		gui.SetState (value);
	}

	public void Use () {
		if (count <= 0)
			return;

		if (PowersController.ins.GetAllEnablePowers ())
			return;

		state = true;
		Substrac ();
		Tween.FloatTween (gui.root, type.ToString (), 100, 0, time, (tween) => {
			progress = tween.Value;
			gui.UpdateIconProgress (1 - tween.Progress);
		}, (tween) => {
			if (count > 0)
				Reload ();
			else
				Finish ();
		}, false);

		PowersController.ins.UsePowerCallback (type);
	}

	public void Substrac () {
		count--;
		gui.counterText.text = count.ToString ();
		gui.canvasGroup.interactable = false;

		if (!developmentMode)
			GameItemsManager.SetPowerCount (type, count);
		PowersController.ins.SetAllPowersState (index, false);
	}

	public void Finish () {
		state = false;
		SetState (false);
		PowersController.ins.SetAllPowersState (index, true);
		PowersController.ins.FinishPowerCallback ();
	}

	public void Reload () {
		PowersController.ins.FinishPowerCallback ();

		Tween.FloatTween (gui.root, type.ToString (), 0, 1, reloadTime, (tween) => {
			gui.UpdateIconProgress (tween.Progress);
		}, (tween) => {
			state = false;
			PowersController.ins.SetAllPowersState (index, true);
			gui.canvasGroup.interactable = true;
			gui.SetCounterAlpha (1);
		}, false);
	}
	#endregion
}