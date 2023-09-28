namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgronomicalObservationPlot", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgronomicalObservationPlotType
    {

        private IDType idField;

        private IDType[] previousIDField;

        private TextType descriptionField;

        private DateTimeType dataSheetStartDateTimeField;

        private DateTimeType dataSheetEndDateTimeField;

        private IDType issuerAssignedIDField;

        private IDType[] recipientAssignedIDField;

        private DateTimeType harvestYearDateTimeField;

        private IDType europeanCAPIDField;

        private TechnicalCharacteristicType[] applicableTechnicalCharacteristicField;

        private AgronomicalObservationPartyType[] ownerAgronomicalObservationPartyField;

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

        [XmlElement("PreviousID")]
        public IDType[] PreviousID
        {
            get
            {
                return this.previousIDField;
            }
            set
            {
                this.previousIDField = value;
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

        public DateTimeType DataSheetStartDateTime
        {
            get
            {
                return this.dataSheetStartDateTimeField;
            }
            set
            {
                this.dataSheetStartDateTimeField = value;
            }
        }

        public DateTimeType DataSheetEndDateTime
        {
            get
            {
                return this.dataSheetEndDateTimeField;
            }
            set
            {
                this.dataSheetEndDateTimeField = value;
            }
        }

        public IDType IssuerAssignedID
        {
            get
            {
                return this.issuerAssignedIDField;
            }
            set
            {
                this.issuerAssignedIDField = value;
            }
        }

        [XmlElement("RecipientAssignedID")]
        public IDType[] RecipientAssignedID
        {
            get
            {
                return this.recipientAssignedIDField;
            }
            set
            {
                this.recipientAssignedIDField = value;
            }
        }

        public DateTimeType HarvestYearDateTime
        {
            get
            {
                return this.harvestYearDateTimeField;
            }
            set
            {
                this.harvestYearDateTimeField = value;
            }
        }

        public IDType EuropeanCAPID
        {
            get
            {
                return this.europeanCAPIDField;
            }
            set
            {
                this.europeanCAPIDField = value;
            }
        }

        [XmlElement("ApplicableTechnicalCharacteristic")]
        public TechnicalCharacteristicType[] ApplicableTechnicalCharacteristic
        {
            get
            {
                return this.applicableTechnicalCharacteristicField;
            }
            set
            {
                this.applicableTechnicalCharacteristicField = value;
            }
        }

        [XmlElement("OwnerAgronomicalObservationParty")]
        public AgronomicalObservationPartyType[] OwnerAgronomicalObservationParty
        {
            get
            {
                return this.ownerAgronomicalObservationPartyField;
            }
            set
            {
                this.ownerAgronomicalObservationPartyField = value;
            }
        }
    }
}