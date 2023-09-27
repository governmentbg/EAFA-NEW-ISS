namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgronomicalObservationTargetObject", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgronomicalObservationTargetObjectType
    {

        private CodeType typeCodeField;

        private CodeType subtypeCodeField;

        private CodeType developmentStageCodeField;

        private TextType descriptionField;

        private AgronomicalObservationScopeType[] observationBasisAgronomicalObservationScopeField;

        private ObservationProcessType usedObservationProcessField;

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

        public CodeType SubtypeCode
        {
            get
            {
                return this.subtypeCodeField;
            }
            set
            {
                this.subtypeCodeField = value;
            }
        }

        public CodeType DevelopmentStageCode
        {
            get
            {
                return this.developmentStageCodeField;
            }
            set
            {
                this.developmentStageCodeField = value;
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

        [XmlElement("ObservationBasisAgronomicalObservationScope")]
        public AgronomicalObservationScopeType[] ObservationBasisAgronomicalObservationScope
        {
            get
            {
                return this.observationBasisAgronomicalObservationScopeField;
            }
            set
            {
                this.observationBasisAgronomicalObservationScopeField = value;
            }
        }

        public ObservationProcessType UsedObservationProcess
        {
            get
            {
                return this.usedObservationProcessField;
            }
            set
            {
                this.usedObservationProcessField = value;
            }
        }
    }
}