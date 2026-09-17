using MiniFactory.Economy;
using MiniFactory.Production;
using ValueSystem.Misc;
using WindowsManager.UI;
using Zenject;

namespace MiniFactory.UI
{
    public sealed class FactoryProductionView : AbstractText
    {
        private double _displayedProduction = -1;

        [Inject] private FactoryEconomy _economy;
        [Inject] private FactoryProduction _production;

        private void OnEnable()
        {
            _economy.OnChanged += Refresh;
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
            _economy.OnChanged -= Refresh;
            _production.OnBoostStarted -= Refresh;
            _production.OnBoostFinished -= Refresh;
        }

        private void Refresh()
        {
            double production = _production.ProductionPerSecond;

            if (production == _displayedProduction)
            {
                return;
            }

            _displayedProduction = production;
            string amount = production < 1000 ? production.ToString("G3") : TextFormatter.FormatNumber((float)production);
            _text.text = amount;
        }
    }
}
