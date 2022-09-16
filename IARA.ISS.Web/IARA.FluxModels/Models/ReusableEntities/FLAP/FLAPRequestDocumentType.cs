using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class FLAPRequestDocumentType
    {

        public IDType ID { get; set; }


        public CodeType TypeCode { get; set; }


        public CodeType FADATypeCode { get; set; }


        public CodeType PurposeCode { get; set; }


        public TextType Purpose { get; set; }


        public FishingCategoryType RelatedFishingCategory { get; set; }


        [XmlElement("RelatedValidationResultDocument")]
        public ValidationResultDocumentType[] RelatedValidationResultDocument { get; set; }
    }
}
