namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AnimalIdentity", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AnimalIdentityType
    {

        private IDType idField;

        private TextType legalBasisField;

        private TextType issuerPartyNameField;

        private IDType versionIDField;

        private NumericType iDLengthNumericField;

        private AnimalLabelType[] specifiedAnimalLabelField;

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

        public TextType IssuerPartyName
        {
            get
            {
                return this.issuerPartyNameField;
            }
            set
            {
                this.issuerPartyNameField = value;
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

        [XmlElement("SpecifiedAnimalLabel")]
        public AnimalLabelType[] SpecifiedAnimalLabel
        {
            get
            {
                return this.specifiedAnimalLabelField;
            }
            set
            {
                this.specifiedAnimalLabelField = value;
            }
        }
    }
}