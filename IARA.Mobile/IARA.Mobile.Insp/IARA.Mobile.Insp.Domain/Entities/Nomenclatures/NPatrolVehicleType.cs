using IARA.Mobile.Domain.Interfaces;
using IARA.Mobile.Insp.Domain.Enums;
using SQLite;

namespace IARA.Mobile.Insp.Domain.Entities.Nomenclatures
{
    public class NPatrolVehicleType : ICodeNomenclature
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public PatrolVehicleType VehicleType { get; set; }
        public bool IsActive { get; set; }
    }
}
