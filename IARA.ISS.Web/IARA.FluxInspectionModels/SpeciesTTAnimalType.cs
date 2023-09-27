namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpeciesTTAnimal", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpeciesTTAnimalType
    {

        private CodeType[] speciesTypeCodeField;

        private CodeType[] scientificSpeciesNameTypeCodeField;

        private CodeType[] regulationSpeciesNameTypeCodeField;

        private CodeType[] tradeSpeciesNameTypeCodeField;

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

        [XmlElement("ScientificSpeciesNameTypeCode")]
        public CodeType[] ScientificSpeciesNameTypeCode
        {
            get
            {
                return this.scientificSpeciesNameTypeCodeField;
            }
            set
            {
                this.scientificSpeciesNameTypeCodeField = value;
            }
        }

        [XmlElement("RegulationSpeciesNameTypeCode")]
        public CodeType[] RegulationSpeciesNameTypeCode
        {
            get
            {
                return this.regulationSpeciesNameTypeCodeField;
            }
            set
            {
                this.regulationSpeciesNameTypeCodeField = value;
            }
        }

        [XmlElement("TradeSpeciesNameTypeCode")]
        public CodeType[] TradeSpeciesNameTypeCode
        {
            get
            {
                return this.tradeSpeciesNameTypeCodeField;
            }
            set
            {
                this.tradeSpeciesNameTypeCodeField = value;
            }
        }
    }
}