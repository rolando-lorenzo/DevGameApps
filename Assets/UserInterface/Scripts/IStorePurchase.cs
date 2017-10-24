using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IStorePurchase {

	string idStoreGooglePlay{ get; set;} 
	bool isAvailableInStore{ get; set;}

}