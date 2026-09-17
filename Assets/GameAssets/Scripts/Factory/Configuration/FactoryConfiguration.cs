using System;
using System.Collections.Generic;

namespace MiniFactory.Configuration
{
    public sealed class FactoryConfiguration
    {
        public string CurrencyIdentifier { get; }

        public MachineConfiguration[] Machines { get; }

        public bool BoostEnabled { get; }

        public double BoostDurationSeconds { get; }

        public double BoostMultiplier { get; }

        public double MaximumOfflineSeconds { get; }
        public PurchaseConfiguration Purchase { get; }

        public FactoryConfiguration(string currencyIdentifier, MachineConfiguration[] machines, bool boostEnabled,
            double boostDurationSeconds, double boostMultiplier, double maximumOfflineSeconds,
            PurchaseConfiguration purchase)
        {
            CurrencyIdentifier = currencyIdentifier;
            Machines = machines;
            BoostEnabled = boostEnabled;
            BoostDurationSeconds = boostDurationSeconds;
            BoostMultiplier = boostMultiplier;
            MaximumOfflineSeconds = maximumOfflineSeconds;
            Purchase = purchase;
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(CurrencyIdentifier))
            {
                throw new ArgumentException("Currency identifier must not be empty.");
            }

            ValidateNumber(BoostDurationSeconds, 1, nameof(BoostDurationSeconds));
            ValidateNumber(BoostMultiplier, 1, nameof(BoostMultiplier));
            ValidateNumber(MaximumOfflineSeconds, 0, nameof(MaximumOfflineSeconds));

            if (Purchase == null || string.IsNullOrWhiteSpace(Purchase.ProductIdentifier)
                || float.IsNaN(Purchase.CurrencyAmount) || float.IsInfinity(Purchase.CurrencyAmount)
                || Purchase.CurrencyAmount <= 0)
            {
                throw new ArgumentException("Purchase configuration must contain a product identifier and positive reward.");
            }

            if (Machines.Length < 3 || Machines[0] == null || !Machines[0].InitiallyUnlocked)
            {
                throw new ArgumentException("Configure at least three machines and unlock the first machine.");
            }

            HashSet<string> identifiers = new HashSet<string>();
            double maximumProduction = 0;

            foreach (MachineConfiguration machine in Machines)
            {
                if (machine == null || string.IsNullOrWhiteSpace(machine.Identifier) || !identifiers.Add(machine.Identifier))
                {
                    throw new ArgumentException("Every machine must have a unique nonempty identifier.");
                }

                ValidateNumber(machine.ProductionPerSecond, double.Epsilon, nameof(machine.ProductionPerSecond));
                ValidateNumber(machine.UnlockPrice, 0, nameof(machine.UnlockPrice));
                ValidateNumber(machine.BaseUpgradePrice, 1, nameof(machine.BaseUpgradePrice));
                ValidateNumber(machine.UpgradePriceMultiplier, 1, nameof(machine.UpgradePriceMultiplier));

                if (machine.MaximumLevel < 1)
                {
                    throw new ArgumentException("MaximumLevel must be positive.");
                }

                double maximumPrice = Math.Ceiling(machine.BaseUpgradePrice * Math.Pow(machine.UpgradePriceMultiplier, Math.Max(0, machine.MaximumLevel - 2)));
                ValidateNumber(maximumPrice, 1, "Maximum upgrade price");

                if (maximumPrice > float.MaxValue || machine.UnlockPrice > float.MaxValue)
                {
                    throw new ArgumentException("Machine prices must fit the ValueSystem currency range.");
                }

                maximumProduction += machine.ProductionPerSecond * machine.MaximumLevel;
            }

            ValidateNumber(maximumProduction * BoostMultiplier * Math.Max(1, MaximumOfflineSeconds), 0, "Maximum income");
        }

        private static void ValidateNumber(double value, double minimum, string name)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || value < minimum)
            {
                throw new ArgumentException($"{name} must be finite and at least {minimum}.");
            }
        }
    }
}
