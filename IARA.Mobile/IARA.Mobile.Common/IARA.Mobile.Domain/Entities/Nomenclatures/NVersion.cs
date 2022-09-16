using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Domain.Entities.Nomenclatures
{
    public class NVersion : INomenclature
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int Version { get; set; }
        public string Name { get; set; }
        public string OSType { get; set; }
        public bool IsActive { get; set; }
    }
}
