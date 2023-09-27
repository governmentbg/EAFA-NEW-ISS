namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TMWContact", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TMWContactType
    {

        private TelecommunicationCommunicationType telephoneTelecommunicationCommunicationField;

        private TelecommunicationCommunicationType faxTelecommunicationCommunicationField;

        private EmailCommunicationType emailURIEmailCommunicationField;

        private WebsiteCommunicationType websiteURIWebsiteCommunicationField;

        private ContactPersonType specifiedContactPersonField;

        public TelecommunicationCommunicationType TelephoneTelecommunicationCommunication
        {
            get => this.telephoneTelecommunicationCommunicationField;
            set => this.telephoneTelecommunicationCommunicationField = value;
        }

        public TelecommunicationCommunicationType FaxTelecommunicationCommunication
        {
            get => this.faxTelecommunicationCommunicationField;
            set => this.faxTelecommunicationCommunicationField = value;
        }

        public EmailCommunicationType EmailURIEmailCommunication
        {
            get => this.emailURIEmailCommunicationField;
            set => this.emailURIEmailCommunicationField = value;
        }

        public WebsiteCommunicationType WebsiteURIWebsiteCommunication
        {
            get => this.websiteURIWebsiteCommunicationField;
            set => this.websiteURIWebsiteCommunicationField = value;
        }

        public ContactPersonType SpecifiedContactPerson
        {
            get => this.specifiedContactPersonField;
            set => this.specifiedContactPersonField = value;
        }
    }
}