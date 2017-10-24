using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUtils  {

	#region Class implementation

	/// <summary>
	/// Formats the price products.
	/// </summary>
	/// <returns>The price products.</returns>
	/// <param name="val">Value.</param>
	public static string FormatPriceProducts (float val){
		return "$"+ val + " USD";
	}

	/// <summary>
	/// Formats the pawprints products.
	/// </summary>
	/// <returns>The pawprints products.</returns>
	/// <param name="val">Value.</param>
	public static string FormatPawprintsProducts (float val){
		return  val + " Huellas";
	}



	#endregion


}