import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MaterialModule } from '@app/shared/material.module';
import { MainNavigation } from '@app/shared/navigation/base/main.navigation';
import { TLCommonModule } from '@app/shared/tl-common.module';
import { TLBoricaPaymentsModule } from '@tl/tl-borica-payments';
import { TLEPayPaymentsModule } from '@tl/tl-epay-payments';
import { NgApexchartsModule } from "ng-apexcharts";
import { CommonApplicationModule } from '../common-app/common-app.module';
import { ApplicationsProcessingComponent } from './applications-processing/applications-processing.component';
import { BuyersFSCApplicationsComponent } from './buyers-fsc-register/applications/buyers-applications.component';
import { BuyersComponent } from './buyers-fsc-register/buyers-register.component';
import { EditShipQuotaComponent } from './catch-quotas-register/edit-ship-quota.component';
import { EditYearlyQuotaComponent } from './catch-quotas-register/edit-yearly-quota.component';
import { ShipQuotasComponent } from './catch-quotas-register/ship-quotas-register.component';
import { TransferShipQuotaComponent } from './catch-quotas-register/transfer-ship-quota.component';
import { TransferYearlyQuotaComponent } from './catch-quotas-register/transfer-yearly-quota.component';
import { YearlyQuotasComponent } from './catch-quotas-register/yearly-quotas-register.component';
import { CommercialFishingApplicationsComponent } from './commercial-fishing-register/applications/commercial-fishing-applications.component';
import { CommercialFishingRegisterComponent } from './commercial-fishing-register/commercial-fishing-register.component';
import { DashboardCardComponent } from './dashboard/dashboard-card.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ErrorLogComponent } from './error-log/error-log.component';
import { LegalEntitiesApplicationsComponent } from './legals/applications/legal-entities-applications.component';
import { LegalEntitiesComponent } from './legals/legal-entities.component';
import { MyProfileAdministrationComponent } from './my-profile-administration/my-profile-administration.component';
import { EditNewsManagementComponent } from './news-management/edit-news-management.component';
import { NewsManagementComponent } from './news-management/news-management.component';
import { EditNomenclatureComponent } from './nomenclatures-register/edit-nomenclatures.component';
import { NomenclaturesComponent } from './nomenclatures-register/nomenclatures.component';
import { EditPermissionComponent } from './permissions-register/components/edit-permission/edit-permission.component';
import { PermissionsRegisterComponent } from './permissions-register/permissions-register.component';
import { EditPoundnetComponent } from './poundnet-register/edit-poundnet.component';
import { PoundnetsComponent } from './poundnet-register/poundnets.component';
import { QualifiedFishersApplicationsComponent } from './qualified-fishers-register/applications/qualified-fishers-applications.component';
import { QualifiedFishersComponent } from './qualified-fishers-register/qualified-fishers.component';
import { RecreationalFishingApplicationsComponent } from './recreational-fishing/applications/recreational-fishing-applications.component';
import { RecreationalFishingAddAssociationComponent } from './recreational-fishing/associations/add-association/recreational-fishing-add-association.component';
import { RecreationalFishingAnnulAssociationComponent } from './recreational-fishing/associations/annul-association/recreational-fishing-annul-association.component';
import { RecreationalFishingEditAssociationComponent } from './recreational-fishing/associations/edit-association/recreational-fishing-edit-association.component';
import { RecreationalFishingAssociationsComponent } from './recreational-fishing/associations/recreational-fishing-associations.component';
import { RecreationalFishingTicketsComponent } from './recreational-fishing/tickets/recreational-fishing-tickets.component';
import { ReportsContentComponent } from './reports/components/report-content/reports-content.component';
import { LegalEntityReportInfoComponent } from './reports/legal-entities-report/components/show-legal-entity-report-info/legal-entity-report-info.component';
import { LegalEntitiesReportComponent } from './reports/legal-entities-report/legal-entities-report.component';
import { PersonsReportInfoComponent } from './reports/persons-report/components/show-persons-report-info/persons-report-info.component';
import { PersonsReportComponent } from './reports/persons-report/persons-report.component';
import { EditRoleComponent } from './roles-register/components/edit-role/edit-role.component';
import { ReplaceRoleComponent } from './roles-register/components/replace-role/replace-role.component';
import { RolesRegisterComponent } from './roles-register/roles-register.component';
import { ScientificFishingApplicationsComponent } from './scientific-fishing-register/applications/scientific-fishing-applications.component';
import { ScientificFishingComponent } from './scientific-fishing-register/scientific-fishing.component';
import { ShipsRegisterApplicationsComponent } from './ships-register/applications/ships-register-applications.component';
import { EditShipRegisterComponent } from './ships-register/edit-ship-register/edit-ship-register.component';
import { ShipsRegisterComponent } from './ships-register/ships-register.component';
import { SystemLogComponent } from './system-log/system-log.component';
import { ViewSystemLogComponent } from './system-log/view-system-log.component';
import { TranslationHelpComponent } from './translation-help/translation-help.component';
import { TranslationLabelsComponent } from './translation-labels/translation-labels.component';
import { EditUserComponent } from './user-management/components/users-content/edit-users/edit-user.component';
import { UsersContentComponent } from './user-management/components/users-content/users-content.component';
import { ExternalUsersComponent } from './user-management/external-users/external-users.component';
import { EditAccessDialogComponent } from './user-management/internal-users/components/edit-access-device-mobile-dialog/edit-access-dialog.component';
import { EditAccessComponent } from './user-management/internal-users/components/edit-access-device-mobile/edit-access.component';
import { InternalUsersComponent } from './user-management/internal-users/internal-users.component';
import { ReportDefinitionComponent } from './reports/report-definition/report-definition.component';
import { MaximumFishingCapacityComponent } from './fishing-capacity/maximum-fishing-capacity/maximum-fishing-capacity.component';
import { EditMaximumFishingCapacityComponent } from './fishing-capacity/maximum-fishing-capacity/edit-maximum-fishing-capacity/edit-maximum-fishing-capacity.component';
import { FishingCapacityApplicationsComponent } from './fishing-capacity/applications/fishing-capacity-applications.component';
import { UnregisteredInspectorsComponent } from './inspectors-register/unregistered-inspectors.component';
import { EditReportDefinitionComponent } from './reports/report-definition/edit-report-definition.component';
import { FishingCapacityCertificatesRegisterComponent } from './fishing-capacity/fishing-capacity-certificates-register/fishing-capacity-certificates-register.component';
import { FishingCapacityRegisterComponent } from './fishing-capacity/fishing-capacity-register/fishing-capacity-register.component';
import { EditCapacityCertificateComponent } from './fishing-capacity/fishing-capacity-certificates-register/edit-capacity-certificate/edit-capacity-certificate.component';
import { TLEGovPaymentsModule } from '@tl/tl-egov-payments';
import { AquacultureFacilitiesApplicationsComponent } from './aquaculture-facilities/applications/aquaculture-facilities-applications.component';
import { AquacultureFacilitiesComponent } from './aquaculture-facilities/aquaculture-facilities.component';
import { PatrolVehiclesComponent } from './patrol-vehicles/patrol-vehicles.component';
import { EditPatrolVehiclesComponent } from './patrol-vehicles/edit-patrol-vehicles.component';
import { TLAngularMapModule } from '@tl/tl-angular-map';
import { CatchesAndSalesComponent } from './catches-and-sales/catches-and-sales.component';
import { FishingCapacityAnalysisComponent } from './fishing-capacity/analysis/fishing-capacity-analysis.component';
import { ReportParameterDefinitionComponent } from './reports/report-parameter-definition/report-parameter-definition.component';
import { EditReportParameterDefinitionComponent } from './reports/report-parameter-definition/edit-report-parameter-definition.component';
import { CrossChecksComponent } from './cross-checks/cross-checks.component';
import { CrossChecksResultsComponent } from './cross-checks/cross-checks-results/cross-checks-results.component';
import { EditCrossCheckComponent } from './cross-checks/edit-cross-check/edit-cross-check.component';
import { IconPickerModule } from 'ngx-icon-picker';
import { ReportSqlComponent } from './reports/report-definition/report-sql.component';
import { MonacoEditorModule } from 'ngx-monaco-editor';
import { EditAuanComponent } from './control-activity/auan-register/edit-auan/edit-auan.component';
import { EditAuanInspectionPickerComponent } from './control-activity/auan-register/edit-auan-inspection-picker/edit-auan-inspection-picker.component';
import { CrossCheckResultsAssignedUserComponent } from './cross-checks/cross-check-results-assigned-user/cross-check-results-assigned-user.component';
import { CrossCheckResultsResolutionComponent } from './cross-checks/cross-check-results-resolution/cross-check-results-resolution.component';
import { StatisticalFormsComponent } from './statistical-forms/statistical-forms.component';
import { EditPenalDecreeComponent } from './control-activity/penal-decrees/edit-penal-decree/edit-penal-decree.component';
import { EditPenalDecreeAuanPickerComponent } from './control-activity/penal-decrees/edit-penal-decree-auan-picker/edit-penal-decree-auan-picker.component';
import { StatisticalFormsApplicationsComponent } from './statistical-forms/applications/statistical-forms-applications.component';
import { ReportViewComponent } from './reports/report-view/report-view.component';
import { FluxVmsRequestsComponent } from './flux-vms-requests/flux-vms-requests.component';
import { ViewFluxVmsRequestsComponent } from './flux-vms-requests/view-flux-vms-requests.component';
import { NgxJsonViewerModule } from 'ngx-json-viewer';
import { ApplicationRegixChecksComponent } from './applications-regix-checks/application-regix-checks.component';
import { ViewApplicationRegixChecksComponent } from './applications-regix-checks/view-application-regix-checks.component';
import { EditInspectorComponent } from './control-activity/inspections/dialogs/edit-inspector/edit-inspector.component';
import { InspectorsTableComponent } from './control-activity/inspections/components/inspectors-table/inspectors-table.component';
import { InspectorsRegisterComponent } from './inspectors-register/inspectors-register.component';
import { InspectionGeneralInfoComponent } from './control-activity/inspections/components/inspection-general-info/inspection-general-info.component';
import { EditPatrolVehicleComponent } from './control-activity/inspections/dialogs/edit-patrol-vehicle/edit-patrol-vehicle.component';
import { PatrolVehiclesTableComponent } from './control-activity/inspections/components/patrol-vehicles-table/patrol-vehicles-table.component';
import { InspectionMapViewerComponent } from './control-activity/inspections/components/inspection-map-viewer/inspection-map-viewer.component';
import { InspectedEntityBasicInfoComponent } from './control-activity/penal-decrees/inspected-entity-basic-info/inspected-entity-basic-info.component';
import { DecreeDeliveryDataComponent } from './control-activity/penal-decrees/decree-delivery-data/decree-delivery-data.component';
import { DecreeSizedFishComponent } from './control-activity/penal-decrees/decree-sized-fish/decree-sized-fish.component';
import { DecreeSizedFishingGearComponent } from './control-activity/penal-decrees/decree-sized-fishing-gear/decree-sized-fishing-gear.component';
import { EditDecreeAgreementComponent } from './control-activity/penal-decrees/edit-decree-agreement/edit-decree-agreement.component';
import { EditDecreeWarningComponent } from './control-activity/penal-decrees/edit-decree-warning/edit-decree-warning.component';
import { EditPenalDecreeStatusComponent } from './control-activity/penal-decrees/edit-penal-decree-status/edit-penal-decree-status.component';
import { DecreeAuanBasicInfoComponent } from './control-activity/penal-decrees/decree-auan-basic-info/decree-auan-basic-info.component';
import { EditInspectionAtSeaComponent } from './control-activity/inspections/dialogs/edit-inspection-at-sea/edit-inspection-at-sea.component';
import { InspectionSelectionComponent } from './control-activity/inspections/dialogs/inspection-selection/inspection-selection.component';
import { InspectedPortComponent } from './control-activity/inspections/components/inspected-port/inspected-port.component';
import { InspectedShipComponent } from './control-activity/inspections/components/inspected-ship/inspected-ship.component';
import { InspectedShipSubjectComponent } from './control-activity/inspections/components/inspected-ship-subject/inspected-ship-subject.component';
import { InspectedShipWithPersonnelComponent } from './control-activity/inspections/components/inspected-ship-with-personnel/inspected-ship-with-personnel.component';
import { InspectionToggleListComponent } from './control-activity/inspections/components/inspection-toggle-list/inspection-toggle-list.component';
import { InspectionToggleComponent } from './control-activity/inspections/components/inspection-toggle/inspection-toggle.component';
import { InspectedShipSectionsComponent } from './control-activity/inspections/components/inspected-ship-sections/inspected-ship-sections.component';
import { PermitLicensesTableComponent } from './control-activity/inspections/components/permit-licenses-table/permit-licenses-table.component';
import { ReviewOldInspectionComponent } from './control-activity/inspections/dialogs/review-old-inspection/review-old-inspection.component';
import { InspectedPermitLicensesTableComponent } from './control-activity/inspections/components/inspected-permit-licenses-table/inspected-permit-licenses-table.component';
import { InspectedLogBooksTableComponent } from './control-activity/inspections/components/inspected-log-books-table/inspected-log-books-table.component';
import { EditPenalPointsComponent } from './control-activity/awarded-points/edit-penal-points/edit-penal-points.component';
import { EditPenalPointsDecreePickerComponent } from './control-activity/awarded-points/edit-penal-points-decree-picker/edit-penal-points-decree-picker.component';
import { EditPenalPointsComplaintStatusComponent } from './control-activity/awarded-points/edit-penal-points-complaint-status/edit-penal-points-complaint-status.component';
import { InspectedCatchesTableComponent } from './control-activity/inspections/components/inspected-catches-table/inspected-catches-table.component';
import { InspectionAdditionalInfoComponent } from './control-activity/inspections/components/inspection-additional-info/inspection-additional-info.component';
import { InspectedFishingGearsTableComponent } from './control-activity/inspections/components/inspected-fishing-gears-table/inspected-fishing-gears-table.component';
import { EditInspectedFishingGearComponent } from './control-activity/inspections/dialogs/edit-inspected-fishing-gear/edit-inspected-fishing-gear.component';
import { EditInspectionAtPortComponent } from './control-activity/inspections/dialogs/edit-inspection-at-port/edit-inspection-at-port.component';
import { EditInspectionTransshipmentComponent } from './control-activity/inspections/dialogs/edit-inspection-transshipment/edit-inspection-transshipment.component';
import { EditObservationAtSeaComponent } from './control-activity/inspections/dialogs/edit-observation-at-sea/edit-observation-at-sea.component';
import { EditInspectionAtMarketComponent } from './control-activity/inspections/dialogs/edit-inspection-at-market/edit-inspection-at-market.component';
import { InspectedBuyerComponent } from './control-activity/inspections/components/inspected-buyer/inspected-buyer.component';
import { InspectedPersonComponent } from './control-activity/inspections/components/inspected-person/inspected-person.component';
import { EditInspectionVehicleComponent } from './control-activity/inspections/dialogs/edit-inspection-vehicle/edit-inspection-vehicle.component';
import { InspectedSubjectComponent } from './control-activity/inspections/components/inspected-subject/inspected-subject.component';
import { TranslationManagementComponent } from './translation-management/translation-management.component';
import { EditTranslationComponent } from './translation-management/edit-translation.component';
import { EditInspectionAquacultureComponent } from './control-activity/inspections/dialogs/edit-inspection-aquaculture/edit-inspection-aquaculture.component';
import { EditInspectionPersonComponent } from './control-activity/inspections/dialogs/edit-inspection-person/edit-inspection-person.component';
import { EditCheckWaterObjectComponent } from './control-activity/inspections/dialogs/edit-check-water-object/edit-check-water-object.component';
import { WaterFishingGearsTableComponent } from './control-activity/inspections/dialogs/edit-check-water-object/components/water-fishing-gears-table/water-fishing-gears-table.component';
import { EditWaterFishingGearComponent } from './control-activity/inspections/dialogs/edit-check-water-object/components/edit-water-fishing-gear/edit-water-fishing-gear.component';
import { EditWaterVesselComponent } from './control-activity/inspections/dialogs/edit-check-water-object/components/edit-water-vessel/edit-water-vessel.component';
import { WaterVesselsTableComponent } from './control-activity/inspections/dialogs/edit-check-water-object/components/water-vessels-table/water-vessels-table.component';
import { EditWaterEngineComponent } from './control-activity/inspections/dialogs/edit-check-water-object/components/edit-water-engine/edit-water-engine.component';
import { WaterEnginesTableComponent } from './control-activity/inspections/dialogs/edit-check-water-object/components/water-engines-table/water-engines-table.component';
import { EditWaterCatchComponent } from './control-activity/inspections/dialogs/edit-check-water-object/components/edit-water-catch/edit-water-catch.component';
import { WaterCatchesTableComponent } from './control-activity/inspections/dialogs/edit-check-water-object/components/water-catches-table/water-catches-table.component';
import { EditInspectionFishingGearComponent } from './control-activity/inspections/dialogs/edit-inspection-fishing-gear/edit-inspection-fishing-gear.component';
import { EditMarketCatchComponent } from './control-activity/inspections/dialogs/edit-inspection-at-market/components/edit-market-catch/edit-market-catch.component';
import { MarketCatchesTableComponent } from './control-activity/inspections/dialogs/edit-inspection-at-market/components/market-catches-table/market-catches-table.component';
import { InspectedLegalComponent } from './control-activity/inspections/components/inspected-legal/inspected-legal.component';
import { AuanWitnessComponent } from './control-activity/auan-register/auan-witness/auan-witness.component';
import { AuanWitnessesArrayComponent } from './control-activity/auan-register/auan-witnesses-array/auan-witnesses-array.component';
import { AuanViolatedRegulationsComponent } from './control-activity/auan-register/auan-violated-regulations/auan-violated-regulations.component';
import { SignInspectionComponent } from './control-activity/inspections/dialogs/sign-inspection/sign-inspection.component';
import { FluxFlapRequestsComponent } from './flux-vms-requests/flux-flap-requests/flux-flap-requests.component';
import { EditFluxFlapRequestComponent } from './flux-vms-requests/flux-flap-requests/edit-flux-flap-request/edit-flux-flap-request.component';
import { InspectedPermitsTableComponent } from './control-activity/inspections/components/inspected-permits-table/inspected-permits-table.component';
import { EditDecreeResolutionComponent } from './control-activity/penal-decrees/edit-decree-resolution/edit-decree-resolution.component';
import { ChooseLawSectionsComponent } from './control-activity/auan-register/choose-law-sections/choose-law-sections.component';
import { PrintConfigurationsComponent } from './print-configurations/print-configurations.component';
import { EditPrintConfigurationComponent } from './print-configurations/components/edit-print-configuration/edit-print-configuration.component';
import { LogBookPageEditExceptionsComponent } from './catches-and-sales/log-book-page-edit-exceptions/log-book-page-edit-exceptions.component';
import { EditLogBookPageEditExceptionComponent } from './catches-and-sales/log-book-page-edit-exceptions/components/edit-log-book-page-edit-exception.component';
import { FluxSalesQueryComponent } from './flux-vms-requests/flux-sales-query/flux-sales-query.component';
import { FluxVesselQueryComponent } from './flux-vms-requests/flux-vessel-query/flux-vessel-query.component';
import { FluxFAQueryComponent } from './flux-vms-requests/flux-fa-query/flux-fa-query.component';
import { FluxAcdrRequestsComponent } from './flux-vms-requests/flux-acdr-requests/flux-acdr-requests.component';
import { EditFluxAcdrRequestsComponent } from './flux-vms-requests/flux-acdr-requests/edit-flux-acdr-request/edit-flux-acdr-request.component';
import { UploadFluxAcdrRequestsComponent } from './flux-vms-requests/flux-acdr-requests/upload-flux-acdr-request/upload-flux-acdr-request.component';
import { FishingActivityReportsComponent } from './fishing-activity-reports/fishing-activity-reports.component';
import { OnlineApplicationsComponent } from './applications-processing/online-applications/online-applications.component';
import { MatBadgeModule } from '@angular/material/badge';
import { AuanDeliveryDataComponent } from './control-activity/auan-register/auan-delivery-data/auan-delivery-data.component';
import { AuanDeliveryComponent } from './control-activity/auan-register/auan-delivery/auan-delivery.component';
import { TLTranslatePipe } from '@app/shared/pipes/tl-translate.pipe';

@NgModule({
    declarations: [
        ApplicationsProcessingComponent,
        DashboardComponent,
        EditAccessComponent,
        EditAccessDialogComponent,
        EditNomenclatureComponent,
        EditPermissionComponent,
        EditPoundnetComponent,
        EditRoleComponent,
        EditUserComponent,
        ExternalUsersComponent,
        InternalUsersComponent,
        LegalEntitiesComponent,
        LegalEntitiesApplicationsComponent,
        NomenclaturesComponent,
        PermissionsRegisterComponent,
        PoundnetsComponent,
        QualifiedFishersComponent,
        QualifiedFishersApplicationsComponent,
        RecreationalFishingAnnulAssociationComponent,
        RecreationalFishingApplicationsComponent,
        RecreationalFishingAssociationsComponent,
        RecreationalFishingAddAssociationComponent,
        RecreationalFishingEditAssociationComponent,
        RecreationalFishingTicketsComponent,
        ReplaceRoleComponent,
        RolesRegisterComponent,
        ScientificFishingComponent,
        ScientificFishingApplicationsComponent,
        UsersContentComponent,
        ReportsContentComponent,
        PersonsReportComponent,
        LegalEntitiesReportComponent,
        PersonsReportInfoComponent,
        LegalEntityReportInfoComponent,
        TranslationLabelsComponent,
        TranslationHelpComponent,
        BuyersComponent,
        BuyersFSCApplicationsComponent,
        MyProfileAdministrationComponent,
        ShipsRegisterComponent,
        ShipsRegisterApplicationsComponent,
        ShipQuotasComponent,
        EditShipQuotaComponent,
        YearlyQuotasComponent,
        EditYearlyQuotaComponent,
        TransferYearlyQuotaComponent,
        TransferShipQuotaComponent,
        ShipsRegisterApplicationsComponent,
        ErrorLogComponent,
        SystemLogComponent,
        ViewSystemLogComponent,
        NewsManagementComponent,
        EditNewsManagementComponent,
        EditShipRegisterComponent,
        DashboardCardComponent,
        CommercialFishingApplicationsComponent,
        CommercialFishingRegisterComponent,
        InspectorsTableComponent,
        InspectionMapViewerComponent,
        InspectedShipComponent,
        InspectionGeneralInfoComponent,
        InspectedShipSubjectComponent,
        EditInspectorComponent,
        EditInspectionAtSeaComponent,
        InspectionSelectionComponent,
        InspectedPortComponent,
        InspectedShipWithPersonnelComponent,
        InspectionToggleListComponent,
        InspectionToggleComponent,
        InspectedShipSectionsComponent,
        ReviewOldInspectionComponent,
        InspectedPermitLicensesTableComponent,
        InspectedPermitsTableComponent,
        InspectedLogBooksTableComponent,
        PermitLicensesTableComponent,
        ReportViewComponent,
        ReportDefinitionComponent,
        MaximumFishingCapacityComponent,
        EditMaximumFishingCapacityComponent,
        InspectorsRegisterComponent,
        EditPatrolVehicleComponent,
        PatrolVehiclesTableComponent,
        FishingCapacityApplicationsComponent,
        FishingCapacityCertificatesRegisterComponent,
        FishingCapacityRegisterComponent,
        EditCapacityCertificateComponent,
        FishingCapacityAnalysisComponent,
        UnregisteredInspectorsComponent,
        EditReportDefinitionComponent,
        AquacultureFacilitiesApplicationsComponent,
        AquacultureFacilitiesComponent,
        PatrolVehiclesComponent,
        EditPatrolVehiclesComponent,
        CatchesAndSalesComponent,
        ReportParameterDefinitionComponent,
        EditReportParameterDefinitionComponent,
        CrossChecksComponent,
        CrossChecksResultsComponent,
        EditCrossCheckComponent,
        ReportSqlComponent,
        EditAuanComponent,
        EditAuanInspectionPickerComponent,
        CrossCheckResultsAssignedUserComponent,
        CrossCheckResultsResolutionComponent,
        StatisticalFormsComponent,
        StatisticalFormsApplicationsComponent,
        EditPenalDecreeComponent,
        EditPenalDecreeAuanPickerComponent,
        ReportViewComponent,
        FluxVmsRequestsComponent,
        ViewFluxVmsRequestsComponent,
        ApplicationRegixChecksComponent,
        ViewApplicationRegixChecksComponent,
        InspectedEntityBasicInfoComponent,
        DecreeDeliveryDataComponent,
        DecreeSizedFishComponent,
        DecreeSizedFishingGearComponent,
        EditDecreeAgreementComponent,
        EditDecreeWarningComponent,
        EditPenalDecreeStatusComponent,
        DecreeAuanBasicInfoComponent,
        EditPenalPointsComponent,
        EditPenalPointsDecreePickerComponent,
        EditPenalPointsComplaintStatusComponent,
        InspectedCatchesTableComponent,
        InspectionAdditionalInfoComponent,
        InspectedFishingGearsTableComponent,
        EditInspectedFishingGearComponent,
        EditInspectionAtPortComponent,
        EditInspectionTransshipmentComponent,
        EditObservationAtSeaComponent,
        EditInspectionAtMarketComponent,
        InspectedBuyerComponent,
        InspectedPersonComponent,
        EditInspectionVehicleComponent,
        InspectedSubjectComponent,
        TranslationManagementComponent,
        EditTranslationComponent,
        EditInspectionAquacultureComponent,
        EditInspectionPersonComponent,
        EditCheckWaterObjectComponent,
        EditWaterFishingGearComponent,
        WaterFishingGearsTableComponent,
        EditWaterVesselComponent,
        WaterVesselsTableComponent,
        EditWaterEngineComponent,
        WaterEnginesTableComponent,
        EditWaterCatchComponent,
        WaterCatchesTableComponent,
        EditInspectionFishingGearComponent,
        EditMarketCatchComponent,
        MarketCatchesTableComponent,
        InspectedLegalComponent,
        AuanWitnessComponent,
        AuanWitnessesArrayComponent,
        AuanViolatedRegulationsComponent,
        SignInspectionComponent,
        FluxFlapRequestsComponent,
        EditFluxFlapRequestComponent,
        EditDecreeResolutionComponent,
        ChooseLawSectionsComponent,
        PrintConfigurationsComponent,
        EditPrintConfigurationComponent,
        LogBookPageEditExceptionsComponent,
        EditLogBookPageEditExceptionComponent,
        FluxSalesQueryComponent,
        FluxVesselQueryComponent,
        FluxFAQueryComponent,
        FluxAcdrRequestsComponent,
        EditFluxAcdrRequestsComponent,
        UploadFluxAcdrRequestsComponent,
        FishingActivityReportsComponent,
        OnlineApplicationsComponent,
        AuanDeliveryDataComponent,
        AuanDeliveryComponent
    ],
    imports: [
        TLCommonModule,
        MaterialModule,
        CommonApplicationModule,
        RouterModule.forChild(MainNavigation.getRoutes()),
        TLEGovPaymentsModule,
        TLEPayPaymentsModule,
        TLBoricaPaymentsModule,
        NgApexchartsModule,
        TLAngularMapModule,
        IconPickerModule,
        MonacoEditorModule.forRoot(),
        NgxJsonViewerModule,
        MatBadgeModule
    ],
    exports: [
        ApplicationsProcessingComponent,
        DashboardComponent,
        EditNomenclatureComponent,
        EditPermissionComponent,
        EditPoundnetComponent,
        EditRoleComponent,
        LegalEntitiesComponent,
        LegalEntitiesApplicationsComponent,
        NomenclaturesComponent,
        PermissionsRegisterComponent,
        PoundnetsComponent,
        QualifiedFishersComponent,
        QualifiedFishersApplicationsComponent,
        RecreationalFishingApplicationsComponent,
        RecreationalFishingAssociationsComponent,
        RecreationalFishingAddAssociationComponent,
        RecreationalFishingEditAssociationComponent,
        RecreationalFishingAnnulAssociationComponent,
        RecreationalFishingTicketsComponent,
        ReplaceRoleComponent,
        RolesRegisterComponent,
        ScientificFishingComponent,
        ScientificFishingApplicationsComponent,
        UsersContentComponent,
        ReportsContentComponent,
        PersonsReportInfoComponent,
        LegalEntityReportInfoComponent,
        TranslationLabelsComponent,
        TranslationHelpComponent,
        BuyersComponent,
        BuyersFSCApplicationsComponent,
        MyProfileAdministrationComponent,
        ShipsRegisterComponent,
        ShipsRegisterApplicationsComponent,
        ShipQuotasComponent,
        EditShipQuotaComponent,
        YearlyQuotasComponent,
        EditYearlyQuotaComponent,
        TransferYearlyQuotaComponent,
        TransferShipQuotaComponent,
        ShipsRegisterApplicationsComponent,
        SystemLogComponent,
        ViewSystemLogComponent,
        ErrorLogComponent,
        NewsManagementComponent,
        EditNewsManagementComponent,
        EditShipRegisterComponent,
        DashboardCardComponent,
        CommercialFishingApplicationsComponent,
        CommercialFishingRegisterComponent,
        InspectorsTableComponent,
        InspectionMapViewerComponent,
        InspectedShipComponent,
        InspectionGeneralInfoComponent,
        InspectedShipSubjectComponent,
        EditInspectorComponent,
        InspectionSelectionComponent,
        InspectedPortComponent,
        InspectedShipWithPersonnelComponent,
        InspectionToggleListComponent,
        InspectionToggleComponent,
        InspectedShipSectionsComponent,
        ReviewOldInspectionComponent,
        InspectedPermitLicensesTableComponent,
        InspectedPermitsTableComponent,
        InspectedLogBooksTableComponent,
        PermitLicensesTableComponent,
        EditInspectionAtSeaComponent,
        ReportViewComponent,
        ReportDefinitionComponent,
        MaximumFishingCapacityComponent,
        EditMaximumFishingCapacityComponent,
        InspectorsRegisterComponent,
        EditPatrolVehicleComponent,
        PatrolVehiclesTableComponent,
        FishingCapacityApplicationsComponent,
        FishingCapacityCertificatesRegisterComponent,
        FishingCapacityRegisterComponent,
        EditCapacityCertificateComponent,
        FishingCapacityAnalysisComponent,
        UnregisteredInspectorsComponent,
        EditReportDefinitionComponent,
        AquacultureFacilitiesApplicationsComponent,
        AquacultureFacilitiesComponent,
        PatrolVehiclesComponent,
        EditPatrolVehiclesComponent,
        CatchesAndSalesComponent,
        ReportParameterDefinitionComponent,
        EditReportParameterDefinitionComponent,
        CrossChecksComponent,
        CrossChecksResultsComponent,
        EditCrossCheckComponent,
        ReportSqlComponent,
        EditAuanComponent,
        EditAuanInspectionPickerComponent,
        CrossCheckResultsAssignedUserComponent,
        CrossCheckResultsResolutionComponent,
        StatisticalFormsComponent,
        StatisticalFormsApplicationsComponent,
        EditPenalDecreeComponent,
        EditPenalDecreeAuanPickerComponent,
        ReportViewComponent,
        FluxVmsRequestsComponent,
        ViewFluxVmsRequestsComponent,
        ApplicationRegixChecksComponent,
        ViewApplicationRegixChecksComponent,
        InspectedEntityBasicInfoComponent,
        DecreeDeliveryDataComponent,
        DecreeSizedFishComponent,
        DecreeSizedFishingGearComponent,
        EditDecreeAgreementComponent,
        EditDecreeWarningComponent,
        EditPenalDecreeStatusComponent,
        DecreeAuanBasicInfoComponent,
        EditPenalPointsComponent,
        EditPenalPointsDecreePickerComponent,
        EditPenalPointsComplaintStatusComponent,
        InspectionAdditionalInfoComponent,
        InspectedFishingGearsTableComponent,
        EditInspectedFishingGearComponent,
        EditInspectionAtPortComponent,
        EditInspectionTransshipmentComponent,
        EditObservationAtSeaComponent,
        EditInspectionAtMarketComponent,
        InspectedBuyerComponent,
        InspectedPersonComponent,
        EditInspectionVehicleComponent,
        InspectedSubjectComponent,
        TranslationManagementComponent,
        EditTranslationComponent,
        EditInspectionAquacultureComponent,
        EditInspectionPersonComponent,
        EditCheckWaterObjectComponent,
        EditWaterFishingGearComponent,
        WaterFishingGearsTableComponent,
        EditWaterVesselComponent,
        WaterVesselsTableComponent,
        EditWaterEngineComponent,
        WaterEnginesTableComponent,
        EditWaterCatchComponent,
        WaterCatchesTableComponent,
        EditInspectionFishingGearComponent,
        EditMarketCatchComponent,
        MarketCatchesTableComponent,
        AuanWitnessComponent,
        AuanWitnessesArrayComponent,
        AuanViolatedRegulationsComponent,
        SignInspectionComponent,
        FluxFlapRequestsComponent,
        EditFluxFlapRequestComponent,
        EditDecreeResolutionComponent,
        ChooseLawSectionsComponent,
        PrintConfigurationsComponent,
        EditPrintConfigurationComponent,
        LogBookPageEditExceptionsComponent,
        EditLogBookPageEditExceptionComponent,
        FluxSalesQueryComponent,
        FluxVesselQueryComponent,
        FluxFAQueryComponent,
        FluxAcdrRequestsComponent,
        EditFluxAcdrRequestsComponent,
        UploadFluxAcdrRequestsComponent,
        FishingActivityReportsComponent,
        OnlineApplicationsComponent,
        AuanDeliveryDataComponent,
        AuanDeliveryComponent
    ],
    providers: [
        TLTranslatePipe
    ]
})
export class IARAApplicationModule {

}