namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProductInformationContact", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProductInformationContactType
    {

        private CodeType[] typeCodeField;

        private TelecommunicationCommunicationType[] telephoneTelecommunicationCommunicationField;

        private TelecommunicationCommunicationType faxTelecommunicationCommunicationField;

        private InternetCommunicationType uRIInternetCommunicationField;

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        [XmlElement("TelephoneTelecommunicationCommunication")]
        public TelecommunicationCommunicationType[] TelephoneTelecommunicationCommunication
        {
            get => this.telephoneTelecommunicationCommunicationField;
            set => this.telephoneTelecommunicationCommunicationField = value;
        }

        public TelecommunicationCommunicationType FaxTelecommunicationCommunication
        {
            get => this.faxTelecommunicationCommunicationField;
            set => this.faxTelecommunicationCommunicationField = value;
        }

        public InternetCommunicationType URIInternetCommunication
        {
            get => this.uRIInternetCommunicationField;
            set => this.uRIInternetCommunicationField = value;
        }
    }
}