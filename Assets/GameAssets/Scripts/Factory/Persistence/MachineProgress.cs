using System;

namespace MiniFactory.Persistence
{
    [Serializable]
    public sealed class MachineProgress
    {
        public string Identifier;
        public int Level;
    }
}
