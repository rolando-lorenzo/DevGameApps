using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAPController : MonoBehaviour {

    private void OnEnable()
    {
        //sucribe to event    
    }

    void Awake()
	{
        Dictionary<string, ProductItem> dictionaryProducts = new Dictionary<string, ProductItem>();
		IAPManager.Instance.InitializePurchasing (dictionaryProducts);
	}
}
	
