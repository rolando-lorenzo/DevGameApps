using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinnerStorePowerups : MonoBehaviour {

	#region Class members
	public Text valueText;
	public Button upButton;
	public Button downButton;
	public delegate void SpinnerChangeAction (int currentValue);
	public event SpinnerChangeAction OnSpinnerPowerupChange;
	#endregion

	#region Class accesors
	public int currentValue { set; get;}
	public int limitValue { set; get;}
	#endregion

	#region MonoBehaviour overrides
	void Awake () {
		currentValue = 1;
		limitValue = 9;
		upButton.onClick.AddListener (() => IncrementValue());
		downButton.onClick.AddListener (() => DecrementValue());
	}
	#endregion

	#region Class implementation
	public void IncrementValue(){
		currentValue += 1;
		if (currentValue > limitValue) {
			currentValue = limitValue;
		}
		showValueText ();
		if (OnSpinnerPowerupChange != null)
			OnSpinnerPowerupChange (currentValue);
	}

	public void DecrementValue(){
		currentValue -= 1;
		if (currentValue < 1) {
			currentValue = 1;
		}
		showValueText ();
		if (OnSpinnerPowerupChange != null)
			OnSpinnerPowerupChange (currentValue);
	}

	public void showValueText(){
		valueText.text = currentValue.ToString ();
	}

	#endregion
}