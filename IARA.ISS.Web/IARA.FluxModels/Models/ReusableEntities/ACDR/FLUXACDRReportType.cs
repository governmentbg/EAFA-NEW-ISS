using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:18")]
    public partial class FLUXACDRReportType
    {
        public CodeType RegionalAreaCode { get; set; }


        public CodeType RegionalSpeciesCode { get; set; }


        public CodeType FishingCategoryCode { get; set; }


        public CodeType FAOFishingGearCode { get; set; }


        public CodeType TypeCode { get; set; }


        public VesselTransportMeansType SpecifiedVesselTransportMeans { get; set; }


        [XmlElement("SpecifiedACDRReportedArea")]
        public List<ACDRReportedAreaType> SpecifiedACDRReportedArea { get; set; }
    }
}
