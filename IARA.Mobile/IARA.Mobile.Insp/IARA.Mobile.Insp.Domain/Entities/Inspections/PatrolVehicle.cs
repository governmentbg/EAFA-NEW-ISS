using IARA.Mobile.Domain.Interfaces;
using IARA.Mobile.Insp.Domain.Enums;
using SQLite;

namespace IARA.Mobile.Insp.Domain.Entities.Inspections
{
    public class PatrolVehicle : IEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int? InstitutionId { get; set; }
        public int? FlagId { get; set; }
        public int? PatrolVehicleTypeId { get; set; }
        public string Name { get; set; }
        public string CallSign { get; set; }
        public string ExternalMark { get; set; }
        public string RegistrationNumber { get; set; }
        public PatrolVehicleType VehicleType { get; set; }

        public string NormalizedName { get; set; }
        public string NormalizedCallSign { get; set; }
        public string NormalizedExternalMark { get; set; }
    }
}
