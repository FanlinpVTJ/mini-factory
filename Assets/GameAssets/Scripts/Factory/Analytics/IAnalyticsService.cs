using System.Collections.Generic;

namespace MiniFactory.Analytics
{
    public interface IAnalyticsService
    {
        void TrackEvent(string eventName);
        void TrackEvent(string eventName, Dictionary<string, string> parameters);
    }
}
