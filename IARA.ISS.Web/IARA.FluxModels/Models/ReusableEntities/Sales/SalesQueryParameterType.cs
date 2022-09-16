using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class SalesQueryParameterType
    {
        public CodeType TypeCode { get; set; }

        public CodeType ValueCode { get; set; }

        public DateTimeType ValueDateTime { get; set; }

        public IDType ValueID { get; set; }
    }
}
