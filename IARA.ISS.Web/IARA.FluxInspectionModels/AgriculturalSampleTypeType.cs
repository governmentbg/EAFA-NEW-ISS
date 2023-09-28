namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalSampleType", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalSampleTypeType
    {

        private CodeType localTypeCodeField;

        private CodeType standardTypeCodeField;

        private TextType materialTypeField;

        private CodeType materialTypeCodeField;

        private AgriculturalSampleTissueType specifiedAgriculturalSampleTissueField;

        private AgriculturalSampleAutopsyType specifiedAgriculturalSampleAutopsyField;

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

        public TextType MaterialType
        {
            get
            {
                return this.materialTypeField;
            }
            set
            {
                this.materialTypeField = value;
            }
        }

        public CodeType MaterialTypeCode
        {
            get
            {
                return this.materialTypeCodeField;
            }
            set
            {
                this.materialTypeCodeField = value;
            }
        }

        public AgriculturalSampleTissueType SpecifiedAgriculturalSampleTissue
        {
            get
            {
                return this.specifiedAgriculturalSampleTissueField;
            }
            set
            {
                this.specifiedAgriculturalSampleTissueField = value;
            }
        }

        public AgriculturalSampleAutopsyType SpecifiedAgriculturalSampleAutopsy
        {
            get
            {
                return this.specifiedAgriculturalSampleAutopsyField;
            }
            set
            {
                this.specifiedAgriculturalSampleAutopsyField = value;
            }
        }
    }
}