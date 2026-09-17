using System;
using System.Collections.Generic;
using System.Globalization;
using MiniFactory.Economy;
using MiniFactory.Production;
using MiniFactory.Purchasing;
using Zenject;

namespace MiniFactory.Analytics
{
    public sealed class FactoryAnalytics : IInitializable, IDisposable
    {
        private readonly IAnalyticsService _analytics;
        private readonly FactoryEconomy _economy;
        private readonly FactoryProduction _production;
        private readonly IPurchaseService _purchaseService;

        public FactoryAnalytics(IAnalyticsService analytics, FactoryEconomy economy,
            FactoryProduction production, IPurchaseService purchaseService)
        {
            _analytics = analytics;
            _economy = economy;
            _production = production;
            _purchaseService = purchaseService;
        }

        public void Initialize()
        {
            _economy.OnMachineUnlocked += TrackMachineUnlocked;
            _economy.OnMachineUpgraded += TrackMachineUpgraded;
            _production.OnBoostStarted += TrackBoostStarted;
            _production.OnBoostFinished += TrackBoostFinished;
            _production.OnOfflineIncomeApplied += TrackOfflineIncomeApplied;
            _purchaseService.OnPurchaseSucceeded += TrackPurchaseSucceeded;
            _purchaseService.OnPurchaseFailed += TrackPurchaseFailed;
            _analytics.TrackEvent("game_started");
        }

        public void Dispose()
        {
            _economy.OnMachineUnlocked -= TrackMachineUnlocked;
            _economy.OnMachineUpgraded -= TrackMachineUpgraded;
            _production.OnBoostStarted -= TrackBoostStarted;
            _production.OnBoostFinished -= TrackBoostFinished;
            _production.OnOfflineIncomeApplied -= TrackOfflineIncomeApplied;
            _purchaseService.OnPurchaseSucceeded -= TrackPurchaseSucceeded;
            _purchaseService.OnPurchaseFailed -= TrackPurchaseFailed;
        }

        private void TrackMachineUnlocked(string machineIdentifier, int level)
        {
            Dictionary<string, string> parameters = CreateMachineParameters(machineIdentifier, level);
            _analytics.TrackEvent("machine_unlocked", parameters);
        }

        private void TrackMachineUpgraded(string machineIdentifier, int level)
        {
            Dictionary<string, string> parameters = CreateMachineParameters(machineIdentifier, level);
            _analytics.TrackEvent("machine_upgraded", parameters);
        }

        private void TrackBoostStarted()
        {
            _analytics.TrackEvent("boost_started");
        }

        private void TrackBoostFinished()
        {
            _analytics.TrackEvent("boost_finished");
        }

        private void TrackOfflineIncomeApplied(double income)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "income", income.ToString(CultureInfo.InvariantCulture) }
            };
            _analytics.TrackEvent("offline_income_applied", parameters);
        }

        private void TrackPurchaseSucceeded(string productIdentifier)
        {
            Dictionary<string, string> parameters = CreatePurchaseParameters(productIdentifier);
            _analytics.TrackEvent("purchase_succeeded", parameters);
        }

        private void TrackPurchaseFailed(string productIdentifier, string reason)
        {
            Dictionary<string, string> parameters = CreatePurchaseParameters(productIdentifier);
            parameters.Add("reason", reason);
            _analytics.TrackEvent("purchase_failed", parameters);
        }

        private Dictionary<string, string> CreateMachineParameters(string machineIdentifier, int level)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "machine_id", machineIdentifier },
                { "level", level.ToString(CultureInfo.InvariantCulture) }
            };

            return parameters;
        }

        private Dictionary<string, string> CreatePurchaseParameters(string productIdentifier)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "product_id", productIdentifier }
            };

            return parameters;
        }
    }
}
