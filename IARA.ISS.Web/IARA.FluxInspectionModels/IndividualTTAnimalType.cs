namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("IndividualTTAnimal", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class IndividualTTAnimalType
    {

        private IDType idField;

        private DateTimeType birthDateTimeField;

        private DateTimeType deathDateTimeField;

        private DelimitedPeriodType specifiedDelimitedPeriodField;

        private SpeciesAdditionalTTProductType specifiedSpeciesAdditionalTTProductField;

        private SpeciesAdditionalTTProductionType[] specifiedSpeciesAdditionalTTProductionField;

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

        public DateTimeType BirthDateTime
        {
            get
            {
                return this.birthDateTimeField;
            }
            set
            {
                this.birthDateTimeField = value;
            }
        }

        public DateTimeType DeathDateTime
        {
            get
            {
                return this.deathDateTimeField;
            }
            set
            {
                this.deathDateTimeField = value;
            }
        }

        public DelimitedPeriodType SpecifiedDelimitedPeriod
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

        public SpeciesAdditionalTTProductType SpecifiedSpeciesAdditionalTTProduct
        {
            get
            {
                return this.specifiedSpeciesAdditionalTTProductField;
            }
            set
            {
                this.specifiedSpeciesAdditionalTTProductField = value;
            }
        }

        [XmlElement("SpecifiedSpeciesAdditionalTTProduction")]
        public SpeciesAdditionalTTProductionType[] SpecifiedSpeciesAdditionalTTProduction
        {
            get
            {
                return this.specifiedSpeciesAdditionalTTProductionField;
            }
            set
            {
                this.specifiedSpeciesAdditionalTTProductionField = value;
            }
        }
    }
}