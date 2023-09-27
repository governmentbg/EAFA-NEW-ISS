namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TTProduction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TTProductionType
    {

        private CodeType processStepCodeField;

        private CodeType productTypeCodeField;

        public CodeType ProcessStepCode
        {
            get => this.processStepCodeField;
            set => this.processStepCodeField = value;
        }

        public CodeType ProductTypeCode
        {
            get => this.productTypeCodeField;
            set => this.productTypeCodeField = value;
        }
    }
}