using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Insp.Domain.Entities.Nomenclatures
{
    public class NVesselActivity : ICodeNomenclature
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool HasAdditionalDescr { get; set; }
        public bool IsFishingActivity { get; set; }
        public bool IsActive { get; set; }
    }
}
