namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TTProductBatch", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TTProductBatchType
    {

        private IDType idField;

        private DateTimeType creationDateTimeField;

        private DateTimeType breakUpDateTimeField;

        private DateTimeType saleDateTimeField;

        private IDType salesNoteIDField;

        private IDType[] fLUXIDField;

        private MeasureType minimumSizeMeasureField;

        private MeasureType maximumSizeMeasureField;

        private QuantityType unitQuantityField;

        private MeasureType weightMeasureField;

        private DelimitedPeriodType specifiedDelimitedPeriodField;

        private TTBatchHistoryRecordType[] aggregatedSpecifiedTTBatchHistoryRecordField;

        private TTBatchHistoryRecordType[] specifiedTTBatchHistoryRecordField;

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

        public DateTimeType CreationDateTime
        {
            get
            {
                return this.creationDateTimeField;
            }
            set
            {
                this.creationDateTimeField = value;
            }
        }

        public DateTimeType BreakUpDateTime
        {
            get
            {
                return this.breakUpDateTimeField;
            }
            set
            {
                this.breakUpDateTimeField = value;
            }
        }

        public DateTimeType SaleDateTime
        {
            get
            {
                return this.saleDateTimeField;
            }
            set
            {
                this.saleDateTimeField = value;
            }
        }

        public IDType SalesNoteID
        {
            get
            {
                return this.salesNoteIDField;
            }
            set
            {
                this.salesNoteIDField = value;
            }
        }

        [XmlElement("FLUXID")]
        public IDType[] FLUXID
        {
            get
            {
                return this.fLUXIDField;
            }
            set
            {
                this.fLUXIDField = value;
            }
        }

        public MeasureType MinimumSizeMeasure
        {
            get
            {
                return this.minimumSizeMeasureField;
            }
            set
            {
                this.minimumSizeMeasureField = value;
            }
        }

        public MeasureType MaximumSizeMeasure
        {
            get
            {
                return this.maximumSizeMeasureField;
            }
            set
            {
                this.maximumSizeMeasureField = value;
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

        public MeasureType WeightMeasure
        {
            get
            {
                return this.weightMeasureField;
            }
            set
            {
                this.weightMeasureField = value;
            }
        }

        public DelimitedPeriodType SpecifiedDelimitedPeriod
        {
            get
            {
                return this.specifiedDelimitedPeriodField;
            }
            set
            {
                this.specifiedDelimitedPeriodField = value;
            }
        }

        [XmlElement("AggregatedSpecifiedTTBatchHistoryRecord")]
        public TTBatchHistoryRecordType[] AggregatedSpecifiedTTBatchHistoryRecord
        {
            get
            {
                return this.aggregatedSpecifiedTTBatchHistoryRecordField;
            }
            set
            {
                this.aggregatedSpecifiedTTBatchHistoryRecordField = value;
            }
        }

        [XmlElement("SpecifiedTTBatchHistoryRecord")]
        public TTBatchHistoryRecordType[] SpecifiedTTBatchHistoryRecord
        {
            get
            {
                return this.specifiedTTBatchHistoryRecordField;
            }
            set
            {
                this.specifiedTTBatchHistoryRecordField = value;
            }
        }
    }
}