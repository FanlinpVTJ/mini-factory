using System;
using MiniFactory.Configuration;
using MiniFactory.Economy;

namespace MiniFactory.Production
{
    public sealed class FactoryProduction : IDisposable
    {
        public event Action OnBoostStarted;
        public event Action OnBoostFinished;
        public event Action<double> OnOfflineIncomeApplied;

        private readonly FactoryConfiguration _configuration;
        private readonly FactoryEconomy _economy;
        private readonly IFactoryClock _clock;
        private long _lastProductionUtcTicks;
        private long _boostEndUtcTicks;
        private double _incomeRemainder;
        private bool _isInitialized;
        private bool _isSuspended;

        public bool IsBoostActive => _configuration.BoostEnabled && BoostRemainingSeconds > 0;
        public bool CanStartBoost => _isInitialized && !_isSuspended && _configuration.BoostEnabled && !IsBoostActive;
        public double BoostRemainingSeconds => Math.Max(0, (_boostEndUtcTicks - CurrentUtcTicks) / (double)TimeSpan.TicksPerSecond);
        public double ProductionPerSecond => _economy.ProductionPerSecond * (IsBoostActive ? _configuration.BoostMultiplier : 1);
        public double LastOfflineIncome { get; private set; }
        public long OfflineIncomeVersion { get; private set; }
        private long CurrentUtcTicks => Math.Max(_clock.UtcTicks, _lastProductionUtcTicks);

        public FactoryProduction(FactoryConfiguration configuration, FactoryEconomy economy, IFactoryClock clock)
        {
            _configuration = configuration;
            _economy = economy;
            _clock = clock;
            _economy.OnBeforeMachineChange += Tick;
        }

        public void Initialize(FactoryProductionProgress progress)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException("Factory production has already been initialized.");
            }

            if (progress.LastProductionUtcTicks < 0 || progress.LastProductionUtcTicks > DateTime.MaxValue.Ticks
                || progress.BoostEndUtcTicks < 0 || progress.BoostEndUtcTicks > DateTime.MaxValue.Ticks
                || double.IsNaN(progress.IncomeRemainder) || double.IsInfinity(progress.IncomeRemainder))
            {
                throw new ArgumentException("Production timestamps must be valid UTC ticks.");
            }

            _lastProductionUtcTicks = progress.LastProductionUtcTicks == 0 ? _clock.UtcTicks : progress.LastProductionUtcTicks;
            _boostEndUtcTicks = _configuration.BoostEnabled ? progress.BoostEndUtcTicks : 0;
            _incomeRemainder = progress.IncomeRemainder;
            _isInitialized = true;
            ApplyOfflineIncome();
        }

        public void Tick()
        {
            if (!_isInitialized || _isSuspended)
            {
                return;
            }

            Advance(double.MaxValue);
        }

        public bool TryStartBoost()
        {
            if (!_isInitialized || _isSuspended || !_configuration.BoostEnabled)
            {
                return false;
            }

            Tick();

            if (IsBoostActive)
            {
                return false;
            }

            long availableTicks = DateTime.MaxValue.Ticks - _lastProductionUtcTicks;
            double durationTicks = _configuration.BoostDurationSeconds * TimeSpan.TicksPerSecond;
            _boostEndUtcTicks = durationTicks >= availableTicks
                ? DateTime.MaxValue.Ticks
                : _lastProductionUtcTicks + (long)durationTicks;
            OnBoostStarted?.Invoke();
            return true;
        }

        public void Suspend()
        {
            if (!_isInitialized || _isSuspended)
            {
                return;
            }

            Tick();
            _isSuspended = true;
        }

        public void Resume()
        {
            if (!_isInitialized || !_isSuspended)
            {
                return;
            }

            _isSuspended = false;
            ApplyOfflineIncome();
        }

        public FactoryProductionProgress CaptureProgress()
        {
            FactoryProductionProgress progress = new FactoryProductionProgress
            {
                LastProductionUtcTicks = _lastProductionUtcTicks,
                BoostEndUtcTicks = _boostEndUtcTicks,
                IncomeRemainder = _incomeRemainder
            };

            return progress;
        }

        public void Dispose()
        {
            _economy.OnBeforeMachineChange -= Tick;
        }

        private void ApplyOfflineIncome()
        {
            double income = Advance(_configuration.MaximumOfflineSeconds);
            LastOfflineIncome = income;

            if (income > 0)
            {
                OfflineIncomeVersion++;
                OnOfflineIncomeApplied?.Invoke(income);
            }
        }

        private double Advance(double maximumSeconds)
        {
            long currentUtcTicks = CurrentUtcTicks;
            double elapsedSeconds = Math.Min((currentUtcTicks - _lastProductionUtcTicks) / (double)TimeSpan.TicksPerSecond, maximumSeconds);
            double boostedSeconds = _configuration.BoostEnabled
                ? Math.Min(elapsedSeconds, Math.Max(0, (_boostEndUtcTicks - _lastProductionUtcTicks) / (double)TimeSpan.TicksPerSecond))
                : 0;
            double income = _economy.ProductionPerSecond * (elapsedSeconds + boostedSeconds * (_configuration.BoostMultiplier - 1));
            bool boostFinished = _boostEndUtcTicks > 0 && currentUtcTicks >= _boostEndUtcTicks;
            _lastProductionUtcTicks = currentUtcTicks;

            if (boostFinished)
            {
                _boostEndUtcTicks = 0;
            }

            double previousBalance = _economy.Balance;
            _incomeRemainder += income;

            if (_incomeRemainder > 0)
            {
                double availableIncome = Math.Min(_incomeRemainder, float.MaxValue - previousBalance);
                _economy.AddIncome(availableIncome);
                _incomeRemainder -= _economy.Balance - previousBalance;

                if (_economy.Balance >= float.MaxValue)
                {
                    _incomeRemainder = 0;
                }
            }

            if (boostFinished)
            {
                OnBoostFinished?.Invoke();
            }

            return _economy.Balance - previousBalance;
        }
    }
}
