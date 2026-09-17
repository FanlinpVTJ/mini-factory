using System;
using System.Collections.Generic;
using MiniFactory.Configuration;
using MiniFactory.Economy;
using MiniFactory.Production;
using NUnit.Framework;
using ValueSystem;
using ValueSystem.Save;

namespace MiniFactory.Tests
{
    public sealed class FactoryLogicTests
    {
        private const string CURRENCY_IDENTIFIER = "coins";
        private const long INITIAL_UTC_TICKS = 100 * TimeSpan.TicksPerSecond;

        [Test]
        public void UnlockMachineSpendsCurrencyAndChangesState()
        {
            FactoryConfiguration configuration = CreateConfiguration(10);
            TestValueSystem valueSystem = new TestValueSystem(CURRENCY_IDENTIFIER, 150);
            FactoryEconomy economy = new FactoryEconomy(configuration, valueSystem);

            bool unlocked = economy.TryUnlock("machine_two");

            Assert.That(unlocked, Is.True);
            Assert.That(economy.GetMachine("machine_two").State, Is.EqualTo(MachineStateType.Unlocked));
            Assert.That(economy.GetMachine("machine_two").Level, Is.EqualTo(1));
            Assert.That(economy.Balance, Is.EqualTo(50));
        }

        [Test]
        public void UpgradeMachineUsesConfiguredPriceProgression()
        {
            FactoryConfiguration configuration = CreateConfiguration(10);
            TestValueSystem valueSystem = new TestValueSystem(CURRENCY_IDENTIFIER, 1000);
            FactoryEconomy economy = new FactoryEconomy(configuration, valueSystem);

            bool firstUpgrade = economy.TryUpgrade("machine_one");
            bool secondUpgrade = economy.TryUpgrade("machine_one");

            Assert.That(firstUpgrade, Is.True);
            Assert.That(secondUpgrade, Is.True);
            Assert.That(economy.GetMachine("machine_one").Level, Is.EqualTo(3));
            Assert.That(economy.Balance, Is.EqualTo(940));
        }

        [Test]
        public void OfflineIncomeDoesNotExceedConfiguredDuration()
        {
            FactoryConfiguration configuration = CreateConfiguration(10);
            TestValueSystem valueSystem = new TestValueSystem(CURRENCY_IDENTIFIER, 0);
            FactoryEconomy economy = new FactoryEconomy(configuration, valueSystem);
            TestFactoryClock clock = new TestFactoryClock(INITIAL_UTC_TICKS + 100 * TimeSpan.TicksPerSecond);
            FactoryProduction production = new FactoryProduction(configuration, economy, clock);
            FactoryProductionProgress progress = new FactoryProductionProgress
            {
                LastProductionUtcTicks = INITIAL_UTC_TICKS
            };

            production.Initialize(progress);

            Assert.That(economy.Balance, Is.EqualTo(100));
            Assert.That(production.LastOfflineIncome, Is.EqualTo(100));
        }

        [Test]
        public void BoostUsesElapsedTimeAndFinishesAtConfiguredTime()
        {
            FactoryConfiguration configuration = CreateConfiguration(30);
            TestValueSystem valueSystem = new TestValueSystem(CURRENCY_IDENTIFIER, 0);
            FactoryEconomy economy = new FactoryEconomy(configuration, valueSystem);
            TestFactoryClock clock = new TestFactoryClock(INITIAL_UTC_TICKS);
            FactoryProduction production = new FactoryProduction(configuration, economy, clock);
            FactoryProductionProgress progress = new FactoryProductionProgress
            {
                LastProductionUtcTicks = INITIAL_UTC_TICKS
            };
            production.Initialize(progress);

            bool started = production.TryStartBoost();
            clock.UtcTicks += 15 * TimeSpan.TicksPerSecond;
            production.Tick();

            Assert.That(started, Is.True);
            Assert.That(production.IsBoostActive, Is.False);
            Assert.That(economy.Balance, Is.EqualTo(250));
        }

        private static FactoryConfiguration CreateConfiguration(double maximumOfflineSeconds)
        {
            MachineConfiguration[] machines =
            {
                new MachineConfiguration("machine_one", true, 10, 0, 20, 2, 5),
                new MachineConfiguration("machine_two", false, 20, 100, 50, 2, 5),
                new MachineConfiguration("machine_three", false, 30, 250, 100, 2, 5)
            };
            PurchaseConfiguration purchase = new PurchaseConfiguration();
            FactoryConfiguration configuration = new FactoryConfiguration(CURRENCY_IDENTIFIER, machines, true,
                10, 2, maximumOfflineSeconds, purchase);

            return configuration;
        }

        private sealed class TestFactoryClock : IFactoryClock
        {
            public long UtcTicks { get; set; }

            public TestFactoryClock(long utcTicks)
            {
                UtcTicks = utcTicks;
            }
        }

        private sealed class TestValueSystem : IValueSystem
        {
            private readonly Dictionary<string, float> _values = new Dictionary<string, float>();

            public IReadOnlyDictionary<string, ValueHandler> ValueHandler => new Dictionary<string, ValueHandler>();

            public TestValueSystem(string identifier, float value)
            {
                _values.Add(identifier, value);
            }

            public void Setup(ValuesSave save)
            {
                throw new NotSupportedException();
            }

            public bool Change(ValueData data, float amount, bool forced = false)
            {
                return Change(data.Id, amount, forced);
            }

            public bool Change(string valueId, float amount, bool forced = false)
            {
                return amount >= 0 ? Add(valueId, amount) : TrySubtract(valueId, -amount, forced);
            }

            public bool Add(ValueData data, float amount)
            {
                return Add(data.Id, amount);
            }

            public bool Add(string valueId, float amount)
            {
                _values[valueId] += amount;
                return true;
            }

            public bool CanSubtract(ValueData data, float amount)
            {
                return CanSubtract(data.Id, amount);
            }

            public bool CanSubtract(string valueId, float amount)
            {
                return _values[valueId] >= amount;
            }

            public ValueHandler GetValueHandler(ValueData data)
            {
                throw new NotSupportedException();
            }

            public ValueHandler GetValueHandler(string valueId)
            {
                throw new NotSupportedException();
            }

            public float GetValue(ValueData data)
            {
                return GetValue(data.Id);
            }

            public float GetValue(string valueId)
            {
                return _values[valueId];
            }

            public bool TrySubtract(ValueData data, float amount, bool forced = false)
            {
                return TrySubtract(data.Id, amount, forced);
            }

            public bool TrySubtract(string valueId, float amount, bool forced = false)
            {
                if (!forced && !CanSubtract(valueId, amount))
                {
                    return false;
                }

                _values[valueId] = Math.Max(0, _values[valueId] - amount);
                return true;
            }

            public void SetValue(ValueData data, float value)
            {
                SetValue(data.Id, value);
            }

            public void SetValue(string id, float value)
            {
                _values[id] = value;
            }
        }
    }
}
