using System;
using IARA.Mobile.Domain.Interfaces;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class FishingGearMarkInspectionNomenclatureDto : IActive
    {
        public int Id { get; set; }
        public int FishingGearId { get; set; }
        public string Number { get; set; }
        public int StatusId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsActive { get; set; }
    }
}
