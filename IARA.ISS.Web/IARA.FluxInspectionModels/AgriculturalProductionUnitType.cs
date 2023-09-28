namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalProductionUnit", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalProductionUnitType
    {

        private IDType idField;

        private CodeType[] typeCodeField;

        private TextType nameField;

        private AgriculturalConstructionType[] dedicatedAgriculturalConstructionField;

        private CropPlotType[] specifiedCropPlotField;

        private ComplaintIssueType[] declaredComplaintIssueField;

        private CropProduceBatchType[] specifiedCropProduceBatchField;

        private FieldCropType[] grownFieldCropField;

        private AgriculturalCertificateType[] specifiedAgriculturalCertificateField;

        private AgriculturalContractType specifiedAgriculturalContractField;

        private StructuredAddressType specifiedStructuredAddressField;

        private SpecifiedPartyType[] relatedSpecifiedPartyField;

        private AgriculturalProducerPartyType[] relatedAgriculturalProducerPartyField;

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

        [XmlElement("DedicatedAgriculturalConstruction")]
        public AgriculturalConstructionType[] DedicatedAgriculturalConstruction
        {
            get
            {
                return this.dedicatedAgriculturalConstructionField;
            }
            set
            {
                this.dedicatedAgriculturalConstructionField = value;
            }
        }

        [XmlElement("SpecifiedCropPlot")]
        public CropPlotType[] SpecifiedCropPlot
        {
            get
            {
                return this.specifiedCropPlotField;
            }
            set
            {
                this.specifiedCropPlotField = value;
            }
        }

        [XmlElement("DeclaredComplaintIssue")]
        public ComplaintIssueType[] DeclaredComplaintIssue
        {
            get
            {
                return this.declaredComplaintIssueField;
            }
            set
            {
                this.declaredComplaintIssueField = value;
            }
        }

        [XmlElement("SpecifiedCropProduceBatch")]
        public CropProduceBatchType[] SpecifiedCropProduceBatch
        {
            get
            {
                return this.specifiedCropProduceBatchField;
            }
            set
            {
                this.specifiedCropProduceBatchField = value;
            }
        }

        [XmlElement("GrownFieldCrop")]
        public FieldCropType[] GrownFieldCrop
        {
            get
            {
                return this.grownFieldCropField;
            }
            set
            {
                this.grownFieldCropField = value;
            }
        }

        [XmlElement("SpecifiedAgriculturalCertificate")]
        public AgriculturalCertificateType[] SpecifiedAgriculturalCertificate
        {
            get
            {
                return this.specifiedAgriculturalCertificateField;
            }
            set
            {
                this.specifiedAgriculturalCertificateField = value;
            }
        }

        public AgriculturalContractType SpecifiedAgriculturalContract
        {
            get
            {
                return this.specifiedAgriculturalContractField;
            }
            set
            {
                this.specifiedAgriculturalContractField = value;
            }
        }

        public StructuredAddressType SpecifiedStructuredAddress
        {
            get
            {
                return this.specifiedStructuredAddressField;
            }
            set
            {
                this.specifiedStructuredAddressField = value;
            }
        }

        [XmlElement("RelatedSpecifiedParty")]
        public SpecifiedPartyType[] RelatedSpecifiedParty
        {
            get
            {
                return this.relatedSpecifiedPartyField;
            }
            set
            {
                this.relatedSpecifiedPartyField = value;
            }
        }

        [XmlElement("RelatedAgriculturalProducerParty")]
        public AgriculturalProducerPartyType[] RelatedAgriculturalProducerParty
        {
            get
            {
                return this.relatedAgriculturalProducerPartyField;
            }
            set
            {
                this.relatedAgriculturalProducerPartyField = value;
            }
        }
    }
}