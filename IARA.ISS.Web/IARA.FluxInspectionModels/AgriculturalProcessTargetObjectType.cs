namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalProcessTargetObject", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalProcessTargetObjectType
    {

        private CodeType typeCodeField;

        private CodeType subordinateTypeCodeField;

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