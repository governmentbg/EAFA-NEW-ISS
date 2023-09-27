namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SampledObjectAnimal", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SampledObjectAnimalType
    {

        private IDType idField;

        private CodeType speciesTypeCodeField;

        private DateTimeType birthDateTimeField;

        private CodeType genderCodeField;

        private DateTimeType deathDateTimeField;

        private IDType motherIDField;

        private IDType fatherIDField;

        private TextType holderNameField;

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

        public CodeType SpeciesTypeCode
        {
            get
            {
                return this.speciesTypeCodeField;
            }
            set
            {
                this.speciesTypeCodeField = value;
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

        public CodeType GenderCode
        {
            get
            {
                return this.genderCodeField;
            }
            set
            {
                this.genderCodeField = value;
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

        public IDType MotherID
        {
            get
            {
                return this.motherIDField;
            }
            set
            {
                this.motherIDField = value;
            }
        }

        public IDType FatherID
        {
            get
            {
                return this.fatherIDField;
            }
            set
            {
                this.fatherIDField = value;
            }
        }

        public TextType HolderName
        {
            get
            {
                return this.holderNameField;
            }
            set
            {
                this.holderNameField = value;
            }
        }
    }
}