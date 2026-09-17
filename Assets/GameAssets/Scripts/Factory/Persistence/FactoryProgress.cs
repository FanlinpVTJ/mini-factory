using System;
using System.Collections.Generic;
using MiniFactory.Production;

namespace MiniFactory.Persistence
{
    [Serializable]
    public sealed class FactoryProgress
    {
        public int Version = 1;
        public string CurrencyIdentifier;
        public MachineProgress[] Machines;
        public FactoryProductionProgress Production;

        public bool IsValid()
        {
            if (Version != 1 || string.IsNullOrWhiteSpace(CurrencyIdentifier)
                || Machines == null || Machines.Length == 0 || Production == null)
            {
                return false;
            }

            if (Production.LastProductionUtcTicks <= 0 || Production.LastProductionUtcTicks > DateTime.MaxValue.Ticks
                || Production.BoostEndUtcTicks < 0 || Production.BoostEndUtcTicks > DateTime.MaxValue.Ticks
                || double.IsNaN(Production.IncomeRemainder) || double.IsInfinity(Production.IncomeRemainder))
            {
                return false;
            }

            HashSet<string> identifiers = new HashSet<string>();

            foreach (MachineProgress machine in Machines)
            {
                if (machine == null || string.IsNullOrWhiteSpace(machine.Identifier)
                    || machine.Level < 0 || !identifiers.Add(machine.Identifier))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
