namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecifiedGeographicalFeature", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecifiedGeographicalFeatureType
    {

        private TextType descriptionField;

        private TextType nameField;

        private IDType coordinateReferenceSystemIDField;

        private IDType idField;

        private IndicatorType collectionIndicatorField;

        private SpecifiedGeographicalPointType includedSpecifiedGeographicalPointField;

        private SpecifiedGeographicalLineType includedSpecifiedGeographicalLineField;

        private SpecifiedGeographicalSurfaceType includedSpecifiedGeographicalSurfaceField;

        private SpecifiedGeographicalMultiPointType includedSpecifiedGeographicalMultiPointField;

        private SpecifiedGeographicalMultiCurveType includedSpecifiedGeographicalMultiCurveField;

        private SpecifiedGeographicalMultiSurfaceType includedSpecifiedGeographicalMultiSurfaceField;

        private SpecifiedPolygonType includedSpecifiedPolygonField;

        private SpecifiedGeographicalGridType includedSpecifiedGeographicalGridField;

        private CSEngineeringCoordinateReferenceSystemType usedCSEngineeringCoordinateReferenceSystemField;

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

        public IDType CoordinateReferenceSystemID
        {
            get
            {
                return this.coordinateReferenceSystemIDField;
            }
            set
            {
                this.coordinateReferenceSystemIDField = value;
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

        public IndicatorType CollectionIndicator
        {
            get
            {
                return this.collectionIndicatorField;
            }
            set
            {
                this.collectionIndicatorField = value;
            }
        }

        public SpecifiedGeographicalPointType IncludedSpecifiedGeographicalPoint
        {
            get
            {
                return this.includedSpecifiedGeographicalPointField;
            }
            set
            {
                this.includedSpecifiedGeographicalPointField = value;
            }
        }

        public SpecifiedGeographicalLineType IncludedSpecifiedGeographicalLine
        {
            get
            {
                return this.includedSpecifiedGeographicalLineField;
            }
            set
            {
                this.includedSpecifiedGeographicalLineField = value;
            }
        }

        public SpecifiedGeographicalSurfaceType IncludedSpecifiedGeographicalSurface
        {
            get
            {
                return this.includedSpecifiedGeographicalSurfaceField;
            }
            set
            {
                this.includedSpecifiedGeographicalSurfaceField = value;
            }
        }

        public SpecifiedGeographicalMultiPointType IncludedSpecifiedGeographicalMultiPoint
        {
            get
            {
                return this.includedSpecifiedGeographicalMultiPointField;
            }
            set
            {
                this.includedSpecifiedGeographicalMultiPointField = value;
            }
        }

        public SpecifiedGeographicalMultiCurveType IncludedSpecifiedGeographicalMultiCurve
        {
            get
            {
                return this.includedSpecifiedGeographicalMultiCurveField;
            }
            set
            {
                this.includedSpecifiedGeographicalMultiCurveField = value;
            }
        }

        public SpecifiedGeographicalMultiSurfaceType IncludedSpecifiedGeographicalMultiSurface
        {
            get
            {
                return this.includedSpecifiedGeographicalMultiSurfaceField;
            }
            set
            {
                this.includedSpecifiedGeographicalMultiSurfaceField = value;
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

        public SpecifiedGeographicalGridType IncludedSpecifiedGeographicalGrid
        {
            get
            {
                return this.includedSpecifiedGeographicalGridField;
            }
            set
            {
                this.includedSpecifiedGeographicalGridField = value;
            }
        }

        public CSEngineeringCoordinateReferenceSystemType UsedCSEngineeringCoordinateReferenceSystem
        {
            get
            {
                return this.usedCSEngineeringCoordinateReferenceSystemField;
            }
            set
            {
                this.usedCSEngineeringCoordinateReferenceSystemField = value;
            }
        }
    }
}