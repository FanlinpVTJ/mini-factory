using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiniFactory.Analytics
{
    public sealed class ConsoleAnalyticsProvider : IAnalyticsProvider
    {
        public void TrackEvent(string eventName, Dictionary<string, string> parameters)
        {
            StringBuilder message = new StringBuilder("Analytics: ");
            message.Append(eventName);

            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                message.Append(" | ");
                message.Append(parameter.Key);
                message.Append('=');
                message.Append(parameter.Value);
            }

            Debug.Log(message.ToString());
        }
    }
}
