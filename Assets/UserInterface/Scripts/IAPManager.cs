using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener {

	private static IAPManager _instance;

    public delegate void EventIAPMessageProgress(string messageResult, bool typeResult);
    public event EventIAPMessageProgress OnIAPMessageProgress;

    public delegate void EventIAPInitialized(bool stateIAP);
    public event EventIAPInitialized OnIAPInitialized;

    private ProductItem productItemBuy { get; set; }

    private static IStoreController storeController;
    private static IExtensionProvider storeExtensionProvider;

    public static string kProductIDConsumable = "consumable";
    public static string kProductIDNonConsumable = "nonconsumable";
    public static string kProductIDSubscription = "subscription";

    public static IAPManager Instance
	{
		get {
			if (_instance == null) {
				GameObject iapm = new GameObject ("IAPManager");
				iapm.AddComponent<IAPManager> ();
			}

			return _instance;
		}
	}

	// Apple App Store-specific product identifier for the subscription product.
	private static string kProductNameAppleSubscription =  "com.unity3d.subscription.new";

	// Google Play Store-specific product identifier subscription product.
	private static string kProductNameGooglePlaySubscription =  "com.unity3d.subscription.original"; 

	void Awake()
	{
		DontDestroyOnLoad (this.gameObject);
		_instance = this;
	}

	/*void Start()
	{
		
		if (storeController == null)
		{
			// Begin to configure our connection to Purchasing
			InitializePurchasing();
		}
	}*/

	public void InitializePurchasing( Dictionary<string, ProductItem> listItemProducts) 
	{
		Debug.Log( "[IAPManager] Initing Unity IAP" );
		// If we have already connected to Purchasing ...
		if (IsInitialized())
		{
            // ... we are done here.
            if (OnIAPInitialized != null)
                OnIAPInitialized(true);
			return;
		}
		Debug.Log( "[IAPManager] Inicializando IAP..." );
		// Create a builder, first passing in a suite of Unity provided stores.
		var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach(var item in listItemProducts)
        {
            ProductItem aux = item.Value;
            builder.AddProduct(aux.idProductItem, ProductType.Consumable);
        }
		
		// Continue adding the non-consumable product.
		builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable);
		// And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
		// if the Product ID was configured differently between Apple and Google stores. Also note that
		// one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
		// must only be referenced here. 
		builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs(){
			{ kProductNameAppleSubscription, AppleAppStore.Name },
			{ kProductNameGooglePlaySubscription, GooglePlay.Name },
		});

		// Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
		// and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
		UnityPurchasing.Initialize(this, builder);
		Debug.Log( "[IAPManager] IAP inicializado !!!" );
	}


	private bool IsInitialized()
	{
		return storeController != null && storeExtensionProvider != null;
	}

    public void BuyObject (ProductItem ProductItemObject)
    {
        productItemBuy = ProductItemObject;
        BuyProductID(ProductItemObject);
    }

	private void BuyProductID(ProductItem productItem)
	{
		// If Purchasing has been initialized ...
		if (IsInitialized())
		{
			// ... look up the Product reference with the general product identifier and the Purchasing 
			// system's products collection.
			Product product = storeController.products.WithID(id: productItem.idProductItem);

			// If the look up found a product for this device's store and that product is ready to be sold ... 
			if (product != null && product.availableToPurchase)
			{
				Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
				// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
				// asynchronously.
				storeController.InitiatePurchase(product);
			}
			// Otherwise ...
			else
			{
                // ... report the product look-up failure situation  
                if (OnIAPMessageProgress != null)
                    OnIAPMessageProgress("Producto no disponible o no se encuentra en tienda.", false);
			}
		}
		// Otherwise ...
		else
		{
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            if (OnIAPMessageProgress != null)
                OnIAPMessageProgress("Producto no disponible o no se encuentra en tienda.", false);
        }
	}


	// Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
	// Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
	public void RestorePurchases()
	{
		// If Purchasing has not yet been set up ...
		if (!IsInitialized())
		{
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            if (OnIAPInitialized != null)
                OnIAPInitialized(true);
            return;
		}

		// If we are running on an Apple device ... 
		if (Application.platform == RuntimePlatform.IPhonePlayer || 
			Application.platform == RuntimePlatform.OSXPlayer)
		{
			// ... begin restoring purchases
			Debug.Log("RestorePurchases started ...");

			// Fetch the Apple store-specific subsystem.
			var apple = storeExtensionProvider.GetExtension<IAppleExtensions>();
			// Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
			// the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
			apple.RestoreTransactions((result) => {
				// The first phase of restoration. If no more responses are received on ProcessPurchase then 
				// no purchases are available to be restored.
				Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
				//this.MostrarDialogo("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore." + Application.platform,true);
			});
		}
		// Otherwise ...
		else
		{
			// We are not running on an Apple device. No work is necessary to restore purchases.
			Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
			//this.MostrarDialogo("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform,true);
		}
	}


	//  
	// --- IStoreListener
	//

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		// Purchasing has succeeded initializing. Collect our Purchasing references.
		Debug.Log("OnInitialized: PASS");

		// Overall Purchasing system, configured with products for this application.
		storeController = controller;
		// Store specific subsystem, for accessing device-specific store features.
		storeExtensionProvider = extensions;

        if (OnIAPInitialized != null)
            OnIAPInitialized(true);
    }


	public void OnInitializeFailed(InitializationFailureReason error)
	{
		// Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
		Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        if (OnIAPInitialized != null)
            OnIAPInitialized(true);
    }


	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs result) 
	{
		if (String.Equals(result.purchasedProduct.definition.id, productItemBuy.idProductItem, StringComparison.Ordinal))
		{
			Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", result.purchasedProduct.definition.id));
            //Se incremnmetan las huellas +200 en el juego

            string msj = "Has comprado "+productItemBuy.nameProduct+" ,Felicidades";
            if (OnIAPMessageProgress != null)
                OnIAPMessageProgress(msj, true);
        }
		else 
		{
            string msj = "No se logró final la compra de "+productItemBuy.nameProduct+" en la tienda";
            if (OnIAPMessageProgress != null)
                OnIAPMessageProgress(msj, true);
        }

		// Return a flag indicating whether this product has completely been received, or if the application needs 
		// to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
		// saving purchased products to the cloud, and when that save is delayed. 
		return PurchaseProcessingResult.Complete;
	}


	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
	{
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        string msj = "No se logró final la compra de " + productItemBuy.nameProduct + " en la tienda";
        if (OnIAPMessageProgress != null)
            OnIAPMessageProgress(msj, true);
    }


}
