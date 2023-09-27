namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TTTransportMeans", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TTTransportMeansType
    {

        private IDType idField;

        private TextType nameField;

        private CodeType typeCodeField;

        private TTPartyType tTResponsibleTTPartyField;

        private DelimitedPeriodType fASpecifiedDelimitedPeriodField;

        private GeographicalAreaType fASpecifiedGeographicalAreaField;

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

        public TTPartyType TTResponsibleTTParty
        {
            get
            {
                return this.tTResponsibleTTPartyField;
            }
            set
            {
                this.tTResponsibleTTPartyField = value;
            }
        }

        public DelimitedPeriodType FASpecifiedDelimitedPeriod
        {
            get
            {
                return this.fASpecifiedDelimitedPeriodField;
            }
            set
            {
                this.fASpecifiedDelimitedPeriodField = value;
            }
        }

        public GeographicalAreaType FASpecifiedGeographicalArea
        {
            get
            {
                return this.fASpecifiedGeographicalAreaField;
            }
            set
            {
                this.fASpecifiedGeographicalAreaField = value;
            }
        }
    }
}