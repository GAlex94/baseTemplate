using System;
using System.Collections;
using DTools;
using UnityEngine;
using UnityEngine.Purchasing;

 namespace baseTemplate
 {
     public class ShopManager : Singleton<ShopManager>, IStoreListener
     {
         private PurchaseConfig purchaseConfig;
         public PurchaseConfig PurchaseConfig => purchaseConfig;

         private IStoreController controller;
         private IExtensionProvider extensions;
         public bool IsInitialized { get; private set; }

         private Action<bool, PurchaseInfo> onPurchaseCompleted;

         private void Awake()
         {
             DontDestroyOnLoad(gameObject);
         }

         private void Start()
         {
             InitializePurchasing();
         }

         public void Init(PurchaseConfig purchaseConfig)
         {
             this.purchaseConfig = purchaseConfig;
         }

         public void InitializePurchasing()
         {
             var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

             foreach (var curPurchase in purchaseConfig.purchases)
             {
                 if (curPurchase.typeBuyProduct == BuyProductType.Real)
                 {
                     builder.AddProduct(curPurchase.productId, curPurchase.behavior,
                         new IDs()
                         {
                             {curPurchase.productId, GooglePlay.Name}, {curPurchase.productIdIOS, AppleAppStore.Name}
                         });
                 }
             }

             UnityPurchasing.Initialize(this, builder);

             if (GameManager.Instance.IsFakeShop)
             {
                 IsInitialized = true;
             }
         }

         public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
         {
             this.controller = controller;
             this.extensions = extensions;

             IsInitialized = true;
         }

         public void BuyProductID(string productId, Action<bool, PurchaseInfo> onPurchaseCompleted)
         {
             if (GameManager.Instance.IsFakeShop)
             {
                 PurchaseInfo curInfo = FindPurchase(productId);
                 this.onPurchaseCompleted = onPurchaseCompleted;
                 StartCoroutine(OnBuyPurchase(curInfo));
                 return;
             }

             if (IsInitialized)
             {
                 this.onPurchaseCompleted = onPurchaseCompleted;
                 Product product = controller.products.WithID(productId);

                 if (product != null && product.availableToPurchase)
                 {
                     controller.InitiatePurchase(product);
                 }
                 else
                 {
                     ToastMessage.ShowCustom(DLocalizationManager.Instance.GetLocalText("cannot_purchase"));
                 }
             }
             else
             {
                 ToastMessage.ShowCustom(DLocalizationManager.Instance.GetLocalText("cannot_purchase"));
             }
         }

         public void RestorePurchases()
         {
             if (!IsInitialized)
             {
                 return;
             }

             if (Application.platform == RuntimePlatform.IPhonePlayer ||
                 Application.platform == RuntimePlatform.OSXPlayer)
             {
                 var apple = extensions.GetExtension<IAppleExtensions>();
                 apple.RestoreTransactions((result) =>
                 {
                     Debug.LogError("RestorePurchases continuing: " + result +
                                    ". If no further messages, no purchases available to restore.");
                 });
             }
             else
             {
                 Debug.LogError("RestorePurchases FAIL. Not supported on this platform. Current = " +
                                Application.platform);
             }
         }

         public void OnInitializeFailed(InitializationFailureReason error)
         {
             Debug.LogError("OnInitializeFailed InitializationFailureReason:" + error);
         }

         public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
         {
             PurchaseInfo curInfo = FindPurchase(e.purchasedProduct.definition.id);
             StartCoroutine(OnBuyPurchase(curInfo));
             return PurchaseProcessingResult.Complete;
         }

         private IEnumerator OnBuyPurchase(PurchaseInfo info)
         {
             yield return null;

             if (info != null)
             {
                 switch (info.type)
                 {
                     case GameProductType.NoAds:
                         BuyNoAds(info);
                         break;
                     case GameProductType.None:
                         break;
                     case GameProductType.UnlockAndNoAds:
                         BuyUnlockStory(info);
                         BuyNoAds(info);
                         break;
                     default:
                         throw new ArgumentOutOfRangeException();
                 }

                 onPurchaseCompleted?.Invoke(true, info);
                 onPurchaseCompleted = null;
             }
             else
             {
                 Debug.LogError("Purchased unknown product!");
             }
         }


         public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
         {
             Debug.LogError(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",
                 product.definition.storeSpecificId, failureReason));

             if (failureReason != PurchaseFailureReason.UserCancelled)
             {
                 ToastMessage.ShowCustom(DLocalizationManager.Instance.GetLocalText("cannot_purchase"));
             }

             PurchaseInfo info = FindPurchase(product.definition.id);
             if (onPurchaseCompleted != null)
                 onPurchaseCompleted(false, info);
             onPurchaseCompleted = null;
         }

         public string GetPurchasePrice(string id)
         {
             if (!IsInitialized) return DLocalizationManager.Instance.GetLocalText("Buy");

             if (GameManager.Instance.IsFakeShop)
             {
                 return "99.99 $";
             }

             Product product = controller.products.WithID(id);
             if (product != null)
                 return product.metadata.localizedPriceString;

             return DLocalizationManager.Instance.GetLocalText("Buy");

         }

         public string GetPurchaseTitle(string id)
         {
             if (IsInitialized)
             {
                 string resultStr = "";
                 if (GameManager.Instance.IsFakeShop)
                 {
                     PurchaseInfo info = FindPurchase(id);
                     if (info != null)
                         resultStr = info.productTitle;
                     else
                         resultStr = "null";
                 }
                 else
                 {
                     Product product = controller.products.WithID(id);
                     if (product != null)
                     {
                         resultStr = product.metadata.localizedTitle;
                     }
                 }

                 int foundIndex = resultStr.IndexOf('(');
                 return foundIndex != -1 ? resultStr.Substring(0, foundIndex - 1) : resultStr;
             }

             return "";
         }

         public PurchaseInfo FindPurchase(string id)
         {
             int foundIndex = Array.FindIndex(purchaseConfig.purchases, purchase => purchase.productId == id);
             return foundIndex != -1 ? purchaseConfig.purchases[foundIndex] : null;
         }

         private void BuyNoAds(PurchaseInfo info)
         {
             DataManager.Instance.DisableAds();
         }

         private void BuyUnlockStory(PurchaseInfo info)
         {
             DataManager.Instance.UnlockAllStory();
         }

     }
 }
