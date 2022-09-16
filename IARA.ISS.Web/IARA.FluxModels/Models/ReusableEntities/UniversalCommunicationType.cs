using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class UniversalCommunicationType
    {
        public IDType URIID { get; set; }

        public CommunicationChannelCodeType ChannelCode { get; set; }

        public TextType LocalNumber { get; set; }

        public TextType CompleteNumber { get; set; }

        public CodeType CountryNumberCode { get; set; }

        public TextType ExtensionNumber { get; set; }

        public CodeType AreaNumberCode { get; set; }

        [XmlElement("Access")]
        public TextType[] Access { get; set; }

        public CodeType UseCode { get; set; }

        public IndicatorType HTMLPreferredIndicator { get; set; }

        public SpecifiedPreferenceType UsageSpecifiedPreference { get; set; }
    }
}
