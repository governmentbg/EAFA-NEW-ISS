namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAWrapParty", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAWrapPartyType
    {

        private IDType idField;

        private PartyTypeCodeType typeCodeField;

        private TextType personNameField;

        private CodeType[] languageCodeField;

        private AAAWrapPersonType appointedAAAWrapPersonField;

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

        public PartyTypeCodeType TypeCode
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

        public TextType PersonName
        {
            get
            {
                return this.personNameField;
            }
            set
            {
                this.personNameField = value;
            }
        }

        [XmlElement("LanguageCode")]
        public CodeType[] LanguageCode
        {
            get
            {
                return this.languageCodeField;
            }
            set
            {
                this.languageCodeField = value;
            }
        }

        public AAAWrapPersonType AppointedAAAWrapPerson
        {
            get
            {
                return this.appointedAAAWrapPersonField;
            }
            set
            {
                this.appointedAAAWrapPersonField = value;
            }
        }
    }
}