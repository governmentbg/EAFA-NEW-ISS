using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class SalesEventType
    {
        public DateTimeType OccurrenceDateTime { get; set; }

        public TextType SellerName { get; set; }

        public TextType BuyerName { get; set; }

        [XmlElement("RelatedSalesBatch")]
        public SalesBatchType[] RelatedSalesBatch { get; set; }
    }
}
