using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Pub.Domain.Entities.Nomenclatures
{
    public class NMunicipality : INomenclature
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int DistrictId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
