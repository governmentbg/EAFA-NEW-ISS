using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:18")]
    public partial class ACDRReportedAreaType
    {
        public CodeType FAOIdentificationCode { get; set; }

        public CodeType SovereigntyWaterCode { get; set; }

        public CodeType LandingCountryCode { get; set; }

        public CodeType CatchStatusCode { get; set; }

        [XmlElement("SpecifiedACDRCatch")]
        public List<ACDRCatchType> SpecifiedACDRCatch { get; set; }
    }
}
