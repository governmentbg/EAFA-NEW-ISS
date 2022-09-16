using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class FLUXBinaryFileType
    {
        public TextType Title { get; set; }


        public BinaryObjectType IncludedBinaryObject { get; set; }


        public TextType Description { get; set; }


        public MeasureType SizeMeasure { get; set; }


        public CodeType TypeCode { get; set; }


        public TextType Name { get; set; }
    }
}
