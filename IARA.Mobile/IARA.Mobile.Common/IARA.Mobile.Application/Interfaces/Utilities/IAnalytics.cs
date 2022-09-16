using System;
using System.Collections.Generic;

namespace IARA.Mobile.Application.Interfaces.Utilities
{
    public interface IAnalytics
    {
        void TrackError(Exception ex, Dictionary<string, string> logData = null);
    }
}
