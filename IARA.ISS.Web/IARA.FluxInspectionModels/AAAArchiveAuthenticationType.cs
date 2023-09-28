namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAArchiveAuthentication", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAArchiveAuthenticationType
    {

        private DateTimeType actualDateTimeField;

        private IDType idField;

        private TextType statementField;

        private TextType informationField;

        private TextType[] signatoryField;

        public DateTimeType ActualDateTime
        {
            get
            {
                return this.actualDateTimeField;
            }
            set
            {
                this.actualDateTimeField = value;
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

        public TextType Statement
        {
            get
            {
                return this.statementField;
            }
            set
            {
                this.statementField = value;
            }
        }

        public TextType Information
        {
            get
            {
                return this.informationField;
            }
            set
            {
                this.informationField = value;
            }
        }

        [XmlElement("Signatory")]
        public TextType[] Signatory
        {
            get
            {
                return this.signatoryField;
            }
            set
            {
                this.signatoryField = value;
            }
        }
    }
}