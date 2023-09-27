namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpeciesAdditionalTTProduction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpeciesAdditionalTTProductionType
    {

        private CodeType processStepCodeField;

        private CodeType productTypeCodeField;

        public CodeType ProcessStepCode
        {
            get
            {
                return this.processStepCodeField;
            }
            set
            {
                this.processStepCodeField = value;
            }
        }

        public CodeType ProductTypeCode
        {
            get
            {
                return this.productTypeCodeField;
            }
            set
            {
                this.productTypeCodeField = value;
            }
        }
    }
}