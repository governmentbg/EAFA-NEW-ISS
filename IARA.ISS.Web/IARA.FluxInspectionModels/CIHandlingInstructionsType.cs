namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIHandlingInstructions", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIHandlingInstructionsType
    {

        private CodeType[] handlingCodeField;

        private TextType[] procedureField;

        private TextType[] descriptionField;

        private CodeType[] descriptionCodeField;

        private WeightUnitMeasureType maximumStackabilityWeightApplicableMeasureField;

        private QuantityType maximumStackabilityApplicableQuantityField;

        private MeasureType maximumStorageHumidityApplicableMeasureField;

        private MeasureType minimumStorageHumidityApplicableMeasureField;

        private IDType idField;

        private TransportSettingTemperatureType[] handlingApplicableTransportSettingTemperatureField;

        private CIInstructedTemperatureType deliveryApplicableCIInstructedTemperatureField;

        private CIInstructedTemperatureType marketDeliveryApplicableCIInstructedTemperatureField;

        private CIInstructedTemperatureType storageApplicableCIInstructedTemperatureField;

        [XmlElement("HandlingCode")]
        public CodeType[] HandlingCode
        {
            get
            {
                return this.handlingCodeField;
            }
            set
            {
                this.handlingCodeField = value;
            }
        }

        [XmlElement("Procedure")]
        public TextType[] Procedure
        {
            get
            {
                return this.procedureField;
            }
            set
            {
                this.procedureField = value;
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

        public WeightUnitMeasureType MaximumStackabilityWeightApplicableMeasure
        {
            get
            {
                return this.maximumStackabilityWeightApplicableMeasureField;
            }
            set
            {
                this.maximumStackabilityWeightApplicableMeasureField = value;
            }
        }

        public QuantityType MaximumStackabilityApplicableQuantity
        {
            get
            {
                return this.maximumStackabilityApplicableQuantityField;
            }
            set
            {
                this.maximumStackabilityApplicableQuantityField = value;
            }
        }

        public MeasureType MaximumStorageHumidityApplicableMeasure
        {
            get
            {
                return this.maximumStorageHumidityApplicableMeasureField;
            }
            set
            {
                this.maximumStorageHumidityApplicableMeasureField = value;
            }
        }

        public MeasureType MinimumStorageHumidityApplicableMeasure
        {
            get
            {
                return this.minimumStorageHumidityApplicableMeasureField;
            }
            set
            {
                this.minimumStorageHumidityApplicableMeasureField = value;
            }
        }

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

        [XmlElement("HandlingApplicableTransportSettingTemperature")]
        public TransportSettingTemperatureType[] HandlingApplicableTransportSettingTemperature
        {
            get
            {
                return this.handlingApplicableTransportSettingTemperatureField;
            }
            set
            {
                this.handlingApplicableTransportSettingTemperatureField = value;
            }
        }

        public CIInstructedTemperatureType DeliveryApplicableCIInstructedTemperature
        {
            get
            {
                return this.deliveryApplicableCIInstructedTemperatureField;
            }
            set
            {
                this.deliveryApplicableCIInstructedTemperatureField = value;
            }
        }

        public CIInstructedTemperatureType MarketDeliveryApplicableCIInstructedTemperature
        {
            get
            {
                return this.marketDeliveryApplicableCIInstructedTemperatureField;
            }
            set
            {
                this.marketDeliveryApplicableCIInstructedTemperatureField = value;
            }
        }

        public CIInstructedTemperatureType StorageApplicableCIInstructedTemperature
        {
            get
            {
                return this.storageApplicableCIInstructedTemperatureField;
            }
            set
            {
                this.storageApplicableCIInstructedTemperatureField = value;
            }
        }
    }
}