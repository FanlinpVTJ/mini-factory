using System.Collections.Generic;

namespace MiniFactory.Analytics
{
    public interface IAnalyticsProvider
    {
        void TrackEvent(string eventName, Dictionary<string, string> parameters);
    }
}
