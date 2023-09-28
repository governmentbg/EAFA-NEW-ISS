namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LaboratoryObservationInstructions", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LaboratoryObservationInstructionsType
    {

        private IDType idField;

        private TextType descriptionField;

        private TextType typeField;

        private CodeType descriptionCodeField;

        private TextType procedureField;

        private DateTimeType latestUpdateDateTimeField;

        private CodeType statusCodeField;

        private NumericType sequenceNumericField;

        private CodeType propertyReferenceCodeField;

        private CodeType interpretationCodeField;

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

        public TextType Description
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

        public TextType Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        public CodeType DescriptionCode
        {
            get
            {
                return this.descriptionCodeField;
            }
            set
            {
                this.descriptionCodeField = value;
            }
        }

        public TextType Procedure
        {
            get
            {
                return this.procedureField;
            }
            set
            {
                this.procedureField = value;
            }
        }

        public DateTimeType LatestUpdateDateTime
        {
            get
            {
                return this.latestUpdateDateTimeField;
            }
            set
            {
                this.latestUpdateDateTimeField = value;
            }
        }

        public CodeType StatusCode
        {
            get
            {
                return this.statusCodeField;
            }
            set
            {
                this.statusCodeField = value;
            }
        }

        public NumericType SequenceNumeric
        {
            get
            {
                return this.sequenceNumericField;
            }
            set
            {
                this.sequenceNumericField = value;
            }
        }

        public CodeType PropertyReferenceCode
        {
            get
            {
                return this.propertyReferenceCodeField;
            }
            set
            {
                this.propertyReferenceCodeField = value;
            }
        }

        public CodeType InterpretationCode
        {
            get
            {
                return this.interpretationCodeField;
            }
            set
            {
                this.interpretationCodeField = value;
            }
        }
    }
}