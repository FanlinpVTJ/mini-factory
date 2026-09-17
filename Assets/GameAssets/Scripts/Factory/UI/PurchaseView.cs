using MiniFactory.Configuration;
using MiniFactory.Purchasing;
using TMPro;
using UnityEngine;
using WindowsManager.UI;
using ValueSystem.Misc;
using Zenject;

namespace MiniFactory.UI
{
    public sealed class PurchaseView : AbstractButton
    {
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private TMP_Text _rewardText;
        [SerializeField] private TMP_Text _stateText;

        [Inject] private FactoryConfiguration _configuration;
        [Inject] private IPurchaseService _purchaseService;

        protected override void OnEnable()
        {
            base.OnEnable();
            _purchaseService.OnChanged += Refresh;
            _purchaseService.OnPurchaseSucceeded += HandlePurchaseSucceeded;
            _purchaseService.OnPurchaseFailed += HandlePurchaseFailed;
            Refresh();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _purchaseService.OnChanged -= Refresh;
            _purchaseService.OnPurchaseSucceeded -= HandlePurchaseSucceeded;
            _purchaseService.OnPurchaseFailed -= HandlePurchaseFailed;
        }

        public override void OnButtonClick()
        {
            _purchaseService.TryPurchase();
        }

        private void Refresh()
        {
            _btn.interactable = _purchaseService.State == PurchaseStateType.Ready;
            _priceText.text = string.IsNullOrEmpty(_purchaseService.LocalizedPrice)
                ? "—"
                : "Купить за: " + _purchaseService.LocalizedPrice;
            _rewardText.text = "+" + TextFormatter.FormatNumber(_configuration.Purchase.CurrencyAmount);

            switch (_purchaseService.State)
            {
                case PurchaseStateType.Initializing:
                    _stateText.text = "Подключение к магазину";
                    break;
                case PurchaseStateType.Ready:
                    _stateText.text = "Купить";
                    break;
                case PurchaseStateType.Purchasing:
                    _stateText.text = "Покупка";
                    break;
                case PurchaseStateType.Unavailable:
                    _stateText.text = "Магазин недоступен";
                    break;
            }
        }

        private void HandlePurchaseSucceeded(string productIdentifier)
        {
            _stateText.text = "Покупка успешна";
        }

        private void HandlePurchaseFailed(string productIdentifier, string reason)
        {
            _stateText.text = "Покупка не выполнена";
        }
    }
}
