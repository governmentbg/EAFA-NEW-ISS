using System;
using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Insp.Domain.Entities.Inspections
{
    public class FishingGearMark : IEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int FishingGearId { get; set; }
        public string Number { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int StatusId { get; set; }
    }
}
