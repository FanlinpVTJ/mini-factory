using System.Collections.Generic;

namespace MiniFactory.Analytics
{
    public sealed class AnalyticsService : IAnalyticsService
    {
        private readonly List<IAnalyticsProvider> _providers;

        public AnalyticsService(List<IAnalyticsProvider> providers)
        {
            _providers = providers;
        }

        public void TrackEvent(string eventName)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            TrackEvent(eventName, parameters);
        }

        public void TrackEvent(string eventName, Dictionary<string, string> parameters)
        {
            foreach (IAnalyticsProvider provider in _providers)
            {
                provider.TrackEvent(eventName, parameters);
            }
        }
    }
}
