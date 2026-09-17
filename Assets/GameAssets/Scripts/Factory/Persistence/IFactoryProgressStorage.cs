using System;

namespace MiniFactory.Persistence
{
    public interface IFactoryProgressStorage
    {
        event Action OnBeforeSaving;

        bool TryLoad(out FactoryProgress progress);
        void UpdateProgress(FactoryProgress progress);
        void Flush();
    }
}
