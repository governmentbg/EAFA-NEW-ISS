namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SampledObjectAdditive", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SampledObjectAdditiveType
    {

        private TextType nameField;

        private TextType descriptionField;

        private CodeType standardTypeCodeField;

        private CodeType localTypeCodeField;

        public TextType Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
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

        public CodeType StandardTypeCode
        {
            get
            {
                return this.standardTypeCodeField;
            }
            set
            {
                this.standardTypeCodeField = value;
            }
        }

        public CodeType LocalTypeCode
        {
            get
            {
                return this.localTypeCodeField;
            }
            set
            {
                this.localTypeCodeField = value;
            }
        }
    }
}