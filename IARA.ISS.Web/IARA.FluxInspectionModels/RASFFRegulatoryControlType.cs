namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RASFFRegulatoryControl", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RASFFRegulatoryControlType
    {

        private CodeType typeCodeField;

        private CodeType reasonCodeField;

        private RASFFCountryType despatchRASFFCountryField;

        private RASFFCountryType destinationRASFFCountryField;

        private RASFFTransportMovementType identifiedRASFFTransportMovementField;

        private RASFFPartyType inspectionPostRASFFPartyField;

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public CodeType ReasonCode
        {
            get => this.reasonCodeField;
            set => this.reasonCodeField = value;
        }

        public RASFFCountryType DespatchRASFFCountry
        {
            get => this.despatchRASFFCountryField;
            set => this.despatchRASFFCountryField = value;
        }

        public RASFFCountryType DestinationRASFFCountry
        {
            get => this.destinationRASFFCountryField;
            set => this.destinationRASFFCountryField = value;
        }

        public RASFFTransportMovementType IdentifiedRASFFTransportMovement
        {
            get => this.identifiedRASFFTransportMovementField;
            set => this.identifiedRASFFTransportMovementField = value;
        }

        public RASFFPartyType InspectionPostRASFFParty
        {
            get => this.inspectionPostRASFFPartyField;
            set => this.inspectionPostRASFFPartyField = value;
        }
    }
}