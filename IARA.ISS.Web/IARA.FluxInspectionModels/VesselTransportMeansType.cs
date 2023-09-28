namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("VesselTransportMeans", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class VesselTransportMeansType
    {

        private IDType[] idField;

        private TextType[] nameField;

        private CodeType[] typeCodeField;

        private DateTimeType commissioningDateTimeField;

        private CodeType operationalStatusCodeField;

        private CodeType hullMaterialCodeField;

        private MeasureType draughtMeasureField;

        private MeasureType speedMeasureField;

        private MeasureType trawlingSpeedMeasureField;

        private CodeType roleCodeField;

        private VesselCountryType registrationVesselCountryField;

        private VesselPositionEventType[] specifiedVesselPositionEventField;

        private RegistrationEventType[] specifiedRegistrationEventField;

        private ConstructionEventType specifiedConstructionEventField;

        private VesselEngineType[] attachedVesselEngineField;

        private VesselDimensionType[] specifiedVesselDimensionField;

        private FishingGearType[] onBoardFishingGearField;

        private VesselEquipmentCharacteristicType[] applicableVesselEquipmentCharacteristicField;

        private VesselAdministrativeCharacteristicType[] applicableVesselAdministrativeCharacteristicField;

        private FLUXPictureType[] illustrateFLUXPictureField;

        private ContactPartyType[] specifiedContactPartyField;

        private VesselCrewType specifiedVesselCrewField;

        private VesselStorageCharacteristicType[] applicableVesselStorageCharacteristicField;

        private VesselTechnicalCharacteristicType[] applicableVesselTechnicalCharacteristicField;

        private FLAPDocumentType[] grantedFLAPDocumentField;

        private FLUXLocationType[] specifiedFLUXLocationField;

        private FACatchType[] specifiedFACatchField;

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

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
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

        public DateTimeType CommissioningDateTime
        {
            get
            {
                return this.commissioningDateTimeField;
            }
            set
            {
                this.commissioningDateTimeField = value;
            }
        }

        public CodeType OperationalStatusCode
        {
            get
            {
                return this.operationalStatusCodeField;
            }
            set
            {
                this.operationalStatusCodeField = value;
            }
        }

        public CodeType HullMaterialCode
        {
            get
            {
                return this.hullMaterialCodeField;
            }
            set
            {
                this.hullMaterialCodeField = value;
            }
        }

        public MeasureType DraughtMeasure
        {
            get
            {
                return this.draughtMeasureField;
            }
            set
            {
                this.draughtMeasureField = value;
            }
        }

        public MeasureType SpeedMeasure
        {
            get
            {
                return this.speedMeasureField;
            }
            set
            {
                this.speedMeasureField = value;
            }
        }

        public MeasureType TrawlingSpeedMeasure
        {
            get
            {
                return this.trawlingSpeedMeasureField;
            }
            set
            {
                this.trawlingSpeedMeasureField = value;
            }
        }

        public CodeType RoleCode
        {
            get
            {
                return this.roleCodeField;
            }
            set
            {
                this.roleCodeField = value;
            }
        }

        public VesselCountryType RegistrationVesselCountry
        {
            get
            {
                return this.registrationVesselCountryField;
            }
            set
            {
                this.registrationVesselCountryField = value;
            }
        }

        [XmlElement("SpecifiedVesselPositionEvent")]
        public VesselPositionEventType[] SpecifiedVesselPositionEvent
        {
            get
            {
                return this.specifiedVesselPositionEventField;
            }
            set
            {
                this.specifiedVesselPositionEventField = value;
            }
        }

        [XmlElement("SpecifiedRegistrationEvent")]
        public RegistrationEventType[] SpecifiedRegistrationEvent
        {
            get
            {
                return this.specifiedRegistrationEventField;
            }
            set
            {
                this.specifiedRegistrationEventField = value;
            }
        }

        public ConstructionEventType SpecifiedConstructionEvent
        {
            get
            {
                return this.specifiedConstructionEventField;
            }
            set
            {
                this.specifiedConstructionEventField = value;
            }
        }

        [XmlElement("AttachedVesselEngine")]
        public VesselEngineType[] AttachedVesselEngine
        {
            get
            {
                return this.attachedVesselEngineField;
            }
            set
            {
                this.attachedVesselEngineField = value;
            }
        }

        [XmlElement("SpecifiedVesselDimension")]
        public VesselDimensionType[] SpecifiedVesselDimension
        {
            get
            {
                return this.specifiedVesselDimensionField;
            }
            set
            {
                this.specifiedVesselDimensionField = value;
            }
        }

        [XmlElement("OnBoardFishingGear")]
        public FishingGearType[] OnBoardFishingGear
        {
            get
            {
                return this.onBoardFishingGearField;
            }
            set
            {
                this.onBoardFishingGearField = value;
            }
        }

        [XmlElement("ApplicableVesselEquipmentCharacteristic")]
        public VesselEquipmentCharacteristicType[] ApplicableVesselEquipmentCharacteristic
        {
            get
            {
                return this.applicableVesselEquipmentCharacteristicField;
            }
            set
            {
                this.applicableVesselEquipmentCharacteristicField = value;
            }
        }

        [XmlElement("ApplicableVesselAdministrativeCharacteristic")]
        public VesselAdministrativeCharacteristicType[] ApplicableVesselAdministrativeCharacteristic
        {
            get
            {
                return this.applicableVesselAdministrativeCharacteristicField;
            }
            set
            {
                this.applicableVesselAdministrativeCharacteristicField = value;
            }
        }

        [XmlElement("IllustrateFLUXPicture")]
        public FLUXPictureType[] IllustrateFLUXPicture
        {
            get
            {
                return this.illustrateFLUXPictureField;
            }
            set
            {
                this.illustrateFLUXPictureField = value;
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

        public VesselCrewType SpecifiedVesselCrew
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

        [XmlElement("ApplicableVesselStorageCharacteristic")]
        public VesselStorageCharacteristicType[] ApplicableVesselStorageCharacteristic
        {
            get
            {
                return this.applicableVesselStorageCharacteristicField;
            }
            set
            {
                this.applicableVesselStorageCharacteristicField = value;
            }
        }

        [XmlElement("ApplicableVesselTechnicalCharacteristic")]
        public VesselTechnicalCharacteristicType[] ApplicableVesselTechnicalCharacteristic
        {
            get
            {
                return this.applicableVesselTechnicalCharacteristicField;
            }
            set
            {
                this.applicableVesselTechnicalCharacteristicField = value;
            }
        }

        [XmlElement("GrantedFLAPDocument")]
        public FLAPDocumentType[] GrantedFLAPDocument
        {
            get
            {
                return this.grantedFLAPDocumentField;
            }
            set
            {
                this.grantedFLAPDocumentField = value;
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

        [XmlElement("SpecifiedFACatch")]
        public FACatchType[] SpecifiedFACatch
        {
            get
            {
                return this.specifiedFACatchField;
            }
            set
            {
                this.specifiedFACatchField = value;
            }
        }
    }
}