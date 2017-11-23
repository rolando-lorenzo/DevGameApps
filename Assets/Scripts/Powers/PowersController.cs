using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowersController : MonoBehaviour {

	#region Class members
	private static PowersController Ins;
	public static PowersController ins {
		get {
			if (Ins == null)
				Ins = FindObjectOfType<PowersController> ();
			return Ins;
		}
	}

	public delegate void UseDelegate (GameItemsManager.StorePower powerType);
	public event UseDelegate OnUse;

	public delegate void FinishDelegate ();
	public event FinishDelegate OnFinish;

	public List<Power> powers = new List<Power> ();
	#endregion

	#region Class accesors
	#endregion

	#region MonoBehaviour overrides
	private void Start () {
		for (int i = 0; i < powers.Count; i++) {
			powers[i].gui = PowersGUI.ins.powersGUI[i];
			powers[i].SetUp (i);
		}
	}
	#endregion

	#region Super class overrides
	#endregion

	#region Class implementation
	public void EnablePower (GameItemsManager.StorePower type) {
		switch (type) {
			case GameItemsManager.StorePower.PorkChop:
				powers[0].Use ();
				break;
			case GameItemsManager.StorePower.Magnet:
				powers[1].Use ();
				break;
			case GameItemsManager.StorePower.PawPrintMultiplier:
				powers[2].Use ();
				break;
		}
	}

	public void SetAllPowersState (int excludeIndex, bool value) {
		for (int i = 0; i < powers.Count; i++) {
			if (i != excludeIndex)
				powers[i].SetState (value);
		}
	}

	public bool GetAllEnablePowers () {
		for (int i = 0; i < powers.Count; i++) {
			if (powers[i].state)
				return true;
		}

		return false;
	}

	public void UsePowerCallback (GameItemsManager.StorePower type) {
		if (OnUse != null)
			OnUse (type);
	}

	public void FinishPowerCallback () {
		if (OnFinish != null)
			OnFinish ();
	}
	#endregion

	#region Interface implementation
	#endregion
}