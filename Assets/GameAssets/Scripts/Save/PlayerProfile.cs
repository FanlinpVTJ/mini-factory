using System;
using MiniFactory.Persistence;
using ValueSystem.Save;
using System.Collections.Generic;

namespace GameSaver
{
    [Serializable]
    public class PlayerProfile : ISaveLoadReciever
    {
        public ValuesSave GlobalValuesSave = new ValuesSave(GameSaver.SaveActiveProfile);
        public FactoryProgress FactoryProgress;
        public List<string> ProcessedPurchaseIdentifiers = new List<string>();

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

            if (ProcessedPurchaseIdentifiers == null)
            {
                ProcessedPurchaseIdentifiers = new List<string>();
            }
        }
    }
}
