namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TradeProductCertification", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TradeProductCertificationType
    {

        private CodeType[] assertionCodeField;

        private TextType responsibleAgencyField;

        private TextType[] assertionField;

        private TextType standardField;

        [XmlElement("AssertionCode")]
        public CodeType[] AssertionCode
        {
            get
            {
                return this.assertionCodeField;
            }
            set
            {
                this.assertionCodeField = value;
            }
        }

        public TextType ResponsibleAgency
        {
            get
            {
                return this.responsibleAgencyField;
            }
            set
            {
                this.responsibleAgencyField = value;
            }
        }

        [XmlElement("Assertion")]
        public TextType[] Assertion
        {
            get
            {
                return this.assertionField;
            }
            set
            {
                this.assertionField = value;
            }
        }

        public TextType Standard
        {
            get
            {
                return this.standardField;
            }
            set
            {
                this.standardField = value;
            }
        }
    }
}