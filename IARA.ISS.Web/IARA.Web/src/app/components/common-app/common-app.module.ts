import { CurrencyPipe, DatePipe } from '@angular/common';
import { NgModule } from '@angular/core';
import { MaterialModule } from '@app/shared/material.module';
import { TLDateDifferencePipe } from '@app/shared/pipes/tl-date-difference.pipe';
import { TLCommonModule } from '@app/shared/tl-common.module';
import { TLAngularMapModule } from '@tl/tl-angular-map';
import { TLEGovPaymentsModule } from '@tl/tl-egov-payments';
import { ApplicationsRegisterComponent } from './applications/applications-register/applications-register.component';
import { ApplicationsTableComponent } from './applications/applications-table/applications-table.component';
import { AssignApplicationByAccessCodeComponent } from './applications/components/assign-application-by-access-code/assign-application-by-access-code.component';
import { ChooseApplicationTypeComponent } from './applications/components/choose-application-type/choose-application-type.component';
import { ChooseApplicationComponent } from './applications/components/choose-application/choose-application.component';
import { EnterEventisNumberComponent } from './applications/components/enter-eventis-number/enter-eventis-number.component';
import { EnterReasonComponent } from './applications/components/enter-reason/enter-reason.component';
import { FileInApplicationStepperComponent } from './applications/components/file-in-application-stepper/file-in-application-stepper.component';
import { PaymentInformationComponent } from './applications/components/payment-information/payment-information.component';
import { UploadFileDialogComponent } from './applications/components/upload-file-dialog/upload-file-dialog.component';
import { AquacultureChangeOfCircumstancesComponent } from './aquaculture-facilities/aquaculture-change-of-circumstances/aquaculture-change-of-circumstances.component';
import { AquacultureDeregistrationComponent } from './aquaculture-facilities/aquaculture-deregistration/aquaculture-deregistration.component';
import { EditAquacultureFacilityComponent } from './aquaculture-facilities/edit-aquaculture-facility/edit-aquaculture-facility.component';
import { EditAquacultureInstallationComponent } from './aquaculture-facilities/edit-aquaculture-facility/edit-aquaculture-installation/edit-aquaculture-installation.component';
import { EditInstallationNetCageComponent } from './aquaculture-facilities/edit-aquaculture-facility/edit-installation-net-cage/edit-installation-net-cage.component';
import { EditAquacultureWaterLawCertificateComponent } from './aquaculture-facilities/edit-aquaculture-facility/edit-water-law-certificate/edit-aquaculture-water-law-certificate.component';
import { BuyerChangeOfCircumstancesComponent } from './buyers/buyer-change-of-circumstances/buyer-change-of-circumstances.component';
import { BuyerTerminationComponent } from './buyers/buyer-termination/buyer-termination.component';
import { EditBuyersComponent } from './buyers/edit-buyers.component';
import { CatchesAndSalesContent } from './catches-and-sales/catches-and-sales-content.component';
import { AddLogBookPageWizardComponent } from './catches-and-sales/components/add-log-book-page-wizard/add-log-book-page-wizard.component';
import { AddShipPageDocumentWizardComponent } from './catches-and-sales/components/add-ship-page-document-wizard/add-ship-page-document-wizard.component';
import { AddShipPageWizardComponent } from './catches-and-sales/components/add-ship-page-wizard/add-ship-page-wizard.component';
import { AdmissionPagesAndDeclarationsTableComponent } from './catches-and-sales/components/admission-pages-and-declarations-table/admission-pages-and-declarations-table.component';
import { AquaculturePagesTableComponent } from './catches-and-sales/components/aquaculture-pages-table/aquaculture-pages-table.component';
import { CatchAquaticOrganismTypeComponent } from './catches-and-sales/components/catch-aquatic-organism-type/catch-aquatic-organism-type.component';
import { CatchAquaticOrganismTypesArrayComponent } from './catches-and-sales/components/catch-aquatic-organism-types-array/catch-aquatic-organism-types-array.component';
import { EditCatchRecordComponent } from './catches-and-sales/components/catch-record/edit-catch-record.component';
import { CommonLogBookPageDataComponent } from './catches-and-sales/components/common-log-book-page-data/common-log-book-page-data.component';
import { EditAdmissionLogBookPageComponent } from './catches-and-sales/components/edit-admission-log-book/edit-admission-log-book-page.component';
import { EditAquacultureLogBookPageComponent } from './catches-and-sales/components/edit-aquaculture-log-book-page/edit-aquaculture-log-book-page.component';
import { EditFirstSaleLogBookPageComponent } from './catches-and-sales/components/edit-first-sale-log-book/edit-first-sale-log-book-page.component';
import { EditOriginDeclarationComponent } from './catches-and-sales/components/edit-origin-declaration/edit-origin-declaration.component';
import { EditTransportationLogBookPageComponent } from './catches-and-sales/components/edit-transporation-log-book/edit-transportation-log-book-page.component';
import { FirstSalePagesTableComponent } from './catches-and-sales/components/first-sale-pages-table/first-sale-pages-table.component';
import { JustifiedCancellationComponent } from './catches-and-sales/components/justified-cancellation/justified-cancellation.component';
import { LogBookPagePersonComponent } from './catches-and-sales/components/log-book-page-person/log-book-page-person.component';
import { EditLogBookPageProductComponent } from './catches-and-sales/components/log-book-page-products/components/edit-log-book-page-product/edit-log-book-page-product.component';
import { LogBookPageProductsComponent } from './catches-and-sales/components/log-book-page-products/log-book-page-products.component';
import { PreviousTripsCatchRecordsComponent } from './catches-and-sales/components/previous-trips-catch-records/previous-trips-catch-records.component';
import { EditShipLogBookPageComponent } from './catches-and-sales/components/ship-log-book/edit-ship-log-book-page.component';
import { ShipPagesAndDeclarationsTableComponent } from './catches-and-sales/components/ship-pages-and-declarations-table/ship-pages-and-declarations-table.component';
import { TransportationPagesAndDeclarationsTableComponent } from './catches-and-sales/components/transportation-pages-and-declarations-table/transportation-pages-and-declarations-table.component';
import { ChoosePermitLicenseForRenewalComponent } from './commercial-fishing/components/choose-permit-license-for-renewal/choose-permit-license-for-renewal.component';
import { ChoosePermitToCopyFromComponent } from './commercial-fishing/components/choose-permit-to-copy-from/choose-permit-to-copy-from.component';
import { EditCommercialFishingComponent } from './commercial-fishing/components/edit-commercial-fishing/edit-commercial-fishing.component';
import { EditLogBookComponent } from './commercial-fishing/components/edit-log-book/edit-log-book.component';
import { EditSuspensionComponent } from './commercial-fishing/components/edit-suspension/edit-suspension.component';
import { EditFishingGearComponent } from './commercial-fishing/components/fishing-gears/components/edit-fishing-gear.component';
import { FishingGearsComponent } from './commercial-fishing/components/fishing-gears/fishing-gears.component';
import { GroudForUseComponent } from './commercial-fishing/components/ground-for-use/ground-for-use.component';
import { LogBooksComponent } from './commercial-fishing/components/log-books/log-books.component';
import { DuplicateEntriesTableComponent } from './duplicates/duplicate-entires-table/duplicate-entries-table.component';
import { DuplicatesApplicationComponent } from './duplicates/duplicates-application.component';
import { AcquiredFishingCapacityComponent } from './fishing-capacity/acquired-fishing-capacity/acquired-fishing-capacity.component';
import { CapacityCertificateDuplicateComponent } from './fishing-capacity/capacity-certificate-duplicate/capacity-certificate-duplicate.component';
import { FishingCapacityFreedActionsComponent } from './fishing-capacity/fishing-capacity-freed-actions/fishing-capacity-freed-actions.component';
import { IncreaseFishingCapacityComponent } from './fishing-capacity/increase-fishing-capacity/increase-fishing-capacity.component';
import { ReduceFishingCapacityComponent } from './fishing-capacity/reduce-fishing-capacity/reduce-fishing-capacity.component';
import { TransferFishingCapacityTableEntryComponent } from './fishing-capacity/transfer-fishing-capacity-table/transfer-fishing-capacity-table-entry/transfer-fishing-capacity-table-entry.component';
import { TransferFishingCapacityTableComponent } from './fishing-capacity/transfer-fishing-capacity-table/transfer-fishing-capacity-table.component';
import { TransferFishingCapacityComponent } from './fishing-capacity/transfer-fishing-capacity/transfer-fishing-capacity.component';
import { EditAuthorizedPersonComponent } from './legals/edit-authorized-person/edit-authorized-person.component';
import { EditLegalEntityComponent } from './legals/edit-legal-entity/edit-legal-entity.component';
import { ChangePasswordComponent } from './my-profile/components/change-password/change-password.component';
import { ChangeUserDataComponent } from './my-profile/components/change-userdata/change-userdata.component';
import { MyProfileContentComponent } from './my-profile/my-profile-content.component';
import { OnlinePaymentPageComponent } from './online-payment-page/online-payment-page.component';
import { EditFisherComponent } from './qualified-fishers/edit-fisher.component';
import { IssueDuplicateTicketComponent } from './recreational-fishing/applications/components/issue-duplicate-ticket/issue-duplicate-ticket.component';
import { RecreationalFishingApplicationsContentComponent } from './recreational-fishing/applications/recreational-fishing-applications-content.component';
import { RecreationalFishingTicketSummaryComponent } from './recreational-fishing/tickets/components/ticket-summary/recreational-fishing-ticket-summary.component';
import { RecreationalFishingTicketComponent } from './recreational-fishing/tickets/components/ticket/recreational-fishing-ticket.component';
import { RecreationalFishingTicketsContentComponent } from './recreational-fishing/tickets/recreational-fishing-tickets-content.component';
import { ReportExecutionComponent } from './reports/report-execution/report-execution.component';
import { EditReportViewComponent } from './reports/report-view-content/edit-report-view.component';
import { ReportViewContentComponent } from './reports/report-view-content/report-view-content.component';
import { EditScientificFishingOutingComponent } from './scientific-fishing/components/edit-scientific-fishing-outing/edit-scientific-fishing-outing.component';
import { EditScientificPermitHolderComponent } from './scientific-fishing/components/edit-scientific-permit-holder/edit-scientific-permit-holder.component';
import { EditScientificPermitComponent } from './scientific-fishing/components/edit-scientific-permit/edit-scientific-permit.component';
import { ScientificFishingContent } from './scientific-fishing/scientific-fishing-content.component';
import { EditShipOwnerComponent } from './ships-register/edit-ship-owner/edit-ship-owner.component';
import { EditShipComponent } from './ships-register/edit-ship/edit-ship.component';
import { ShipChangeOfCircumstancesComponent } from './ships-register/ship-change-of-circumstances/ship-change-of-circumstances.component';
import { ShipDeregistrationComponent } from './ships-register/ship-deregistration/ship-deregistration.component';
import { StatisticalFormsAquaFarmComponent } from './statistical-forms/components/statistical-forms-aqua-farm/statistical-forms-aqua-farm.component';
import { StatisticalFormsBasicInfoComponent } from './statistical-forms/components/statistical-forms-basic-info/statistical-forms-basic-info.component';
import { StatisticalFormsEmployeesInfoTableComponent } from './statistical-forms/components/statistical-forms-employees-info/statistical-forms-employees-info-table.component';
import { StatisticalFormsEmployeesInfoComponent } from './statistical-forms/components/statistical-forms-employees-info/statistical-forms-employees-info.component';
import { StatisticalFormsFishVesselComponent } from './statistical-forms/components/statistical-forms-fish-vessel/statistical-forms-fish-vessel.component';
import { StatisticalFormsReworkComponent } from './statistical-forms/components/statistical-forms-rework/statistical-forms-rework.component';
import { StatisticalFormsContentComponent } from './statistical-forms/statistical-forms-content.component';
import { UserRegistrationModule } from './user-registration/user-registration.module';

@NgModule({
    declarations: [
        AcquiredFishingCapacityComponent,
        AddLogBookPageWizardComponent,
        AddShipPageDocumentWizardComponent,
        AddShipPageWizardComponent,
        ApplicationsRegisterComponent,
        ApplicationsTableComponent,
        AquacultureChangeOfCircumstancesComponent,
        AquacultureDeregistrationComponent,
        AssignApplicationByAccessCodeComponent,
        BuyerChangeOfCircumstancesComponent,
        BuyerTerminationComponent,
        CatchAquaticOrganismTypeComponent,
        CatchAquaticOrganismTypesArrayComponent,
        CatchesAndSalesContent,
        ChangePasswordComponent,
        ChangeUserDataComponent,
        ChooseApplicationComponent,
        ChooseApplicationTypeComponent,
        ChoosePermitLicenseForRenewalComponent,
        CommonLogBookPageDataComponent,
        EditAdmissionLogBookPageComponent,
        EditAquacultureFacilityComponent,
        EditAquacultureInstallationComponent,
        EditAquacultureLogBookPageComponent,
        EditAquacultureWaterLawCertificateComponent,
        EditAuthorizedPersonComponent,
        EditBuyersComponent,
        EditCatchRecordComponent,
        EditCommercialFishingComponent,
        EditFirstSaleLogBookPageComponent,
        EditFisherComponent,
        EditFishingGearComponent,
        EditInstallationNetCageComponent,
        EditLegalEntityComponent,
        EditLogBookComponent,
        EditLogBookPageProductComponent,
        EditOriginDeclarationComponent,
        EditReportViewComponent,
        EditScientificFishingOutingComponent,
        EditScientificPermitComponent,
        EditScientificPermitHolderComponent,
        EditShipComponent,
        EditShipLogBookPageComponent,
        EditShipOwnerComponent,
        EditSuspensionComponent,
        EditTransportationLogBookPageComponent,
        EnterEventisNumberComponent,
        EnterReasonComponent,
        FileInApplicationStepperComponent,
        FishingCapacityFreedActionsComponent,
        FishingGearsComponent,
        GroudForUseComponent,
        IncreaseFishingCapacityComponent,
        JustifiedCancellationComponent,
        LogBookPagePersonComponent,
        LogBookPagePersonComponent,
        LogBookPageProductsComponent,
        LogBooksComponent,
        MyProfileContentComponent,
        OnlinePaymentPageComponent,
        PaymentInformationComponent,
        RecreationalFishingApplicationsContentComponent,
        RecreationalFishingTicketComponent,
        RecreationalFishingTicketsContentComponent,
        RecreationalFishingTicketSummaryComponent,
        ReduceFishingCapacityComponent,
        ReportExecutionComponent,
        ReportViewContentComponent,
        ScientificFishingContent,
        ShipChangeOfCircumstancesComponent,
        ShipDeregistrationComponent,
        StatisticalFormsAquaFarmComponent,
        StatisticalFormsBasicInfoComponent,
        StatisticalFormsContentComponent,
        StatisticalFormsEmployeesInfoComponent,
        StatisticalFormsEmployeesInfoTableComponent,
        StatisticalFormsFishVesselComponent,
        StatisticalFormsReworkComponent,
        TransferFishingCapacityComponent,
        TransferFishingCapacityTableComponent,
        TransferFishingCapacityTableEntryComponent,
        CapacityCertificateDuplicateComponent,
        UploadFileDialogComponent,
        IssueDuplicateTicketComponent,
        ShipPagesAndDeclarationsTableComponent,
        AdmissionPagesAndDeclarationsTableComponent,
        TransportationPagesAndDeclarationsTableComponent,
        FirstSalePagesTableComponent,
        AquaculturePagesTableComponent,
        DuplicatesApplicationComponent,
        DuplicateEntriesTableComponent,
        PreviousTripsCatchRecordsComponent,
        ChoosePermitToCopyFromComponent
    ],
    imports: [
        TLCommonModule,
        MaterialModule,
        UserRegistrationModule,
        TLEGovPaymentsModule,
        TLAngularMapModule
    ],
    exports: [
        ScientificFishingContent,
        EditScientificPermitComponent,
        EditScientificFishingOutingComponent,
        EditScientificPermitHolderComponent,
        RecreationalFishingTicketSummaryComponent,
        RecreationalFishingTicketsContentComponent,
        RecreationalFishingApplicationsContentComponent,
        MyProfileContentComponent,
        ChangePasswordComponent,
        ApplicationsRegisterComponent,
        ApplicationsTableComponent,
        FileInApplicationStepperComponent,
        EnterEventisNumberComponent,
        AssignApplicationByAccessCodeComponent,
        EnterReasonComponent,
        ChooseApplicationComponent,
        ChooseApplicationTypeComponent,
        EditShipComponent,
        EditShipOwnerComponent,
        ShipChangeOfCircumstancesComponent,
        ShipDeregistrationComponent,
        UploadFileDialogComponent,
        PaymentInformationComponent,
        OnlinePaymentPageComponent,
        EditLegalEntityComponent,
        EditBuyersComponent,
        BuyerChangeOfCircumstancesComponent,
        BuyerTerminationComponent,
        EditFisherComponent,
        EditCommercialFishingComponent,
        FishingGearsComponent,
        EditFishingGearComponent,
        EditLogBookComponent,
        GroudForUseComponent,
        ChoosePermitLicenseForRenewalComponent,
        IncreaseFishingCapacityComponent,
        ReduceFishingCapacityComponent,
        TransferFishingCapacityComponent,
        TransferFishingCapacityTableComponent,
        TransferFishingCapacityTableEntryComponent,
        CapacityCertificateDuplicateComponent,
        FishingCapacityFreedActionsComponent,
        AcquiredFishingCapacityComponent,
        EditSuspensionComponent,
        EditAquacultureFacilityComponent,
        EditAquacultureInstallationComponent,
        EditInstallationNetCageComponent,
        EditAquacultureWaterLawCertificateComponent,
        AquacultureChangeOfCircumstancesComponent,
        AquacultureDeregistrationComponent,
        CatchesAndSalesContent,
        EditShipLogBookPageComponent,
        EditCatchRecordComponent,
        CatchAquaticOrganismTypeComponent,
        CatchAquaticOrganismTypesArrayComponent,
        EditFirstSaleLogBookPageComponent,
        EditAdmissionLogBookPageComponent,
        EditTransportationLogBookPageComponent,
        EditAquacultureLogBookPageComponent,
        EditOriginDeclarationComponent,
        LogBookPagePersonComponent,
        LogBookPagePersonComponent,
        LogBookPageProductsComponent,
        EditLogBookPageProductComponent,
        AddLogBookPageWizardComponent,
        JustifiedCancellationComponent,
        LogBooksComponent,
        CommonLogBookPageDataComponent,
        AddShipPageDocumentWizardComponent,
        AddShipPageWizardComponent,
        StatisticalFormsContentComponent,
        StatisticalFormsBasicInfoComponent,
        StatisticalFormsAquaFarmComponent,
        StatisticalFormsReworkComponent,
        StatisticalFormsFishVesselComponent,
        ReportViewContentComponent,
        ReportExecutionComponent,
        EditReportViewComponent,
        StatisticalFormsEmployeesInfoTableComponent,
        StatisticalFormsEmployeesInfoComponent,
        IssueDuplicateTicketComponent,
        ShipPagesAndDeclarationsTableComponent,
        AdmissionPagesAndDeclarationsTableComponent,
        TransportationPagesAndDeclarationsTableComponent,
        FirstSalePagesTableComponent,
        AquaculturePagesTableComponent,
        DuplicatesApplicationComponent,
        DuplicateEntriesTableComponent,
        PreviousTripsCatchRecordsComponent,
        ChoosePermitToCopyFromComponent
    ],
    providers: [CurrencyPipe, TLDateDifferencePipe, DatePipe]
})
export class CommonApplicationModule {
}