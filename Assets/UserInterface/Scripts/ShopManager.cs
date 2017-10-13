using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour {

	[System.Serializable]
	public class Players
	{
		public string name;
		public string price;
		public GameObject obj;
		public int priceMount;
		public bool desblock;
		public Sprite sprite;
	}

	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
