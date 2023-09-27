namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TTProductTradeDelivery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TTProductTradeDeliveryType
    {

        private IDType idField;

        private TTPartyType responsibleTTPartyField;

        private TTProductType[] includedTTProductField;

        private TTTransportMovementType[][] relatedTTConsignmentField;

        private TTEventType[] specifiedDeliveryTTEventField;

        private TTEventType[] specifiedDespatchTTEventField;

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        public TTPartyType ResponsibleTTParty
        {
            get
            {
                return this.responsibleTTPartyField;
            }
            set
            {
                this.responsibleTTPartyField = value;
            }
        }

        [XmlElement("IncludedTTProduct")]
        public TTProductType[] IncludedTTProduct
        {
            get
            {
                return this.includedTTProductField;
            }
            set
            {
                this.includedTTProductField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayItemAttribute("SpecifiedTTTransportMovement", typeof(TTTransportMovementType), IsNullable = false)]
        public TTTransportMovementType[][] RelatedTTConsignment
        {
            get
            {
                return this.relatedTTConsignmentField;
            }
            set
            {
                this.relatedTTConsignmentField = value;
            }
        }

        [XmlElement("SpecifiedDeliveryTTEvent")]
        public TTEventType[] SpecifiedDeliveryTTEvent
        {
            get
            {
                return this.specifiedDeliveryTTEventField;
            }
            set
            {
                this.specifiedDeliveryTTEventField = value;
            }
        }

        [XmlElement("SpecifiedDespatchTTEvent")]
        public TTEventType[] SpecifiedDespatchTTEvent
        {
            get
            {
                return this.specifiedDespatchTTEventField;
            }
            set
            {
                this.specifiedDespatchTTEventField = value;
            }
        }
    }
}