using System;
using MiniFactory.Persistence;
using ValueSystem.Save;

namespace GameSaver
{
    [Serializable]
    public class PlayerProfile : ISaveLoadReciever
    {
        public ValuesSave GlobalValuesSave = new ValuesSave(GameSaver.SaveActiveProfile);
        public FactoryProgress FactoryProgress;

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (GlobalValuesSave == null)
            {
                GlobalValuesSave = new ValuesSave(GameSaver.SaveActiveProfile);
            }

            GlobalValuesSave.SetSaveAction(GameSaver.SaveActiveProfile);
        }
    }
}
