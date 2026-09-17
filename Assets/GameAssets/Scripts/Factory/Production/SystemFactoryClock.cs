using System;

namespace MiniFactory.Production
{
    public sealed class SystemFactoryClock : IFactoryClock
    {
        public long UtcTicks => DateTime.UtcNow.Ticks;
    }
}
