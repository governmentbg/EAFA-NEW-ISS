using System;
using System.Collections.Generic;

namespace IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.LocalDb
{
    public class SFPermitReviewDto
    {
        public int Id { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public List<int> PermitReasonsIds { get; set; }
        public bool IsAllowedDuringMatingSeason { get; set; }

        public string RequesterFirstName { get; set; }
        public string RequesterMiddleName { get; set; }
        public string RequesterLastName { get; set; }
        public string RequesterEgn { get; set; }
        public string RequesterScientificOrganizationName { get; set; }
        public string RequesterPosition { get; set; }

        public List<SFHolderDto> Holders { get; set; }

        public DateTime ResearchPeriodFrom { get; set; }
        public DateTime ResearchPeriodTo { get; set; }
        public string ResearchWaterArea { get; set; }
        public string ResearchGoalsDescription { get; set; }

        public string FishTypesDescription { get; set; }
        public string FishTypesApp4ZBRDesc { get; set; }
        public string FishTypesCrayFish { get; set; }
        public string FishingGearDescription { get; set; }

        public bool IsShipRegistered { get; set; }
        public int? ShipId { get; set; }
        public string ShipName { get; set; }
        public string ShipExternalMark { get; set; }
        public string ShipCaptainName { get; set; }

        public string CoordinationCommittee { get; set; }
        public string CoordinationLetterNo { get; set; }
        public DateTime? CoordinationDate { get; set; }
    }
}
