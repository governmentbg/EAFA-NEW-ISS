namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SampledObjectCargo", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SampledObjectCargoType
    {

        private CodeType storageTypeCodeField;

        private TextType nameField;

        public CodeType StorageTypeCode
        {
            get
            {
                return this.storageTypeCodeField;
            }
            set
            {
                this.storageTypeCodeField = value;
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
    }
}