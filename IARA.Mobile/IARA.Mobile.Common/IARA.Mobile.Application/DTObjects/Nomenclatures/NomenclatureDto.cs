using IARA.Mobile.Domain.Interfaces;

namespace IARA.Mobile.Application.DTObjects.Nomenclatures
{
    public class NomenclatureDto<T> : IActive
    {
        public T Value { get; set; }
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class NomenclatureDto : NomenclatureDto<int>
    {
    }
}
