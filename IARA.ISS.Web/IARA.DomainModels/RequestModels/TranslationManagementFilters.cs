namespace IARA.DomainModels.RequestModels
{
    public class TranslationManagementFilters : BaseRequestModel
    {
        public string Code { get; set; }

        public string GroupCode { get; set; }

        public string TranslationType { get; set; }

        public string TranslationValueBG { get; set; }

        public string TranslationValueEN { get; set; }
    }
}
