namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CropInputChemical", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CropInputChemicalType
    {

        private CodeType typeCodeField;

        private PercentType presencePercentField;

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

        public PercentType PresencePercent
        {
            get
            {
                return this.presencePercentField;
            }
            set
            {
                this.presencePercentField = value;
            }
        }
    }
}