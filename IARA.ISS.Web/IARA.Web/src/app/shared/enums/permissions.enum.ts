export enum PermissionsEnum {
    // Poundnets
    PoundnetsRead = 'PoundnetsRead',
    PoundnetsAddRecords = 'PoundnetsAddRecords',
    PoundnetsEditRecords = 'PoundnetsEditRecords',
    PoundnetsDeleteRecords = 'PoundnetsDeleteRecords',
    PoundnetsRestoreRecords = 'PoundnetsRestoreRecords',

    // Qualified fishers
    QualifiedFishersReadAll = 'QualifiedFishersReadAll',
    QualifiedFishersRead = 'QualifiedFishersRead',
    QualifiedFishersAddRecords = 'QualifiedFishersAddRecords',
    QualifiedFishersEditRecords = 'QualifiedFishersEditRecords',
    QualifiedFishersDeleteRecords = 'QualifiedFishersDeleteRecords',
    QualifiedFishersRestoreRecords = 'QualifiedFishersRestoreRecords',
    QualifiedFishersAddDiplomaFishermanRecords = 'QualifiedFishersAddDiplomaFishermanRecords',

    QualifiedFishersApplicationsReadAll = 'QualifiedFishersApplicationsReadAll',
    QualifiedFishersApplicationsRead = 'QualifiedFishersApplicationsRead',
    QualifiedFishersApplicationsAddRecords = 'QualifiedFishersApplicationsAddRecords',
    QualifiedFishersApplicationsEditRecords = 'QualifiedFishersApplicationsEditRecords',
    QualifiedFishersApplicationsDeleteRecords = 'QualifiedFishersApplicationsDeleteRecords',
    QualifiedFishersApplicationsRestoreRecords = 'QualifiedFishersApplicationsRestoreRecords',
    QualifiedFishersApplicationsCancel = 'QualifiedFishersApplicationsCancel',
    QualifiedFishersApplicationsInspectAndCorrectRegiXData = 'QualifiedFishersApplicationsInspectAndCorrectRegiXData',
    QualifiedFishersApplicationsProcessPaymentData = 'QualifiedFishersApplicationsProcessPaymentData',
    QualifiedFishersApplicationsCheckDataRegularity = 'QualifiedFishersApplicationsCheckDataRegularity',

    // Scientific fishing
    ScientificFishingReadAll = 'ScientificFishingReadAll',
    ScientificFishingRead = 'ScientificFishingRead',
    ScientificFishingAddRecords = 'ScientificFishingAddRecords',
    ScientificFishingEditRecords = 'ScientificFishingEditRecords',
    ScientificFishingDeleteRecords = 'ScientificFishingDeleteRecords',
    ScientificFishingRestoreRecords = 'ScientificFishingRestoreRecords',
    ScientificFishingAddOutings = 'ScientificFishingAddOutings',

    ScientificFishingApplicationsReadAll = 'ScientificFishingApplicationsReadAll',
    ScientificFishingApplicationsRead = 'ScientificFishingApplicationsRead',
    ScientificFishingApplicationsAddRecords = 'ScientificFishingApplicationsAddRecords',
    ScientificFishingApplicationsEditRecords = 'ScientificFishingApplicationsEditRecords',
    ScientificFishingApplicationsDeleteRecords = 'ScientificFishingApplicationsDeleteRecords',
    ScientificFishingApplicationsRestoreRecords = 'ScientificFishingApplicationsRestoreRecords',
    ScientificFishingApplicationsCancel = 'ScientificFishingApplicationsCancel',
    ScientificFishingApplicationsInspectAndCorrectRegiXData = 'ScientificFishingApplicationsInspectAndCorrectRegiXData',
    ScientificFishingApplicationsProcessPaymentData = 'ScientificFishingApplicationsProcessPaymentData',
    ScientificFishingApplicationsCheckDataRegularity = 'ScientificFishingApplicationsCheckDataRegularity',

    // External users
    ExternalUsersRead = 'ExternalUsersRead',
    ExternalUsersEditRecords = 'ExternalUsersEditRecords',
    ExternalUsersDeleteRecords = 'ExternalUsersDeleteRecords',
    ExternalUsersRestoreRecords = 'ExternalUsersRestoreRecords',

    // Internal users
    InternalUsersRead = 'InternalUsersRead',
    InternalUsersAddRecords = 'InternalUsersAddRecords',
    InternalUsersEditRecords = 'InternalUsersEditRecords',
    InternalUsersDeleteRecords = 'InternalUsersDeleteRecords',
    InternalUsersRestoreRecords = 'InternalUsersRestoreRecords',
    InternalUsersAddMobileDevices = 'InternalUsersAddMobileDevices',

    // Legal entities
    LegalEntitiesReadAll = 'LegalEntitiesReadAll',
    LegalEntitiesRead = 'LegalEntitiesRead',
    LegalEntitiesEditRecords = 'LegalEntitiesEditRecords',
    LegalEntitiesDeleteRecords = 'LegalEntitiesDeleteRecords',
    LegalEntitiesRestoreRecords = 'LegalEntitiesRestoreRecords',
    LegalEntitiesAddRecords = 'LegalEntitiesAddRecords',

    LegalEntitiesApplicationsReadAll = 'LegalEntitiesApplicationsReadAll',
    LegalEntitiesApplicationsRead = "LegalEntitiesApplicationsRead",
    LegalEntitiesApplicationsAddRecords = "LegalEntitiesApplicationsAddRecords",
    LegalEntitiesApplicationsEditRecords = "LegalEntitiesApplicationsEditRecords",
    LegalEntitiesApplicationsDeleteRecords = "LegalEntitiesApplicationsDeleteRecords",
    LegalEntitiesApplicationsRestoreRecords = "LegalEntitiesApplicationsRestoreRecords",
    LegalEntitiesApplicationsCancel = 'LegalEntitiesApplicationsCancel',
    LegalEntitiesApplicationsInspectAndCorrectRegiXData = 'LegalEntitiesApplicationsInspectAndCorrectRegiXData',
    LegalEntitiesApplicationsProcessPaymentData = 'LegalEntitiesApplicationsProcessPaymentData',
    LegalEntitiesApplicationsCheckDataRegularity = 'LegalEntitiesApplicationsCheckDataRegularity',

    // Permissions
    PermissionsRegisterRead = 'PermissionsRegisterRead',
    PermissionsRegisterEditRecords = 'PermissionsRegisterEditRecords',

    // Roles
    RolesRegisterRead = 'RolesRegisterRead',
    RolesRegisterAddRecords = 'RolesRegisterAddRecords',
    RolesRegisterEditRecords = 'RolesRegisterEditRecords',
    RolesRegisterDeleteRecords = 'RolesRegisterDeleteRecords',
    RolesRegisterRestoreRecords = 'RolesRegisterRestoreRecords',

    // Print configurations
    PrintConfigurationsRead = 'PrintConfigurationsRead',
    PrintConfigurationsAddRecords = 'PrintConfigurationsAddRecords',
    PrintConfigurationsEditRecords = 'PrintConfigurationsEditRecords',
    PrintConfigurationsDeleteRecords = 'PrintConfigurationsDeleteRecords',
    PrintConfigurationsRestoreRecords = 'PrintConfigurationsRestoreRecords',

    // Nomenclatures
    NomenclaturesRead = 'NomenclaturesRead',
    NomenclaturesAddRecords = 'NomenclaturesAddRecords',
    NomenclaturesEditRecords = 'NomenclaturesEditRecords',
    NomenclaturesDeleteRecords = 'NomenclaturesDeleteRecords',
    NomenclaturesRestoreRecords = 'NomenclaturesRestoreRecords',

    // Associations
    AssociationsReadAll = 'AssociationsReadAll',
    AssociationsRead = 'AssociationsRead',
    AssociationsAddRecords = 'AssociationsAddRecords',
    AssociationsEditRecords = 'AssociationsEditRecords',
    AssociationsDeleteRecords = 'AssociationsDeleteRecords',
    AssociationsRestoreRecords = 'AssociationsRestoreRecords',

    AssociationsTicketsRead = 'AssociationsTicketsRead',
    AssociationsTicketsAddRecords = 'AssociationsTicketsAddRecords',
    AssociationsTicketsEditRecords = 'AssociationsTicketsEditRecords',
    AssociationsTicketsDeleteRecords = 'AssociationsTicketsDeleteRecords',
    AssociationsTicketsRestoreRecords = 'AssociationsTicketsRestoreRecords',
    AssociationsTicketsCancel = 'AssociationsTicketsCancel',
    AssociationsTicketsInspectAndCorrectRegiXData = 'AssociationsTicketsInspectAndCorrectRegiXData',
    AssociationsTicketsProcessPaymentData = 'AssociationsTicketsProcessPaymentData',
    AssociationsTicketsCheckDataRegularity = 'AssociationsTicketsCheckDataRegularity',

    // Persons report
    PersonsReportRead = 'PersonsReportRead',

    //Legal Entities report
    LegalEntitiesReportRead = 'LegalEntitiesReportRead',

    // Applications
    ApplicationsRead = 'ApplicationsRead',
    ApplicationsAddRecords = 'ApplicationsAddRecords',
    ApplicationsEditRecords = 'ApplicationsEditRecords',
    ApplicationsDeleteRecords = 'ApplicationsDeleteRecords',
    ApplicationsRestoreRecords = 'ApplicationsRestoreRecords',
    ApplicationsEnterEventisNumber = 'ApplicationsEnterEventisNumber',
    ApplicationsInspectAndCorrectRegiXData = 'ApplicationsInspectAndCorrectRegiXData',
    ApplicationsCancelRecords = 'ApplicationsCancelRecords',
    ApplicationsReadRegister = 'ApplicationsReadRegister',
    ReAssignApplication = 'ReAssignApplication',
    ApplicationsProcessPaymentData = 'ApplicationsProcessPaymentData',

    // Applications register delivery
    ApplicationRegisterDeliveryRead = 'ApplicationRegisterDeliveryRead',
    ApplicationRegisterDeliveryAddRecords = 'ApplicationRegisterDeliveryAddRecords',
    ApplicationRegisterDeliveryEditRecords = 'ApplicationRegisterDeliveryEditRecords',

    // Translation management
    TranslationRead = 'TranslationRead',
    TranslationAddRecords = 'TranslationAddRecords',
    TranslationEditRecords = 'TranslationEditRecords',
    TranslationDeleteRecords = 'TranslationDeleteRecords',
    TranslationRestoreRecords = 'TranslationRestoreRecords',

    // Ticket applications
    TicketsPublicRead = 'TicketsPublicRead',
    TicketsPublicAddRecords = 'TicketsPublicAddRecords',
    TicketsPublicEditRecords = 'TicketsPublicEditRecords',

    TicketsReadAll = 'TicketsReadAll',
    TicketsRead = 'TicketsRead',
    TicketsAddRecords = 'TicketsAddRecords',
    TicketsEditRecords = 'TicketsEditRecords',
    TicketsDeleteRecords = 'TicketsDeleteRecords',
    TicketsRestoreRecords = 'TicketsRestoreRecords',
    TicketsCancelRecord = 'TicketsCancelRecord',
    TicketsInspectAndCorrectRegiXData = 'TicketsInspectAndCorrectRegiXData',
    TicketsProcessPaymentData = 'TicketsProcessPaymentData',
    TicketsCheckDataRegularity = 'TicketsCheckDataRegularity',
    TicketsEditAllOnlineRecords = 'TicketsEditAllOnlineRecords',

    // Buyers and First Sale Centers
    BuyersReadAll = 'BuyersReadAll',
    BuyersRead = 'BuyersRead',
    BuyersAddRecords = 'BuyersAddRecords',
    BuyersEditRecords = 'BuyersEditRecords',
    BuyersDeleteRecords = 'BuyersDeleteRecords',
    BuyersRestoreRecords = 'BuyersRestoreRecords',

    BuyersApplicationsReadAll = 'BuyersApplicationsReadAll',
    BuyersApplicationsRead = 'BuyersApplicationsRead',
    BuyersApplicationsAddRecords = 'BuyersApplicationsAddRecords',
    BuyersApplicationsEditRecords = 'BuyersApplicationsEditRecords',
    BuyersApplicationsDeleteRecords = 'BuyersApplicationsDeleteRecords',
    BuyersApplicationsRestoreRecords = 'BuyersApplicationsRestoreRecords',
    BuyersApplicationsCancel = 'BuyersApplicationsCancel',
    BuyersApplicationsInspectAndCorrectRegiXData = 'BuyersApplicationsInspectAndCorrectRegiXData',
    BuyersApplicationsProcessPaymentData = 'BuyersApplicationsProcessPaymentData',
    BuyersApplicationsCheckDataRegularity = 'BuyersApplicationsCheckDataRegularity',

    BuyerLogBookRead = 'BuyerLogBookRead',
    BuyerLogBookEdit = 'BuyerLogBookEdit',
    BuyerLogBookAdd = 'BuyerLogBookAdd',
    BuyerLogBookDelete = 'BuyerLogBookDelete',
    BuyerLogBookRestore = 'BuyerLogBookRestore',

    // Online Application processing in Public app
    OnlineSubmittedApplicationsRead = 'OnlineSubmittedApplicationsRead',
    OnlineSubmittedApplicationsAddRecords = 'OnlineSubmittedApplicationsAddRecords',
    OnlineSubmittedApplicationsEditRecords = 'OnlineSubmittedApplicationsEditRecords',
    OnlineSubmittedApplicationsCancelRecords = 'OnlineSubmittedApplicationsCancelRecords',
    OnlineSubmittedApplicationsProcessPaymentData = 'OnlineSubmittedApplicationsProcessPaymentData',
    OnlineSubmittedApplicationsDownloadRecords = 'OnlineSubmittedApplicationsDownloadRecords',
    OnlineSubmittedApplicationsUploadRecords = 'OnlineSubmittedApplicationsUploadRecords',
    OnlineSubmittedApplicationsReadRegister = 'OnlineSubmittedApplicationsReadRegister',

    // Ships register
    ShipsRegisterReadAll = 'ShipsRegisterReadAll',
    ShipsRegisterRead = 'ShipsRegisterRead',
    ShipsRegisterAddRecords = 'ShipsRegisterAddRecords',
    ShipsRegisterEditRecords = 'ShipsRegisterEditRecords',
    ShipsRegisterDeleteRecords = 'ShipsRegisterDeleteRecords',
    ShipsRegisterRestoreRecords = 'ShipsRegisterRestoreRecords',
    ShipsRegisterThirdPartyShips = 'ShipsRegisterThirdPartyShips',
    ShipsRegisterSendFluxData = 'ShipsRegisterSendFluxData',

    ShipsRegisterApplicationReadAll = 'ShipsRegisterApplicationReadAll',
    ShipsRegisterApplicationsRead = 'ShipsRegisterApplicationsRead',
    ShipsRegisterApplicationsAddRecords = 'ShipsRegisterApplicationsAddRecords',
    ShipsRegisterApplicationsEditRecords = 'ShipsRegisterApplicationsEditRecords',
    ShipsRegisterApplicationsDeleteRecords = 'ShipsRegisterApplicationsDeleteRecords',
    ShipsRegisterApplicationsRestoreRecords = 'ShipsRegisterApplicationsRestoreRecords',
    ShipsRegisterApplicationsCancel = 'ShipsRegisterApplicationsCancel',
    ShipsRegisterApplicationsInspectAndCorrectRegiXData = 'ShipsRegisterApplicationsInspectAndCorrectRegiXData',
    ShipsRegisterApplicationsProcessPaymentData = 'ShipsRegisterApplicationsProcessPaymentData',
    ShipsRegisterApplicationsCheckDataRegularity = 'ShipsRegisterApplicationsCheckDataRegularity',

    // Catch quotas
    YearlyQuotasRead = 'YearlyQuotasRead',
    YearlyQuotasAddRecords = 'YearlyQuotasAddRecords',
    YearlyQuotasEditRecords = 'YearlyQuotasEditRecords',
    YearlyQuotasDeleteRecords = 'YearlyQuotasDeleteRecords',
    YearlyQuotasRestoreRecords = 'YearlyQuotasRestoreRecords',
    YearlyQuotasTransferRecords = 'YearlyQuotasTransferRecords',
    ShipQuotasRead = 'ShipQuotasRead',
    ShipQuotasAddRecords = 'ShipQuotasAddRecords',
    ShipQuotasEditRecords = 'ShipQuotasEditRecords',
    ShipQuotasDeleteRecords = 'ShipQuotasDeleteRecords',
    ShipQuotasRestoreRecords = 'ShipQuotasRestoreRecords',
    ShipQuotasTransferRecords = 'ShipQuotasTransferRecords',

    // System log
    SystemLogRead = 'SystemLogRead',

    //Error log
    ErrorLogRead = 'ErrorLogRead',

    //News management
    NewsManagementRead = 'NewsManagementRead',
    NewsManagementAddRecords = 'NewsManagementAddRecords',
    NewsManagementEditRecords = 'NewsManagementEditRecords',
    NewsManagementDeleteRecords = 'NewsManagementDeleteRecords',
    NewsManagementRestoreRecords = 'NewsManagementRestoreRecords',

    // Inspections
    InspectionsReadAll = 'InspectionsReadAll',
    InspectionsRead = 'InspectionsRead',
    InspectionsAddRecords = 'InspectionsAddRecords',
    InspectionsEditRecords = 'InspectionsEditRecords',
    InspectionsDeleteRecords = 'InspectionsDeleteRecords',
    InspectionsRestoreRecords = 'InspectionsRestoreRecords',
    InspectionEditNumber = 'InspectionEditNumber',
    InspectionDownload = 'InspectionDownload',
    InspectionExport = 'InspectionExport',
    InspectionResolveCrossCheck = 'InspectionResolveCrossCheck',
    InspectionLockedEdit = 'InspectionLockedEdit',

    // Commercial Fishing
    CommercialFishingPermitRegisterReadAll = 'CommercialFishingPermitRegisterReadAll',
    CommercialFishingPermitRegisterRead = 'CommercialFishingPermitRegisterRead',
    CommercialFishingPermitRegisterAddRecords = 'CommercialFishingPermitRegisterAddRecords',
    CommercialFishingPermitRegisterEditRecords = 'CommercialFishingPermitRegisterEditRecords',
    CommercialFishingPermitRegisterDeleteRecords = 'CommercialFishingPermitRegisterDeleteRecords',
    CommercialFishingPermitRegisterRestoreRecords = 'CommercialFishingPermitRegisterRestoreRecords',

    CommercialFishingPermitLicenseRegisterReadAll = 'CommercialFishingPermitLicenseRegisterReadAll',
    CommercialFishingPermitLicenseRegisterRead = 'CommercialFishingPermitLicenseRegisterRead',
    CommercialFishingPermitLicenseRegisterAddRecords = 'CommercialFishingPermitLicenseRegisterAddRecords',
    CommercialFishingPermitLicenseRegisterEditRecords = 'CommercialFishingPermitLicenseRegisterEditRecords',
    CommercialFishingPermitLicenseRegisterDeleteRecords = 'CommercialFishingPermitLicenseRegisterDeleteRecords',
    CommercialFishingPermitLicenseRegisterRestoreRecords = 'CommercialFishingPermitLicenseRegisterRestoreRecords',

    CommercialFishingPermitApplicationsReadAll = 'CommercialFishingPermitApplicationsReadAll',
    CommercialFishingPermitApplicationsRead = 'CommercialFishingPermitApplicationsRead',
    CommercialFishingPermitApplicationsAddRecords = 'CommercialFishingPermitApplicationsAddRecords',
    CommercialFishingPermitApplicationsEditRecords = 'CommercialFishingPermitApplicationsEditRecords',
    CommercialFishingPermitApplicationsDeleteRecords = 'CommercialFishingPermitApplicationsDeleteRecords',
    CommercialFishingPermitApplicationsRestoreRecords = 'CommercialFishingPermitApplicationsRestoreRecords',
    CommercialFishingPermitApplicationsCancel = 'CommercialFishingPermitApplicationsCancel',
    CommercialFishingPermitApplicationsInspectAndCorrectRegiXData = 'CommercialFishingPermitApplicationsInspectAndCorrectRegiXData',
    CommercialFishingPermitApplicationsProcessPaymentData = 'CommercialFishingPermitApplicationsProcessPaymentData',
    CommercialFishingPermitApplicationsCheckDataRegularity = 'CommercialFishingPermitApplicationsCheckDataRegularity',

    CommercialFishingPermitLicenseApplicationsReadAll = 'CommercialFishingPermitLicenseApplicationsReadAll',
    CommercialFishingPermitLicenseApplicationsRead = 'CommercialFishingPermitLicenseApplicationsRead',
    CommercialFishingPermitLicenseApplicationsAddRecords = 'CommercialFishingPermitLicenseApplicationsAddRecords',
    CommercialFishingPermitLicenseApplicationsEditRecords = 'CommercialFishingPermitLicenseApplicationsEditRecords',
    CommercialFishingPermitLicenseApplicationsDeleteRecords = 'CommercialFishingPermitLicenseApplicationsDeleteRecords',
    CommercialFishingPermitLicenseApplicationsRestoreRecords = 'CommercialFishingPermitLicenseApplicationsRestoreRecords',
    CommercialFishingPermitLicenseApplicationsCancel = 'CommercialFishingPermitLicenseApplicationsCancel',
    CommercialFishingPermitLicenseApplicationsInspectAndCorrectRegiXData = 'CommercialFishingPermitLicenseApplicationsInspectAndCorrectRegiXData',
    CommercialFishingPermitLicenseApplicationsProcessPaymentData = 'CommercialFishingPermitLicenseApplicationsProcessPaymentData',
    CommercialFishingPermitLicenseApplicationsCheckDataRegularity = 'CommercialFishingPermitLicenseApplicationsCheckDataRegularity',

    PermitLicenseLogBookRead = 'PermitLicenseLogBookRead',
    PermitLicenseLogBookAdd = 'PermitLicenseLogBookAdd',
    PermitLicenseLogBookEdit = 'PermitLicenseLogBookEdit',
    PermitLicenseLogBookDelete = 'PermitLicenseLogBookDelete',
    PermitLicenseLogBookRestore = 'PermitLicenseLogBookRestore',
    PermitLicenseLogBookRenewMoreThanMaxPages = 'PermitLicenseLogBookRenewMoreThanMaxPages',

    PermitSuspensionRead = 'PermitSuspensionRead',
    PermitSuspensionAdd = 'PermitSuspensionAdd',
    PermitSuspensionEdit = 'PermitSuspensionEdit',
    PermitSuspensionDelete = 'PermitSuspensionDelete',
    PermitSuspensionRestore = 'PermitSuspensionRestore',

    PermitLicenseSuspensionRead = 'PermitLicenseSuspensionRead',
    PermitLicenseSuspensionAdd = 'PermitLicenseSuspensionAdd',
    PermitLicenseSuspensionEdit = 'PermitLicenseSuspensionEdit',
    PermitLicenseSuspensionDelete = 'PermitLicenseSuspensionDelete',
    PermitLicenseSuspensionRestore = 'PermitLicenseSuspensionRestore',

    // Report Definition
    ReportReadAll = 'ReportReadAll',
    ReportRead = 'ReportRead',
    ReportAddRecords = 'ReportAddRecords',
    ReportEditRecords = 'ReportEditRecords',
    ReportDeleteRecords = 'ReportDeleteRecords',
    ReportRestoreRecords = 'ReportRestoreRecords',

    // Report Parameter Definition
    ReportParameterRead = 'ReportParameterRead',
    ReportParameterAddRecords = 'ReportParameterAddRecords',
    ReportParameterEditRecords = 'ReportParameterEditRecords',
    ReportParameterDeleteRecords = 'ReportParameterDeleteRecords',
    ReportParameterRestoreRecords = 'ReportParameterRestoreRecords',

    // Maximum capacity
    MaximumCapacityRead = 'MaximumCapacityRead',
    MaximumCapacityAddRecords = 'MaximumCapacityAddRecords',
    MaximumCapacityEditRecords = 'MaximumCapacityEditRecords',

    // Fishing capacity certificates
    FishingCapacityCertificatesRead = 'FishingCapacityCertificatesRead',
    FishingCapacityCertificatesEditRecords = 'FishingCapacityCertificatesEditRecords',
    FishingCapacityCertificatesDeleteRecords = 'FishingCapacityCertificatesDeleteRecords',
    FishingCapacityCertificatesRestoreRecords = 'FishingCapacityCertificatesRestoreRecords',

    // Fishing capacity
    FishingCapacityReadAll = 'FishingCapacityReadAll',
    FishingCapacityRead = 'FishingCapacityRead',
    FishingCapacityAddRecords = 'FishingCapacityAddRecords',
    FishingCapacityEditRecords = 'FishingCapacityEditRecords',
    FishingCapacityDeleteRecords = 'FishingCapacityDeleteRecords',
    FishingCapacityRestoreRecords = 'FishingCapacityRestoreRecords',
    FishingCapacityAnalysis = 'FishingCapacityAnalysis',

    FishingCapacityApplicationsReadAll = 'FishingCapacityApplicationsReadAll',
    FishingCapacityApplicationsRead = 'FishingCapacityApplicationsRead',
    FishingCapacityApplicationsAddRecords = 'FishingCapacityApplicationsAddRecords',
    FishingCapacityApplicationsEditRecords = 'FishingCapacityApplicationsEditRecords',
    FishingCapacityApplicationsDeleteRecords = 'FishingCapacityApplicationsDeleteRecords',
    FishingCapacityApplicationsRestoreRecords = 'FishingCapacityApplicationsRestoreRecords',
    FishingCapacityApplicationsCancel = 'FishingCapacityApplicationsCancel',
    FishingCapacityApplicationsInspectAndCorrectRegiXData = 'FishingCapacityApplicationsInspectAndCorrectRegiXData',
    FishingCapacityApplicationsProcessPaymentData = 'FishingCapacityApplicationsProcessPaymentData',
    FishingCapacityApplicationsCheckDataRegularity = 'FishingCapacityApplicationsCheckDataRegularity',

    //Inspectors
    InspectorsRead = 'InspectorsRead',
    InspectorsAddRecords = 'InspectorsAddRecords',
    InspectorsEditRecords = 'InspectorsEditRecords',
    InspectorsDeleteRecords = 'InspectorsDeleteRecords',
    InspectorsRestoreRecords = 'InspectorsRestoreRecords',

    // Aquaculture facilities
    AquacultureFacilitiesReadAll = 'AquacultureFacilitiesReadAll',
    AquacultureFacilitiesRead = 'AquacultureFacilitiesRead',
    AquacultureFacilitiesAddRecords = 'AquacultureFacilitiesAddRecords',
    AquacultureFacilitiesEditRecords = 'AquacultureFacilitiesEditRecords',
    AquacultureFacilitiesDeleteRecords = 'AquacultureFacilitiesDeleteRecords',
    AquacultureFacilitiesRestoreRecords = 'AquacultureFacilitiesRestoreRecords',
    AquacultureFacilitiesCancel = 'AquacultureFacilitiesCancel',

    AquacultureFacilitiesApplicationsReadAll = 'AquacultureFacilitiesApplicationsReadAll',
    AquacultureFacilitiesApplicationsRead = 'AquacultureFacilitiesApplicationsRead',
    AquacultureFacilitiesApplicationsAddRecords = 'AquacultureFacilitiesApplicationsAddRecords',
    AquacultureFacilitiesApplicationsEditRecords = 'AquacultureFacilitiesApplicationsEditRecords',
    AquacultureFacilitiesApplicationsDeleteRecords = 'AquacultureFacilitiesApplicationsDeleteRecords',
    AquacultureFacilitiesApplicationsRestoreRecords = 'AquacultureFacilitiesApplicationsRestoreRecords',
    AquacultureFacilitiesApplicationsCancel = 'AquacultureFacilitiesApplicationsCancel',
    AquacultureFacilitiesApplicationsInspectAndCorrectRegiXData = 'AquacultureFacilitiesApplicationsInspectAndCorrectRegiXData',
    AquacultureFacilitiesApplicationsProcessPaymentData = 'AquacultureFacilitiesApplicationsProcessPaymentData',
    AquacultureFacilitiesApplicationsCheckDataRegularity = 'AquacultureFacilitiesApplicationsCheckDataRegularity',

    AquacultureLogBook1Read = 'AquacultureLogBook1Read',
    AquacultureLogBookEdit = 'AquacultureLogBookEdit',
    AquacultureLogBookAdd = 'AquacultureLogBookAdd',
    AquacultureLogBookDelete = 'AquacultureLogBookDelete',
    AquacultureLogBookRestore = 'AquacultureLogBookRestore',

    //PatrolVehicles
    PatrolVehiclesRead = 'PatrolVehiclesRead',
    PatrolVehiclesAddRecords = 'PatrolVehiclesAddRecords',
    PatrolVehiclesEditRecords = 'PatrolVehiclesEditRecords',
    PatrolVehiclesDeleteRecords = 'PatrolVehiclesDeleteRecords',
    PatrolVehiclesRestoreRecords = 'PatrolVehiclesRestoreRecords',

    // Cross checks
    CrossChecksRead = 'CrossChecksRead',
    CrossChecksAddRecords = 'CrossChecksAddRecords',
    CrossChecksEditRecords = 'CrossChecksEditRecords',
    CrossChecksDeleteRecords = 'CrossChecksDeleteRecords',
    CrossChecksRestoreRecords = 'CrossChecksRestoreRecords',
    CrossChecksExecuteRecords = 'CrossChecksExecuteRecords',

    // Cross check results
    CrossCheckResultsRead = 'CrossCheckResultsRead',
    CrossCheckResultsDeleteRecords = 'CrossCheckResultsDeleteRecords',
    CrossCheckResultsRestoreRecords = 'CrossCheckResultsRestoreRecords',
    CrossCheckResultsAssign = 'CrossCheckResultsAssign',
    CrossCheckResultsResolve = 'CrossCheckResultsResolve',
    CrossCheckResultAutoAssing = 'CrossCheckResultAutoAssing',

    // Catches and sales (log books & log book pages)
    FishLogBooksReadAll = 'FishLogBooksReadAll',
    FishLogBookRead = 'FishLogBookRead',

    FirstSaleLogBooksReadAll = 'FirstSaleLogBooksReadAll',
    FirstSaleLogBookRead = 'FirstSaleLogBookRead',

    AdmissionLogBooksReadAll = 'AdmissionLogBooksReadAll',
    AdmissionLogBookRead = 'AdmissionLogBookRead',

    TransportationLogBooksReadAll = 'TransportationLogBooksReadAll',
    TransportationLogBookRead = 'TransportationLogBookRead',

    AquacultureLogBooksReadAll = 'AquacultureLogBooksReadAll',
    AquacultureLogBookRead = 'AquacultureLogBookRead',

    FishLogBookPageReadAll = 'FishLogBookPageReadAll',
    FishLogBookPageRead = 'FishLogBookPageRead',
    FishLogBookPageAdd = 'FishLogBookPageAdd',
    FishLogBookPageEdit = 'FishLogBookPageEdit',
    FishLogBookPageCancel = 'FishLogBookPageCancel',
    FishLogBookPageEditNumber = 'FishLogBookPageEditNumber',
    FishLogBookPageEditFiles = 'FishLogBookPageEditFiles',

    FirstSaleLogBookPageReadAll = 'FirstSaleLogBookPageReadAll',
    FirstSaleLogBookPageRead = 'FirstSaleLogBookPageRead',
    FirstSaleLogBookPageAdd = 'FirstSaleLogBookPageAdd',
    FirstSaleLogBookPageEdit = 'FirstSaleLogBookPageEdit',
    FirstSaleLogBookPageCancel = 'FirstSaleLogBookPageCancel',
    FirstSaleLogBookPageEditNumber = 'FirstSaleLogBookPageEditNumber',
    FirstSaleLogBookPageEditCommonData = 'FirstSaleLogBookPageEditCommonData',
    FirstSaleLogBookPageEditFiles = 'FirstSaleLogBookPageEditFiles',

    AdmissionLogBookPageReadAll = 'AdmissionLogBookPageReadAll',
    AdmissionLogBookPageRead = 'AdmissionLogBookPageRead',
    AdmissionLogBookPageAdd = 'AdmissionLogBookPageAdd',
    AdmissionLogBookPageEdit = 'AdmissionLogBookPageEdit',
    AdmissionLogBookPageCancel = 'AdmissionLogBookPageCancel',
    AdmissionLogBookPageEditNumber = 'AdmissionLogBookPageEditNumber',
    AdmissionLogBookPageEditCommonData = 'AdmissionLogBookPageEditCommonData',
    AdmissionLogBookPageEditFiles = 'AdmissionLogBookPageEditFiles',

    TransportationLogBookPageReadAll = 'TransportationLogBookPageReadAll',
    TransportationLogBookPageRead = 'TransportationLogBookPageRead',
    TransportationLogBookPageAdd = 'TransportationLogBookPageAdd',
    TransportationLogBookPageEdit = 'TransportationLogBookPageEdit',
    TransportationLogBookPageCancel = 'TransportationLogBookPageCancel',
    TransportationLogBookPageEditNumber = 'TransportationLogBookPageEditNumber',
    TransportationLogBookPageEditCommonData = 'TransportationLogBookPageEditCommonData',
    TransportationLogBookPageEditFiles = 'TransportationLogBookPageEditFiles',

    AquacultureLogBookPageReadAll = 'AquacultureLogBookPageReadAll',
    AquacultureLogBookPageRead = 'AquacultureLogBookPageRead',
    AquacultureLogBookPageAdd = 'AquacultureLogBookPageAdd',
    AquacultureLogBookPageEdit = 'AquacultureLogBookPageEdit',
    AquacultureLogBookPageCancel = 'AquacultureLogBookPageCancel',
    AquacultureLogBookPageEditNumber = 'AquacultureLogBookPageEditNumber',
    AquacultureLogBookPageEditFiles = 'AquacultureLogBookPageEditFiles',

    // Log book page edit exceptions

    LogBookPageEditExceptionsRead = 'LogBookPageEditExceptionsRead',
    LogBookPageEditExceptionsAddRecords = 'LogBookPageEditExceptionsAddRecords',
    LogBookPageEditExceptionsEditRecords = 'LogBookPageEditExceptionsEditRecords',
    LogBookPageEditExceptionsDeleteRecords = 'LogBookPageEditExceptionsDeleteRecords',
    LogBookPageEditExceptionsRestoreRecords = 'LogBookPageEditExceptionsRestoreRecords',

    // Fishing activity reports
    FishingActivityReportsRead = 'FishingActivityReportsRead',
    FishingActivityReportsGenerateLanding = 'FishingActivityReportsGenerateLanding',
    FishingActivityReportsReplay = 'FishingActivityReportsReplay',
    FishingActivityReportsSendToFlux = 'FishingActivityReportsSendToFlux',
    FishingActivityReportsUpload = 'FishingActivityReportsUpload',

    // AUANs
    AuanRegisterReadAll = 'AuanRegisterReadAll',
    AuanRegisterRead = 'AuanRegisterRead',
    AuanRegisterAddRecords = 'AuanRegisterAddRecords',
    AuanRegisterEditRecords = 'AuanRegisterEditRecords',
    AuanRegisterDeleteRecords = 'AuanRegisterDeleteRecords',
    AuanRegisterRestoreRecords = 'AuanRegisterRestoreRecords',
    AuanRegisterCancel = 'AuanRegisterCancel',
    AuanRegisterReturnForCorrections = 'AuanRegisterReturnForCorrections',

    //Impersonate user
    ImpersonateUser = 'ImpersonateUser',

    // Statistical forms
    StatisticalFormsAquaFarmReadAll = 'StatisticalFormsAquaFarmReadAll',
    StatisticalFormsAquaFarmRead = 'StatisticalFormsAquaFarmRead',
    StatisticalFormsAquaFarmAddRecords = 'StatisticalFormsAquaFarmAddRecords',
    StatisticalFormsAquaFarmEditRecords = 'StatisticalFormsAquaFarmEditRecords',
    StatisticalFormsAquaFarmDeleteRecords = 'StatisticalFormsAquaFarmDeleteRecords',
    StatisticalFormsAquaFarmRestoreRecords = 'StatisticalFormsAquaFarmRestoreRecords',

    StatisticalFormsAquaFarmApplicationsReadAll = 'StatisticalFormsAquaFarmApplicationsReadAll',
    StatisticalFormsAquaFarmApplicationsRead = 'StatisticalFormsAquaFarmApplicationsRead',
    StatisticalFormsAquaFarmApplicationsAddRecords = 'StatisticalFormsAquaFarmApplicationsAddRecords',
    StatisticalFormsAquaFarmApplicationsEditRecords = 'StatisticalFormsAquaFarmApplicationsEditRecords',
    StatisticalFormsAquaFarmApplicationsDeleteRecords = 'StatisticalFormsAquaFarmApplicationsDeleteRecords',
    StatisticalFormsAquaFarmApplicationsRestoreRecords = 'StatisticalFormsAquaFarmApplicationsRestoreRecords',
    StatisticalFormsAquaFarmApplicationsCancel = 'StatisticalFormsAquaFarmApplicationsCancel',
    StatisticalFormsAquaFarmApplicationsInspectAndCorrectRegiXData = 'StatisticalFormsAquaFarmApplicationsInspectAndCorrectRegiXData',
    StatisticalFormsAquaFarmApplicationsProcessPaymentData = 'StatisticalFormsAquaFarmApplicationsProcessPaymentData',
    StatisticalFormsAquaFarmApplicationsCheckDataRegularity = 'StatisticalFormsAquaFarmApplicationsCheckDataRegularity',

    StatisticalFormsReworkReadAll = 'StatisticalFormsReworkReadAll',
    StatisticalFormsReworkRead = 'StatisticalFormsReworkRead',
    StatisticalFormsReworkAddRecords = 'StatisticalFormsReworkAddRecords',
    StatisticalFormsReworkEditRecords = 'StatisticalFormsReworkEditRecords',
    StatisticalFormsReworkDeleteRecords = 'StatisticalFormsReworkDeleteRecords',
    StatisticalFormsReworkRestoreRecords = 'StatisticalFormsReworkRestoreRecords',

    StatisticalFormsReworkApplicationsReadAll = 'StatisticalFormsReworkApplicationsReadAll',
    StatisticalFormsReworkApplicationsRead = 'StatisticalFormsReworkApplicationsRead',
    StatisticalFormsReworkApplicationsAddRecords = 'StatisticalFormsReworkApplicationsAddRecords',
    StatisticalFormsReworkApplicationsEditRecords = 'StatisticalFormsReworkApplicationsEditRecords',
    StatisticalFormsReworkApplicationsDeleteRecords = 'StatisticalFormsReworkApplicationsDeleteRecords',
    StatisticalFormsReworkApplicationsRestoreRecords = 'StatisticalFormsReworkApplicationsRestoreRecords',
    StatisticalFormsReworkApplicationsCancel = 'StatisticalFormsReworkApplicationsCancel',
    StatisticalFormsReworkApplicationsInspectAndCorrectRegiXData = 'StatisticalFormsReworkApplicationsInspectAndCorrectRegiXData',
    StatisticalFormsReworkApplicationsProcessPaymentData = 'StatisticalFormsReworkApplicationsProcessPaymentData',
    StatisticalFormsReworkApplicationsCheckDataRegularity = 'StatisticalFormsReworkApplicationsCheckDataRegularity',

    StatisticalFormsFishVesselReadAll = 'StatisticalFormsFishVesselReadAll',
    StatisticalFormsFishVesselRead = 'StatisticalFormsFishVesselRead',
    StatisticalFormsFishVesselAddRecords = 'StatisticalFormsFishVesselAddRecords',
    StatisticalFormsFishVesselEditRecords = 'StatisticalFormsFishVesselEditRecords',
    StatisticalFormsFishVesselDeleteRecords = 'StatisticalFormsFishVesselDeleteRecords',
    StatisticalFormsFishVesselRestoreRecords = 'StatisticalFormsFishVesselRestoreRecords',

    StatisticalFormsFishVesselsApplicationsReadAll = 'StatisticalFormsFishVesselsApplicationsReadAll',
    StatisticalFormsFishVesselsApplicationsRead = 'StatisticalFormsFishVesselsApplicationsRead',
    StatisticalFormsFishVesselsApplicationsAddRecords = 'StatisticalFormsFishVesselsApplicationsAddRecords',
    StatisticalFormsFishVesselsApplicationsEditRecords = 'StatisticalFormsFishVesselsApplicationsEditRecords',
    StatisticalFormsFishVesselsApplicationsDeleteRecords = 'StatisticalFormsFishVesselsApplicationsDeleteRecords',
    StatisticalFormsFishVesselsApplicationsRestoreRecords = 'StatisticalFormsFishVesselsApplicationsRestoreRecords',
    StatisticalFormsFishVesselsApplicationsCancel = 'StatisticalFormsFishVesselsApplicationsCancel',
    StatisticalFormsFishVesselsApplicationsInspectAndCorrectRegiXData = 'StatisticalFormsFishVesselsApplicationsInspectAndCorrectRegiXData',
    StatisticalFormsFishVesselsApplicationsProcessPaymentData = 'StatisticalFormsFishVesselsApplicationsProcessPaymentData',
    StatisticalFormsFishVesselsApplicationsCheckDataRegularity = 'StatisticalFormsFishVesselsApplicationsCheckDataRegularity',

    // Penal decrees
    PenalDecreesReadAll = 'PenalDecreesReadAll',
    PenalDecreesRead = 'PenalDecreesRead',
    PenalDecreesAddRecords = 'PenalDecreesAddRecords',
    PenalDecreesEditRecords = 'PenalDecreesEditRecords',
    PenalDecreesDeleteRecords = 'PenalDecreesDeleteRecords',
    PenalDecreesRestoreRecords = 'PenalDecreesRestoreRecords',
    PenalDecreesSubmitRecords = 'PenalDecreesSubmitRecords',
    PenalDecreesCancelRecords = 'PenalDecreesCancelRecords',
    PenalDecreescanReturnForFurtherCorrectionsRecords = 'PenalDecreescanReturnForFurtherCorrectionsRecords',

    PenalDecreeStatusesRead = 'PenalDecreeStatusesRead',
    PenalDecreeStatusesAddRecords = 'PenalDecreeStatusesAddRecords',
    PenalDecreeStatusesEditRecords = 'PenalDecreeStatusesEditRecords',
    PenalDecreeStatusesDeleteRecords = 'PenalDecreeStatusesDeleteRecords',
    PenalDecreeStatusesRestoreRecords = 'PenalDecreeStatusesRestoreRecords',

    //Application RegiX checks
    ApplicationRegiXChecksRead = 'ApplicationRegiXChecksRead',

    //FLUX VMS requests
    FLUXVMSRequestsRead = 'FLUXVMSRequestsRead',
    FLUXVMSRequestsAddRecords = 'FLUXVMSRequestsAddRecords',
    FLUXVMSRequestsEditRecords = 'FLUXVMSRequestsEditRecords',

    // Awarded points
    AwardedPointsReadAll = 'AwardedPointsReadAll',
    AwardedPointsRead = 'AwardedPointsRead',
    AwardedPointsAddRecords = 'AwardedPointsAddRecords',
    AwardedPointsEditRecords = 'AwardedPointsEditRecords',
    AwardedPointsDeleteRecords = 'AwardedPointsDeleteRecords',
    AwardedPointsRestoreRecords = 'AwardedPointsRestoreRecords',
}
