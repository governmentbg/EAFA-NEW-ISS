namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpeciesTTProduct", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpeciesTTProductType
    {

        private CodeType[] speciesTypeCodeField;

        private CodeType[] scientificSpeciesNameCodeField;

        private CodeType[] regulationSpeciesNameCodeField;

        private CodeType[] tradeSpeciesNameCodeField;

        private CodeType[] additionalTypeCodeField;

        [XmlElement("SpeciesTypeCode")]
        public CodeType[] SpeciesTypeCode
        {
            get
            {
                return this.speciesTypeCodeField;
            }
            set
            {
                this.speciesTypeCodeField = value;
            }
        }

        [XmlElement("ScientificSpeciesNameCode")]
        public CodeType[] ScientificSpeciesNameCode
        {
            get
            {
                return this.scientificSpeciesNameCodeField;
            }
            set
            {
                this.scientificSpeciesNameCodeField = value;
            }
        }

        [XmlElement("RegulationSpeciesNameCode")]
        public CodeType[] RegulationSpeciesNameCode
        {
            get
            {
                return this.regulationSpeciesNameCodeField;
            }
            set
            {
                this.regulationSpeciesNameCodeField = value;
            }
        }

        [XmlElement("TradeSpeciesNameCode")]
        public CodeType[] TradeSpeciesNameCode
        {
            get
            {
                return this.tradeSpeciesNameCodeField;
            }
            set
            {
                this.tradeSpeciesNameCodeField = value;
            }
        }

        [XmlElement("AdditionalTypeCode")]
        public CodeType[] AdditionalTypeCode
        {
            get
            {
                return this.additionalTypeCodeField;
            }
            set
            {
                this.additionalTypeCodeField = value;
            }
        }
    }
}