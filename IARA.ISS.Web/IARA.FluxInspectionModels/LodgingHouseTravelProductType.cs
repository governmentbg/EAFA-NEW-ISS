namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LodgingHouseTravelProduct", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LodgingHouseTravelProductType
    {

        private CodeType typeCodeField;

        private CodeType categoryCodeField;

        private IDType idField;

        private TextType brandNameField;

        private TextType nameField;

        private TextType supplierNameField;

        private IDType supplierIDField;

        private TextType salesManagerNameField;

        private IDType travelLicenceIDField;

        private IndicatorType reservationGuaranteeRequiredIndicatorField;

        private TextType reservationGuaranteeField;

        private TextType indemnityClauseField;

        private TextType cancellationPolicyField;

        private TextType salesOfficeInformationField;

        private TextType descriptionField;

        private TravelProductFeatureType[] definedTravelProductFeatureField;

        private TravelProductPeriodType[] applicableTravelProductPeriodField;

        private LodgingHouseTravelProductComponentType[] includedLodgingHouseTravelProductComponentField;

        private ProductAvailabilityType[] applicableProductAvailabilityField;

        private BasicPriceType[] applicableBasicPriceField;

        private ExtraPriceType[] applicableExtraPriceField;

        private DiscountPriceType[] applicableDiscountPriceField;

        private ServicePriceType[] applicableServicePriceField;

        private TravelProductTaxType[] applicableTravelProductTaxField;

        private CancellationPriceType[] applicableCancellationPriceField;

        private TravelProductReservationItemType heldTravelProductReservationItemField;

        private TravelProductPeriodType specifiedTravelProductPeriodField;

        private LodgingHouseTravelProductComponentType[] specifiedLodgingHouseTravelProductComponentField;

        private DailyPriceType[] chargedDailyPriceField;

        private LodgingHouseCustomerClassType[] applicableLodgingHouseCustomerClassField;

        private LodgingHouseReservationRestrictionType[] specifiedLodgingHouseReservationRestrictionField;

        private TravelProductIntermediarySaleType[] specifiedTravelProductIntermediarySaleField;

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public CodeType CategoryCode
        {
            get => this.categoryCodeField;
            set => this.categoryCodeField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType BrandName
        {
            get => this.brandNameField;
            set => this.brandNameField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public TextType SupplierName
        {
            get => this.supplierNameField;
            set => this.supplierNameField = value;
        }

        public IDType SupplierID
        {
            get => this.supplierIDField;
            set => this.supplierIDField = value;
        }

        public TextType SalesManagerName
        {
            get => this.salesManagerNameField;
            set => this.salesManagerNameField = value;
        }

        public IDType TravelLicenceID
        {
            get => this.travelLicenceIDField;
            set => this.travelLicenceIDField = value;
        }

        public IndicatorType ReservationGuaranteeRequiredIndicator
        {
            get => this.reservationGuaranteeRequiredIndicatorField;
            set => this.reservationGuaranteeRequiredIndicatorField = value;
        }

        public TextType ReservationGuarantee
        {
            get => this.reservationGuaranteeField;
            set => this.reservationGuaranteeField = value;
        }

        public TextType IndemnityClause
        {
            get => this.indemnityClauseField;
            set => this.indemnityClauseField = value;
        }

        public TextType CancellationPolicy
        {
            get => this.cancellationPolicyField;
            set => this.cancellationPolicyField = value;
        }

        public TextType SalesOfficeInformation
        {
            get => this.salesOfficeInformationField;
            set => this.salesOfficeInformationField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        [XmlElement("DefinedTravelProductFeature")]
        public TravelProductFeatureType[] DefinedTravelProductFeature
        {
            get => this.definedTravelProductFeatureField;
            set => this.definedTravelProductFeatureField = value;
        }

        [XmlElement("ApplicableTravelProductPeriod")]
        public TravelProductPeriodType[] ApplicableTravelProductPeriod
        {
            get => this.applicableTravelProductPeriodField;
            set => this.applicableTravelProductPeriodField = value;
        }

        [XmlElement("IncludedLodgingHouseTravelProductComponent")]
        public LodgingHouseTravelProductComponentType[] IncludedLodgingHouseTravelProductComponent
        {
            get => this.includedLodgingHouseTravelProductComponentField;
            set => this.includedLodgingHouseTravelProductComponentField = value;
        }

        [XmlElement("ApplicableProductAvailability")]
        public ProductAvailabilityType[] ApplicableProductAvailability
        {
            get => this.applicableProductAvailabilityField;
            set => this.applicableProductAvailabilityField = value;
        }

        [XmlElement("ApplicableBasicPrice")]
        public BasicPriceType[] ApplicableBasicPrice
        {
            get => this.applicableBasicPriceField;
            set => this.applicableBasicPriceField = value;
        }

        [XmlElement("ApplicableExtraPrice")]
        public ExtraPriceType[] ApplicableExtraPrice
        {
            get => this.applicableExtraPriceField;
            set => this.applicableExtraPriceField = value;
        }

        [XmlElement("ApplicableDiscountPrice")]
        public DiscountPriceType[] ApplicableDiscountPrice
        {
            get => this.applicableDiscountPriceField;
            set => this.applicableDiscountPriceField = value;
        }

        [XmlElement("ApplicableServicePrice")]
        public ServicePriceType[] ApplicableServicePrice
        {
            get => this.applicableServicePriceField;
            set => this.applicableServicePriceField = value;
        }

        [XmlElement("ApplicableTravelProductTax")]
        public TravelProductTaxType[] ApplicableTravelProductTax
        {
            get => this.applicableTravelProductTaxField;
            set => this.applicableTravelProductTaxField = value;
        }

        [XmlElement("ApplicableCancellationPrice")]
        public CancellationPriceType[] ApplicableCancellationPrice
        {
            get => this.applicableCancellationPriceField;
            set => this.applicableCancellationPriceField = value;
        }

        public TravelProductReservationItemType HeldTravelProductReservationItem
        {
            get => this.heldTravelProductReservationItemField;
            set => this.heldTravelProductReservationItemField = value;
        }

        public TravelProductPeriodType SpecifiedTravelProductPeriod
        {
            get => this.specifiedTravelProductPeriodField;
            set => this.specifiedTravelProductPeriodField = value;
        }

        [XmlElement("SpecifiedLodgingHouseTravelProductComponent")]
        public LodgingHouseTravelProductComponentType[] SpecifiedLodgingHouseTravelProductComponent
        {
            get => this.specifiedLodgingHouseTravelProductComponentField;
            set => this.specifiedLodgingHouseTravelProductComponentField = value;
        }

        [XmlElement("ChargedDailyPrice")]
        public DailyPriceType[] ChargedDailyPrice
        {
            get => this.chargedDailyPriceField;
            set => this.chargedDailyPriceField = value;
        }

        [XmlElement("ApplicableLodgingHouseCustomerClass")]
        public LodgingHouseCustomerClassType[] ApplicableLodgingHouseCustomerClass
        {
            get => this.applicableLodgingHouseCustomerClassField;
            set => this.applicableLodgingHouseCustomerClassField = value;
        }

        [XmlElement("SpecifiedLodgingHouseReservationRestriction")]
        public LodgingHouseReservationRestrictionType[] SpecifiedLodgingHouseReservationRestriction
        {
            get => this.specifiedLodgingHouseReservationRestrictionField;
            set => this.specifiedLodgingHouseReservationRestrictionField = value;
        }

        [XmlElement("SpecifiedTravelProductIntermediarySale")]
        public TravelProductIntermediarySaleType[] SpecifiedTravelProductIntermediarySale
        {
            get => this.specifiedTravelProductIntermediarySaleField;
            set => this.specifiedTravelProductIntermediarySaleField = value;
        }
    }
}