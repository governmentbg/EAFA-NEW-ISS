using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Insp.Domain.Entities.Nomenclatures
{
    public class NCatchZone : ICodeNomenclature
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public float X { get; set; }
        public float Y { get; set; }

        public float Width { get; set; }
        public float Height { get; set; }
    }
}
