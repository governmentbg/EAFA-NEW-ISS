using IARA.Mobile.Insp.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Helpers;

namespace IARA.Mobile.Insp.Utilities
{
    public class OfflineFilesUtility : IOfflineFiles
    {
        public void DeleteFiles(string inspectionIdentifier)
        {
            InspectionFilesHelper.DeleteFiles(inspectionIdentifier);
        }
    }
}
