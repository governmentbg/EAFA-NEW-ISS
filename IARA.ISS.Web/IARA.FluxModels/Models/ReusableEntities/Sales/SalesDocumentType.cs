using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
[Serializable]


    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20" +
        "")]
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
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }


        public CodeType CurrencyCode
        {
            get
            {
                return this.currencyCodeField;
            }
            set
            {
                this.currencyCodeField = value;
            }
        }


        [XmlElement("TransportDocumentID")]
        public IDType[] TransportDocumentID
        {
            get
            {
                return this.transportDocumentIDField;
            }
            set
            {
                this.transportDocumentIDField = value;
            }
        }


        [XmlElement("SalesNoteID")]
        public IDType[] SalesNoteID
        {
            get
            {
                return this.salesNoteIDField;
            }
            set
            {
                this.salesNoteIDField = value;
            }
        }


        [XmlElement("TakeoverDocumentID")]
        public IDType[] TakeoverDocumentID
        {
            get
            {
                return this.takeoverDocumentIDField;
            }
            set
            {
                this.takeoverDocumentIDField = value;
            }
        }


        [XmlElement("SpecifiedSalesBatch")]
        public SalesBatchType[] SpecifiedSalesBatch
        {
            get
            {
                return this.specifiedSalesBatchField;
            }
            set
            {
                this.specifiedSalesBatchField = value;
            }
        }


        [XmlElement("SpecifiedSalesEvent")]
        public SalesEventType[] SpecifiedSalesEvent
        {
            get
            {
                return this.specifiedSalesEventField;
            }
            set
            {
                this.specifiedSalesEventField = value;
            }
        }


        [XmlElement("SpecifiedFishingActivity")]
        public FishingActivityType[] SpecifiedFishingActivity
        {
            get
            {
                return this.specifiedFishingActivityField;
            }
            set
            {
                this.specifiedFishingActivityField = value;
            }
        }


        [XmlElement("SpecifiedFLUXLocation")]
        public FLUXLocationType[] SpecifiedFLUXLocation
        {
            get
            {
                return this.specifiedFLUXLocationField;
            }
            set
            {
                this.specifiedFLUXLocationField = value;
            }
        }


        [XmlElement("SpecifiedSalesParty")]
        public SalesPartyType[] SpecifiedSalesParty
        {
            get
            {
                return this.specifiedSalesPartyField;
            }
            set
            {
                this.specifiedSalesPartyField = value;
            }
        }


        public VehicleTransportMeansType SpecifiedVehicleTransportMeans
        {
            get
            {
                return this.specifiedVehicleTransportMeansField;
            }
            set
            {
                this.specifiedVehicleTransportMeansField = value;
            }
        }


        [XmlElement("RelatedValidationResultDocument")]
        public ValidationResultDocumentType[] RelatedValidationResultDocument
        {
            get
            {
                return this.relatedValidationResultDocumentField;
            }
            set
            {
                this.relatedValidationResultDocumentField = value;
            }
        }


        [XmlArrayItem("ChargeAmount", IsNullable = false)]
        public AmountType[] TotalSalesPrice
        {
            get
            {
                return this.totalSalesPriceField;
            }
            set
            {
                this.totalSalesPriceField = value;
            }
        }


        public FLUXLocationType DepartureSpecifiedFLUXLocation
        {
            get
            {
                return this.departureSpecifiedFLUXLocationField;
            }
            set
            {
                this.departureSpecifiedFLUXLocationField = value;
            }
        }


        public FLUXLocationType ArrivalSpecifiedFLUXLocation
        {
            get
            {
                return this.arrivalSpecifiedFLUXLocationField;
            }
            set
            {
                this.arrivalSpecifiedFLUXLocationField = value;
            }
        }
    }
}
