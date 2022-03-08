using UnityEngine;
using System.Collections;
using System;

//using UnityEngine.Purchasing;

public class UnityInAppsIntegration : MonoBehaviour//, IStoreListener
{
/*    public static UnityInAppsIntegration THIS;
    private static IStoreController m_StoreController;                                                                  // Reference to the Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider;                                                         // Reference to store-specific Purchasing subsystems.
	public GameObject InAppMenu;
    // Product identifiers for all products capable of being purchased: "convenience" general identifiers for use with Purchasing, and their store-specific identifier counterparts 
    // for use with and outside of Unity Purchasing. Define store-specific identifiers also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)
    private static string[] kProductIDConsumableArray = new string[4];                                                       // General handle for the consumable product.

    private static string kProductIDConsumable = "consumable";
	private static string kProductIDConsumablenoads = "noads";
	private static string kProductIDConsumabletwohundred = "twohundred";
	private static string kProductIDConsumablefivehundred = "fivehundred";
	private static string kProductIDConsumablethousand = "thousand";
	private static string kProductIDConsumablethreethousand = "threethousand";
	private static string kProductIDConsumablehundred = "hundredcoins";
	private static string kProductIDConsumablefifty = "fiftycoins";// General handle for the consumable product.
    private static string kProductIDSubscription = "subscription";                                                   // General handle for the subscription product.
	private static string kProductIDNonConsumable = "nonconsumable";                                                  // General handle for the non-consumable product.

    private static string kProductNameAppleConsumable = "com.unity3d.test.services.purchasing.consumable";             // Apple App Store identifier for the consumable product.
    private static string kProductNameAppleNonConsumable = "com.unity3d.test.services.purchasing.nonconsumable";      // Apple App Store identifier for the non-consumable product.
    private static string kProductNameAppleSubscription = "com.unity3d.test.services.purchasing.subscription";       // Apple App Store identifier for the subscription product.

    private static string kProductNameGooglePlayConsumable = "com.unity3d.test.services.purchasing.consumable";        // Google Play Store identifier for the consumable product.
    private static string kProductNameGooglePlayNonConsumable = "com.unity3d.test.services.purchasing.nonconsumable";     // Google Play Store identifier for the non-consumable product.
    private static string kProductNameGooglePlaySubscription = "com.unity3d.test.services.purchasing.subscription";  // Google Play Store identifier for the subscription product.

    void Start()
    {
        THIS = this;
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

//        for (int i = 0; i < LevelEditorBase.THIS.InAppIDs.Length; i++)
//        {
//			kProductIDConsumableArray[i] = LevelEditorBase.THIS.InAppIDs[i];
//            builder.AddProduct(kProductIDConsumableArray[i], ProductType.Consumable, new IDs() { { kProductIDConsumableArray[i], AppleAppStore.Name }, { kProductIDConsumableArray[i], GooglePlay.Name }, });
//        }

		builder.AddProduct(kProductIDConsumable, ProductType.Consumable, new IDs() { { kProductIDConsumable, AppleAppStore.Name }, {kProductIDConsumable, GooglePlay.Name }, });
//		builder.AddProduct(kProductIDConsumablenoads, ProductType.Consumable, new IDs() { { kProductIDConsumablenoads, AppleAppStore.Name }, {kProductIDConsumablenoads, GooglePlay.Name }, });
//		builder.AddProduct(kProductIDConsumablefifty, ProductType.Consumable, new IDs() { { kProductIDConsumable, AppleAppStore.Name }, {kProductIDConsumablefifty, GooglePlay.Name }, });
		builder.AddProduct(kProductIDConsumablethousand, ProductType.Consumable, new IDs() { { kProductIDConsumable, AppleAppStore.Name }, {kProductIDConsumablethousand, GooglePlay.Name }, });
		builder.AddProduct(kProductIDConsumablethreethousand, ProductType.Consumable, new IDs() { { kProductIDConsumable, AppleAppStore.Name }, {kProductIDConsumablethreethousand, GooglePlay.Name }, });
		builder.AddProduct(kProductIDConsumablefivehundred, ProductType.Consumable, new IDs() { { kProductIDConsumable, AppleAppStore.Name }, {kProductIDConsumablefivehundred, GooglePlay.Name }, });
//		builder.AddProduct(kProductIDConsumablehundred, ProductType.Consumable, new IDs() { { kProductIDConsumable, AppleAppStore.Name }, {kProductIDConsumablehundred, GooglePlay.Name }, });
		builder.AddProduct(kProductIDConsumabletwohundred, ProductType.Consumable, new IDs() { { kProductIDConsumable, AppleAppStore.Name }, {kProductIDConsumabletwohundred, GooglePlay.Name }, });




		// Create a builder, first passing in a suite of Unity provided stores.

        // Add a product to sell / restore by way of its identifier, associating the general identifier with its store-specific identifiers.
        //builder.AddProduct(kProductIDConsumable, ProductType.Consumable, new IDs() { { kProductNameAppleConsumable, AppleAppStore.Name }, { kProductNameGooglePlayConsumable, GooglePlay.Name }, });// Continue adding the non-consumable product.
        //builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable, new IDs() { { kProductNameAppleNonConsumable, AppleAppStore.Name }, { kProductNameGooglePlayNonConsumable, GooglePlay.Name }, });// And finish adding the subscription product.
        //builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs() { { kProductNameAppleSubscription, AppleAppStore.Name }, { kProductNameGooglePlaySubscription, GooglePlay.Name }, });// Kick off the remainder of the set-up with an asynchrounous call, passing the configuration and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    public void BuyConsumable()
    {
        // Buy the consumable product using its general identifier. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kProductIDConsumable);
    }


    public void BuyNonConsumable()
    {
        // Buy the non-consumable product using its general identifier. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
		BuyProductID(kProductIDConsumablenoads);
    }


    public void BuySubscription()
    {
        // Buy the subscription product using its the general identifier. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kProductIDSubscription);
    }


    public void BuyProductID(string productId)
    {
		Debug.Log (productId);
        // If the stores throw an unexpected exception, use try..catch to protect my logic here.
        try
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // ... look up the Product reference with the general product identifier and the Purchasing system's products collection.
                Product product = m_StoreController.products.WithID(productId);

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
                      m_StoreController.InitiatePurchase(product);



                }
                // Otherwise ...
                else
                {
                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }
        // Complete the unexpected exception handling ...
        catch (Exception e)
        {
            // ... by reporting any unexpected exception for later diagnosis.
            Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases. Apple currently requires explicit purchase restoration for IAP.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
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
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        // A consumable product has been purchased by this user.
		if(string.Equals(args.purchasedProduct.definition.id,kProductIDConsumablenoads,StringComparison.Ordinal)){
				Debug.Log (args.purchasedProduct.definition.id+"Purchased");
			PlayerPrefs.SetInt("NoAds", 1);
//				InitScriptName.InitScript.Gems += 10;
		}
		else if(string.Equals(args.purchasedProduct.definition.id,kProductIDConsumablehundred,StringComparison.Ordinal)){
				Debug.Log (args.purchasedProduct.definition.id+"Purchased");
//				InitScriptName.InitScript.Gems += 50;
			}
		else if(string.Equals(args.purchasedProduct.definition.id,kProductIDConsumabletwohundred,StringComparison.Ordinal)){
				Debug.Log (args.purchasedProduct.definition.id+"Purchased");
//				InitScriptName.InitScript.Gems += 100;
			TotalCoins.staticInstance.AddCoins(200);
			}
		else if(string.Equals(args.purchasedProduct.definition.id,kProductIDConsumablethreethousand,StringComparison.Ordinal)){
				Debug.Log (args.purchasedProduct.definition.id+"Purchased");
			TotalCoins.staticInstance.AddCoins(3000);
//				InitScriptName.InitScript.Gems += 250;
		}else if(string.Equals(args.purchasedProduct.definition.id,kProductIDConsumablethousand,StringComparison.Ordinal)){
			Debug.Log (args.purchasedProduct.definition.id+"Purchased");
			TotalCoins.staticInstance.AddCoins(1000);
			//				InitScriptName.InitScript.Gems += 250;
		}else if(string.Equals(args.purchasedProduct.definition.id,kProductIDConsumablefivehundred,StringComparison.Ordinal)){
			Debug.Log (args.purchasedProduct.definition.id+"Purchased");
			TotalCoins.staticInstance.AddCoins(500);
			//				InitScriptName.InitScript.Gems += 250;
		}

        

        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing this reason with the user.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }


	public void Back(){
		InAppMenu.SetActive (false);
	}
*/
}
