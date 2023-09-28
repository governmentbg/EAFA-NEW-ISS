namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalProducerParty", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalProducerPartyType
    {

        private IDType idField;

        private TextType nameField;

        private TextType descriptionField;

        private CodeType[] classificationCodeField;

        private AgriculturalProductionUnitType[] managedAgriculturalProductionUnitField;

        private AgriculturalProducerPartyType[] parentAgriculturalProducerPartyField;

        private SpecifiedPartyType[] designatedSpecifiedPartyField;

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

        [XmlElement("ClassificationCode")]
        public CodeType[] ClassificationCode
        {
            get
            {
                return this.classificationCodeField;
            }
            set
            {
                this.classificationCodeField = value;
            }
        }

        [XmlElement("ManagedAgriculturalProductionUnit")]
        public AgriculturalProductionUnitType[] ManagedAgriculturalProductionUnit
        {
            get
            {
                return this.managedAgriculturalProductionUnitField;
            }
            set
            {
                this.managedAgriculturalProductionUnitField = value;
            }
        }

        [XmlElement("ParentAgriculturalProducerParty")]
        public AgriculturalProducerPartyType[] ParentAgriculturalProducerParty
        {
            get
            {
                return this.parentAgriculturalProducerPartyField;
            }
            set
            {
                this.parentAgriculturalProducerPartyField = value;
            }
        }

        [XmlElement("DesignatedSpecifiedParty")]
        public SpecifiedPartyType[] DesignatedSpecifiedParty
        {
            get
            {
                return this.designatedSpecifiedPartyField;
            }
            set
            {
                this.designatedSpecifiedPartyField = value;
            }
        }
    }
}