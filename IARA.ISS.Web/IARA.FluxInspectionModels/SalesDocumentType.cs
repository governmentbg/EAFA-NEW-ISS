

namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SalesDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SalesDocumentType
    {

        private IDType[] idField;

        private CodeType currencyCodeField;

        private IDType[] transportDocumentIDField;

        private IDType[] salesNoteIDField;

        private IDType[] takeoverDocumentIDField;

        private SalesBatchType[] specifiedSalesBatchField;

        private SalesEventType[] specifiedSalesEventField;

        private FishingActivityType[] specifiedFishingActivityField;

        private FLUXLocationType[] specifiedFLUXLocationField;

        private SalesPartyType[] specifiedSalesPartyField;

        private VehicleTransportMeansType specifiedVehicleTransportMeansField;

        private ValidationResultDocumentType[] relatedValidationResultDocumentField;

        private AmountType[] totalSalesPriceField;

        private FLUXLocationType departureSpecifiedFLUXLocationField;

        private FLUXLocationType arrivalSpecifiedFLUXLocationField;

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public CodeType CurrencyCode
        {
            get => this.currencyCodeField;
            set => this.currencyCodeField = value;
        }

        [XmlElement("TransportDocumentID")]
        public IDType[] TransportDocumentID
        {
            get => this.transportDocumentIDField;
            set => this.transportDocumentIDField = value;
        }

        [XmlElement("SalesNoteID")]
        public IDType[] SalesNoteID
        {
            get => this.salesNoteIDField;
            set => this.salesNoteIDField = value;
        }

        [XmlElement("TakeoverDocumentID")]
        public IDType[] TakeoverDocumentID
        {
            get => this.takeoverDocumentIDField;
            set => this.takeoverDocumentIDField = value;
        }

        [XmlElement("SpecifiedSalesBatch")]
        public SalesBatchType[] SpecifiedSalesBatch
        {
            get => this.specifiedSalesBatchField;
            set => this.specifiedSalesBatchField = value;
        }

        [XmlElement("SpecifiedSalesEvent")]
        public SalesEventType[] SpecifiedSalesEvent
        {
            get => this.specifiedSalesEventField;
            set => this.specifiedSalesEventField = value;
        }

        [XmlElement("SpecifiedFishingActivity")]
        public FishingActivityType[] SpecifiedFishingActivity
        {
            get => this.specifiedFishingActivityField;
            set => this.specifiedFishingActivityField = value;
        }

        [XmlElement("SpecifiedFLUXLocation")]
        public FLUXLocationType[] SpecifiedFLUXLocation
        {
            get => this.specifiedFLUXLocationField;
            set => this.specifiedFLUXLocationField = value;
        }

        [XmlElement("SpecifiedSalesParty")]
        public SalesPartyType[] SpecifiedSalesParty
        {
            get => this.specifiedSalesPartyField;
            set => this.specifiedSalesPartyField = value;
        }

        public VehicleTransportMeansType SpecifiedVehicleTransportMeans
        {
            get => this.specifiedVehicleTransportMeansField;
            set => this.specifiedVehicleTransportMeansField = value;
        }

        [XmlElement("RelatedValidationResultDocument")]
        public ValidationResultDocumentType[] RelatedValidationResultDocument
        {
            get => this.relatedValidationResultDocumentField;
            set => this.relatedValidationResultDocumentField = value;
        }

        [XmlArrayItem("ChargeAmount", IsNullable = false)]
        public AmountType[] TotalSalesPrice
        {
            get => this.totalSalesPriceField;
            set => this.totalSalesPriceField = value;
        }

        public FLUXLocationType DepartureSpecifiedFLUXLocation
        {
            get => this.departureSpecifiedFLUXLocationField;
            set => this.departureSpecifiedFLUXLocationField = value;
        }

        public FLUXLocationType ArrivalSpecifiedFLUXLocation
        {
            get => this.arrivalSpecifiedFLUXLocationField;
            set => this.arrivalSpecifiedFLUXLocationField = value;
        }
    }
}
