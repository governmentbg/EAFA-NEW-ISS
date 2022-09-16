using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:Standard:QualifiedDataType:20")]
    public enum CommunicationChannelCodeListAgencyIDContentType
    {
        [XmlEnum("6")]
        Item6,
    }
}
