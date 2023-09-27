namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIUniversalCommunication", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIUniversalCommunicationType
    {

        private IDType uRIIDField;

        private CommunicationChannelCodeType channelCodeField;

        private TextType completeNumberField;

        public IDType URIID
        {
            get
            {
                return this.uRIIDField;
            }
            set
            {
                this.uRIIDField = value;
            }
        }

        public CommunicationChannelCodeType ChannelCode
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

        public TextType CompleteNumber
        {
            get
            {
                return this.completeNumberField;
            }
            set
            {
                this.completeNumberField = value;
            }
        }
    }
}