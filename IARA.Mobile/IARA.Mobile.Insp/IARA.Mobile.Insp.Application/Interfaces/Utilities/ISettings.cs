using System;

namespace IARA.Mobile.Insp.Application.Interfaces.Utilities
{
    public interface ISettings
    {
        bool SuccessfulLogin { get; set; }

        bool IsDeviceAllowed { get; set; }

        bool IsInspectorAllowed { get; set; }

        double FontSize { get; set; }

        DateTime? LastInspectionFetchDate { get; set; }

        int[] Fleets { get; set; }

        string LastVersion { get; set; }

        int LockInspectionAfterHours { get; set; }
        int LatestSubmissionDateForInspection { get; set; }

        void Clear();
    }
}
