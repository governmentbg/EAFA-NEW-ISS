namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalProcessCropInput", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalProcessCropInputType
    {

        private IDType idField;

        private CodeType typeCodeField;

        private TextType descriptionField;

        private CodeType subordinateTypeCodeField;

        private MeasureType weightMeasureField;

        private MeasureType volumeMeasureField;

        private MeasureType expectedVolumeMeasureField;

        private MeasureType appliedSurfaceUnitVolumeMeasureField;

        private CropInputBatchType[] specifiedCropInputBatchField;

        private CropInputChemicalType[] componentCropInputChemicalField;

        private TechnicalCharacteristicType[] applicableTechnicalCharacteristicField;

        private CropDataSheetPartyType supplierCropDataSheetPartyField;

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

        public MeasureType WeightMeasure
        {
            get
            {
                return this.weightMeasureField;
            }
            set
            {
                this.weightMeasureField = value;
            }
        }

        public MeasureType VolumeMeasure
        {
            get
            {
                return this.volumeMeasureField;
            }
            set
            {
                this.volumeMeasureField = value;
            }
        }

        public MeasureType ExpectedVolumeMeasure
        {
            get
            {
                return this.expectedVolumeMeasureField;
            }
            set
            {
                this.expectedVolumeMeasureField = value;
            }
        }

        public MeasureType AppliedSurfaceUnitVolumeMeasure
        {
            get
            {
                return this.appliedSurfaceUnitVolumeMeasureField;
            }
            set
            {
                this.appliedSurfaceUnitVolumeMeasureField = value;
            }
        }

        [XmlElement("SpecifiedCropInputBatch")]
        public CropInputBatchType[] SpecifiedCropInputBatch
        {
            get
            {
                return this.specifiedCropInputBatchField;
            }
            set
            {
                this.specifiedCropInputBatchField = value;
            }
        }

        [XmlElement("ComponentCropInputChemical")]
        public CropInputChemicalType[] ComponentCropInputChemical
        {
            get
            {
                return this.componentCropInputChemicalField;
            }
            set
            {
                this.componentCropInputChemicalField = value;
            }
        }

        [XmlElement("ApplicableTechnicalCharacteristic")]
        public TechnicalCharacteristicType[] ApplicableTechnicalCharacteristic
        {
            get
            {
                return this.applicableTechnicalCharacteristicField;
            }
            set
            {
                this.applicableTechnicalCharacteristicField = value;
            }
        }

        public CropDataSheetPartyType SupplierCropDataSheetParty
        {
            get
            {
                return this.supplierCropDataSheetPartyField;
            }
            set
            {
                this.supplierCropDataSheetPartyField = value;
            }
        }
    }
}