using System;
using System.Collections.Generic;
using MiniFactory.Configuration;
using UnityEngine;
using UnityEngine.Purchasing;
using ValueSystem;
using Zenject;

namespace MiniFactory.Purchasing
{
    public sealed class UnityPurchaseService : IPurchaseService, IInitializable, IDisposable
    {
        public event Action OnChanged;
        public event Action<string> OnPurchaseSucceeded;
        public event Action<string, string> OnPurchaseFailed;

        private readonly FactoryConfiguration _configuration;
        private readonly IValueSystem _valueSystem;
        private StoreController _storeController;
        private Product _product;

        public PurchaseStateType State { get; private set; } = PurchaseStateType.Initializing;
        public string LocalizedPrice => _product == null ? string.Empty : _product.metadata.localizedPriceString;

        public UnityPurchaseService(FactoryConfiguration configuration, IValueSystem valueSystem)
        {
            _configuration = configuration;
            _valueSystem = valueSystem;
        }

        public async void Initialize()
        {
            _storeController = UnityIAPServices.StoreController();
            Subscribe();

            try
            {
                await _storeController.Connect();

                if (State == PurchaseStateType.Unavailable)
                {
                    return;
                }

                List<ProductDefinition> products = new List<ProductDefinition>
                {
                    new ProductDefinition(_configuration.Purchase.ProductIdentifier, ProductType.Consumable)
                };
                _storeController.FetchProducts(products);
            }
            catch (Exception exception)
            {
                SetUnavailable(exception.Message);
            }
        }

        public bool TryPurchase()
        {
            if (State != PurchaseStateType.Ready)
            {
                return false;
            }

            State = PurchaseStateType.Purchasing;
            OnChanged?.Invoke();
            _storeController.PurchaseProduct(_configuration.Purchase.ProductIdentifier);
            return true;
        }

        public void Dispose()
        {
            if (_storeController == null)
            {
                return;
            }

            _storeController.OnStoreDisconnected -= HandleStoreDisconnected;
            _storeController.OnProductsFetched -= HandleProductsFetched;
            _storeController.OnProductsFetchFailed -= HandleProductsFetchFailed;
            _storeController.OnPurchasesFetched -= HandlePurchasesFetched;
            _storeController.OnPurchasesFetchFailed -= HandlePurchasesFetchFailed;
            _storeController.OnPurchasePending -= HandlePurchasePending;
            _storeController.OnPurchaseFailed -= HandlePurchaseFailed;
            _storeController.OnPurchaseDeferred -= HandlePurchaseDeferred;
        }

        private void Subscribe()
        {
            _storeController.OnStoreDisconnected += HandleStoreDisconnected;
            _storeController.OnProductsFetched += HandleProductsFetched;
            _storeController.OnProductsFetchFailed += HandleProductsFetchFailed;
            _storeController.OnPurchasesFetched += HandlePurchasesFetched;
            _storeController.OnPurchasesFetchFailed += HandlePurchasesFetchFailed;
            _storeController.OnPurchasePending += HandlePurchasePending;
            _storeController.OnPurchaseFailed += HandlePurchaseFailed;
            _storeController.OnPurchaseDeferred += HandlePurchaseDeferred;
        }

        private void HandleProductsFetched(List<Product> products)
        {
            foreach (Product product in products)
            {
                if (product.definition.id == _configuration.Purchase.ProductIdentifier)
                {
                    _product = product;
                    break;
                }
            }

            if (_product == null)
            {
                SetUnavailable("Configured product was not returned by the store.");
                return;
            }

            _storeController.FetchPurchases();
        }

        private void HandlePurchasesFetched(Orders orders)
        {
            State = PurchaseStateType.Ready;
            OnChanged?.Invoke();
        }

        private void HandlePurchasePending(PendingOrder order)
        {
            bool expectedProduct = false;

            foreach (CartItem item in order.CartOrdered.Items())
            {
                if (item.Product.definition.id == _configuration.Purchase.ProductIdentifier)
                {
                    expectedProduct = true;
                    break;
                }
            }

            if (!expectedProduct)
            {
                State = PurchaseStateType.Ready;
                OnPurchaseFailed?.Invoke(_configuration.Purchase.ProductIdentifier, "The order does not contain the configured product.");
                OnChanged?.Invoke();
                return;
            }

            string transactionIdentifier = order.Info.TransactionID;

            if (!global::GameSaver.GameSaver.GetActiveProfile().ProcessedPurchaseIdentifiers.Contains(transactionIdentifier))
            {
                bool rewardGranted = _valueSystem.Add(_configuration.CurrencyIdentifier,
                    _configuration.Purchase.CurrencyAmount);

                if (!rewardGranted)
                {
                    State = PurchaseStateType.Ready;
                    OnPurchaseFailed?.Invoke(_configuration.Purchase.ProductIdentifier, "The currency reward could not be granted.");
                    OnChanged?.Invoke();
                    return;
                }

                global::GameSaver.GameSaver.GetActiveProfile().ProcessedPurchaseIdentifiers.Add(transactionIdentifier);
                global::GameSaver.GameSaver.ForceSave();
            }

            _storeController.ConfirmPurchase(order);
            State = PurchaseStateType.Ready;
            OnPurchaseSucceeded?.Invoke(_configuration.Purchase.ProductIdentifier);
            OnChanged?.Invoke();
        }

        private void HandlePurchaseFailed(FailedOrder order)
        {
            State = PurchaseStateType.Ready;
            OnPurchaseFailed?.Invoke(_configuration.Purchase.ProductIdentifier,
                $"{order.FailureReason}: {order.Details}");
            OnChanged?.Invoke();
        }

        private void HandlePurchaseDeferred(DeferredOrder order)
        {
            State = PurchaseStateType.Ready;
            OnPurchaseFailed?.Invoke(_configuration.Purchase.ProductIdentifier, "Purchase approval is pending.");
            OnChanged?.Invoke();
        }

        private void HandleStoreDisconnected(StoreConnectionFailureDescription failure)
        {
            SetUnavailable(failure.ToString());
        }

        private void HandleProductsFetchFailed(ProductFetchFailed failure)
        {
            SetUnavailable(failure.ToString());
        }

        private void HandlePurchasesFetchFailed(PurchasesFetchFailureDescription failure)
        {
            State = PurchaseStateType.Ready;
            OnPurchaseFailed?.Invoke(_configuration.Purchase.ProductIdentifier, failure.ToString());
            OnChanged?.Invoke();
            Debug.LogWarning($"Previous purchases could not be fetched: {failure}");
        }

        private void SetUnavailable(string reason)
        {
            State = PurchaseStateType.Unavailable;
            OnPurchaseFailed?.Invoke(_configuration.Purchase.ProductIdentifier, reason);
            OnChanged?.Invoke();
            Debug.LogError($"Unity IAP is unavailable: {reason}");
        }
    }
}
