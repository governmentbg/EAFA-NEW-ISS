using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class SpecifiedPreferenceType
    {
        public NumericType PriorityRankingNumeric { get; set; }

        public IndicatorType PreferredIndicator { get; set; }

        public SpecifiedPeriodType UnavailableSpecifiedPeriod { get; set; }

        public SpecifiedPeriodType AvailableSpecifiedPeriod { get; set; }
    }
}
