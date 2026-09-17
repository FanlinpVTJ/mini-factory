using MiniFactory.Configuration;
using MiniFactory.Economy;
using MiniFactory.Production;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ValueSystem;
using ValueSystem.Misc;
using Zenject;

namespace MiniFactory.UI
{
    public sealed class MachineView : MonoBehaviour
    {
        [SerializeField] private string _machineIdentifier;
        [SerializeField] private TMP_Text _stateText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _productionText;
        [SerializeField] private TMP_Text _unlockPriceText;
        [SerializeField] private TMP_Text _upgradePriceText;
        [SerializeField] private Button _unlockButton;
        [SerializeField] private Button _upgradeButton;

        private ValueHandler _currencyHandler;
        private MachineState _displayedMachine;
        private double _displayedProduction = -1;

        [Inject] private FactoryConfiguration _configuration;
        [Inject] private FactoryEconomy _economy;
        [Inject] private FactoryProduction _production;
        [Inject] private IValueSystem _valueSystem;

        private void Awake()
        {
            _currencyHandler = _valueSystem.GetValueHandler(_configuration.CurrencyIdentifier);
        }

        private void OnEnable()
        {
            _unlockButton.onClick.AddListener(Unlock);
            _upgradeButton.onClick.AddListener(Upgrade);
            _economy.OnChanged += Refresh;
            _currencyHandler.OnValueChanged += RefreshButtons;
            _production.OnBoostStarted += Refresh;
            _production.OnBoostFinished += Refresh;
            Refresh();
        }

        private void Start()
        {
            Refresh();
        }

        private void OnDisable()
        {
            _unlockButton.onClick.RemoveListener(Unlock);
            _upgradeButton.onClick.RemoveListener(Upgrade);
            _economy.OnChanged -= Refresh;
            _currencyHandler.OnValueChanged -= RefreshButtons;
            _production.OnBoostStarted -= Refresh;
            _production.OnBoostFinished -= Refresh;
        }

        private void Unlock()
        {
            _economy.TryUnlock(_machineIdentifier);
        }

        private void Upgrade()
        {
            _economy.TryUpgrade(_machineIdentifier);
        }

        private void Refresh()
        {
            MachineState machine = _economy.GetMachine(_machineIdentifier);

            if (machine != _displayedMachine)
            {
                _displayedMachine = machine;
                bool locked = machine.State == MachineStateType.Locked;
                _stateText.text = locked ? "Закрыта" : "Работает";
                _levelText.SetText("Уровень: {0}", machine.Level);
                _unlockPriceText.text = "Открыть за: " + TextFormatter.FormatNumber((float)machine.UnlockPrice);
                _upgradePriceText.text = machine.CanUpgrade
                    ? "Улучшить за: " + TextFormatter.FormatNumber((float)machine.NextUpgradePrice)
                    : locked ? "—" : "Максимум";
                _unlockButton.gameObject.SetActive(locked);
                _upgradeButton.gameObject.SetActive(!locked);
            }

            double production = machine.ProductionPerSecond * (_production.IsBoostActive ? _configuration.BoostMultiplier : 1);

            if (production != _displayedProduction)
            {
                _displayedProduction = production;
                string amount = production < 1000 ? production.ToString("G3") : TextFormatter.FormatNumber((float)production);
                _productionText.text = amount + "/с";
            }

            RefreshButtons();
        }

        private void RefreshButtons()
        {
            MachineState machine = _economy.GetMachine(_machineIdentifier);
            _unlockButton.interactable = machine.State == MachineStateType.Locked
                && _currencyHandler.CanSubtract((float)machine.UnlockPrice);
            _upgradeButton.interactable = machine.CanUpgrade
                && _currencyHandler.CanSubtract((float)machine.NextUpgradePrice);
        }
    }
}
