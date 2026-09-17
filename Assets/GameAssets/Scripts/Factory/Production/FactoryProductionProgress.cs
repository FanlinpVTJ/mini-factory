using System;

namespace MiniFactory.Production
{
    [Serializable]
    public sealed class FactoryProductionProgress
    {
        public long LastProductionUtcTicks;
        public long BoostEndUtcTicks;
        public double IncomeRemainder;
    }
}
