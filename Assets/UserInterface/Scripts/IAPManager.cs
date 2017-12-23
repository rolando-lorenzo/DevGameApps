using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener {

	private static IAPManager _instance;

	public delegate void EventIAPMessageProgress(string messageResult, bool isError);
	public static event EventIAPMessageProgress OnIAPMessageProgress;

	public delegate void EventIAPInitialized(bool isError);
	public static event EventIAPInitialized OnIAPInitialized;

	public delegate void EventIAPSuccessPurchased(string messageResult, IStorePurchase item);
	public static event EventIAPSuccessPurchased OnIAPSuccessPurchasedInStore;

    private IStorePurchase productItemBuy { get; set; }

	private static IStoreController storeController;
    private static IExtensionProvider storeExtensionProvider;

    public static string kProductIDConsumable = "consumable";
	public static string kProductIDNonConsumable = "nonconsumable";
	public static string kProductIDSubscription = "subscription";

    List<Array> listProduct = new List<Array>();
    Product product { set; get; }


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

    private void Start()
    {
      // StartCoroutine(VerifyEvents());
    }

    private IEnumerator VerifyEvents()
    {
        yield return new WaitForSeconds(1.5f);
        Debug.Log("wall verify events");
        Debug.Log(OnIAPInitialized);
        Debug.Log(OnIAPMessageProgress);
        Debug.Log(OnIAPSuccessPurchasedInStore);
    }

    public void InitializePurchasing( List<IStorePurchase> listItemProducts) 
	{
		Debug.Log( "[IAPManager] Initing Unity IAP" );
		// If we have already connected to Purchasing ...
		if (IsInitialized())
		{
			// ... we are done here.
			if (OnIAPInitialized != null)
				OnIAPInitialized(false);
			return;
		}
		Debug.Log( "[IAPManager] Inicializando IAP..." );
        // Create a builder, first passing in a suite of Unity provided stores.
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());



		foreach(IStorePurchase item in listItemProducts)
		{
			if (item is ProductItem) {
				ProductItem currentProduct = ((ProductItem)item);


				if (currentProduct is ProductUpgradeItem) { //If is Upgrade
					ProductUpgradeItem currentUp = ((ProductUpgradeItem)currentProduct);
					if (currentUp.levelsUpgradesIdGooglePlay != null 
						&& currentUp.levelsUpgradesIdGooglePlay.Count > 0) {
						foreach (string lvs in currentUp.levelsUpgradesIdGooglePlay) {
							Debug.Log ("Agregando item(ProductUpgradeItem) para el store: " + lvs);
							builder.AddProduct (lvs, ProductType.Consumable);
						}
					}
				} else { //If is PowerUp or Package
					Debug.Log ("Agregando item(ProductItem) para el store: "+currentProduct.idStoreGooglePlay);
					builder.AddProduct(currentProduct.idStoreGooglePlay, ProductType.Consumable);
				}
			}

			if (item is CharacterItem) {
				CharacterItem currentProduct = ((CharacterItem)item);
				Debug.Log ("Agregando item(CharacterItem) para el store: "+currentProduct.idStoreGooglePlay);
				builder.AddProduct(currentProduct.idStoreGooglePlay, ProductType.Consumable);
			}

            if (item is WallpaperItem)
            {
                WallpaperItem currentProduct = ((WallpaperItem) item);
                Debug.Log("Agregando item(WallpaperItem) para el store: " + currentProduct.idStoreGooglePlay);
                builder.AddProduct(currentProduct.idStoreGooglePlay, ProductType.Consumable);
            }
        }

		// Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
		// and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
		UnityPurchasing.Initialize(this, builder);
		Debug.Log( "[IAPManager] IAP inicializado !!!" );
	}


	private bool IsInitialized()
	{
		return storeController != null && storeExtensionProvider != null;
	}

	public void BuyInStore (IStorePurchase productItemObject)
	{
		productItemBuy = productItemObject;
		BuyProductID(productItemObject);
	}

	private void BuyProductID(IStorePurchase productItem)
	{
		// If Purchasing has been initialized ...
		if (IsInitialized())
		{
			// ... look up the Product reference with the general product identifier and the Purchasing 
			// system's products collection.
			Product product = null;
			if (productItem is ProductItem) {
				ProductItem currentProduct = ((ProductItem)productItem);
				product = storeController.products.WithID (id: currentProduct.idStoreGooglePlay);
			} else if (productItem is CharacterItem) {
				CharacterItem currentCharacter = ((CharacterItem)productItem);
				product = storeController.products.WithID (id: currentCharacter.idStoreGooglePlay);
			}
            else if (productItem is WallpaperItem)
            {
                WallpaperItem currentCharacter = ((WallpaperItem)productItem);
                product = storeController.products.WithID(id: currentCharacter.idStoreGooglePlay);
            }
            else {
				if (OnIAPMessageProgress != null)
                    OnIAPMessageProgress("msg_err_produc_not_avaliable", true);
			}

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
                    OnIAPMessageProgress("msg_err_produc_not_avaliable", true);
			}
		}
		// Otherwise ...
		else
		{
			// ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
			// retrying initiailization.
			if (OnIAPMessageProgress != null)
                OnIAPMessageProgress("msg_err_init_api_iap", true);
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
				OnIAPInitialized(false);
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
			if (OnIAPMessageProgress != null)
                OnIAPMessageProgress("msg_err_platfrom_not_support", true);
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

        extensions.GetExtension<IAppleExtensions>().RegisterPurchaseDeferredListener(OnDeferred);

        Debug.Log("Available items:");
        foreach (var item in controller.products.all)
        {
            if (item.availableToPurchase)
            {
                string[] productStore = {
                        item.definition.storeSpecificId,
                        item.definition.id,
                        item.metadata.localizedPrice.ToString(),
                        item.metadata.localizedPriceString,
                        item.metadata.localizedTitle,
                        item.metadata.localizedDescription,
                        item.metadata.isoCurrencyCode,
                        item.transactionID,
                        item.receipt
                    };

                listProduct.Add(productStore);
                
                Debug.Log(string.Join(" - ",productStore));
            }
        }

        if (OnIAPInitialized != null)
			OnIAPInitialized(false);
	}

    /// <summary>
    /// iOS Specific.
    /// This is called as part of Apple's 'Ask to buy' functionality,
    /// when a purchase is requested by a minor and referred to a parent
    /// for approval.
    ///
    /// When the purchase is approved or rejected, the normal purchase events
    /// will fire.
    /// </summary>
    /// <param name="item">Item.</param>
    private void OnDeferred(Product item)
    {
        Debug.Log("Purchase deferred: " + item.definition.id);
    }


    public void OnInitializeFailed(InitializationFailureReason error)
	{
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("Billing failed to initialize!");
        switch (error)
        {
            case InitializationFailureReason.AppNotKnown:
                Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
                break;
            case InitializationFailureReason.PurchasingUnavailable:
                // Ask the user if billing is disabled in device settings.
                Debug.Log("Billing disabled!");
                break;
            case InitializationFailureReason.NoProductsAvailable:
                // Developer configuration error; check product metadata.
                Debug.Log("No products available for purchase!");
                break;
        }

        string msg = "msg_err_init_api_iap";
		if (OnIAPMessageProgress != null)
			OnIAPMessageProgress(msg, true);
	}


	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs result) 
	{
		if (String.Equals(result.purchasedProduct.definition.id, productItemBuy.idStoreGooglePlay, StringComparison.Ordinal))
		{
			Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", result.purchasedProduct.definition.id));

			string nameItemPurchased = "";
			if (productItemBuy is ProductItem) {
				ProductItem currentProduct = ((ProductItem)productItemBuy);
				nameItemPurchased = currentProduct.nameProduct.text;
			} 

			if (productItemBuy is CharacterItem) {
				CharacterItem currentCharacter = ((CharacterItem)productItemBuy);
				nameItemPurchased = currentCharacter.nameCharacter.text;
			}

            if (productItemBuy is WallpaperItem)
            {
                WallpaperItem currentWallpaper = ((WallpaperItem)productItemBuy);
                nameItemPurchased = currentWallpaper.name;
            }

            string msj = "Compra exitosa !! Has adquirido "+nameItemPurchased+". Felicidades !!";
            Debug.Log(msj);
			if (OnIAPSuccessPurchasedInStore != null)
                OnIAPSuccessPurchasedInStore("msg_info_success_purchased", productItemBuy);

            /*string msj = "msg_err_purchased_error";
            if (OnIAPMessageProgress != null)
                OnIAPMessageProgress(msj, true);*/
        }
		else 
		{
            string msj = "msg_err_purchased_error";
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
       
        string msj = "msg_err_purchased_error";
		if (OnIAPMessageProgress != null)
			OnIAPMessageProgress(msj, true);
	}


}
