using System;

namespace IARA.Interfaces.Common
{
    public interface IFileVersionTrackerService : IDisposable
    {
        string GetVersion();
        void NotifyVersionChange();
    }
}
