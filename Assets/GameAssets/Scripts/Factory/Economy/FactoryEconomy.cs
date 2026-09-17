using System;
using System.Collections.Generic;
using MiniFactory.Configuration;
using MiniFactory.Persistence;
using MiniFactory.Production;
using ValueSystem;

namespace MiniFactory.Economy
{
    public sealed class FactoryEconomy
    {
        public event Action OnChanged;
        public event Action OnBeforeMachineChange;
        public event Action OnMachineChanged;
        public event Action<string, int> OnMachineUnlocked;
        public event Action<string, int> OnMachineUpgraded;

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
            OnBeforeMachineChange?.Invoke();
            MachineState machine = GetMachine(identifier);

            if (machine.State != MachineStateType.Locked || !_valueSystem.TrySubtract(_currencyIdentifier, (float)machine.UnlockPrice))
            {
                return false;
            }

            _machines[identifier] = new MachineState(_configurations[identifier], 1);
            RecalculateProduction();
            OnMachineUnlocked?.Invoke(identifier, 1);
            OnMachineChanged?.Invoke();
            OnChanged?.Invoke();
            return true;
        }

        public bool TryUpgrade(string identifier)
        {
            OnBeforeMachineChange?.Invoke();
            MachineState machine = GetMachine(identifier);

            if (!machine.CanUpgrade || !_valueSystem.TrySubtract(_currencyIdentifier, (float)machine.NextUpgradePrice))
            {
                return false;
            }

            int level = machine.Level + 1;
            _machines[identifier] = new MachineState(_configurations[identifier], level);
            RecalculateProduction();
            OnMachineUpgraded?.Invoke(identifier, level);
            OnMachineChanged?.Invoke();
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

        public FactoryProgress CaptureProgress(FactoryProductionProgress production)
        {
            MachineProgress[] machines = new MachineProgress[_machines.Count];
            int index = 0;

            foreach (MachineState machine in _machines.Values)
            {
                machines[index] = new MachineProgress
                {
                    Identifier = machine.Identifier,
                    Level = machine.Level
                };
                index++;
            }

            FactoryProgress progress = new FactoryProgress
            {
                CurrencyIdentifier = _currencyIdentifier,
                Machines = machines,
                Production = production
            };

            return progress;
        }

        public void RestoreProgress(FactoryProgress progress)
        {
            if (progress.Version != 1 || progress.CurrencyIdentifier != _currencyIdentifier)
            {
                throw new ArgumentException("Factory progress does not match the current currency or save version.");
            }

            Dictionary<string, MachineState> restoredMachines = new Dictionary<string, MachineState>();

            foreach (MachineProgress machine in progress.Machines)
            {
                if (_configurations.TryGetValue(machine.Identifier, out MachineConfiguration configuration))
                {
                    restoredMachines.Add(machine.Identifier, new MachineState(configuration, Math.Min(machine.Level, configuration.MaximumLevel)));
                }
            }

            foreach (KeyValuePair<string, MachineState> machine in restoredMachines)
            {
                _machines[machine.Key] = machine.Value;
            }

            RecalculateProduction();
            OnChanged?.Invoke();
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
