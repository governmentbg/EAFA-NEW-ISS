namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalManMachineSystem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalManMachineSystemType
    {

        private CropProductionAgriculturalProcessType[] specifiedCropProductionAgriculturalProcessField;

        [XmlElement("SpecifiedCropProductionAgriculturalProcess")]
        public CropProductionAgriculturalProcessType[] SpecifiedCropProductionAgriculturalProcess
        {
            get
            {
                return this.specifiedCropProductionAgriculturalProcessField;
            }
            set
            {
                this.specifiedCropProductionAgriculturalProcessField = value;
            }
        }
    }
}