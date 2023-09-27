namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalZoneArea", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalZoneAreaType
    {

        private IDType idField;

        private TextType nameField;

        private IDType[] thirdPartyIssuedIDField;

        private TextType designatedSectionField;

        private CodeType multiSurfaceTypeCodeField;

        private AgriculturalCharacteristicType[] applicableAgriculturalCharacteristicField;

        private ReferencedLocationType specifiedReferencedLocationField;

        private AgriculturalZoneAreaType[] subordinateAgriculturalZoneAreaField;

        private CropProduceType[] harvestedCropProduceField;

        private SpecifiedAgriculturalApplicationType[] appliedSpecifiedAgriculturalApplicationField;

        private CropPlotType[] specifiedCropPlotField;

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

        [XmlElement("ThirdPartyIssuedID")]
        public IDType[] ThirdPartyIssuedID
        {
            get
            {
                return this.thirdPartyIssuedIDField;
            }
            set
            {
                this.thirdPartyIssuedIDField = value;
            }
        }

        public TextType DesignatedSection
        {
            get
            {
                return this.designatedSectionField;
            }
            set
            {
                this.designatedSectionField = value;
            }
        }

        public CodeType MultiSurfaceTypeCode
        {
            get
            {
                return this.multiSurfaceTypeCodeField;
            }
            set
            {
                this.multiSurfaceTypeCodeField = value;
            }
        }

        [XmlElement("ApplicableAgriculturalCharacteristic")]
        public AgriculturalCharacteristicType[] ApplicableAgriculturalCharacteristic
        {
            get
            {
                return this.applicableAgriculturalCharacteristicField;
            }
            set
            {
                this.applicableAgriculturalCharacteristicField = value;
            }
        }

        public ReferencedLocationType SpecifiedReferencedLocation
        {
            get
            {
                return this.specifiedReferencedLocationField;
            }
            set
            {
                this.specifiedReferencedLocationField = value;
            }
        }

        [XmlElement("SubordinateAgriculturalZoneArea")]
        public AgriculturalZoneAreaType[] SubordinateAgriculturalZoneArea
        {
            get
            {
                return this.subordinateAgriculturalZoneAreaField;
            }
            set
            {
                this.subordinateAgriculturalZoneAreaField = value;
            }
        }

        [XmlElement("HarvestedCropProduce")]
        public CropProduceType[] HarvestedCropProduce
        {
            get
            {
                return this.harvestedCropProduceField;
            }
            set
            {
                this.harvestedCropProduceField = value;
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
    }
}