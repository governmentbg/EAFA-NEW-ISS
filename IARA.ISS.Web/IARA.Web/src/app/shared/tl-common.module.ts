import { ClipboardModule } from '@angular/cdk/clipboard';
import { CommonModule, CurrencyPipe, DecimalPipe } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDividerModule } from '@angular/material/divider';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { TLBoricaPaymentsModule } from '@tl/tl-borica-payments';
import { TLEGovPaymentsModule } from '@tl/tl-egov-payments';
import { TLEPayPaymentsModule } from '@tl/tl-epay-payments';
import { IconModule } from '@visurel/iconify-angular';
import { NgxPermissionsModule } from 'ngx-permissions';
import { QuillModule } from 'ngx-quill';
import { NgxTextDiffModule } from 'ngx-text-diff';
import { AddressRegistrationComponent } from './components/address-registration/address-registration.component';
import { SingleAddressRegistrationComponent } from './components/address-registration/single-address-registration/single-address-registration.component';
import { ApplicantRelationToRecipientComponent } from './components/applicant-relation-to-recipient/applicant-relation-to-recipient.component';
import { ApplicationSubmittedByComponent } from './components/application-submitted-by/application-submitted-by.component';
import { ApplicationSubmittedForComponent } from './components/application-submitted-for/application-submitted-for.component';
import { CancellationDialogComponent } from './components/cancellation-dialog/cancellation-dialog.component';
import { CancellationHistoryDialogComponent } from './components/cancellation-history-dialog/cancellation-history-dialog.component';
import { ChangeOfCircumstancesComponent } from './components/change-of-circumstances/change-of-circumstances.component';
import { SingleChangeOfCircumstancesComponent } from './components/change-of-circumstances/single-change-of-circumstances/single-change-of-circumstances.component';
import { CheckboxListComponent } from './components/check-box-list/check-box-list.component';
import { CommonDocumentComponent } from './components/common-document/common-document.component';
import { ConfirmationDialogComponent } from './components/confirmation-dialog/confirmation-dialog.component';
import { TLDataTableModule } from './components/data-table/data-table.module';
import { DateRangeIndefiniteComponent } from './components/date-range-indefinite/date-range-indefinite.component';
import { DeliveryDataComponent } from './components/delivery-data/delivery-data.component';
import { DialogWrapperComponent } from './components/dialog-wrapper/dialog-wrapper.component';
import { EgnLncInputComponent } from './components/egn-lnc-input/egn-lnc-input.component';
import { ErrorSnackbarComponent } from './components/error-snackbar/error-snackbar.component';
import { FileUploadFormArrayComponent } from './components/file-upload-form-array/file-upload-form-array.component';
import { FileUploadFormComponent } from './components/file-upload-form/file-upload-form.component';
import { TLFileUploadModule } from './components/file-upload/file-upload.module';
import { FishingGearSimpleComponent } from './components/fishing-gear-simple/fishing-gear-simple.component';
import { TLInputControlsModule } from './components/input-controls/tl-input-controls.module';
import { ItemListComponent } from './components/item-list/item-list.component';
import { LetterOfAttorneyComponent } from './components/letter-of-attorney/letter-of-attorney.component';
import { EGovOfflinePaymenDataComponent } from './components/online-payment-data/egov-offline-payment-data/egov-offline-payment-data.component';
import { OnlinePaymentDataComponent } from './components/online-payment-data/online-payment-data.component';
import { OverlappingLogBooksComponent } from './components/overlapping-log-books/overlapping-log-books.component';
import { PaymentDataComponent } from './components/payment-data/payment-data.component';
import { PaymentTariffComponent } from './components/payment-tariff/payment-tariff.component';
import { PaymentTariffsComponent } from './components/payment-tariffs/payment-tariffs.component';
import { RegisterDeliveryComponent } from './components/register-delivery/register-delivery.component';
import { RegixChecksResultsComponent } from './components/regix-checks-results/regix-checks-results.component';
import { RegixDataComponent } from './components/regix-data/regix-data.component';
import { SearchPanelModule } from './components/search-panel/search-panel.module';
import { SimpleSelectTableComponent } from './components/simple-select-table/simple-select-table.component';
import { SpinnerModule } from './components/spinner/spinner.module';
import { TLAuditModule } from './components/tl-audit/tl-audit.module';
import { TLCardModule } from './components/tl-card/tl-card.module';
import { TLExpansionPanelModule } from './components/tl-expansion-panel/tl-expansion-panel.module';
import { TLHelpModule } from './components/tl-help/tl-help.module';
import { TLIconButtonComponent } from './components/tl-icon-button/tl-icon-button.component';
import { TLIconButtonModule } from './components/tl-icon-button/tl-icon-button.module';
import { TLIconModule } from './components/tl-icon/tl-icon.module';
import { TLPictureUploaderModule } from './components/tl-picture-uploader/tl-picture-uploader.module';
import { TLPopoverModule } from './components/tl-popover/tl-popover.module';
import { UsageDocumentComponent } from './components/usage-document/usage-document.component';
import { VesselSimpleComponent } from './components/vessel-simple/vessel-simple.component';
import { MatVerticalStepperScrollerDirective } from './directives/mat-vertical-stepper-scroller.directive';
import { NotifierGroupDirective } from './directives/notifier/notifier-group.directive';
import { NotifierDirective } from './directives/notifier/notifier.directive';
import { TLResizableDirective } from './directives/resizable.directive';
import { ValidityCheckerGroupDirective } from './directives/validity-checker/validity-checker-group.directive';
import { ValidityCheckerDirective } from './directives/validity-checker/validity-checker.directive';
import { MaterialModule } from './material.module';
import { TLPipesModule } from './pipes/tl-pipes.module';

@NgModule({
    imports: [
        BrowserModule,
        CommonModule,
        TLFileUploadModule,
        FlexLayoutModule,
        FontAwesomeModule,
        FormsModule,
        IconModule,
        MaterialModule,
        ReactiveFormsModule,
        SearchPanelModule,
        SpinnerModule,
        TLAuditModule,
        TLDataTableModule,
        TLHelpModule,
        TLIconButtonModule,
        TLIconModule,
        TLInputControlsModule,
        TLPipesModule,
        TLPopoverModule,
        TLCardModule,
        TLPictureUploaderModule,
        TLExpansionPanelModule,
        NgxPermissionsModule.forRoot(),
        NgxDatatableModule,
        ClipboardModule,
        TLEGovPaymentsModule,
        TLBoricaPaymentsModule,
        TLEPayPaymentsModule,
        NgxTextDiffModule,
        MatDividerModule,
        QuillModule.forRoot()
    ],
    exports: [
        NgxDatatableModule,
        NgxPermissionsModule,
        RouterModule,
        BrowserModule,
        CommonModule,
        ConfirmationDialogComponent,
        DialogWrapperComponent,
        ErrorSnackbarComponent,
        SingleAddressRegistrationComponent,
        AddressRegistrationComponent,
        EgnLncInputComponent,
        RegixDataComponent,
        FileUploadFormComponent,
        FileUploadFormArrayComponent,
        LetterOfAttorneyComponent,
        TLFileUploadModule,
        FlexLayoutModule,
        FontAwesomeModule,
        FormsModule,
        IconModule,
        MaterialModule,
        ReactiveFormsModule,
        SearchPanelModule,
        SpinnerModule,
        TLAuditModule,
        TLDataTableModule,
        TLHelpModule,
        TLIconButtonComponent,
        TLIconButtonModule,
        TLIconModule,
        TLInputControlsModule,
        TLPipesModule,
        TLPopoverModule,
        TLCardModule,
        TLExpansionPanelModule,
        ClipboardModule,
        CheckboxListComponent,
        MatDividerModule,
        MatVerticalStepperScrollerDirective,
        TLResizableDirective,
        ValidityCheckerDirective,
        ValidityCheckerGroupDirective,
        NotifierDirective,
        NotifierGroupDirective,
        TLPictureUploaderModule,
        PaymentDataComponent,
        RegixChecksResultsComponent,
        OnlinePaymentDataComponent,
        RegisterDeliveryComponent,
        ApplicantRelationToRecipientComponent,
        CancellationDialogComponent,
        NgxTextDiffModule,
        QuillModule,
        ApplicationSubmittedByComponent,
        ApplicationSubmittedForComponent,
        SingleChangeOfCircumstancesComponent,
        ChangeOfCircumstancesComponent,
        DeliveryDataComponent,
        VesselSimpleComponent,
        SimpleSelectTableComponent,
        CommonDocumentComponent,
        UsageDocumentComponent,
        FishingGearSimpleComponent,
        ItemListComponent,
        DateRangeIndefiniteComponent,
        PaymentTariffsComponent,
        PaymentTariffComponent,
        CancellationHistoryDialogComponent,
        OverlappingLogBooksComponent
    ],
    declarations: [
        DialogWrapperComponent,
        ErrorSnackbarComponent,
        ConfirmationDialogComponent,
        SingleAddressRegistrationComponent,
        AddressRegistrationComponent,
        EgnLncInputComponent,
        RegixDataComponent,
        FileUploadFormComponent,
        FileUploadFormArrayComponent,
        LetterOfAttorneyComponent,
        MatVerticalStepperScrollerDirective,
        TLResizableDirective,
        ValidityCheckerDirective,
        ValidityCheckerGroupDirective,
        NotifierDirective,
        CheckboxListComponent,
        NotifierGroupDirective,
        PaymentDataComponent,
        RegixChecksResultsComponent,
        OnlinePaymentDataComponent,
        EGovOfflinePaymenDataComponent,
        RegisterDeliveryComponent,
        ApplicantRelationToRecipientComponent,
        CancellationDialogComponent,
        ApplicationSubmittedByComponent,
        ApplicationSubmittedForComponent,
        SingleChangeOfCircumstancesComponent,
        ChangeOfCircumstancesComponent,
        DeliveryDataComponent,
        VesselSimpleComponent,
        SimpleSelectTableComponent,
        CommonDocumentComponent,
        UsageDocumentComponent,
        FishingGearSimpleComponent,
        ItemListComponent,
        DateRangeIndefiniteComponent,
        PaymentTariffsComponent,
        PaymentTariffComponent,
        CancellationHistoryDialogComponent,
        OverlappingLogBooksComponent
    ],
    providers: [CurrencyPipe, DecimalPipe]
})
export class TLCommonModule {
}