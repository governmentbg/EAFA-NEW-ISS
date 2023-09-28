namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("VesselCrew", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class VesselCrewType
    {

        private CodeType typeCodeField;

        private QuantityType memberQuantityField;

        private QuantityType minimumSizeQuantityField;

        private QuantityType maximumSizeQuantityField;

        private QuantityType onDeckSizeQuantityField;

        private QuantityType aboveDeckSizeQuantityField;

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

        public QuantityType MemberQuantity
        {
            get
            {
                return this.memberQuantityField;
            }
            set
            {
                this.memberQuantityField = value;
            }
        }

        public QuantityType MinimumSizeQuantity
        {
            get
            {
                return this.minimumSizeQuantityField;
            }
            set
            {
                this.minimumSizeQuantityField = value;
            }
        }

        public QuantityType MaximumSizeQuantity
        {
            get
            {
                return this.maximumSizeQuantityField;
            }
            set
            {
                this.maximumSizeQuantityField = value;
            }
        }

        public QuantityType OnDeckSizeQuantity
        {
            get
            {
                return this.onDeckSizeQuantityField;
            }
            set
            {
                this.onDeckSizeQuantityField = value;
            }
        }

        public QuantityType AboveDeckSizeQuantity
        {
            get
            {
                return this.aboveDeckSizeQuantityField;
            }
            set
            {
                this.aboveDeckSizeQuantityField = value;
            }
        }
    }
}