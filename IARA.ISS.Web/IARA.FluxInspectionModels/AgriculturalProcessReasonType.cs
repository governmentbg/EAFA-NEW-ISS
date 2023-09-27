namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalProcessReason", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalProcessReasonType
    {

        private CodeType typeCodeField;

        private CodeType subordinateTypeCodeField;

        private CodeType detailTypeCodeField;

        private TextType identificationField;

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

        public CodeType DetailTypeCode
        {
            get
            {
                return this.detailTypeCodeField;
            }
            set
            {
                this.detailTypeCodeField = value;
            }
        }

        public TextType Identification
        {
            get
            {
                return this.identificationField;
            }
            set
            {
                this.identificationField = value;
            }
        }
    }
}