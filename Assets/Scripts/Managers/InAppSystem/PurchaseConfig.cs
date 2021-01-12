using System;
using UnityEngine;

namespace baseTemplate
{
    public enum GameProductType
    {
        None,
        NoAds,
        UnlockAndNoAds,
    }

    public enum BuyProductType
    {
        Real,
        WatchAds,
    }

    public enum GameProductEnum
    {
        None,
        NoAds,
        UnlockAndNoAds,
    }


    [Serializable]
    public class PurchaseInfo
    {
#if UNITY_EDITOR
        public string NameEditor;
#endif
        public GameProductType type;
        public GameProductEnum gameProduct;
        public BuyProductType typeBuyProduct;
        public  UnityEngine.Purchasing.ProductType behavior;
        public string productId;
        public string productIdIOS;
        
        public int cost;

        public string productTitle;
        public string productDescr;
        
        public int productValue;
        public Sprite productIcon;
    }

    [CreateAssetMenu(fileName = "PurchaseConfig", menuName = "Data/BasicConfig/PurchaseConfig")]
    public class PurchaseConfig : ScriptableObject
    {
        public PurchaseInfo[] purchases;
    }
}
