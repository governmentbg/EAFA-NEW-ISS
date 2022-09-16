using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class LogicalQueryParameterType
    {
        public TextType LogicalOperator { get; set; }

        public IndicatorType NegationIndicator { get; set; }

        public LogicalQueryParameterType SubordinateLogicalQueryParameter { get; set; }

        public PrimitiveQueryParameterType FirstSubordinatePrimitiveQueryParameter { get; set; }

        public PrimitiveQueryParameterType SecondSubordinatePrimitiveQueryParameter { get; set; }
    }
}
