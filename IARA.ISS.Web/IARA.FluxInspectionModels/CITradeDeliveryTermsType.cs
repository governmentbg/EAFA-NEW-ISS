namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CITradeDeliveryTerms", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CITradeDeliveryTermsType
    {

        private DeliveryTermsCodeType deliveryTypeCodeField;

        private TextType[] descriptionField;

        private CodeType deliveryDiscontinuationCodeField;

        private IndicatorType partialDeliveryAllowedIndicatorField;

        private CITradeLocationType relevantCITradeLocationField;

        public DeliveryTermsCodeType DeliveryTypeCode
        {
            get
            {
                return this.deliveryTypeCodeField;
            }
            set
            {
                this.deliveryTypeCodeField = value;
            }
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        public CodeType DeliveryDiscontinuationCode
        {
            get
            {
                return this.deliveryDiscontinuationCodeField;
            }
            set
            {
                this.deliveryDiscontinuationCodeField = value;
            }
        }

        public IndicatorType PartialDeliveryAllowedIndicator
        {
            get
            {
                return this.partialDeliveryAllowedIndicatorField;
            }
            set
            {
                this.partialDeliveryAllowedIndicatorField = value;
            }
        }

        public CITradeLocationType RelevantCITradeLocation
        {
            get
            {
                return this.relevantCITradeLocationField;
            }
            set
            {
                this.relevantCITradeLocationField = value;
            }
        }
    }
}