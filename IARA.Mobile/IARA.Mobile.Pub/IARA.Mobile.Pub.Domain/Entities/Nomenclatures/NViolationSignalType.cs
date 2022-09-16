using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Pub.Domain.Entities.Nomenclatures
{
    public class NViolationSignalType : INomenclature
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
