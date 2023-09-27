namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ObservationProcess", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ObservationProcessType
    {

        private IDType idField;

        private TextType nameField;

        private CodeType typeCodeField;

        private TextType descriptionField;

        private CodeType methodologyTypeCodeField;

        private CodeType notationTypeCodeField;

        private CodeType trapTypeCodeField;

        private CodeType observationCodeField;

        private ExpectedMeasurementType[] actualExpectedMeasurementField;

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

        public CodeType MethodologyTypeCode
        {
            get
            {
                return this.methodologyTypeCodeField;
            }
            set
            {
                this.methodologyTypeCodeField = value;
            }
        }

        public CodeType NotationTypeCode
        {
            get
            {
                return this.notationTypeCodeField;
            }
            set
            {
                this.notationTypeCodeField = value;
            }
        }

        public CodeType TrapTypeCode
        {
            get
            {
                return this.trapTypeCodeField;
            }
            set
            {
                this.trapTypeCodeField = value;
            }
        }

        public CodeType ObservationCode
        {
            get
            {
                return this.observationCodeField;
            }
            set
            {
                this.observationCodeField = value;
            }
        }

        [XmlElement("ActualExpectedMeasurement")]
        public ExpectedMeasurementType[] ActualExpectedMeasurement
        {
            get
            {
                return this.actualExpectedMeasurementField;
            }
            set
            {
                this.actualExpectedMeasurementField = value;
            }
        }
    }
}