using System;
using UnityEngine;

namespace MiniFactory.Configuration
{
    [Serializable]
    public sealed class PurchaseConfiguration
    {
        [SerializeField] private string _productIdentifier = "coins_pack_small";
        [SerializeField, Min(1)] private float _currencyAmount = 500;

        public string ProductIdentifier => _productIdentifier;
        public float CurrencyAmount => _currencyAmount;
    }
}
