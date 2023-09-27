namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSTransportMovement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSTransportMovementType
    {

        private IDType idField;

        private TransportModeCodeType modeCodeField;

        private SPSTransportMeansType usedSPSTransportMeansField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TransportModeCodeType ModeCode
        {
            get => this.modeCodeField;
            set => this.modeCodeField = value;
        }

        public SPSTransportMeansType UsedSPSTransportMeans
        {
            get => this.usedSPSTransportMeansField;
            set => this.usedSPSTransportMeansField = value;
        }
    }
}