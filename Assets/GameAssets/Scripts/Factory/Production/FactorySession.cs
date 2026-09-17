using System;
using MiniFactory.Economy;
using MiniFactory.Persistence;
using Zenject;

namespace MiniFactory.Production
{
    public sealed class FactorySession : IInitializable, ITickable, IDisposable
    {
        private const long SAVE_INTERVAL_TICKS = 10 * TimeSpan.TicksPerSecond;

        private readonly FactoryEconomy _economy;
        private readonly FactoryProduction _production;
        private readonly IFactoryProgressStorage _storage;
        private readonly IFactoryClock _clock;
        private long _lastSaveUtcTicks;
        private bool _isInitialized;
        private bool _isSuspended;

        public FactorySession(FactoryEconomy economy, FactoryProduction production,
            IFactoryProgressStorage storage, IFactoryClock clock)
        {
            _economy = economy;
            _production = production;
            _storage = storage;
            _clock = clock;
        }

        public void Initialize()
        {
            FactoryProductionProgress productionProgress = new FactoryProductionProgress();

            if (_storage.TryLoad(out FactoryProgress progress))
            {
                _economy.RestoreProgress(progress);
                productionProgress = progress.Production;
            }

            _production.Initialize(productionProgress);
            _storage.OnBeforeSaving += CaptureProgress;
            _economy.OnMachineChanged += Save;
            _production.OnBoostStarted += Save;
            _production.OnBoostFinished += Save;
            _isInitialized = true;
            Save();
        }

        public void Tick()
        {
            if (!_isInitialized || _isSuspended)
            {
                return;
            }

            _production.Tick();

            if (_clock.UtcTicks - _lastSaveUtcTicks >= SAVE_INTERVAL_TICKS)
            {
                Save();
            }
        }

        public void SetSuspended(bool suspended)
        {
            if (!_isInitialized || _isSuspended == suspended)
            {
                return;
            }

            _isSuspended = suspended;

            if (suspended)
            {
                _production.Suspend();
            }
            else
            {
                _production.Resume();
            }

            Save();
        }

        public void Save()
        {
            if (!_isInitialized)
            {
                return;
            }

            _storage.Flush();
            _lastSaveUtcTicks = _clock.UtcTicks;
        }

        public void Dispose()
        {
            if (!_isInitialized)
            {
                return;
            }

            SetSuspended(true);
            _storage.OnBeforeSaving -= CaptureProgress;
            _economy.OnMachineChanged -= Save;
            _production.OnBoostStarted -= Save;
            _production.OnBoostFinished -= Save;
            _production.Dispose();
            _isInitialized = false;
        }

        private void CaptureProgress()
        {
            FactoryProgress progress = _economy.CaptureProgress(_production.CaptureProgress());
            _storage.UpdateProgress(progress);
        }
    }
}
