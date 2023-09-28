namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LaboratoryObservationAnalysisMethod", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LaboratoryObservationAnalysisMethodType
    {

        private TextType nameField;

        private CodeType obligatoryTypeCodeField;

        private CodeType standardTypeCodeField;

        private CodeType localTypeCodeField;

        private TextType externalReferenceField;

        private TextType informationField;

        private MeasureType sampledObjectMinimumRequiredObjectSizeMeasureField;

        private CodeType certificationTypeCodeField;

        private IDType certificationIDField;

        private IDType idField;

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

        public CodeType ObligatoryTypeCode
        {
            get
            {
                return this.obligatoryTypeCodeField;
            }
            set
            {
                this.obligatoryTypeCodeField = value;
            }
        }

        public CodeType StandardTypeCode
        {
            get
            {
                return this.standardTypeCodeField;
            }
            set
            {
                this.standardTypeCodeField = value;
            }
        }

        public CodeType LocalTypeCode
        {
            get
            {
                return this.localTypeCodeField;
            }
            set
            {
                this.localTypeCodeField = value;
            }
        }

        public TextType ExternalReference
        {
            get
            {
                return this.externalReferenceField;
            }
            set
            {
                this.externalReferenceField = value;
            }
        }

        public TextType Information
        {
            get
            {
                return this.informationField;
            }
            set
            {
                this.informationField = value;
            }
        }

        public MeasureType SampledObjectMinimumRequiredObjectSizeMeasure
        {
            get
            {
                return this.sampledObjectMinimumRequiredObjectSizeMeasureField;
            }
            set
            {
                this.sampledObjectMinimumRequiredObjectSizeMeasureField = value;
            }
        }

        public CodeType CertificationTypeCode
        {
            get
            {
                return this.certificationTypeCodeField;
            }
            set
            {
                this.certificationTypeCodeField = value;
            }
        }

        public IDType CertificationID
        {
            get
            {
                return this.certificationIDField;
            }
            set
            {
                this.certificationIDField = value;
            }
        }

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
    }
}