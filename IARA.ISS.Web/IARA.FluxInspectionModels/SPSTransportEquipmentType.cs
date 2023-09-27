namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSTransportEquipment", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSTransportEquipmentType
    {

        private IDType idField;

        private SPSSealType[] affixedSPSSealField;

        private SPSTemperatureType[] settingSPSTemperatureField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("AffixedSPSSeal")]
        public SPSSealType[] AffixedSPSSeal
        {
            get => this.affixedSPSSealField;
            set => this.affixedSPSSealField = value;
        }

        [XmlElement("SettingSPSTemperature")]
        public SPSTemperatureType[] SettingSPSTemperature
        {
            get => this.settingSPSTemperatureField;
            set => this.settingSPSTemperatureField = value;
        }
    }
}