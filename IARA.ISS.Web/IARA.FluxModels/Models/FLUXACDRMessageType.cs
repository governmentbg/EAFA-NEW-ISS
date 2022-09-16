using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXACDRMessage:4")]
    [XmlRoot("FLUXACDRMessage", Namespace = "urn:un:unece:uncefact:data:standard:FLUXACDRMessage:4", IsNullable = false)]
    public partial class FLUXACDRMessageType
    {
        public FLUXReportDocumentType FLUXReportDocument { get; set; }

        public AggregatedCatchReportDocumentType AggregatedCatchReportDocument { get; set; }

        [XmlElement("FLUXACDRReport")]
        public List<FLUXACDRReportType> FLUXACDRReport { get; set; }
    }
}
