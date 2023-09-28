namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CSEngineeringCoordinateReferenceSystem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CSEngineeringCoordinateReferenceSystemType
    {

        private IDType idField;

        private TextType nameField;

        private TextType levelField;

        private CSEngineeringCoordinateReferenceSystemType[] subordinateCSEngineeringCoordinateReferenceSystemField;

        private DelimitedPeriodType[] specifiedDelimitedPeriodField;

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

        public TextType Level
        {
            get
            {
                return this.levelField;
            }
            set
            {
                this.levelField = value;
            }
        }

        [XmlElement("SubordinateCSEngineeringCoordinateReferenceSystem")]
        public CSEngineeringCoordinateReferenceSystemType[] SubordinateCSEngineeringCoordinateReferenceSystem
        {
            get
            {
                return this.subordinateCSEngineeringCoordinateReferenceSystemField;
            }
            set
            {
                this.subordinateCSEngineeringCoordinateReferenceSystemField = value;
            }
        }

        [XmlElement("SpecifiedDelimitedPeriod")]
        public DelimitedPeriodType[] SpecifiedDelimitedPeriod
        {
            get
            {
                return this.specifiedDelimitedPeriodField;
            }
            set
            {
                this.specifiedDelimitedPeriodField = value;
            }
        }
    }
}