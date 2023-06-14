using System;

namespace IARA.Mobile.Insp.Application.Interfaces.Utilities
{
    public interface ISettings
    {
        bool SuccessfulLogin { get; set; }

        bool IsDeviceAllowed { get; set; }

        double FontSize { get; set; }

        DateTime? LastInspectionFetchDate { get; set; }

        int[] Fleets { get; set; }

        string LastVersion { get; set; }

        void Clear();
    }
}
