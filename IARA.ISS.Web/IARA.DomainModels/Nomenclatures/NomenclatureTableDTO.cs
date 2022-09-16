namespace IARA.DomainModels.Nomenclatures
{
    public class NomenclatureTableDTO : NomenclatureDTO<int>
    {
        public int? GroupId { get; set; }
        public string TableName { get; set; }
        public bool CanInsertRows { get; set; }
        public bool CanEditRows { get; set; }
        public bool CanDeleteRows { get; set; }
    }
}
