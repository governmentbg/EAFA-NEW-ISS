using System;

namespace IARA.DomainModels.DTOModels.FishingCapacity
{
    public class MaximumFishingCapacityDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Regulation { get; set; }
        public decimal GrossTonnage { get; set; }
        public decimal Power { get; set; }
        public bool IsActive { get; set; }
    }
}
