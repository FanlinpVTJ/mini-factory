using System;
using ProjectGameSaver = GameSaver.GameSaver;

namespace MiniFactory.Persistence
{
    public sealed class GameSaverFactoryProgressStorage : IFactoryProgressStorage
    {
        public event Action OnBeforeSaving
        {
            add => ProjectGameSaver.OnBeforeSavingSave += value;
            remove => ProjectGameSaver.OnBeforeSavingSave -= value;
        }

        public bool TryLoad(out FactoryProgress progress)
        {
            progress = ProjectGameSaver.GetActiveProfile().FactoryProgress;
            return progress != null && progress.IsValid();
        }

        public void UpdateProgress(FactoryProgress progress)
        {
            ProjectGameSaver.GetActiveProfile().FactoryProgress = progress;
        }

        public void Flush()
        {
            ProjectGameSaver.ForceSave();
        }
    }
}
