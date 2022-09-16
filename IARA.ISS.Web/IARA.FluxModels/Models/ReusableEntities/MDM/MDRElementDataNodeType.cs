using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class MDRElementDataNodeType
    {
        public TextType Name { get; set; }


        public TextType Value { get; set; }


        public CodeType TypeCode { get; set; }


        public FLUXBinaryFileType ValueFLUXBinaryFile { get; set; }
    }
}
