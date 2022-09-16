using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class MDRQueryIdentityType
    {
        public IDType ID { get; set; }


        [XmlElement("ValidFromDateTime")]
        public DateTimeType[] ValidFromDateTime { get; set; }

        public IDType VersionID { get; set; }


        public DelimitedPeriodType ValidityDelimitedPeriod { get; set; }
    }
}
