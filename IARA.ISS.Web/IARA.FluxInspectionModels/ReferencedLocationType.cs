namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ReferencedLocation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ReferencedLocationType
    {

        private IDType[] idField;

        private TextType[] nameField;

        private CodeType typeCodeField;

        private TextType[] descriptionField;

        private CodeType referenceTypeCodeField;

        private SpecifiedGeographicalCoordinateType physicalSpecifiedGeographicalCoordinateField;

        private StructuredAddressType physicalStructuredAddressField;

        private LaboratoryObservationReferenceType[] specifiedLaboratoryObservationReferenceField;

        private SpecifiedPolygonType includedSpecifiedPolygonField;

        private CSEngineeringCoordinateReferenceSystemType definedCSEngineeringCoordinateReferenceSystemField;

        private SpecifiedGeographicalFeatureType physicalSpecifiedGeographicalFeatureField;

        private SpecifiedAgriculturalApplicationType[] appliedSpecifiedAgriculturalApplicationField;

        private SpecifiedGeographicalPointType physicalSpecifiedGeographicalPointField;

        [XmlElement("ID")]
        public IDType[] ID
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

        [XmlElement("Name")]
        public TextType[] Name
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

        public CodeType ReferenceTypeCode
        {
            get
            {
                return this.referenceTypeCodeField;
            }
            set
            {
                this.referenceTypeCodeField = value;
            }
        }

        public SpecifiedGeographicalCoordinateType PhysicalSpecifiedGeographicalCoordinate
        {
            get
            {
                return this.physicalSpecifiedGeographicalCoordinateField;
            }
            set
            {
                this.physicalSpecifiedGeographicalCoordinateField = value;
            }
        }

        public StructuredAddressType PhysicalStructuredAddress
        {
            get
            {
                return this.physicalStructuredAddressField;
            }
            set
            {
                this.physicalStructuredAddressField = value;
            }
        }

        [XmlElement("SpecifiedLaboratoryObservationReference")]
        public LaboratoryObservationReferenceType[] SpecifiedLaboratoryObservationReference
        {
            get
            {
                return this.specifiedLaboratoryObservationReferenceField;
            }
            set
            {
                this.specifiedLaboratoryObservationReferenceField = value;
            }
        }

        public SpecifiedPolygonType IncludedSpecifiedPolygon
        {
            get
            {
                return this.includedSpecifiedPolygonField;
            }
            set
            {
                this.includedSpecifiedPolygonField = value;
            }
        }

        public CSEngineeringCoordinateReferenceSystemType DefinedCSEngineeringCoordinateReferenceSystem
        {
            get
            {
                return this.definedCSEngineeringCoordinateReferenceSystemField;
            }
            set
            {
                this.definedCSEngineeringCoordinateReferenceSystemField = value;
            }
        }

        public SpecifiedGeographicalFeatureType PhysicalSpecifiedGeographicalFeature
        {
            get
            {
                return this.physicalSpecifiedGeographicalFeatureField;
            }
            set
            {
                this.physicalSpecifiedGeographicalFeatureField = value;
            }
        }

        [XmlElement("AppliedSpecifiedAgriculturalApplication")]
        public SpecifiedAgriculturalApplicationType[] AppliedSpecifiedAgriculturalApplication
        {
            get
            {
                return this.appliedSpecifiedAgriculturalApplicationField;
            }
            set
            {
                this.appliedSpecifiedAgriculturalApplicationField = value;
            }
        }

        public SpecifiedGeographicalPointType PhysicalSpecifiedGeographicalPoint
        {
            get
            {
                return this.physicalSpecifiedGeographicalPointField;
            }
            set
            {
                this.physicalSpecifiedGeographicalPointField = value;
            }
        }
    }
}