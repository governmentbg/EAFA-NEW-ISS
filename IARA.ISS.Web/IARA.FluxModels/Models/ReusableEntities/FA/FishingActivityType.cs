using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class FishingActivityType
    {
        [XmlElement("ID")]
        public IDType[] ID { get; set; }

        public CodeType TypeCode { get; set; }

        public DateTimeType OccurrenceDateTime { get; set; }

        public CodeType ReasonCode { get; set; }

        public CodeType VesselRelatedActivityCode { get; set; }

        public CodeType FisheryTypeCode { get; set; }

        public CodeType SpeciesTargetCode { get; set; }

        public QuantityType OperationsQuantity { get; set; }

        public MeasureType FishingDurationMeasure { get; set; }

        [XmlElement("SpecifiedFACatch")]
        public FACatchType[] SpecifiedFACatch { get; set; }

        [XmlElement("RelatedFLUXLocation")]
        public FLUXLocationType[] RelatedFLUXLocation { get; set; }

        [XmlElement("SpecifiedGearProblem")]
        public GearProblemType[] SpecifiedGearProblem { get; set; }

        [XmlElement("SpecifiedFLUXCharacteristic")]
        public FLUXCharacteristicType[] SpecifiedFLUXCharacteristic { get; set; }

        [XmlElement("SpecifiedFishingGear")]
        public FishingGearType[] SpecifiedFishingGear { get; set; }

        public VesselStorageCharacteristicType SourceVesselStorageCharacteristic { get; set; }

        public VesselStorageCharacteristicType DestinationVesselStorageCharacteristic { get; set; }

        [XmlElement("RelatedFishingActivity")]
        public FishingActivityType[] RelatedFishingActivity { get; set; }

        [XmlElement("SpecifiedFLAPDocument")]
        public FLAPDocumentType[] SpecifiedFLAPDocument { get; set; }

        [XmlElement("SpecifiedDelimitedPeriod")]
        public DelimitedPeriodType[] SpecifiedDelimitedPeriod { get; set; }

        public FishingTripType SpecifiedFishingTrip { get; set; }

        [XmlElement("RelatedVesselTransportMeans")]
        public VesselTransportMeansType[] RelatedVesselTransportMeans { get; set; }
    }
}
