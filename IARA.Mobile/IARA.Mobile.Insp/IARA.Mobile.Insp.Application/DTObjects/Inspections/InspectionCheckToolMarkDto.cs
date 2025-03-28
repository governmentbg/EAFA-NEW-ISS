﻿using System.Collections.Generic;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionCheckToolMarkDto : InspectionEditDto
    {
        public VesselDuringInspectionDto InspectedShip { get; set; }
        public PortVisitDto Port { get; set; }
        public int? PoundNetId { get; set; }
        public int? CheckReasonId { get; set; }
        public int? RecheckReasonId { get; set; }
        public string OtherRecheckReason { get; set; }
        public string OwnerComment { get; set; }


        public int? PermitLicenseRegisterId { get; set; }

        public int? PermitLicenseYear { get; set; }


        public int? PermitId { get; set; }
        public string UnregisteredPermitNumber { get; set; }
        public int? UnregisteredPermitYear { get; set; }
        public List<InspectedFishingGearDto> FishingGears { get; set; }
    }
}
