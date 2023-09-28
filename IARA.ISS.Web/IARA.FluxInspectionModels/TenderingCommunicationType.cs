namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TenderingCommunication", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TenderingCommunicationType
    {

        private CodeType channelCodeField;

        private IDType emailURIIDField;

        private TextType localNumberField;

        public CodeType ChannelCode
        {
            get
            {
                return this.channelCodeField;
            }
            set
            {
                this.channelCodeField = value;
            }
        }

        public IDType EmailURIID
        {
            get
            {
                return this.emailURIIDField;
            }
            set
            {
                this.emailURIIDField = value;
            }
        }

        public TextType LocalNumber
        {
            get
            {
                return this.localNumberField;
            }
            set
            {
                this.localNumberField = value;
            }
        }
    }
}