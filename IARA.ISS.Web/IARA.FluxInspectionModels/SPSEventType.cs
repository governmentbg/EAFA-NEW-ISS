namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSEventType
    {

        private SPSLocationType occurrenceSPSLocationField;

        public SPSLocationType OccurrenceSPSLocation
        {
            get => this.occurrenceSPSLocationField;
            set => this.occurrenceSPSLocationField = value;
        }
    }
}