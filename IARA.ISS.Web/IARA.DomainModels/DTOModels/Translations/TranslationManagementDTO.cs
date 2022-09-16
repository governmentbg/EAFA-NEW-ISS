namespace IARA.DomainModels.DTOModels.Translations
{
    public class TranslationManagementDTO
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string GroupCode { get; set; }

        public string GroupType { get; set; }

        public string ValueBg { get; set; }

        public string ValueEn { get; set; }

        public bool IsActive { get; set; }
    }
}
