using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class SizeDistributionType
    {
        public CodeType CategoryCode { get; set; }


        [XmlElement("ClassCode")]
        public CodeType[] ClassCode { get; set; }
    }
}
