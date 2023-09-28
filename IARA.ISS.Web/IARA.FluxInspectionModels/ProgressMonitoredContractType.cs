namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProgressMonitoredContract", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProgressMonitoredContractType
    {

        private AmountType fixedFeeCostAmountField;

        private AmountType nonFeeBearingCostAmountField;

        private IDType idField;

        private PercentType completionPercentField;

        private ContractTypeCodeType[] typeCodeField;

        private FundingTypeCodeType fundingTypeCodeField;

        private SecurityClassificationTypeCodeType securityTypeCodeField;

        private TextType nameField;

        private TextType descriptionField;

        private DateTimeType issueDateTimeField;

        private AmountType priceAmountField;

        private AmountType targetPriceAmountField;

        private AmountType estimatedTotalPriceAmountField;

        private AmountType initialFundingTargetPriceAmountField;

        private AmountType adjustedFundingTargetPriceAmountField;

        private AmountType ceilingPriceAmountField;

        private AmountType estimatedCeilingPriceAmountField;

        private IndicatorType extensionIndicatorField;

        private DateType startDateField;

        private DateType workStartDateField;

        private DateType definitizationStartDateField;

        private PercentType targetFeePercentField;

        private DateType expectedEndDateField;

        private DateType estimatedEndDateField;

        private DateType plannedEndDateField;

        private DateType lastItemDeliveryDateField;

        private DateType overTargetBaselineDateField;

        private AmountType originalNegotiatedCostAmountField;

        private AmountType targetFeeCostAmountField;

        private AmountType negotiatedCostAmountField;

        private AmountType authorizedUnpricedWorkEstimatedCostAmountField;

        private AmountType cumulativeNegotiatedChangesCostAmountField;

        private AmountType minimumAwardFeeCostAmountField;

        private AmountType maximumAwardFeeCostAmountField;

        private AmountType originalAwardFeeCostAmountField;

        private AmountType currentTargetCostAmountField;

        private AmountType totalAllocatedBudgetCostAmountField;

        private AmountType initialFundingCeilingCostAmountField;

        private AmountType adjustedFundingCeilingCostAmountField;

        private DateTimeType[] fiscalFundingYearDateTimeField;

        private TextType customerNameField;

        private IDType planIDField;

        private ProjectPeriodType[] validityProjectPeriodField;

        private ContractorPartyType[] identifiedContractorPartyField;

        private AppropriatingPartyType identifiedAppropriatingPartyField;

        private ProjectDocumentType[] referenceProjectDocumentField;

        private ProjectDocumentType fundsAuthorizationReferenceProjectDocumentField;

        private ProjectPeriodType appropriationEffectiveProjectPeriodField;

        private ProjectPeriodType[] effectiveProjectPeriodField;

        private AwardedContractChangeType[] modificationAwardedContractChangeField;

        private ProjectCostType[] associatedProjectCostField;

        private ProjectCostType[] bestCaseEstimateAtCompletionAssociatedProjectCostField;

        private ProjectCostType[] budgetBaselineAssociatedProjectCostField;

        private ProjectCostType[] mostLikelyEstimateAtCompletionAssociatedProjectCostField;

        private ProjectCostType[] worstCaseEstimateAtCompletionAssociatedProjectCostField;

        private SpecifiedProgrammeType principalSpecifiedProgrammeField;

        private ProgressMonitoredProjectPortfolioType[] containedProgressMonitoredProjectPortfolioField;

        private ContractPerformanceMeasurementType incentiveContractPerformanceMeasurementField;

        private ProgressMonitoredProjectType[] containedProgressMonitoredProjectField;

        private ProjectContractLineItemType[] specifiedProjectContractLineItemField;

        public AmountType FixedFeeCostAmount
        {
            get => this.fixedFeeCostAmountField;
            set => this.fixedFeeCostAmountField = value;
        }

        public AmountType NonFeeBearingCostAmount
        {
            get => this.nonFeeBearingCostAmountField;
            set => this.nonFeeBearingCostAmountField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public PercentType CompletionPercent
        {
            get => this.completionPercentField;
            set => this.completionPercentField = value;
        }

        [XmlElement("TypeCode")]
        public ContractTypeCodeType[] TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public FundingTypeCodeType FundingTypeCode
        {
            get => this.fundingTypeCodeField;
            set => this.fundingTypeCodeField = value;
        }

        public SecurityClassificationTypeCodeType SecurityTypeCode
        {
            get => this.securityTypeCodeField;
            set => this.securityTypeCodeField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public DateTimeType IssueDateTime
        {
            get => this.issueDateTimeField;
            set => this.issueDateTimeField = value;
        }

        public AmountType PriceAmount
        {
            get => this.priceAmountField;
            set => this.priceAmountField = value;
        }

        public AmountType TargetPriceAmount
        {
            get => this.targetPriceAmountField;
            set => this.targetPriceAmountField = value;
        }

        public AmountType EstimatedTotalPriceAmount
        {
            get => this.estimatedTotalPriceAmountField;
            set => this.estimatedTotalPriceAmountField = value;
        }

        public AmountType InitialFundingTargetPriceAmount
        {
            get => this.initialFundingTargetPriceAmountField;
            set => this.initialFundingTargetPriceAmountField = value;
        }

        public AmountType AdjustedFundingTargetPriceAmount
        {
            get => this.adjustedFundingTargetPriceAmountField;
            set => this.adjustedFundingTargetPriceAmountField = value;
        }

        public AmountType CeilingPriceAmount
        {
            get => this.ceilingPriceAmountField;
            set => this.ceilingPriceAmountField = value;
        }

        public AmountType EstimatedCeilingPriceAmount
        {
            get => this.estimatedCeilingPriceAmountField;
            set => this.estimatedCeilingPriceAmountField = value;
        }

        public IndicatorType ExtensionIndicator
        {
            get => this.extensionIndicatorField;
            set => this.extensionIndicatorField = value;
        }

        public DateType StartDate
        {
            get => this.startDateField;
            set => this.startDateField = value;
        }

        public DateType WorkStartDate
        {
            get => this.workStartDateField;
            set => this.workStartDateField = value;
        }

        public DateType DefinitizationStartDate
        {
            get => this.definitizationStartDateField;
            set => this.definitizationStartDateField = value;
        }

        public PercentType TargetFeePercent
        {
            get => this.targetFeePercentField;
            set => this.targetFeePercentField = value;
        }

        public DateType ExpectedEndDate
        {
            get => this.expectedEndDateField;
            set => this.expectedEndDateField = value;
        }

        public DateType EstimatedEndDate
        {
            get => this.estimatedEndDateField;
            set => this.estimatedEndDateField = value;
        }

        public DateType PlannedEndDate
        {
            get => this.plannedEndDateField;
            set => this.plannedEndDateField = value;
        }

        public DateType LastItemDeliveryDate
        {
            get => this.lastItemDeliveryDateField;
            set => this.lastItemDeliveryDateField = value;
        }

        public DateType OverTargetBaselineDate
        {
            get => this.overTargetBaselineDateField;
            set => this.overTargetBaselineDateField = value;
        }

        public AmountType OriginalNegotiatedCostAmount
        {
            get => this.originalNegotiatedCostAmountField;
            set => this.originalNegotiatedCostAmountField = value;
        }

        public AmountType TargetFeeCostAmount
        {
            get => this.targetFeeCostAmountField;
            set => this.targetFeeCostAmountField = value;
        }

        public AmountType NegotiatedCostAmount
        {
            get => this.negotiatedCostAmountField;
            set => this.negotiatedCostAmountField = value;
        }

        public AmountType AuthorizedUnpricedWorkEstimatedCostAmount
        {
            get => this.authorizedUnpricedWorkEstimatedCostAmountField;
            set => this.authorizedUnpricedWorkEstimatedCostAmountField = value;
        }

        public AmountType CumulativeNegotiatedChangesCostAmount
        {
            get => this.cumulativeNegotiatedChangesCostAmountField;
            set => this.cumulativeNegotiatedChangesCostAmountField = value;
        }

        public AmountType MinimumAwardFeeCostAmount
        {
            get => this.minimumAwardFeeCostAmountField;
            set => this.minimumAwardFeeCostAmountField = value;
        }

        public AmountType MaximumAwardFeeCostAmount
        {
            get => this.maximumAwardFeeCostAmountField;
            set => this.maximumAwardFeeCostAmountField = value;
        }

        public AmountType OriginalAwardFeeCostAmount
        {
            get => this.originalAwardFeeCostAmountField;
            set => this.originalAwardFeeCostAmountField = value;
        }

        public AmountType CurrentTargetCostAmount
        {
            get => this.currentTargetCostAmountField;
            set => this.currentTargetCostAmountField = value;
        }

        public AmountType TotalAllocatedBudgetCostAmount
        {
            get => this.totalAllocatedBudgetCostAmountField;
            set => this.totalAllocatedBudgetCostAmountField = value;
        }

        public AmountType InitialFundingCeilingCostAmount
        {
            get => this.initialFundingCeilingCostAmountField;
            set => this.initialFundingCeilingCostAmountField = value;
        }

        public AmountType AdjustedFundingCeilingCostAmount
        {
            get => this.adjustedFundingCeilingCostAmountField;
            set => this.adjustedFundingCeilingCostAmountField = value;
        }

        [XmlElement("FiscalFundingYearDateTime")]
        public DateTimeType[] FiscalFundingYearDateTime
        {
            get => this.fiscalFundingYearDateTimeField;
            set => this.fiscalFundingYearDateTimeField = value;
        }

        public TextType CustomerName
        {
            get => this.customerNameField;
            set => this.customerNameField = value;
        }

        public IDType PlanID
        {
            get => this.planIDField;
            set => this.planIDField = value;
        }

        [XmlElement("ValidityProjectPeriod")]
        public ProjectPeriodType[] ValidityProjectPeriod
        {
            get => this.validityProjectPeriodField;
            set => this.validityProjectPeriodField = value;
        }

        [XmlElement("IdentifiedContractorParty")]
        public ContractorPartyType[] IdentifiedContractorParty
        {
            get => this.identifiedContractorPartyField;
            set => this.identifiedContractorPartyField = value;
        }

        public AppropriatingPartyType IdentifiedAppropriatingParty
        {
            get => this.identifiedAppropriatingPartyField;
            set => this.identifiedAppropriatingPartyField = value;
        }

        [XmlElement("ReferenceProjectDocument")]
        public ProjectDocumentType[] ReferenceProjectDocument
        {
            get => this.referenceProjectDocumentField;
            set => this.referenceProjectDocumentField = value;
        }

        public ProjectDocumentType FundsAuthorizationReferenceProjectDocument
        {
            get => this.fundsAuthorizationReferenceProjectDocumentField;
            set => this.fundsAuthorizationReferenceProjectDocumentField = value;
        }

        public ProjectPeriodType AppropriationEffectiveProjectPeriod
        {
            get => this.appropriationEffectiveProjectPeriodField;
            set => this.appropriationEffectiveProjectPeriodField = value;
        }

        [XmlElement("EffectiveProjectPeriod")]
        public ProjectPeriodType[] EffectiveProjectPeriod
        {
            get => this.effectiveProjectPeriodField;
            set => this.effectiveProjectPeriodField = value;
        }

        [XmlElement("ModificationAwardedContractChange")]
        public AwardedContractChangeType[] ModificationAwardedContractChange
        {
            get => this.modificationAwardedContractChangeField;
            set => this.modificationAwardedContractChangeField = value;
        }

        [XmlElement("AssociatedProjectCost")]
        public ProjectCostType[] AssociatedProjectCost
        {
            get => this.associatedProjectCostField;
            set => this.associatedProjectCostField = value;
        }

        [XmlElement("BestCaseEstimateAtCompletionAssociatedProjectCost")]
        public ProjectCostType[] BestCaseEstimateAtCompletionAssociatedProjectCost
        {
            get => this.bestCaseEstimateAtCompletionAssociatedProjectCostField;
            set => this.bestCaseEstimateAtCompletionAssociatedProjectCostField = value;
        }

        [XmlElement("BudgetBaselineAssociatedProjectCost")]
        public ProjectCostType[] BudgetBaselineAssociatedProjectCost
        {
            get => this.budgetBaselineAssociatedProjectCostField;
            set => this.budgetBaselineAssociatedProjectCostField = value;
        }

        [XmlElement("MostLikelyEstimateAtCompletionAssociatedProjectCost")]
        public ProjectCostType[] MostLikelyEstimateAtCompletionAssociatedProjectCost
        {
            get => this.mostLikelyEstimateAtCompletionAssociatedProjectCostField;
            set => this.mostLikelyEstimateAtCompletionAssociatedProjectCostField = value;
        }

        [XmlElement("WorstCaseEstimateAtCompletionAssociatedProjectCost")]
        public ProjectCostType[] WorstCaseEstimateAtCompletionAssociatedProjectCost
        {
            get => this.worstCaseEstimateAtCompletionAssociatedProjectCostField;
            set => this.worstCaseEstimateAtCompletionAssociatedProjectCostField = value;
        }

        public SpecifiedProgrammeType PrincipalSpecifiedProgramme
        {
            get => this.principalSpecifiedProgrammeField;
            set => this.principalSpecifiedProgrammeField = value;
        }

        [XmlElement("ContainedProgressMonitoredProjectPortfolio")]
        public ProgressMonitoredProjectPortfolioType[] ContainedProgressMonitoredProjectPortfolio
        {
            get => this.containedProgressMonitoredProjectPortfolioField;
            set => this.containedProgressMonitoredProjectPortfolioField = value;
        }

        public ContractPerformanceMeasurementType IncentiveContractPerformanceMeasurement
        {
            get => this.incentiveContractPerformanceMeasurementField;
            set => this.incentiveContractPerformanceMeasurementField = value;
        }

        [XmlElement("ContainedProgressMonitoredProject")]
        public ProgressMonitoredProjectType[] ContainedProgressMonitoredProject
        {
            get => this.containedProgressMonitoredProjectField;
            set => this.containedProgressMonitoredProjectField = value;
        }

        [XmlElement("SpecifiedProjectContractLineItem")]
        public ProjectContractLineItemType[] SpecifiedProjectContractLineItem
        {
            get => this.specifiedProjectContractLineItemField;
            set => this.specifiedProjectContractLineItemField = value;
        }
    }
}