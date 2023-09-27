namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TTProductIdentity", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TTProductIdentityType
    {

        private IDType idField;

        private NumericType iDLengthNumericField;

        private TextType legalBasisField;

        private IDType versionIDField;

        private TTProductLabelType[] specifiedTTProductLabelField;

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

        public NumericType IDLengthNumeric
        {
            get
            {
                return this.iDLengthNumericField;
            }
            set
            {
                this.iDLengthNumericField = value;
            }
        }

        public TextType LegalBasis
        {
            get
            {
                return this.legalBasisField;
            }
            set
            {
                this.legalBasisField = value;
            }
        }

        public IDType VersionID
        {
            get
            {
                return this.versionIDField;
            }
            set
            {
                this.versionIDField = value;
            }
        }

        [XmlElement("SpecifiedTTProductLabel")]
        public TTProductLabelType[] SpecifiedTTProductLabel
        {
            get
            {
                return this.specifiedTTProductLabelField;
            }
            set
            {
                this.specifiedTTProductLabelField = value;
            }
        }
    }
}