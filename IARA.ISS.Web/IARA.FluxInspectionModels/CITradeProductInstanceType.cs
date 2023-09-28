namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CITradeProductInstance", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CITradeProductInstanceType
    {

        private IDType globalSerialIDField;

        private IDType batchIDField;

        private IDType kanbanIDField;

        private IDType supplierAssignedSerialIDField;

        private DateTimeType bestBeforeDateTimeField;

        private DateTimeType expiryDateTimeField;

        private DateTimeType sellByDateTimeField;

        private IDType[] serialIDField;

        private IDType[] registrationIDField;

        private CIProductCharacteristicType[] productCIProductCharacteristicField;

        private CISupplyChainEventType productionCISupplyChainEventField;

        private CISupplyChainEventType packagingCISupplyChainEventField;

        private CIMaterialGoodsCharacteristicType[] applicableCIMaterialGoodsCharacteristicField;

        public IDType GlobalSerialID
        {
            get
            {
                return this.globalSerialIDField;
            }
            set
            {
                this.globalSerialIDField = value;
            }
        }

        public IDType BatchID
        {
            get
            {
                return this.batchIDField;
            }
            set
            {
                this.batchIDField = value;
            }
        }

        public IDType KanbanID
        {
            get
            {
                return this.kanbanIDField;
            }
            set
            {
                this.kanbanIDField = value;
            }
        }

        public IDType SupplierAssignedSerialID
        {
            get
            {
                return this.supplierAssignedSerialIDField;
            }
            set
            {
                this.supplierAssignedSerialIDField = value;
            }
        }

        public DateTimeType BestBeforeDateTime
        {
            get
            {
                return this.bestBeforeDateTimeField;
            }
            set
            {
                this.bestBeforeDateTimeField = value;
            }
        }

        public DateTimeType ExpiryDateTime
        {
            get
            {
                return this.expiryDateTimeField;
            }
            set
            {
                this.expiryDateTimeField = value;
            }
        }

        public DateTimeType SellByDateTime
        {
            get
            {
                return this.sellByDateTimeField;
            }
            set
            {
                this.sellByDateTimeField = value;
            }
        }

        [XmlElement("SerialID")]
        public IDType[] SerialID
        {
            get
            {
                return this.serialIDField;
            }
            set
            {
                this.serialIDField = value;
            }
        }

        [XmlElement("RegistrationID")]
        public IDType[] RegistrationID
        {
            get
            {
                return this.registrationIDField;
            }
            set
            {
                this.registrationIDField = value;
            }
        }

        [XmlElement("ProductCIProductCharacteristic")]
        public CIProductCharacteristicType[] ProductCIProductCharacteristic
        {
            get
            {
                return this.productCIProductCharacteristicField;
            }
            set
            {
                this.productCIProductCharacteristicField = value;
            }
        }

        public CISupplyChainEventType ProductionCISupplyChainEvent
        {
            get
            {
                return this.productionCISupplyChainEventField;
            }
            set
            {
                this.productionCISupplyChainEventField = value;
            }
        }

        public CISupplyChainEventType PackagingCISupplyChainEvent
        {
            get
            {
                return this.packagingCISupplyChainEventField;
            }
            set
            {
                this.packagingCISupplyChainEventField = value;
            }
        }

        [XmlElement("ApplicableCIMaterialGoodsCharacteristic")]
        public CIMaterialGoodsCharacteristicType[] ApplicableCIMaterialGoodsCharacteristic
        {
            get
            {
                return this.applicableCIMaterialGoodsCharacteristicField;
            }
            set
            {
                this.applicableCIMaterialGoodsCharacteristicField = value;
            }
        }
    }
}