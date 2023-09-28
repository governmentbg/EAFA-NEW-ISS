namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalSampledObject", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalSampledObjectType
    {

        private IDType idField;

        private TextType nameField;

        private CodeType standardTypeCodeField;

        private CodeType localTypeCodeField;

        private TextType typeNameField;

        private DateTimeType productionDateTimeField;

        private CodeType samplingProcedureTypeCodeField;

        private IDType samplingProcedureIDField;

        private IDType countryOfOriginIDField;

        private TextType countryOfOriginSubDivisionNameField;

        private MeasureType sizeMeasureField;

        private QuantityType batchQuantityField;

        private MeasureType batchPopulationSizeMeasureField;

        private TextType treatmentField;

        private IDType batchIdentificationIDField;

        private LaboratoryObservationNoteType conclusionSpecifiedLaboratoryObservationNoteField;

        private LaboratoryObservationNoteType additionalInformationSpecifiedLaboratoryObservationNoteField;

        private LaboratoryObservationPartyType[] authorizedLaboratoryObservationPartyField;

        private LaboratoryObservationPartyType producerLaboratoryObservationPartyField;

        private LaboratoryObservationPartyType carrierLaboratoryObservationPartyField;

        private LaboratoryObservationPartyType customerLaboratoryObservationPartyField;

        private LaboratoryObservationPartyType packerLaboratoryObservationPartyField;

        private LaboratoryObservationPartyType supplierLaboratoryObservationPartyField;

        private LaboratoryObservationPartyType ownerLaboratoryObservationPartyField;

        private LaboratoryObservationPartyType[] traderLaboratoryObservationPartyField;

        private SampledObjectAdditiveType[] appliedSampledObjectAdditiveField;

        private SampledObjectCropType specifiedSampledObjectCropField;

        private SampledObjectContractType[] specifiedSampledObjectContractField;

        private SampledObjectIllnessType specifiedSampledObjectIllnessField;

        private MilkTankType containingMilkTankField;

        private SampledObjectCargoType specifiedSampledObjectCargoField;

        private SampledObjectSoilType specifiedAsSampledObjectSoilField;

        private SampledObjectSilageType[] specifiedAsSampledObjectSilageField;

        private SampledObjectAnimalType specifiedAsSampledObjectAnimalField;

        private SampledObjectWaterLocationType specifiedSampledObjectWaterLocationField;

        private LaboratoryObservationInstructionsType[] specifiedLaboratoryObservationInstructionsField;

        private LaboratoryObservationReferenceType[] specifiedLaboratoryObservationReferenceField;

        private PhotographicPictureType[] conclusionRelatedPhotographicPictureField;

        private SpecifiedBinaryFileType[] conclusionRelatedSpecifiedBinaryFileField;

        private PhotographicPictureType[] informationRelatedPhotographicPictureField;

        private SpecifiedBinaryFileType[] informationRelatedSpecifiedBinaryFileField;

        private ReferencedTransportMeansType[] usedReferencedTransportMeansField;

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

        public TextType TypeName
        {
            get
            {
                return this.typeNameField;
            }
            set
            {
                this.typeNameField = value;
            }
        }

        public DateTimeType ProductionDateTime
        {
            get
            {
                return this.productionDateTimeField;
            }
            set
            {
                this.productionDateTimeField = value;
            }
        }

        public CodeType SamplingProcedureTypeCode
        {
            get
            {
                return this.samplingProcedureTypeCodeField;
            }
            set
            {
                this.samplingProcedureTypeCodeField = value;
            }
        }

        public IDType SamplingProcedureID
        {
            get
            {
                return this.samplingProcedureIDField;
            }
            set
            {
                this.samplingProcedureIDField = value;
            }
        }

        public IDType CountryOfOriginID
        {
            get
            {
                return this.countryOfOriginIDField;
            }
            set
            {
                this.countryOfOriginIDField = value;
            }
        }

        public TextType CountryOfOriginSubDivisionName
        {
            get
            {
                return this.countryOfOriginSubDivisionNameField;
            }
            set
            {
                this.countryOfOriginSubDivisionNameField = value;
            }
        }

        public MeasureType SizeMeasure
        {
            get
            {
                return this.sizeMeasureField;
            }
            set
            {
                this.sizeMeasureField = value;
            }
        }

        public QuantityType BatchQuantity
        {
            get
            {
                return this.batchQuantityField;
            }
            set
            {
                this.batchQuantityField = value;
            }
        }

        public MeasureType BatchPopulationSizeMeasure
        {
            get
            {
                return this.batchPopulationSizeMeasureField;
            }
            set
            {
                this.batchPopulationSizeMeasureField = value;
            }
        }

        public TextType Treatment
        {
            get
            {
                return this.treatmentField;
            }
            set
            {
                this.treatmentField = value;
            }
        }

        public IDType BatchIdentificationID
        {
            get
            {
                return this.batchIdentificationIDField;
            }
            set
            {
                this.batchIdentificationIDField = value;
            }
        }

        public LaboratoryObservationNoteType ConclusionSpecifiedLaboratoryObservationNote
        {
            get
            {
                return this.conclusionSpecifiedLaboratoryObservationNoteField;
            }
            set
            {
                this.conclusionSpecifiedLaboratoryObservationNoteField = value;
            }
        }

        public LaboratoryObservationNoteType AdditionalInformationSpecifiedLaboratoryObservationNote
        {
            get
            {
                return this.additionalInformationSpecifiedLaboratoryObservationNoteField;
            }
            set
            {
                this.additionalInformationSpecifiedLaboratoryObservationNoteField = value;
            }
        }

        [XmlElement("AuthorizedLaboratoryObservationParty")]
        public LaboratoryObservationPartyType[] AuthorizedLaboratoryObservationParty
        {
            get
            {
                return this.authorizedLaboratoryObservationPartyField;
            }
            set
            {
                this.authorizedLaboratoryObservationPartyField = value;
            }
        }

        public LaboratoryObservationPartyType ProducerLaboratoryObservationParty
        {
            get
            {
                return this.producerLaboratoryObservationPartyField;
            }
            set
            {
                this.producerLaboratoryObservationPartyField = value;
            }
        }

        public LaboratoryObservationPartyType CarrierLaboratoryObservationParty
        {
            get
            {
                return this.carrierLaboratoryObservationPartyField;
            }
            set
            {
                this.carrierLaboratoryObservationPartyField = value;
            }
        }

        public LaboratoryObservationPartyType CustomerLaboratoryObservationParty
        {
            get
            {
                return this.customerLaboratoryObservationPartyField;
            }
            set
            {
                this.customerLaboratoryObservationPartyField = value;
            }
        }

        public LaboratoryObservationPartyType PackerLaboratoryObservationParty
        {
            get
            {
                return this.packerLaboratoryObservationPartyField;
            }
            set
            {
                this.packerLaboratoryObservationPartyField = value;
            }
        }

        public LaboratoryObservationPartyType SupplierLaboratoryObservationParty
        {
            get
            {
                return this.supplierLaboratoryObservationPartyField;
            }
            set
            {
                this.supplierLaboratoryObservationPartyField = value;
            }
        }

        public LaboratoryObservationPartyType OwnerLaboratoryObservationParty
        {
            get
            {
                return this.ownerLaboratoryObservationPartyField;
            }
            set
            {
                this.ownerLaboratoryObservationPartyField = value;
            }
        }

        [XmlElement("TraderLaboratoryObservationParty")]
        public LaboratoryObservationPartyType[] TraderLaboratoryObservationParty
        {
            get
            {
                return this.traderLaboratoryObservationPartyField;
            }
            set
            {
                this.traderLaboratoryObservationPartyField = value;
            }
        }

        [XmlElement("AppliedSampledObjectAdditive")]
        public SampledObjectAdditiveType[] AppliedSampledObjectAdditive
        {
            get
            {
                return this.appliedSampledObjectAdditiveField;
            }
            set
            {
                this.appliedSampledObjectAdditiveField = value;
            }
        }

        public SampledObjectCropType SpecifiedSampledObjectCrop
        {
            get
            {
                return this.specifiedSampledObjectCropField;
            }
            set
            {
                this.specifiedSampledObjectCropField = value;
            }
        }

        [XmlElement("SpecifiedSampledObjectContract")]
        public SampledObjectContractType[] SpecifiedSampledObjectContract
        {
            get
            {
                return this.specifiedSampledObjectContractField;
            }
            set
            {
                this.specifiedSampledObjectContractField = value;
            }
        }

        public SampledObjectIllnessType SpecifiedSampledObjectIllness
        {
            get
            {
                return this.specifiedSampledObjectIllnessField;
            }
            set
            {
                this.specifiedSampledObjectIllnessField = value;
            }
        }

        public MilkTankType ContainingMilkTank
        {
            get
            {
                return this.containingMilkTankField;
            }
            set
            {
                this.containingMilkTankField = value;
            }
        }

        public SampledObjectCargoType SpecifiedSampledObjectCargo
        {
            get
            {
                return this.specifiedSampledObjectCargoField;
            }
            set
            {
                this.specifiedSampledObjectCargoField = value;
            }
        }

        public SampledObjectSoilType SpecifiedAsSampledObjectSoil
        {
            get
            {
                return this.specifiedAsSampledObjectSoilField;
            }
            set
            {
                this.specifiedAsSampledObjectSoilField = value;
            }
        }

        [XmlElement("SpecifiedAsSampledObjectSilage")]
        public SampledObjectSilageType[] SpecifiedAsSampledObjectSilage
        {
            get
            {
                return this.specifiedAsSampledObjectSilageField;
            }
            set
            {
                this.specifiedAsSampledObjectSilageField = value;
            }
        }

        public SampledObjectAnimalType SpecifiedAsSampledObjectAnimal
        {
            get
            {
                return this.specifiedAsSampledObjectAnimalField;
            }
            set
            {
                this.specifiedAsSampledObjectAnimalField = value;
            }
        }

        public SampledObjectWaterLocationType SpecifiedSampledObjectWaterLocation
        {
            get
            {
                return this.specifiedSampledObjectWaterLocationField;
            }
            set
            {
                this.specifiedSampledObjectWaterLocationField = value;
            }
        }

        [XmlElement("SpecifiedLaboratoryObservationInstructions")]
        public LaboratoryObservationInstructionsType[] SpecifiedLaboratoryObservationInstructions
        {
            get
            {
                return this.specifiedLaboratoryObservationInstructionsField;
            }
            set
            {
                this.specifiedLaboratoryObservationInstructionsField = value;
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

        [XmlElement("ConclusionRelatedPhotographicPicture")]
        public PhotographicPictureType[] ConclusionRelatedPhotographicPicture
        {
            get
            {
                return this.conclusionRelatedPhotographicPictureField;
            }
            set
            {
                this.conclusionRelatedPhotographicPictureField = value;
            }
        }

        [XmlElement("ConclusionRelatedSpecifiedBinaryFile")]
        public SpecifiedBinaryFileType[] ConclusionRelatedSpecifiedBinaryFile
        {
            get
            {
                return this.conclusionRelatedSpecifiedBinaryFileField;
            }
            set
            {
                this.conclusionRelatedSpecifiedBinaryFileField = value;
            }
        }

        [XmlElement("InformationRelatedPhotographicPicture")]
        public PhotographicPictureType[] InformationRelatedPhotographicPicture
        {
            get
            {
                return this.informationRelatedPhotographicPictureField;
            }
            set
            {
                this.informationRelatedPhotographicPictureField = value;
            }
        }

        [XmlElement("InformationRelatedSpecifiedBinaryFile")]
        public SpecifiedBinaryFileType[] InformationRelatedSpecifiedBinaryFile
        {
            get
            {
                return this.informationRelatedSpecifiedBinaryFileField;
            }
            set
            {
                this.informationRelatedSpecifiedBinaryFileField = value;
            }
        }

        [XmlElement("UsedReferencedTransportMeans")]
        public ReferencedTransportMeansType[] UsedReferencedTransportMeans
        {
            get
            {
                return this.usedReferencedTransportMeansField;
            }
            set
            {
                this.usedReferencedTransportMeansField = value;
            }
        }
    }
}