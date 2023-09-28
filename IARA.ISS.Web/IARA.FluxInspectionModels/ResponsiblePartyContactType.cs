namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ResponsiblePartyContact", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ResponsiblePartyContactType
    {

        private TelecommunicationCommunicationType[] telephoneTelecommunicationCommunicationField;

        private TelecommunicationCommunicationType emergencyTelephoneTelecommunicationCommunicationField;

        private TelecommunicationCommunicationType[] faxTelecommunicationCommunicationField;

        private InternetCommunicationType[] emailURIInternetCommunicationField;

        [XmlElement("TelephoneTelecommunicationCommunication")]
        public TelecommunicationCommunicationType[] TelephoneTelecommunicationCommunication
        {
            get => this.telephoneTelecommunicationCommunicationField;
            set => this.telephoneTelecommunicationCommunicationField = value;
        }

        public TelecommunicationCommunicationType EmergencyTelephoneTelecommunicationCommunication
        {
            get => this.emergencyTelephoneTelecommunicationCommunicationField;
            set => this.emergencyTelephoneTelecommunicationCommunicationField = value;
        }

        [XmlElement("FaxTelecommunicationCommunication")]
        public TelecommunicationCommunicationType[] FaxTelecommunicationCommunication
        {
            get => this.faxTelecommunicationCommunicationField;
            set => this.faxTelecommunicationCommunicationField = value;
        }

        [XmlElement("EmailURIInternetCommunication")]
        public InternetCommunicationType[] EmailURIInternetCommunication
        {
            get => this.emailURIInternetCommunicationField;
            set => this.emailURIInternetCommunicationField = value;
        }
    }
}