namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CropProductionAgriculturalProcess", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CropProductionAgriculturalProcessType
    {

        private CodeType typeCodeField;

        private CodeType subordinateTypeCodeField;

        private DateTimeType earliestStartDateTimeField;

        private DateTimeType actualStartDateTimeField;

        private DateTimeType latestEndDateTimeField;

        private DateTimeType actualEndDateTimeField;

        private CodeType statusCodeField;

        private TextType descriptionField;

        private CropProduceBatchType[] harvestedCropProduceBatchField;

        private SpecifiedAgriculturalDeviceType[] usedSpecifiedAgriculturalDeviceField;

        private CropProductionAgriculturalProcessType[][] allocatedAgriculturalManMachineSystemField;

        private SpecifiedAgriculturalApplicationType[] appliedSpecifiedAgriculturalApplicationField;

        private FieldCropType[] specifiedFieldCropField;

        public CodeType TypeCode
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

        public CodeType SubordinateTypeCode
        {
            get
            {
                return this.subordinateTypeCodeField;
            }
            set
            {
                this.subordinateTypeCodeField = value;
            }
        }

        public DateTimeType EarliestStartDateTime
        {
            get
            {
                return this.earliestStartDateTimeField;
            }
            set
            {
                this.earliestStartDateTimeField = value;
            }
        }

        public DateTimeType ActualStartDateTime
        {
            get
            {
                return this.actualStartDateTimeField;
            }
            set
            {
                this.actualStartDateTimeField = value;
            }
        }

        public DateTimeType LatestEndDateTime
        {
            get
            {
                return this.latestEndDateTimeField;
            }
            set
            {
                this.latestEndDateTimeField = value;
            }
        }

        public DateTimeType ActualEndDateTime
        {
            get
            {
                return this.actualEndDateTimeField;
            }
            set
            {
                this.actualEndDateTimeField = value;
            }
        }

        public CodeType StatusCode
        {
            get
            {
                return this.statusCodeField;
            }
            set
            {
                this.statusCodeField = value;
            }
        }

        public TextType Description
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

        [XmlElement("HarvestedCropProduceBatch")]
        public CropProduceBatchType[] HarvestedCropProduceBatch
        {
            get
            {
                return this.harvestedCropProduceBatchField;
            }
            set
            {
                this.harvestedCropProduceBatchField = value;
            }
        }

        [XmlElement("UsedSpecifiedAgriculturalDevice")]
        public SpecifiedAgriculturalDeviceType[] UsedSpecifiedAgriculturalDevice
        {
            get
            {
                return this.usedSpecifiedAgriculturalDeviceField;
            }
            set
            {
                this.usedSpecifiedAgriculturalDeviceField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayItemAttribute("SpecifiedCropProductionAgriculturalProcess", typeof(CropProductionAgriculturalProcessType), IsNullable = false)]
        public CropProductionAgriculturalProcessType[][] AllocatedAgriculturalManMachineSystem
        {
            get
            {
                return this.allocatedAgriculturalManMachineSystemField;
            }
            set
            {
                this.allocatedAgriculturalManMachineSystemField = value;
            }
        }

        [XmlElement("AppliedSpecifiedAgriculturalApplication")]
        public SpecifiedAgriculturalApplicationType[] AppliedSpecifiedAgriculturalApplication
        {
            get
            {
                return this.appliedSpecifiedAgriculturalApplicationField;
            }
            set
            {
                this.appliedSpecifiedAgriculturalApplicationField = value;
            }
        }

        [XmlElement("SpecifiedFieldCrop")]
        public FieldCropType[] SpecifiedFieldCrop
        {
            get
            {
                return this.specifiedFieldCropField;
            }
            set
            {
                this.specifiedFieldCropField = value;
            }
        }
    }
}