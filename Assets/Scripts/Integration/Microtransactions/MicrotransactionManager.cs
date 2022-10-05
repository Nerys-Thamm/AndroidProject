using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

/// <summary>
///  Manages the microtransactions for the game.
/// </summary>
public class MicrotransactionManager : MonoBehaviour, IStoreListener
{
    //Button references
    public Button removeAdsButton;  //Button to remove ads


    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("AdsDisabled", 0) == 1) //If ads are disabled
        {
            removeAdsButton.interactable = false; //Disable the button
        }
        else
        {
            removeAdsButton.interactable = true; //Enable the button
            removeAdsButton.onClick.AddListener(() => { BuyRemoveAds(); }); //Add listener to button
        }
        if (m_StoreController == null) //If the store controller is null
        {
            InitializePurchasing(); //Initialize the store
        }
    }

    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    private static string kProductIDRemoveAds = "remove_ads";  // Product ID to remove ads.
    private static string kProductIDPurchaseCurrency = "purchase_currency"; //Product ID to purchase currency

    /// <summary>
    ///  Called when Unity IAP is ready to make purchases.
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="extensions"></param>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }

    /// <summary>
    ///  Called when Unity IAP encounters an unrecoverable initialization error.
    /// </summary>
    /// <param name="error"></param>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    /// <summary>
    ///  Checks if the initializtion of the store has failed.
    /// </summary>
    /// <returns></returns>
    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    /// <summary>
    ///  Initialize Unity IAP. If we are running in the editor, we simulate the store.
    /// </summary>
    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.
        builder.AddProduct(kProductIDRemoveAds, ProductType.NonConsumable);
        builder.AddProduct(kProductIDPurchaseCurrency, ProductType.Consumable);

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }

    /// <summary>
    ///  Purchase a product asynchronously using its general identifier. Expect a response either
    /// </summary>
    /// <param name="purchaseEvent"></param>
    /// <returns></returns>
    PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        if (string.Equals(purchaseEvent.purchasedProduct.definition.id, kProductIDRemoveAds, System.StringComparison.Ordinal))
        {
            PlayerPrefs.SetInt("AdsDisabled", 1);
            removeAdsButton.interactable = false;

            PlayerPrefs.Save();
        }
        else if (string.Equals(purchaseEvent.purchasedProduct.definition.id, kProductIDPurchaseCurrency, System.StringComparison.Ordinal))
        {
            PlayerPrefs.SetInt("Currency", PlayerPrefs.GetInt("Currency", 0) + 1000);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", purchaseEvent.purchasedProduct.definition.id));
            // Return a flag indicating whether this product has completely been received, or if the application needs
            // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still
            // saving purchased products to the cloud, and when that save is delayed.
        }
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    ///  Called when a purchase fails.
    /// </summary>
    /// <param name="product"></param>
    /// <param name="failureReason"></param>
    void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing this reason with the user.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }

    /// <summary>
    ///  Buy the remove ads product using its general identifier. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
    /// </summary>
    public void BuyRemoveAds()
    {
        BuyProductID(kProductIDRemoveAds);
    }

    /// <summary>
    ///  Buy the purchase currency product using its general identifier. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
    /// </summary>
    public void BuyCurrency()
    {
        BuyProductID(kProductIDPurchaseCurrency);
    }

    /// <summary>
    ///  Buy the product with the given product ID.
    /// </summary>
    /// <param name="productId"></param>
    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
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
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }
}
