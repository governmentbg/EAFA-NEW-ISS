namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ISReport", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ISReportType
    {

        private CodeType typeCodeField;

        private CodeType[] contextTypeCodeField;

        private DateTimeType referenceDateTimeField;

        private IDType[] idField;

        private FLUXReportDocumentType includedFLUXReportDocumentField;

        private ISEventType specifiedISEventField;

        private VesselTransportMeansType[] specifiedVesselTransportMeansField;

        private VehicleTransportMeansType[] specifiedVehicleTransportMeansField;

        private ContactPartyType[] specifiedContactPartyField;

        private ISRItemCharacteristicType[] relatedISRItemCharacteristicField;

        private ISReportType[] dependentISReportField;

        private FishingActivityType[] specifiedFishingActivityField;

        private FLUXLocationType[] specifiedFLUXLocationField;

        private FACatchType[] relatedFACatchField;

        private SurveillanceMeansType[] specifiedSurveillanceMeansField;

        private ISRInfringementSuspicionType[] specifiedISRInfringementSuspicionField;

        private InspectedVesselStatusType[] specifiedInspectedVesselStatusField;

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

        [XmlElement("ContextTypeCode")]
        public CodeType[] ContextTypeCode
        {
            get
            {
                return this.contextTypeCodeField;
            }
            set
            {
                this.contextTypeCodeField = value;
            }
        }

        public DateTimeType ReferenceDateTime
        {
            get
            {
                return this.referenceDateTimeField;
            }
            set
            {
                this.referenceDateTimeField = value;
            }
        }

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

        public FLUXReportDocumentType IncludedFLUXReportDocument
        {
            get
            {
                return this.includedFLUXReportDocumentField;
            }
            set
            {
                this.includedFLUXReportDocumentField = value;
            }
        }

        public ISEventType SpecifiedISEvent
        {
            get
            {
                return this.specifiedISEventField;
            }
            set
            {
                this.specifiedISEventField = value;
            }
        }

        [XmlElement("SpecifiedVesselTransportMeans")]
        public VesselTransportMeansType[] SpecifiedVesselTransportMeans
        {
            get
            {
                return this.specifiedVesselTransportMeansField;
            }
            set
            {
                this.specifiedVesselTransportMeansField = value;
            }
        }

        [XmlElement("SpecifiedVehicleTransportMeans")]
        public VehicleTransportMeansType[] SpecifiedVehicleTransportMeans
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

        [XmlElement("SpecifiedContactParty")]
        public ContactPartyType[] SpecifiedContactParty
        {
            get
            {
                return this.specifiedContactPartyField;
            }
            set
            {
                this.specifiedContactPartyField = value;
            }
        }

        [XmlElement("RelatedISRItemCharacteristic")]
        public ISRItemCharacteristicType[] RelatedISRItemCharacteristic
        {
            get
            {
                return this.relatedISRItemCharacteristicField;
            }
            set
            {
                this.relatedISRItemCharacteristicField = value;
            }
        }

        [XmlElement("DependentISReport")]
        public ISReportType[] DependentISReport
        {
            get
            {
                return this.dependentISReportField;
            }
            set
            {
                this.dependentISReportField = value;
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

        [XmlElement("RelatedFACatch")]
        public FACatchType[] RelatedFACatch
        {
            get
            {
                return this.relatedFACatchField;
            }
            set
            {
                this.relatedFACatchField = value;
            }
        }

        [XmlElement("SpecifiedSurveillanceMeans")]
        public SurveillanceMeansType[] SpecifiedSurveillanceMeans
        {
            get
            {
                return this.specifiedSurveillanceMeansField;
            }
            set
            {
                this.specifiedSurveillanceMeansField = value;
            }
        }

        [XmlElement("SpecifiedISRInfringementSuspicion")]
        public ISRInfringementSuspicionType[] SpecifiedISRInfringementSuspicion
        {
            get
            {
                return this.specifiedISRInfringementSuspicionField;
            }
            set
            {
                this.specifiedISRInfringementSuspicionField = value;
            }
        }

        [XmlElement("SpecifiedInspectedVesselStatus")]
        public InspectedVesselStatusType[] SpecifiedInspectedVesselStatus
        {
            get
            {
                return this.specifiedInspectedVesselStatusField;
            }
            set
            {
                this.specifiedInspectedVesselStatusField = value;
            }
        }
    }
}