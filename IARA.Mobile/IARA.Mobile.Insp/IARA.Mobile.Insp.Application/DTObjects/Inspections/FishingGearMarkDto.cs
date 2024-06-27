using IARA.Mobile.Insp.Domain.Enums;
using System;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class FishingGearMarkDto
    {
        public int? Id { get; set; }
        public PrefixInputDto FullNumber { get; set; }
        public int StatusId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public FishingGearMarkStatus SelectedStatus { get; set; }
        public bool IsActive { get; set; }
    }
}
