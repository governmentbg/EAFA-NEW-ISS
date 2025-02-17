﻿import { Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { AdmissionLogBookPageEditDTO } from '@app/models/generated/dtos/AdmissionLogBookPageEditDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { CommonLogBookPageDataDTO } from '@app/models/generated/dtos/CommonLogBookPageDataDTO';
import { LogBookPageProductDTO } from '@app/models/generated/dtos/LogBookPageProductDTO';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { CatchesAndSalesDialogParamsModel } from '../../models/catches-and-sales-dialog-params.model';
import { BasicLogBookPageDocumentDataDTO } from '@app/models/generated/dtos/BasicLogBookPageDocumentDataDTO';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { LogBookPageDocumentTypesEnum } from '../../enums/log-book-page-document-types.enum';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { CatchesAndSalesUtils } from '../../utils/catches-and-sales.utils';
import { DateDifference } from '@app/models/common/date-difference.model';
import { DateUtils } from '@app/shared/utils/date.utils';
import { SystemParametersService } from '@app/services/common-app/system-parameters.service';
import { SystemPropertiesDTO } from '@app/models/generated/dtos/SystemPropertiesDTO';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { LogBookPageEditExceptionDTO } from '@app/models/generated/dtos/LogBookPageEditExceptionDTO';
import { SecurityService } from '@app/services/common-app/security.service';
import { DatePipe } from '@angular/common';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { AddLogBookPageWizardComponent } from '../add-log-book-page-wizard/add-log-book-page-wizard.component';
import { AddLogBookPageDialogParams } from '../add-log-book-page-wizard/models/add-log-book-page-wizard-dialog-params.model';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';

@Component({
    selector: 'edit-admission-log-book-page',
    templateUrl: './edit-admission-log-book-page.component.html'
})
export class EditAdmissionLogBookPageComponent implements OnInit, IDialogComponent {
    public readonly pageCode: PageCodeEnum = PageCodeEnum.AdmissionLogBookPage;
    public readonly logBookType: LogBookTypesEnum = LogBookTypesEnum.Admission;
    public readonly currentDate: Date = new Date();

    public form!: FormGroup;
    public viewMode!: boolean;
    public service!: ICatchesAndSalesService;
    public originPossibleProducts: LogBookPageProductDTO[] = [];
    public isAdd: boolean = false;

    public noAvailableProducts: boolean = false;
    public isLogBookPageDateLockedError: boolean = false;
    public isCommonLogBookPageDataReadonly: boolean = true;
    public hasEditCommonDataPermission: boolean = false;

    public getControlErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.getControlErrorLabelText.bind(this);

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private model!: AdmissionLogBookPageEditDTO;
    private id: number | undefined;
    /** Needed only for Admission log book pages, when the log book is for Person/Legal */
    public logBookPermitLicenseId: number | undefined;
    private shipPageDocumentData: BasicLogBookPageDocumentDataDTO | undefined;
    private commonLogBookPageData: CommonLogBookPageDataDTO | undefined;
    private logBookId!: number;
    private logBookTypeId!: number;
    private pageNumber: number | undefined;
    private pageStatus: LogBookPageStatusesEnum | undefined;
    private hasMissingPagesRangePermission: boolean = false;

    private lockAdmissionLogBookPeriod!: number;
    private lockAdmissionLogBookDaysPeriod!: number;
    private logBookPageEditExceptions: LogBookPageEditExceptionDTO[] = [];
    private currentUserId: number;

    private readonly translationService: FuseTranslationLoaderService;
    private readonly snackbar: MatSnackBar;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly datePipe: DatePipe;
    private readonly editBasicInformationDialog: TLMatDialog<AddLogBookPageWizardComponent>;
    private readonly systemParametersService: SystemParametersService;

    public constructor(
        translationService: FuseTranslationLoaderService,
        snackbar: MatSnackBar,
        confirmDialog: TLConfirmDialog,
        datePipe: DatePipe,
        editBasicInformationDialog: TLMatDialog<AddLogBookPageWizardComponent>,
        systemParametersService: SystemParametersService,
        authService: SecurityService
    ) {
        this.translationService = translationService;
        this.snackbar = snackbar;
        this.confirmDialog = confirmDialog;
        this.datePipe = datePipe;
        this.editBasicInformationDialog = editBasicInformationDialog;
        this.systemParametersService = systemParametersService;

        this.currentUserId = authService.User!.userId;
    }

    public async ngOnInit(): Promise<void> {
        const systemParameters: SystemPropertiesDTO = await this.systemParametersService.systemParameters();
        this.lockAdmissionLogBookPeriod = systemParameters.lockAdmissionLogBookAfterHours!;
        this.lockAdmissionLogBookDaysPeriod = systemParameters.addAdmissionPagesDaysTolerance!;

        if (!this.viewMode) {
            this.logBookPageEditExceptions = await this.service.getLogBookPageEditExceptions().toPromise();
        }

        if (this.id !== null && this.id !== undefined) {
            this.service.getAdmissionLogBookPage(this.id).subscribe({
                next: (result: AdmissionLogBookPageEditDTO) => {
                    this.model = result;
                    this.model.status = this.service.getPageStatusTranslation(LogBookPageStatusesEnum[this.model.status! as keyof typeof LogBookPageStatusesEnum]);

                    this.originPossibleProducts = this.model.originalPossibleProducts ?? [];
                    this.model.originalPossibleProducts = []; // за да не се мапират обратно към бекенда

                    this.commonLogBookPageData = this.model.commonData;

                    if (!this.viewMode) {
                        this.setHandoverDateControlValidators();
                    }

                    this.fillForm();
                }
            });
        }
        else if (this.shipPageDocumentData !== null && this.shipPageDocumentData !== undefined) {
            this.service.getPossibleProducts(this.shipPageDocumentData!.shipLogBookPageId!, LogBookPageDocumentTypesEnum.AdmissionDocument).subscribe({
                next: (result: LogBookPageProductDTO[]) => {
                    this.model = new AdmissionLogBookPageEditDTO({
                        commonData: this.shipPageDocumentData!.sourceData,
                        logBookId: this.shipPageDocumentData!.logBookId,
                        logBookTypeId: this.logBookTypeId,
                        //logBookNumber: this.shipPageDocumentData!.logBookNumber,
                        pageNumber: this.shipPageDocumentData!.documentNumber,
                        acceptingPerson: this.shipPageDocumentData!.personData,
                        originalPossibleProducts: result.slice(),
                        products: JSON.parse(JSON.stringify(result.slice())),
                        logBookPermitLicenseId: this.logBookPermitLicenseId
                    });

                    if (this.shipPageDocumentData!.pageStatus !== undefined && this.shipPageDocumentData!.pageStatus !== null) {
                        this.model.status = this.service.getPageStatusTranslation(this.shipPageDocumentData!.pageStatus);
                    }

                    this.originPossibleProducts = this.model.originalPossibleProducts ?? [];
                    this.model.originalPossibleProducts = []; // за да не се мапират обратно към бекенда

                    if (this.originPossibleProducts.length === 0) {
                        this.noAvailableProducts = true;
                    }

                    this.fillForm();
                }
            });
        }
        else if (this.commonLogBookPageData !== null && this.commonLogBookPageData !== undefined) {
            let originDeclarationId: number | undefined;
            let transportationDocumentId: number | undefined;

            if (this.commonLogBookPageData.transportationDocumentId !== null && this.commonLogBookPageData.transportationDocumentId !== undefined) {
                transportationDocumentId = this.commonLogBookPageData.transportationDocumentId;
            }
            else {
                originDeclarationId = this.commonLogBookPageData.originDeclarationId;
            }

            this.service.getNewAdmissionLogBookPage(this.logBookId, originDeclarationId, transportationDocumentId).subscribe({
                next: (result: AdmissionLogBookPageEditDTO) => {
                    this.model = result;
                    this.model.commonData = this.commonLogBookPageData;
                    this.model.logBookPermitLicenseId = this.logBookPermitLicenseId;

                    if (this.pageNumber !== undefined && this.pageNumber !== null) {
                        this.model.pageNumber = this.pageNumber;
                    }

                    if (this.pageStatus !== undefined && this.pageStatus !== null) {
                        this.model.status = this.service.getPageStatusTranslation(this.pageStatus);
                    }

                    this.originPossibleProducts = this.model.originalPossibleProducts ?? [];
                    this.model.originalPossibleProducts = []; // за да не се мапират обратно към бекенда

                    if (this.originPossibleProducts.length === 0) {
                        this.noAvailableProducts = true;
                    }

                    this.fillForm();
                }
            });
        }
    }

    public setData(data: CatchesAndSalesDialogParamsModel, buttons: DialogWrapperData): void {
        this.id = data.id;
        this.service = data.service as ICatchesAndSalesService;
        this.viewMode = data.viewMode;
        this.shipPageDocumentData = data.shipPageDocumentData;
        this.logBookId = data.logBookId;
        this.logBookTypeId = data.logBookTypeId;
        this.pageNumber = data.pageNumber;
        this.pageStatus = data.pageStatus;
        this.commonLogBookPageData = data.commonData;
        this.logBookPermitLicenseId = data.logBookPermitLicenseId;
        this.hasEditCommonDataPermission = data.canEditCommonDataPermission;

        if (this.pageStatus === LogBookPageStatusesEnum.Missing) {
            this.isAdd = false;
        }
        else {
            if (this.id === null || this.id === undefined) {
                this.isAdd = true;
            }
            else {
                this.isAdd = false;
            }
        }

        this.buildForm();

        if (this.viewMode === true) {
            this.form.disable();
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.isFormValid()) {
            this.fillModel();
            this.model = CommonUtils.sanitizeModelStrings(this.model);

            this.confirmDialog.open({
                title: this.translationService.getValue('catches-and-sales.complete-page-confirm-dialog-title'),
                message: this.translationService.getValue('catches-and-sales.complete-page-confirm-dialog-message'),
                okBtnLabel: this.translationService.getValue('catches-and-sales.complete-page-confirm-dialog-ok-btn-label')
            }).subscribe({
                next: (ok: boolean | undefined) => {
                    if (ok) {
                        if (this.id === null || this.id === undefined) {
                            this.addAdmissionLogBookPage(dialogClose);
                        }
                        else {
                            this.service.editAdmissionLogBookPage(this.model).subscribe({
                                next: () => {
                                    dialogClose(this.model);
                                },
                                error: (response: HttpErrorResponse) => {
                                    this.addOrEditAdmissionLogBookPageErrorHandle(response, dialogClose);
                                }
                            });
                        }
                    }
                }
            });
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public getControlErrorLabelText(controlName: string, errorValue: unknown, errorCode: string): TLError | undefined {
        if (controlName === 'handoverDateControl') {
            if (errorCode === 'logBookPageDateLocked' || errorCode === 'logBookPageDatePeriodLocked') {
                const message: string = this.translationService.getValue('catches-and-sales.admission-page-date-cannot-be-chosen-error');
                if (this.isLogBookPageDateLockedError || errorCode === 'logBookPageDatePeriodLocked') {
                    return new TLError({ text: message, type: 'error' });
                }
                else {
                    return new TLError({ text: message, type: 'warn' });
                }
            }
            else if (errorCode === 'mindate') {
                const minDate: Date | undefined = this.getMinHandoverDate();
                if (minDate !== undefined && minDate !== null) {
                    const dateString: string = this.datePipe.transform(minDate, 'dd.MM.YYYY') ?? "";
                    let messageText: string = this.translationService.getValue('validation.min');
                    messageText = messageText[0].toUpperCase() + messageText.substr(1);
                    return new TLError({ text: `${messageText}: ${dateString}` });
                }
            }
        }

        return undefined;
    }

    public editBasicInformation(): void {
        const title: string = this.translationService.getValue('catches-and-sales.edit-admission-page-basic-information-dialog-title');

        if (this.hasEditCommonDataPermission) {
            this.editBasicInformationDialog.open({
                title: title,
                TCtor: AddLogBookPageWizardComponent,
                translteService: this.translationService,
                viewMode: false,
                headerCancelButton: {
                    cancelBtnClicked: this.closeEditBasicInformationDialogBtnClicked.bind(this)
                },
                saveBtn: {
                    id: 'save',
                    color: 'accent',
                    translateValue: this.translationService.getValue('catches-and-sales.edit-admission-page-basic-information-ok-btn')
                },
                cancelBtn: {
                    id: 'cancel',
                    color: 'primary',
                    translateValue: this.translationService.getValue('common.cancel'),
                },
                componentData: new AddLogBookPageDialogParams({
                    service: this.service,
                    logBookType: this.logBookType,
                    logBookId: this.logBookId,
                    logBookTypeId: this.logBookTypeId,
                    pageNumber: this.pageNumber,
                    editLogBookPageBasicInfo: true
                }),
                disableDialogClose: true
            }, '1500px').subscribe({
                next: (result: CommonLogBookPageDataDTO | undefined) => {
                    if (result !== undefined && result !== null) {
                        this.form.get('commonLogBookPageDataControl')!.setValue(result);
                        this.form.get('productsControl')!.setValue([]);

                        if (result.transportationDocumentId !== undefined && result.transportationDocumentId !== null) {
                            this.service.getPossibleProductsByTransportationDocument(result.transportationDocumentId, LogBookPageDocumentTypesEnum.AdmissionDocument).subscribe({
                                next: (products: LogBookPageProductDTO[]) => {
                                    const logBookProducts: LogBookPageProductDTO[] = this.getProductsFromBasicInfo(products);
                                    this.originPossibleProducts = products;
                                    this.model.originalPossibleProducts = [];

                                    this.form.get('productsControl')!.setValue(logBookProducts);
                                }
                            });
                        }
                        else if (result.originDeclarationId !== undefined && result.originDeclarationId !== null) {
                            this.service.getPossibleProductsByOriginDeclarationId(result.originDeclarationId, LogBookPageDocumentTypesEnum.AdmissionDocument).subscribe({
                                next: (products: LogBookPageProductDTO[]) => {
                                    const logBookProducts: LogBookPageProductDTO[] = this.getProductsFromBasicInfo(products);
                                    this.originPossibleProducts = products;
                                    this.model.originalPossibleProducts = [];

                                    this.form.get('productsControl')!.setValue(logBookProducts);
                                }
                            });
                        }
                        else {
                            this.originPossibleProducts = [];
                            this.model.originalPossibleProducts = [];

                            this.form.get('productsControl')!.setValue([]);
                        }
                    }
                }
            });
        }
    }

    private buildForm(): void {
        this.form = new FormGroup({
            pageNumberControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)]),
            statusControl: new FormControl(),
            handoverDateControl: new FormControl(undefined, [Validators.required, this.checkDateValidityVsLockPeriodsValidator()]),
            storageLocationControl: new FormControl(undefined, [Validators.required, Validators.maxLength(500)]),

            commonLogBookPageDataControl: new FormControl(),
            acceptingPersonControl: new FormControl(undefined, Validators.required),
            productsControl: new FormControl(),

            filesControl: new FormControl()
        });

        this.setHandoverDateControlValidators();
    }

    private fillModel(): void {
        this.model.pageNumber = this.form.get('pageNumberControl')!.value;
        this.model.handoverDate = this.form.get('handoverDateControl')!.value;
        this.model.storageLocation = this.form.get('storageLocationControl')!.value;
        this.model.commonData = this.form.get('commonLogBookPageDataControl')!.value;
        this.model.acceptingPerson = this.form.get('acceptingPersonControl')!.value;
        this.model.products = this.form.get('productsControl')!.value;
        this.model.files = this.form.get('filesControl')!.value;
    }

    private fillForm(): void {
        this.form.get('pageNumberControl')!.setValue(this.model.pageNumber);
        this.form.get('handoverDateControl')!.setValue(this.model.handoverDate);
        this.form.get('storageLocationControl')!.setValue(this.model.storageLocation);
        this.form.get('commonLogBookPageDataControl')!.setValue(this.model.commonData);
        this.form.get('acceptingPersonControl')!.setValue(this.model.acceptingPerson);
        this.form.get('productsControl')!.setValue(this.model.products);
        this.form.get('filesControl')!.setValue(this.model.files);

        this.isCommonLogBookPageDataReadonly = this.model.commonData?.originDeclarationNumber?.includes('missing data') !== true;

        if (!this.isAdd) {
            this.form.get('statusControl')!.setValue(this.model.status);
        }
    }

    private addAdmissionLogBookPage(dialogClose: DialogCloseCallback): void {
        this.service.addAdmissionLogBookPage(this.model, this.hasMissingPagesRangePermission).subscribe({
            next: (id: number) => {
                this.model.id = id;
                dialogClose(this.model);
            },
            error: (response: HttpErrorResponse) => {
                this.addOrEditAdmissionLogBookPageErrorHandle(response, dialogClose);
            }
        });
    }

    private addOrEditAdmissionLogBookPageErrorHandle(response: HttpErrorResponse, dialogClose: DialogCloseCallback): void {
        const error: ErrorModel | undefined = response.error;

        if (error?.code === ErrorCode.PageNumberNotInLogbook) {
            this.snackbar.open(
                this.translationService.getValue('catches-and-sales.admission-page-not-in-range-error'),
                undefined,
                {
                    duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                });
        }
        else if (error?.code === ErrorCode.PageNumberNotInLogBookLicense) {
            this.snackbar.open(
                this.translationService.getValue('catches-and-sales.admission-page-not-in-log-book-license-range-error'),
                undefined,
                {
                    duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                });
        }
        else if (error?.code === ErrorCode.LogBookPageAlreadySubmitted) {
            this.snackbar.open(
                this.translationService.getValue('catches-and-sales.admission-page-already-submitted-error'),
                undefined,
                {
                    duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                });
        }
        else if (error?.code === ErrorCode.LogBookPageAlreadySubmittedOtherLogBook) {
            const message: string = this.translationService.getValue('catches-and-sales.admission-page-already-submitted-other-logbook-error');
            this.snackbar.open(
                `${message}: ${error.messages[0]}`,
                undefined,
                {
                    duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                });
        }
        else if (error?.code === ErrorCode.SendFLUXSalesFailed) {
            if (!IS_PUBLIC_APP) { // show snackbar only when not public app
                this.snackbar.open(this.translationService.getValue('catches-and-sales.admission-page-send-to-flux-sales-error'), undefined, {
                    duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                });
            }

            dialogClose();
        }
        else if (error?.code === ErrorCode.MaxNumberMissingPagesExceeded) {
            if (error!.messages === null || error!.messages === undefined || error!.messages.length < 2) {
                throw new Error('In MaxNumberMissingPagesExceeded exception at least the last used page number and a number saying the difference should be passed in the messages property.');
            }

            const lastUsedPageNum: number = Number(error!.messages[0]);
            const diff: number = Number(error!.messages[1]);
            const pageToAdd: number = this.model.pageNumber!;

            // confirmation message

            let message: string = '';

            if (lastUsedPageNum === 0) { // няма добавени страници все още към този дневник
                const currentStartPage: number = Number(error!.messages[2]);

                const msg1: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-no-pages-first-message');
                const msg2: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-second-message');
                const msg3: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-third-message');
                const msg4: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-forth-message');
                const msg5: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-fifth-message');
                const msg6: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-sixth-message');

                message = `${msg1} ${currentStartPage} ${msg2} ${pageToAdd} ${msg3} ${diff} ${msg4}.\n\n${msg5} ${diff} ${msg6}.`;
            }
            else {
                const msg1: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-first-message');
                const msg2: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-second-message');
                const msg3: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-third-message');
                const msg4: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-forth-message');
                const msg5: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-fifth-message');
                const msg6: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-sixth-message');

                message = `${msg1} ${lastUsedPageNum} ${msg2} ${pageToAdd} ${msg3} ${diff} ${msg4}.\n\n${msg5} ${diff} ${msg6}.`;
            }

            // button label

            const btnMsg1: string = this.translationService.getValue('catches-and-sales.admission-page-permit-generate-missing-pages-first-part');
            const btnMsg2: string = this.translationService.getValue('catches-and-sales.admission-page-permit-generate-missing-pages-second-part');

            this.confirmDialog.open({
                title: this.translationService.getValue('catches-and-sales.admission-page-generate-missing-pages-permission-dialog-title'),
                message: message,
                okBtnLabel: `${btnMsg1} ${diff} ${btnMsg2}`,
                okBtnColor: 'warn'
            }).subscribe({
                next: (ok: boolean | undefined) => {
                    this.hasMissingPagesRangePermission = ok ?? false;

                    if (this.hasMissingPagesRangePermission) {
                        this.addAdmissionLogBookPage(dialogClose); // start add method again
                    }
                }
            });
        }
        else if (error?.code === ErrorCode.CannotAddEditLockedAdmissionPage) {
            const msg1: string = this.translationService.getValue('catches-and-sales.admission-page-cannot-add-page-after-error');
            const msg2: string = this.translationService.getValue('catches-and-sales.admission-page-hours-after-the-admission');
            const msg3: string = this.translationService.getValue('catches-and-sales.admission-page-to-add-page-after-locked-period-contanct-admin');

            const msg: string = `${msg1} ${this.lockAdmissionLogBookPeriod} ${msg2}. ${msg3}.`;

            this.snackbar.open(
                msg,
                undefined,
                {
                    duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                });
        }
    }

    private checkDateValidityVsLockPeriodsValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === null || control === undefined) {
                return null;
            }

            if (this.model === null || this.model === undefined) {
                return null;
            }

            const admissionDate: Date | undefined = control.value;

            if (admissionDate === null || admissionDate === undefined) {
                return null;
            }

            const now: Date = new Date();

            admissionDate.setHours(now.getHours());
            admissionDate.setMinutes(now.getMinutes());
            admissionDate.setSeconds(now.getSeconds());
            admissionDate.setMilliseconds(now.getMilliseconds());

            const logBookTypeId: number = this.logBookTypeId!;
            const logBookId: number = this.model.logBookId!;

            if (CatchesAndSalesUtils.checkIfPageDateIsUnlocked(this.logBookPageEditExceptions, this.currentUserId, logBookTypeId, logBookId, admissionDate, now)) {
                return null;
            }

            const difference: DateDifference | undefined = CatchesAndSalesUtils.getDateTimeDifference(admissionDate, now);

            if (difference === null || difference === undefined) {
                return null;
            }

            if (difference.minutes === 0 && difference.hours === 0 && difference.days === 0) {
                return null;
            }

            const hoursDifference: number = CatchesAndSalesUtils.convertDateDifferenceToHours(difference);

            if (hoursDifference > this.lockAdmissionLogBookPeriod && this.isLogBookPageDateLockedError) {
                return {
                    logBookPageDateLocked: {
                        lockedPeriod: this.lockAdmissionLogBookPeriod,
                        periodType: 'hours'
                    }
                };
            }

            if (CatchesAndSalesUtils.pageHasLogBookPageDateLockedViaDaysAfterMonth(admissionDate, now, this.lockAdmissionLogBookDaysPeriod)) {
                return {
                    logBookPageDatePeriodLocked: {
                        lockedPeriod: this.lockAdmissionLogBookDaysPeriod,
                        periodType: 'days-after-month'
                    }
                };
            }

            return null;
        }
    }

    private setHandoverDateControlValidators(): void {
        this.form.get('handoverDateControl')!.setValidators([
            Validators.required,
            TLValidators.minDate(undefined, this.getMinHandoverDate()),
            this.checkDateValidityVsLockPeriodsValidator()
        ]);

        this.form.get('handoverDateControl')!.markAsPending({ emitEvent: false });
        this.form.get('handoverDateControl')!.updateValueAndValidity({ emitEvent: false });
    }

    private getMinHandoverDate(): Date | undefined {
        let result: Date | undefined;

        if (this.commonLogBookPageData !== undefined && this.commonLogBookPageData !== null) {
            if (this.commonLogBookPageData.transportationDocumentDate !== undefined && this.commonLogBookPageData.transportationDocumentDate !== null) {
                result = this.commonLogBookPageData.transportationDocumentDate;
            }
            else if (this.commonLogBookPageData.originDeclarationDate !== undefined && this.commonLogBookPageData.originDeclarationDate !== null) {
                result = this.commonLogBookPageData.originDeclarationDate;
            }
        }

        return result;
    }


    private isFormValid(): boolean {
        if (this.form.valid) {
            return true;
        }
        else {
            const errors: ValidationErrors = {};

            for (const key of Object.keys(this.form.controls)) {
                if (key === 'handoverDateControl' && !this.isLogBookPageDateLockedError) {
                    for (const error in this.form.controls[key].errors) {
                        if (error !== 'logBookPageDateLocked') {
                            errors[key] = this.form.controls[key].errors![error];
                        }
                    }
                }
                else {
                    const controlErrors: ValidationErrors | null = this.form.controls[key].errors;
                    if (controlErrors !== null) {
                        errors[key] = controlErrors;
                    }
                }
            }

            return Object.keys(errors).length === 0 ? true : false;
        }
    }

    private getProductsFromBasicInfo(products: LogBookPageProductDTO[]): LogBookPageProductDTO[] {
        const productsToDelete: LogBookPageProductDTO[] = this.model.products ?? [];

        for (const product of productsToDelete) {
            product.isActive = false;
            product.hasMissingProperties = false;
        }

        const result: LogBookPageProductDTO[] = products.concat(...productsToDelete);

        return result;
    }

    private closeEditBasicInformationDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}