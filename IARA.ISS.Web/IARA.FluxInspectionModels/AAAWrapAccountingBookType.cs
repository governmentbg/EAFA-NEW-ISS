namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAWrapAccountingBook", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAWrapAccountingBookType
    {

        private IDType idField;

        private IndicatorType testConditionIndicatorField;

        private AAAWrapProcessedEntityType[] specifiedAAAWrapProcessedEntityField;

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        public IndicatorType TestConditionIndicator
        {
            get
            {
                return this.testConditionIndicatorField;
            }
            set
            {
                this.testConditionIndicatorField = value;
            }
        }

        [XmlElement("SpecifiedAAAWrapProcessedEntity")]
        public AAAWrapProcessedEntityType[] SpecifiedAAAWrapProcessedEntity
        {
            get
            {
                return this.specifiedAAAWrapProcessedEntityField;
            }
            set
            {
                this.specifiedAAAWrapProcessedEntityField = value;
            }
        }
    }
}