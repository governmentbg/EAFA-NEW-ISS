namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RASFFHealthHazard", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RASFFHealthHazardType
    {

        private CodeType categoryCodeField;

        private TextType[] nameField;

        private TextType[] subordinateNameField;

        private TextType[] qualificationField;

        private IndicatorType inverseOrderSubordinateNameIndicatorField;

        private IndicatorType inverseOrderQualificationIndicatorField;

        private IDType idField;

        private ReferencedRegulationType primaryReferencedRegulationField;

        private ReferencedRegulationType[] additionalReferencedRegulationField;

        private SampleObservationResultCharacteristicType specifiedSampleObservationResultCharacteristicField;

        public CodeType CategoryCode
        {
            get => this.categoryCodeField;
            set => this.categoryCodeField = value;
        }

        [XmlElement("Name")]
        public TextType[] Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        [XmlElement("SubordinateName")]
        public TextType[] SubordinateName
        {
            get => this.subordinateNameField;
            set => this.subordinateNameField = value;
        }

        [XmlElement("Qualification")]
        public TextType[] Qualification
        {
            get => this.qualificationField;
            set => this.qualificationField = value;
        }

        public IndicatorType InverseOrderSubordinateNameIndicator
        {
            get => this.inverseOrderSubordinateNameIndicatorField;
            set => this.inverseOrderSubordinateNameIndicatorField = value;
        }

        public IndicatorType InverseOrderQualificationIndicator
        {
            get => this.inverseOrderQualificationIndicatorField;
            set => this.inverseOrderQualificationIndicatorField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public ReferencedRegulationType PrimaryReferencedRegulation
        {
            get => this.primaryReferencedRegulationField;
            set => this.primaryReferencedRegulationField = value;
        }

        [XmlElement("AdditionalReferencedRegulation")]
        public ReferencedRegulationType[] AdditionalReferencedRegulation
        {
            get => this.additionalReferencedRegulationField;
            set => this.additionalReferencedRegulationField = value;
        }

        public SampleObservationResultCharacteristicType SpecifiedSampleObservationResultCharacteristic
        {
            get => this.specifiedSampleObservationResultCharacteristicField;
            set => this.specifiedSampleObservationResultCharacteristicField = value;
        }
    }
}