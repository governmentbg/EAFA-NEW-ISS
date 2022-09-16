using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:18")]
    public partial class FLAPIdentityType
    {
        public IDType ID { get; set; }


        public IDType RequestID { get; set; }


        public CodeType FADATypeCode { get; set; }


        public CodeType FCTypeCode { get; set; }


        [XmlElement("StatusCode")]
        public CodeType[] StatusCode { get; set; }
    }
}
