using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class AuthorizationStatusType
    {
        public CodeType ConditionCode { get; set; }


        public DateTimeType ChangedDateTime { get; set; }


        [XmlElement("Description")]
        public TextType[] Description { get; set; }
    }
}
