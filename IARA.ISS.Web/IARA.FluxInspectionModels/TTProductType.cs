namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TTProduct", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TTProductType
    {

        private CodeType[] descriptionCodeField;

        private CodeType[] productionModeCodeField;

        private CodeType[] typeCodeField;

        private CodeType[] speciesNameCodeField;

        private TTPartyType responsibleTTPartyField;

        private TTProductTradeDeliveryType applicableTTProductTradeDeliveryField;

        private TTTransportMovementType[][] specifiedTTConsignmentField;

        private TTProductBatchType[] specifiedTTProductBatchField;

        private TTProductIdentityType[] specifiedTTProductIdentityField;

        private DelimitedPeriodType[] specifiedDelimitedPeriodField;

        private TTProductProcessEventType[] specifiedTTProductProcessEventField;

        private ProductCertificateType[] specifiedProductCertificateField;

        private SpeciesTTProductType[] relatedSpeciesTTProductField;

        private TTLocationType[] relatedTTLocationField;

        private TTTransportMovementType[] relatedTTTransportMovementField;

        [XmlElement("DescriptionCode")]
        public CodeType[] DescriptionCode
        {
            get
            {
                return this.descriptionCodeField;
            }
            set
            {
                this.descriptionCodeField = value;
            }
        }

        [XmlElement("ProductionModeCode")]
        public CodeType[] ProductionModeCode
        {
            get
            {
                return this.productionModeCodeField;
            }
            set
            {
                this.productionModeCodeField = value;
            }
        }

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }

        [XmlElement("SpeciesNameCode")]
        public CodeType[] SpeciesNameCode
        {
            get
            {
                return this.speciesNameCodeField;
            }
            set
            {
                this.speciesNameCodeField = value;
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

        public TTProductTradeDeliveryType ApplicableTTProductTradeDelivery
        {
            get
            {
                return this.applicableTTProductTradeDeliveryField;
            }
            set
            {
                this.applicableTTProductTradeDeliveryField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayItemAttribute("SpecifiedTTTransportMovement", typeof(TTTransportMovementType), IsNullable = false)]
        public TTTransportMovementType[][] SpecifiedTTConsignment
        {
            get
            {
                return this.specifiedTTConsignmentField;
            }
            set
            {
                this.specifiedTTConsignmentField = value;
            }
        }

        [XmlElement("SpecifiedTTProductBatch")]
        public TTProductBatchType[] SpecifiedTTProductBatch
        {
            get
            {
                return this.specifiedTTProductBatchField;
            }
            set
            {
                this.specifiedTTProductBatchField = value;
            }
        }

        [XmlElement("SpecifiedTTProductIdentity")]
        public TTProductIdentityType[] SpecifiedTTProductIdentity
        {
            get
            {
                return this.specifiedTTProductIdentityField;
            }
            set
            {
                this.specifiedTTProductIdentityField = value;
            }
        }

        [XmlElement("SpecifiedDelimitedPeriod")]
        public DelimitedPeriodType[] SpecifiedDelimitedPeriod
        {
            get
            {
                return this.specifiedDelimitedPeriodField;
            }
            set
            {
                this.specifiedDelimitedPeriodField = value;
            }
        }

        [XmlElement("SpecifiedTTProductProcessEvent")]
        public TTProductProcessEventType[] SpecifiedTTProductProcessEvent
        {
            get
            {
                return this.specifiedTTProductProcessEventField;
            }
            set
            {
                this.specifiedTTProductProcessEventField = value;
            }
        }

        [XmlElement("SpecifiedProductCertificate")]
        public ProductCertificateType[] SpecifiedProductCertificate
        {
            get
            {
                return this.specifiedProductCertificateField;
            }
            set
            {
                this.specifiedProductCertificateField = value;
            }
        }

        [XmlElement("RelatedSpeciesTTProduct")]
        public SpeciesTTProductType[] RelatedSpeciesTTProduct
        {
            get
            {
                return this.relatedSpeciesTTProductField;
            }
            set
            {
                this.relatedSpeciesTTProductField = value;
            }
        }

        [XmlElement("RelatedTTLocation")]
        public TTLocationType[] RelatedTTLocation
        {
            get
            {
                return this.relatedTTLocationField;
            }
            set
            {
                this.relatedTTLocationField = value;
            }
        }

        [XmlElement("RelatedTTTransportMovement")]
        public TTTransportMovementType[] RelatedTTTransportMovement
        {
            get
            {
                return this.relatedTTTransportMovementField;
            }
            set
            {
                this.relatedTTTransportMovementField = value;
            }
        }
    }
}