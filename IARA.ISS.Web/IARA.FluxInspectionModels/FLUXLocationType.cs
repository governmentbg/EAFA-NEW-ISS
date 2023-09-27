namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FLUXLocation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FLUXLocationType
    {

        private CodeType typeCodeField;

        private IDType countryIDField;

        private IDType idField;

        private CodeType geopoliticalRegionCodeField;

        private TextType[] nameField;

        private IDType sovereignRightsCountryIDField;

        private IDType jurisdictionCountryIDField;

        private CodeType regionalFisheriesManagementOrganizationCodeField;

        private FLUXGeographicalCoordinateType specifiedPhysicalFLUXGeographicalCoordinateField;

        private StructuredAddressType[] postalStructuredAddressField;

        private StructuredAddressType physicalStructuredAddressField;

        private SpecifiedPolygonType[] boundarySpecifiedPolygonField;

        private FLUXCharacteristicType[] applicableFLUXCharacteristicField;

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

        public IDType CountryID
        {
            get
            {
                return this.countryIDField;
            }
            set
            {
                this.countryIDField = value;
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

        public IDType SovereignRightsCountryID
        {
            get
            {
                return this.sovereignRightsCountryIDField;
            }
            set
            {
                this.sovereignRightsCountryIDField = value;
            }
        }

        public IDType JurisdictionCountryID
        {
            get
            {
                return this.jurisdictionCountryIDField;
            }
            set
            {
                this.jurisdictionCountryIDField = value;
            }
        }

        public CodeType RegionalFisheriesManagementOrganizationCode
        {
            get
            {
                return this.regionalFisheriesManagementOrganizationCodeField;
            }
            set
            {
                this.regionalFisheriesManagementOrganizationCodeField = value;
            }
        }

        public FLUXGeographicalCoordinateType SpecifiedPhysicalFLUXGeographicalCoordinate
        {
            get
            {
                return this.specifiedPhysicalFLUXGeographicalCoordinateField;
            }
            set
            {
                this.specifiedPhysicalFLUXGeographicalCoordinateField = value;
            }
        }

        [XmlElement("PostalStructuredAddress")]
        public StructuredAddressType[] PostalStructuredAddress
        {
            get
            {
                return this.postalStructuredAddressField;
            }
            set
            {
                this.postalStructuredAddressField = value;
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

        [XmlElement("BoundarySpecifiedPolygon")]
        public SpecifiedPolygonType[] BoundarySpecifiedPolygon
        {
            get
            {
                return this.boundarySpecifiedPolygonField;
            }
            set
            {
                this.boundarySpecifiedPolygonField = value;
            }
        }

        [XmlElement("ApplicableFLUXCharacteristic")]
        public FLUXCharacteristicType[] ApplicableFLUXCharacteristic
        {
            get
            {
                return this.applicableFLUXCharacteristicField;
            }
            set
            {
                this.applicableFLUXCharacteristicField = value;
            }
        }
    }
}