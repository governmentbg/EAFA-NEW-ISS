namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FLAPDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FLAPDocumentType
    {

        private IDType idField;

        private IDType[] vesselIDField;

        private IDType[] joinedVesselIDField;

        private TextType nameField;

        private IndicatorType firstApplicationIndicatorField;

        private TextType remarksField;

        private DateTimeType decisionDateTimeField;

        private CodeType typeCodeField;

        private FLUXCharacteristicType[] specifiedFLUXCharacteristicField;

        private VesselCrewType[] specifiedVesselCrewField;

        private DelimitedPeriodType[] applicableDelimitedPeriodField;

        private DelimitedPeriodType entryIntoForceDelimitedPeriodField;

        private VesselTransportCharterType[] specifiedVesselTransportCharterField;

        private ContactPartyType[] specifiedContactPartyField;

        private TargetedQuotaType[] specifiedTargetedQuotaField;

        private FLUXBinaryFileType[] attachedFLUXBinaryFileField;

        private FLUXLocationType[] specifiedFLUXLocationField;

        private ValidationResultDocumentType[] relatedValidationResultDocumentField;

        private FLAPRequestDocumentType[] relatedFLAPRequestDocumentField;

        private AuthorizationStatusType[] specifiedAuthorizationStatusField;

        private VesselTransportMeansType[] specifiedVesselTransportMeansField;

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

        [XmlElement("VesselID")]
        public IDType[] VesselID
        {
            get
            {
                return this.vesselIDField;
            }
            set
            {
                this.vesselIDField = value;
            }
        }

        [XmlElement("JoinedVesselID")]
        public IDType[] JoinedVesselID
        {
            get
            {
                return this.joinedVesselIDField;
            }
            set
            {
                this.joinedVesselIDField = value;
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

        public IndicatorType FirstApplicationIndicator
        {
            get
            {
                return this.firstApplicationIndicatorField;
            }
            set
            {
                this.firstApplicationIndicatorField = value;
            }
        }

        public TextType Remarks
        {
            get
            {
                return this.remarksField;
            }
            set
            {
                this.remarksField = value;
            }
        }

        public DateTimeType DecisionDateTime
        {
            get
            {
                return this.decisionDateTimeField;
            }
            set
            {
                this.decisionDateTimeField = value;
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

        [XmlElement("SpecifiedFLUXCharacteristic")]
        public FLUXCharacteristicType[] SpecifiedFLUXCharacteristic
        {
            get
            {
                return this.specifiedFLUXCharacteristicField;
            }
            set
            {
                this.specifiedFLUXCharacteristicField = value;
            }
        }

        [XmlElement("SpecifiedVesselCrew")]
        public VesselCrewType[] SpecifiedVesselCrew
        {
            get
            {
                return this.specifiedVesselCrewField;
            }
            set
            {
                this.specifiedVesselCrewField = value;
            }
        }

        [XmlElement("ApplicableDelimitedPeriod")]
        public DelimitedPeriodType[] ApplicableDelimitedPeriod
        {
            get
            {
                return this.applicableDelimitedPeriodField;
            }
            set
            {
                this.applicableDelimitedPeriodField = value;
            }
        }

        public DelimitedPeriodType EntryIntoForceDelimitedPeriod
        {
            get
            {
                return this.entryIntoForceDelimitedPeriodField;
            }
            set
            {
                this.entryIntoForceDelimitedPeriodField = value;
            }
        }

        [XmlElement("SpecifiedVesselTransportCharter")]
        public VesselTransportCharterType[] SpecifiedVesselTransportCharter
        {
            get
            {
                return this.specifiedVesselTransportCharterField;
            }
            set
            {
                this.specifiedVesselTransportCharterField = value;
            }
        }

        [XmlElement("SpecifiedContactParty")]
        public ContactPartyType[] SpecifiedContactParty
        {
            get
            {
                return this.specifiedContactPartyField;
            }
            set
            {
                this.specifiedContactPartyField = value;
            }
        }

        [XmlElement("SpecifiedTargetedQuota")]
        public TargetedQuotaType[] SpecifiedTargetedQuota
        {
            get
            {
                return this.specifiedTargetedQuotaField;
            }
            set
            {
                this.specifiedTargetedQuotaField = value;
            }
        }

        [XmlElement("AttachedFLUXBinaryFile")]
        public FLUXBinaryFileType[] AttachedFLUXBinaryFile
        {
            get
            {
                return this.attachedFLUXBinaryFileField;
            }
            set
            {
                this.attachedFLUXBinaryFileField = value;
            }
        }

        [XmlElement("SpecifiedFLUXLocation")]
        public FLUXLocationType[] SpecifiedFLUXLocation
        {
            get
            {
                return this.specifiedFLUXLocationField;
            }
            set
            {
                this.specifiedFLUXLocationField = value;
            }
        }

        [XmlElement("RelatedValidationResultDocument")]
        public ValidationResultDocumentType[] RelatedValidationResultDocument
        {
            get
            {
                return this.relatedValidationResultDocumentField;
            }
            set
            {
                this.relatedValidationResultDocumentField = value;
            }
        }

        [XmlElement("RelatedFLAPRequestDocument")]
        public FLAPRequestDocumentType[] RelatedFLAPRequestDocument
        {
            get
            {
                return this.relatedFLAPRequestDocumentField;
            }
            set
            {
                this.relatedFLAPRequestDocumentField = value;
            }
        }

        [XmlElement("SpecifiedAuthorizationStatus")]
        public AuthorizationStatusType[] SpecifiedAuthorizationStatus
        {
            get
            {
                return this.specifiedAuthorizationStatusField;
            }
            set
            {
                this.specifiedAuthorizationStatusField = value;
            }
        }

        [XmlElement("SpecifiedVesselTransportMeans")]
        public VesselTransportMeansType[] SpecifiedVesselTransportMeans
        {
            get
            {
                return this.specifiedVesselTransportMeansField;
            }
            set
            {
                this.specifiedVesselTransportMeansField = value;
            }
        }
    }
}