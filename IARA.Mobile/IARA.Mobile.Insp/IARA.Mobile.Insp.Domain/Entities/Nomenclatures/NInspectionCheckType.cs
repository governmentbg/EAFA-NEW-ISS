using IARA.Mobile.Domain.Interfaces;
using IARA.Mobile.Insp.Domain.Enums;
using SQLite;

namespace IARA.Mobile.Insp.Domain.Entities.Nomenclatures
{
    public class NInspectionCheckType : ICodeNomenclature
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsMandatory { get; set; }
        public ToggleTypeEnum Type { get; set; }
        public int InsectionTypeId { get; set; }
        public bool HasDescription { get; set; }
        public string DescriptionLabel { get; set; }
        public bool IsActive { get; set; }
    }
}
