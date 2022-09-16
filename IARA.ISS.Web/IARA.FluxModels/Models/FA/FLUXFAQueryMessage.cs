using System;
using System.Xml.Serialization;


namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXFAQueryMessage:3")]
    [XmlRoot("FLUXFAQueryMessage", Namespace = "urn:un:unece:uncefact:data:standard:FLUXFAQueryMessage:3", IsNullable = false)]
    public partial class FLUXFAQueryMessageType
    {
        public FAQueryType FAQuery { get; set; }
    }



}
