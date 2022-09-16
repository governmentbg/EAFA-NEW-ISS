using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXSalesQueryMessage:3")]
    [XmlRoot("FLUXSalesQueryMessage", Namespace = "urn:un:unece:uncefact:data:standard:FLUXSalesQueryMessage:3", IsNullable = false)]
    public partial class FLUXSalesQueryMessageType
    {
        public SalesQueryType SalesQuery { get; set; }
    }

}
