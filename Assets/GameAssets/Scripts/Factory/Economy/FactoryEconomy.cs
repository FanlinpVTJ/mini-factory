using System;
using System.Collections.Generic;
using MiniFactory.Configuration;
using ValueSystem;

namespace MiniFactory.Economy
{
    public sealed class FactoryEconomy
    {
        public event Action OnChanged;

        private readonly Dictionary<string, MachineConfiguration> _configurations = new Dictionary<string, MachineConfiguration>();
        private readonly Dictionary<string, MachineState> _machines = new Dictionary<string, MachineState>();
        private readonly IValueSystem _valueSystem;
        private readonly string _currencyIdentifier;

        public double Balance => _valueSystem.GetValue(_currencyIdentifier);
        public double ProductionPerSecond { get; private set; }

        public FactoryEconomy(FactoryConfiguration configuration, IValueSystem valueSystem)
        {
            configuration.Validate();
            _valueSystem = valueSystem;
            _currencyIdentifier = configuration.CurrencyIdentifier;

            foreach (MachineConfiguration machine in configuration.Machines)
            {
                _configurations.Add(machine.Identifier, machine);
                _machines.Add(machine.Identifier, new MachineState(machine, machine.InitiallyUnlocked ? 1 : 0));
            }

            RecalculateProduction();
        }

        public MachineState GetMachine(string identifier)
        {
            return _machines[identifier];
        }

        public bool TryUnlock(string identifier)
        {
            MachineState machine = GetMachine(identifier);

            if (machine.State != MachineStateType.Locked || !_valueSystem.TrySubtract(_currencyIdentifier, (float)machine.UnlockPrice))
            {
                return false;
            }

            _machines[identifier] = new MachineState(_configurations[identifier], 1);
            RecalculateProduction();
            OnChanged?.Invoke();
            return true;
        }

        public bool TryUpgrade(string identifier)
        {
            MachineState machine = GetMachine(identifier);

            if (!machine.CanUpgrade || !_valueSystem.TrySubtract(_currencyIdentifier, (float)machine.NextUpgradePrice))
            {
                return false;
            }

            _machines[identifier] = new MachineState(_configurations[identifier], machine.Level + 1);
            RecalculateProduction();
            OnChanged?.Invoke();
            return true;
        }

        public void AddIncome(double amount)
        {
            if (double.IsNaN(amount) || double.IsInfinity(amount) || amount < 0 || Balance + amount > float.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            if (amount == 0)
            {
                return;
            }

            if (_valueSystem.Add(_currencyIdentifier, (float)amount))
            {
                OnChanged?.Invoke();
            }
        }

        private void RecalculateProduction()
        {
            ProductionPerSecond = 0;

            foreach (MachineState machine in _machines.Values)
            {
                ProductionPerSecond += machine.ProductionPerSecond;
            }
        }
    }
}
