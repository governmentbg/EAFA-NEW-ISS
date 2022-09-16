using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class ValidationResultDocumentType
    {
        public IDType ValidatorID { get; set; }

        public DateTimeType CreationDateTime { get; set; }

        [XmlElement("RelatedValidationQualityAnalysis")]
        public ValidationQualityAnalysisType[] RelatedValidationQualityAnalysis { get; set; }
    }
}
