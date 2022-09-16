using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class VesselEventType
    {
        public IDType VesselID { get; set; }


        public CodeType TypeCode { get; set; }


        [XmlElement("Description")]
        public TextType[] Description { get; set; }


        public DateTimeType OccurrenceDateTime { get; set; }


        [XmlElement("ID")]
        public IDType[] ID { get; set; }


        public DelimitedPeriodType DiscreteDelimitedPeriod { get; set; }


        public ValidationResultDocumentType RelatedValidationResultDocument { get; set; }

        [XmlElement("RelatedVesselHistoricalCharacteristic")]
        public VesselHistoricalCharacteristicType[] RelatedVesselHistoricalCharacteristic { get; set; }


        public VesselTransportMeansType RelatedVesselTransportMeans { get; set; }


        [XmlElement("RelatedValidationQualityAnalysis")]
        public ValidationQualityAnalysisType[] RelatedValidationQualityAnalysis { get; set; }
    }
}
