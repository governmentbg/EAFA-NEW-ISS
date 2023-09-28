namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAWrapFormality", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAWrapFormalityType
    {

        private IDType idField;

        private TextType nameField;

        private TextType manifestField;

        private IDType nomenclatureIDField;

        private TextType nomenclatureNameField;

        private AAAWrapAccountingPeriodType[] specifiedAAAWrapAccountingPeriodField;

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

        public TextType Manifest
        {
            get
            {
                return this.manifestField;
            }
            set
            {
                this.manifestField = value;
            }
        }

        public IDType NomenclatureID
        {
            get
            {
                return this.nomenclatureIDField;
            }
            set
            {
                this.nomenclatureIDField = value;
            }
        }

        public TextType NomenclatureName
        {
            get
            {
                return this.nomenclatureNameField;
            }
            set
            {
                this.nomenclatureNameField = value;
            }
        }

        [XmlElement("SpecifiedAAAWrapAccountingPeriod")]
        public AAAWrapAccountingPeriodType[] SpecifiedAAAWrapAccountingPeriod
        {
            get
            {
                return this.specifiedAAAWrapAccountingPeriodField;
            }
            set
            {
                this.specifiedAAAWrapAccountingPeriodField = value;
            }
        }
    }
}