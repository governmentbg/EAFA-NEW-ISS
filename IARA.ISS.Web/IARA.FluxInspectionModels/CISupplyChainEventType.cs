namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISupplyChainEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISupplyChainEventType
    {

        private IDType idField;

        private DateTimeType occurrenceDateTimeField;

        private CodeType typeCodeField;

        private TextType[] descriptionField;

        private BinaryObjectType[] descriptionBinaryObjectField;

        private QuantityType unitQuantityField;

        private DateTimeType latestOccurrenceDateTimeField;

        private DateTimeType earliestOccurrenceDateTimeField;

        private TimeOnlyFormattedDateTimeType timeOccurrenceDateTimeField;

        private DateTimeType dueDateTimeField;

        private CISpecifiedPeriodType occurrenceCISpecifiedPeriodField;

        private CILogisticsLocationType occurrenceCILogisticsLocationField;

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

        public DateTimeType OccurrenceDateTime
        {
            get
            {
                return this.occurrenceDateTimeField;
            }
            set
            {
                this.occurrenceDateTimeField = value;
            }
        }

        public CodeType TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        [XmlElement("DescriptionBinaryObject")]
        public BinaryObjectType[] DescriptionBinaryObject
        {
            get
            {
                return this.descriptionBinaryObjectField;
            }
            set
            {
                this.descriptionBinaryObjectField = value;
            }
        }

        public QuantityType UnitQuantity
        {
            get
            {
                return this.unitQuantityField;
            }
            set
            {
                this.unitQuantityField = value;
            }
        }

        public DateTimeType LatestOccurrenceDateTime
        {
            get
            {
                return this.latestOccurrenceDateTimeField;
            }
            set
            {
                this.latestOccurrenceDateTimeField = value;
            }
        }

        public DateTimeType EarliestOccurrenceDateTime
        {
            get
            {
                return this.earliestOccurrenceDateTimeField;
            }
            set
            {
                this.earliestOccurrenceDateTimeField = value;
            }
        }

        public TimeOnlyFormattedDateTimeType TimeOccurrenceDateTime
        {
            get
            {
                return this.timeOccurrenceDateTimeField;
            }
            set
            {
                this.timeOccurrenceDateTimeField = value;
            }
        }

        public DateTimeType DueDateTime
        {
            get
            {
                return this.dueDateTimeField;
            }
            set
            {
                this.dueDateTimeField = value;
            }
        }

        public CISpecifiedPeriodType OccurrenceCISpecifiedPeriod
        {
            get
            {
                return this.occurrenceCISpecifiedPeriodField;
            }
            set
            {
                this.occurrenceCISpecifiedPeriodField = value;
            }
        }

        public CILogisticsLocationType OccurrenceCILogisticsLocation
        {
            get
            {
                return this.occurrenceCILogisticsLocationField;
            }
            set
            {
                this.occurrenceCILogisticsLocationField = value;
            }
        }
    }
}