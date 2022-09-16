namespace IARA.DomainModels.Nomenclatures
{
    public class NomenclatureDTO<T>
    {
        public T Value { get; set; }
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
