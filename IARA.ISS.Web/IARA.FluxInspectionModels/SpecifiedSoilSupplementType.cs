namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecifiedSoilSupplement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecifiedSoilSupplementType
    {

        private CodeType typeCodeField;

        private DateTimeType applicationDateTimeField;

        private MeasureType weightMeasureField;

        private CropDataSheetPartyType supplierCropDataSheetPartyField;

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public DateTimeType ApplicationDateTime
        {
            get => this.applicationDateTimeField;
            set => this.applicationDateTimeField = value;
        }

        public MeasureType WeightMeasure
        {
            get => this.weightMeasureField;
            set => this.weightMeasureField = value;
        }

        public CropDataSheetPartyType SupplierCropDataSheetParty
        {
            get => this.supplierCropDataSheetPartyField;
            set => this.supplierCropDataSheetPartyField = value;
        }
    }
}