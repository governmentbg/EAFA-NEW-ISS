namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSCargo", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSCargoType
    {

        private CargoTypeClassificationCodeType typeCodeField;

        public CargoTypeClassificationCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }
    }
}