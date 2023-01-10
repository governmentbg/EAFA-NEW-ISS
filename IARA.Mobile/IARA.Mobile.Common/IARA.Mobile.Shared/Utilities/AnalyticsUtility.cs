using System;
using System.Collections.Generic;
using IARA.Mobile.Application.Interfaces.Utilities;
using Microsoft.AppCenter.Crashes;

namespace IARA.Mobile.Shared.Utilities
{
    public class AnalyticsUtility : IAnalytics
    {
        public void TrackError(Exception ex, Dictionary<string, string> logData = null)
        {
            Dictionary<string, string> data = new Dictionary<string, string> { { "Date", DateTime.Now.Date.ToString("MM/dd/yyyy HH:mm tt") }, };
            foreach (KeyValuePair<string, string> item in logData)
            {
                data.Add(item.Key, item.Value);
            }
            Crashes.TrackError(ex, data);
        }
    }
}
