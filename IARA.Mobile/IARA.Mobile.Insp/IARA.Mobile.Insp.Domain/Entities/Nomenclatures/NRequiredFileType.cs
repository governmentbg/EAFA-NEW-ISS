using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Insp.Domain.Entities.Nomenclatures
{
    public class NRequiredFileType : INomenclature
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int FileTypeId { get; set; }
        public string Name { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsActive { get; set; }
    }
}
