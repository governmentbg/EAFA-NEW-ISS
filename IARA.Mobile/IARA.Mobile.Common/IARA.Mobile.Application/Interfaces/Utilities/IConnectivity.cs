using System;
using IARA.Mobile.Domain.Enums;

namespace IARA.Mobile.Application.Interfaces.Utilities
{
    public interface IConnectivity
    {
        void RunServerHealthChecker();

        event EventHandler<InternetStatus> ConnectivityChanged;
        event EventHandler OfflineDataPosted;
    }
}
