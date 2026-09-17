using System;
using UnityEngine;

namespace MiniFactory.Configuration
{
    [Serializable]
    public sealed class MachineConfiguration
    {
        [SerializeField] private string _identifier;
        [SerializeField] private bool _initiallyUnlocked;
        [SerializeField, Min(0)] private double _productionPerSecond;
        [SerializeField, Min(0)] private double _unlockPrice;
        [SerializeField, Min(1)] private double _baseUpgradePrice;
        [SerializeField, Min(1)] private double _upgradePriceMultiplier;
        [SerializeField, Min(1)] private int _maximumLevel;

        public string Identifier => _identifier;
        public bool InitiallyUnlocked => _initiallyUnlocked;
        public double ProductionPerSecond => _productionPerSecond;
        public double UnlockPrice => _unlockPrice;
        public double BaseUpgradePrice => _baseUpgradePrice;
        public double UpgradePriceMultiplier => _upgradePriceMultiplier;
        public int MaximumLevel => _maximumLevel;

        public MachineConfiguration(string identifier, bool initiallyUnlocked, double productionPerSecond,
            double unlockPrice, double baseUpgradePrice, double upgradePriceMultiplier, int maximumLevel)
        {
            _identifier = identifier;
            _initiallyUnlocked = initiallyUnlocked;
            _productionPerSecond = productionPerSecond;
            _unlockPrice = unlockPrice;
            _baseUpgradePrice = baseUpgradePrice;
            _upgradePriceMultiplier = upgradePriceMultiplier;
            _maximumLevel = maximumLevel;
        }
    }
}
