namespace IARA.DomainModels.Nomenclatures
{
    public class ColumnDTO
    {
        public string DisplayName { get; set; }
        public string PropertyName { get; set; }
        public bool IsForeignKey { get; set; }
        public string DataType { get; set; }
        public bool IsRequired { get; set; }
        public int? MaxLength { get; set; }
    }
}
