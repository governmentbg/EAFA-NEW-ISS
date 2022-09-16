using System.Collections.Generic;

namespace IARA.RegixAbstractions.Models
{
    public class VesselRequest
    {
        public bool SearchByOwners { get; set; } = false;

        public string MainEngineNumber { get; set; }
        public bool MainEngineNumberChanged { get; set; }
        public string HullNumber { get; set; }
        public bool HullMaterialTypeChanged { get; set; }
        public int? VesselType { get; set; }
        public bool VesselTypeChanged { get; set; }
        public decimal? TotalLength { get; set; }
        public bool TotalLengthChanged { get; set; }
        public List<string> OwnersBulstatEGN { get; set; }
        public bool OwnersChanged { get; set; }

        public decimal? LengthBetweenPerpendiculars { get; set; }

        public bool LengthBetweenPerpendicularsChanged { get; set; }

        public string RegistrationVolume { get; set; }
        public bool RegistrationVolumeChanged { get; set; }
        public string RegistrationPage { get; set; }
        public bool RegistrationPageChanged { get; set; }
    }
}
