using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Helpers;

namespace IARA.Mobile.Pub.Utilities
{
    public class FilesUtility : IOfflineFiles
    {
        public void DeleteFiles(string identifier)
        {
            CatchRecordFilesHelper.DeleteFiles(identifier);
        }
    }
}
