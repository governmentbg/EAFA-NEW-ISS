namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CITransportDangerousGoods", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CITransportDangerousGoodsType
    {

        private CodeType uNDGIdentificationCodeField;

        private DangerousGoodsRegulationCodeType regulationCodeField;

        private TextType[] regulationNameField;

        private TextType[] technicalNameField;

        private IDType eMSIDField;

        private DangerousGoodsPackagingLevelCodeType packagingDangerLevelCodeField;

        private IDType hazardClassificationIDField;

        private IDType additionalHazardClassificationIDField;

        private TextType properShippingNameField;

        private CodeType[] limitedQuantityCodeField;

        private CITradeContactType transportExpertCITradeContactField;

        private MeasurementType flashpointTemperatureMeasurementField;

        public CodeType UNDGIdentificationCode
        {
            get
            {
                return this.uNDGIdentificationCodeField;
            }
            set
            {
                this.uNDGIdentificationCodeField = value;
            }
        }

        public DangerousGoodsRegulationCodeType RegulationCode
        {
            get
            {
                return this.regulationCodeField;
            }
            set
            {
                this.regulationCodeField = value;
            }
        }

        [XmlElement("RegulationName")]
        public TextType[] RegulationName
        {
            get
            {
                return this.regulationNameField;
            }
            set
            {
                this.regulationNameField = value;
            }
        }

        [XmlElement("TechnicalName")]
        public TextType[] TechnicalName
        {
            get
            {
                return this.technicalNameField;
            }
            set
            {
                this.technicalNameField = value;
            }
        }

        public IDType EMSID
        {
            get
            {
                return this.eMSIDField;
            }
            set
            {
                this.eMSIDField = value;
            }
        }

        public DangerousGoodsPackagingLevelCodeType PackagingDangerLevelCode
        {
            get
            {
                return this.packagingDangerLevelCodeField;
            }
            set
            {
                this.packagingDangerLevelCodeField = value;
            }
        }

        public IDType HazardClassificationID
        {
            get
            {
                return this.hazardClassificationIDField;
            }
            set
            {
                this.hazardClassificationIDField = value;
            }
        }

        public IDType AdditionalHazardClassificationID
        {
            get
            {
                return this.additionalHazardClassificationIDField;
            }
            set
            {
                this.additionalHazardClassificationIDField = value;
            }
        }

        public TextType ProperShippingName
        {
            get
            {
                return this.properShippingNameField;
            }
            set
            {
                this.properShippingNameField = value;
            }
        }

        [XmlElement("LimitedQuantityCode")]
        public CodeType[] LimitedQuantityCode
        {
            get
            {
                return this.limitedQuantityCodeField;
            }
            set
            {
                this.limitedQuantityCodeField = value;
            }
        }

        public CITradeContactType TransportExpertCITradeContact
        {
            get
            {
                return this.transportExpertCITradeContactField;
            }
            set
            {
                this.transportExpertCITradeContactField = value;
            }
        }

        public MeasurementType FlashpointTemperatureMeasurement
        {
            get
            {
                return this.flashpointTemperatureMeasurementField;
            }
            set
            {
                this.flashpointTemperatureMeasurementField = value;
            }
        }
    }
}