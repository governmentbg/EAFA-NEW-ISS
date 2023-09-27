namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecificationQuery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecificationQueryType
    {

        private IDType idField;

        private CodeType typeCodeField;

        private TextType contentField;

        private CodeType contractualLanguageCodeField;

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

        public TextType Content
        {
            get
            {
                return this.contentField;
            }
            set
            {
                this.contentField = value;
            }
        }

        public CodeType ContractualLanguageCode
        {
            get
            {
                return this.contractualLanguageCodeField;
            }
            set
            {
                this.contractualLanguageCodeField = value;
            }
        }
    }
}