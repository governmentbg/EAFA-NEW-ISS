using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class DataSetVersionType
    {
        public IDType ID { get; set; }

        public TextType Name { get; set; }

        public DateTimeType ValidityStartDateTime { get; set; }

        public DateTimeType ValidityEndDateTime { get; set; }
    }
}
