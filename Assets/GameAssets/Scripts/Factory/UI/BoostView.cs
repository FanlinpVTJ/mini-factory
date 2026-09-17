using System;
using MiniFactory.Configuration;
using MiniFactory.Production;
using TMPro;
using UnityEngine;
using WindowsManager.UI;
using Zenject;

namespace MiniFactory.UI
{
    public sealed class BoostView : AbstractButton
    {
        [SerializeField] private TMP_Text _stateText;
        [SerializeField] private TMP_Text _multiplierText;
        [SerializeField] private TMP_Text _remainingTimeText;

        private double _displayedSeconds = -1;
        private bool _displayedActive;
        private bool _displayedAvailable;

        [Inject] private FactoryConfiguration _configuration;
        [Inject] private FactoryProduction _production;

        protected override void OnEnable()
        {
            base.OnEnable();
            _production.OnBoostStarted += Refresh;
            _production.OnBoostFinished += Refresh;
            _multiplierText.SetText("×{0}", (float)_configuration.BoostMultiplier);
            Refresh();
        }

        private void Update()
        {
            UpdateDisplay(false);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _production.OnBoostStarted -= Refresh;
            _production.OnBoostFinished -= Refresh;
        }

        public override void OnButtonClick()
        {
            _production.TryStartBoost();
            Refresh();
        }

        private void Refresh()
        {
            UpdateDisplay(true);
        }

        private void UpdateDisplay(bool force)
        {
            bool active = _production.IsBoostActive;
            bool available = _production.CanStartBoost;
            double remainingSeconds = Math.Ceiling(_production.BoostRemainingSeconds);
            _btn.interactable = available;

            if (force || active != _displayedActive || available != _displayedAvailable)
            {
                _displayedActive = active;
                _displayedAvailable = available;
                _stateText.text = !_configuration.BoostEnabled ? "Boost выключен"
                    : active ? "Boost активен" : available ? "Boost готов" : "Boost недоступен";
            }

            if (force || remainingSeconds != _displayedSeconds)
            {
                _displayedSeconds = remainingSeconds;

                if (active)
                {
                    _remainingTimeText.SetText("Осталось: {0} с", (float)remainingSeconds);
                }
                else
                {
                    _remainingTimeText.text = string.Empty;
                }
            }
        }
    }
}
