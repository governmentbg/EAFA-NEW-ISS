namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecifiedLocation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecifiedLocationType
    {

        private CodeType typeCodeField;

        private CodeType geopoliticalRegionCodeField;

        private TextType geopoliticalRegionNameField;

        private TextType[] descriptionField;

        private AdministrativeCountrySubDivisionType[] specifiedAdministrativeCountrySubDivisionField;

        private AgronomicalObservationPlotType[] specifiedAgronomicalObservationPlotField;

        private AgriculturalAreaType[] specifiedAgriculturalAreaField;

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

        public CodeType GeopoliticalRegionCode
        {
            get
            {
                return this.geopoliticalRegionCodeField;
            }
            set
            {
                this.geopoliticalRegionCodeField = value;
            }
        }

        public TextType GeopoliticalRegionName
        {
            get
            {
                return this.geopoliticalRegionNameField;
            }
            set
            {
                this.geopoliticalRegionNameField = value;
            }
        }

        [XmlElement("Description")]
        public TextType[] Description
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

        [XmlElement("SpecifiedAdministrativeCountrySubDivision")]
        public AdministrativeCountrySubDivisionType[] SpecifiedAdministrativeCountrySubDivision
        {
            get
            {
                return this.specifiedAdministrativeCountrySubDivisionField;
            }
            set
            {
                this.specifiedAdministrativeCountrySubDivisionField = value;
            }
        }

        [XmlElement("SpecifiedAgronomicalObservationPlot")]
        public AgronomicalObservationPlotType[] SpecifiedAgronomicalObservationPlot
        {
            get
            {
                return this.specifiedAgronomicalObservationPlotField;
            }
            set
            {
                this.specifiedAgronomicalObservationPlotField = value;
            }
        }

        [XmlElement("SpecifiedAgriculturalArea")]
        public AgriculturalAreaType[] SpecifiedAgriculturalArea
        {
            get
            {
                return this.specifiedAgriculturalAreaField;
            }
            set
            {
                this.specifiedAgriculturalAreaField = value;
            }
        }
    }
}