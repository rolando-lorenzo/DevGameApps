using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowersGUI : MonoBehaviour {

	#region Class members
	public static PowersGUI ins;
	public PowerGUI[] powersGUI;
	#endregion

	#region Class accesors
	#endregion

	#region MonoBehaviour overrides
	private void Awake () {
		ins = this;
	}
	#endregion

	#region Super class overrides
	#endregion

	#region Class implementation
	public void SetUpAllPowers () {
		foreach (PowerGUI item in powersGUI) {
			item.SetUp ();
		}
	}
	#endregion

	#region Interface implementation
	#endregion
}