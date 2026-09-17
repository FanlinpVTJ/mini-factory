using System;
using MiniFactory.Configuration;

namespace MiniFactory.Economy
{
    public sealed class MachineState
    {
        private readonly MachineConfiguration _configuration;

        public string Identifier => _configuration.Identifier;
        public int Level { get; }
        public MachineStateType State => Level == 0 ? MachineStateType.Locked : MachineStateType.Unlocked;
        public double ProductionPerSecond => _configuration.ProductionPerSecond * Level;
        public double UnlockPrice => (float)_configuration.UnlockPrice;
        public bool CanUpgrade => Level > 0 && Level < _configuration.MaximumLevel;
        public double NextUpgradePrice => CanUpgrade
            ? (float)Math.Ceiling(_configuration.BaseUpgradePrice * Math.Pow(_configuration.UpgradePriceMultiplier, Level - 1))
            : 0;

        public MachineState(MachineConfiguration configuration, int level)
        {
            if (level < 0 || level > configuration.MaximumLevel)
            {
                throw new ArgumentOutOfRangeException(nameof(level));
            }

            _configuration = configuration;
            Level = level;
        }
    }
}
