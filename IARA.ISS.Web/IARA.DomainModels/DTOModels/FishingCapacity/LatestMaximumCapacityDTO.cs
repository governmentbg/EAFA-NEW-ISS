using System;

namespace IARA.DomainModels.DTOModels.FishingCapacity
{
    public class LatestMaximumCapacityDTO
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public DateTime? PrevDate { get; set; }
    }
}
