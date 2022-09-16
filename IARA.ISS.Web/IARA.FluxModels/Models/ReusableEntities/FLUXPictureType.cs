using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
[Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class FLUXPictureType
    {
        public IDType ID { get; set; }

        public CodeType TypeCode { get; set; }

        public DateTimeType TakenDateTime { get; set; }

        public IDType AreaIncludedID { get; set; }

        public TextType Description { get; set; }

        public BinaryObjectType DigitalImageBinaryObject { get; set; }
    }
}
