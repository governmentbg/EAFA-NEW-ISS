namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgronomicalObservationScope", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgronomicalObservationScopeType
    {

        private CodeType typeCodeField;

        private CodeType subtypeCodeField;

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

        public CodeType SubtypeCode
        {
            get
            {
                return this.subtypeCodeField;
            }
            set
            {
                this.subtypeCodeField = value;
            }
        }
    }
}