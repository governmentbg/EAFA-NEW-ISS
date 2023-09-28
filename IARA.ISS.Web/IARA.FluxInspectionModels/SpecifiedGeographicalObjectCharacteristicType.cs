namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecifiedGeographicalObjectCharacteristic", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecifiedGeographicalObjectCharacteristicType
    {

        private IDType idField;

        private TextType descriptionField;

        private TextType descriptionReferenceField;

        private TextType nameField;

        private IndicatorType geometryCollectionIndicatorField;

        private IndicatorType physicalIndicatorField;

        private TextType relevantGeometryTypeField;

        private TextType shapeTypeField;

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

        public TextType DescriptionReference
        {
            get
            {
                return this.descriptionReferenceField;
            }
            set
            {
                this.descriptionReferenceField = value;
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

        public IndicatorType GeometryCollectionIndicator
        {
            get
            {
                return this.geometryCollectionIndicatorField;
            }
            set
            {
                this.geometryCollectionIndicatorField = value;
            }
        }

        public IndicatorType PhysicalIndicator
        {
            get
            {
                return this.physicalIndicatorField;
            }
            set
            {
                this.physicalIndicatorField = value;
            }
        }

        public TextType RelevantGeometryType
        {
            get
            {
                return this.relevantGeometryTypeField;
            }
            set
            {
                this.relevantGeometryTypeField = value;
            }
        }

        public TextType ShapeType
        {
            get
            {
                return this.shapeTypeField;
            }
            set
            {
                this.shapeTypeField = value;
            }
        }
    }
}