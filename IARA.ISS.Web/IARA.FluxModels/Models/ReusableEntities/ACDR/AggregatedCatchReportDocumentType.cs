using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:18")]
    public partial class AggregatedCatchReportDocumentType
    {
        public DelimitedPeriodType EffectiveDelimitedPeriod { get; set; }


        public FLUXPartyType OwnerFLUXParty { get; set; }
    }
}
