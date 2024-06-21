import { Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { CatchesAndSalesDialogParamsModel } from '../../models/catches-and-sales-dialog-params.model';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TransportationLogBookPageEditDTO } from '@app/models/generated/dtos/TransportationLogBookPageEditDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { CommonLogBookPageDataDTO } from '@app/models/generated/dtos/CommonLogBookPageDataDTO';
import { LogBookPageProductDTO } from '@app/models/generated/dtos/LogBookPageProductDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';
import { BasicLogBookPageDocumentDataDTO } from '@app/models/generated/dtos/BasicLogBookPageDocumentDataDTO';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { LogBookPageDocumentTypesEnum } from '../../enums/log-book-page-document-types.enum';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { SystemPropertiesDTO } from '@app/models/generated/dtos/SystemPropertiesDTO';
import { SystemParametersService } from '@app/services/common-app/system-parameters.service';
import { LogBookPageEditExceptionDTO } from '@app/models/generated/dtos/LogBookPageEditExceptionDTO';
import { SecurityService } from '@app/services/common-app/security.service';
import { CatchesAndSalesUtils } from '../../utils/catches-and-sales.utils';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { DatePipe } from '@angular/common';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';

@Component({
    selector: 'edit-transportation-log-book-page',
    templateUrl: './edit-transportation-log-book-page.component.html'
})
export class EditTransportationLogBookPageComponent implements OnInit, IDialogComponent {
    public readonly pageCode: PageCodeEnum = PageCodeEnum.TransportationLogBookPage;
    public readonly logBookType: LogBookTypesEnum = LogBookTypesEnum.Transportation;
    public readonly currentDate: Date = new Date();

    public form!: FormGroup;
    public viewMode!: boolean;
    public service!: ICatchesAndSalesService;
    public originPossibleProducts: LogBookPageProductDTO[] = [];
    public isAdd: boolean = false;
    public canAddProducts: boolean = false;

    public getControlErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.getControlErrorLabelText.bind(this);

    private lockTransportationLogBookPeriod!: number;
    private logBookPageEditExceptions: LogBookPageEditExceptionDTO[] = [];
    private currentUserId: number;

    public noAvailableProducts: boolean = false;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private model!: TransportationLogBookPageEditDTO;
    private id: number | undefined;
    private commonLogBookPageData: CommonLogBookPageDataDTO | undefined;
    private shipPageDocumentData: BasicLogBookPageDocumentDataDTO | undefined;
    private logBookId!: number;
    private logBookTypeId!: number;
    /** Needed only for Transportation log book pages, when the log book is for Person/Legal */
    public logBookPermitLicenseId: number | undefined;
    private pageNumber: number | undefined;
    private pageStatus: LogBookPageStatusesEnum | undefined;
    private translationService: FuseTranslationLoaderService;
    private snackbar: MatSnackBar;
    private confirmDialog: TLConfirmDialog;
    private hasMissingPagesRangePermission: boolean = false;
    private readonly datePipe: DatePipe;
    private readonly systemParametersService: SystemParametersService;

    public constructor(
        translationService: FuseTranslationLoaderService,
        snackbar: MatSnackBar,
        confirmDialog: TLConfirmDialog,
        datePipe: DatePipe,
        systemParametersService: SystemParametersService,
        authService: SecurityService
    ) {
        this.translationService = translationService;
        this.snackbar = snackbar;
        this.confirmDialog = confirmDialog;
        this.datePipe = datePipe;
        this.systemParametersService = systemParametersService;

        this.currentUserId = authService.User!.userId;
    }

    public async ngOnInit(): Promise<void> {
        const systemParameters: SystemPropertiesDTO = await this.systemParametersService.systemParameters();
        this.lockTransportationLogBookPeriod = systemParameters.addTransportationPagesDaysTolerance!;

        if (!this.viewMode) {
            this.logBookPageEditExceptions = await this.service.getLogBookPageEditExceptions().toPromise();
        }

        if (this.id !== null && this.id !== undefined) {
            this.service.getTransportationLogBookPage(this.id).subscribe({
                next: (result: TransportationLogBookPageEditDTO) => {
                    this.model = result;
                    this.model.status = this.service.getPageStatusTranslation(LogBookPageStatusesEnum[this.model.status! as keyof typeof LogBookPageStatusesEnum]);

                    for (const product of this.model.products ?? []) {
                        product.hasMissingProperties = false;
                    }

                    this.originPossibleProducts = this.model.originalPossibleProducts?.slice() ?? [];
                    this.model.originalPossibleProducts = []; // за да не се мапират обратно към бекенда

                    this.commonLogBookPageData = this.model.commonData;

                    if (!this.viewMode) {
                        this.setMinLoadingDateControlValidators();
                    }

                    if (this.model.commonData!.originDeclarationId === null || this.model.commonData!.originDeclarationId === undefined) {
                        this.canAddProducts = true;
                    }

                    this.fillForm();
                }
            });
        }
        else if (this.shipPageDocumentData !== null && this.shipPageDocumentData !== undefined) {
            this.service.getPossibleProducts(this.shipPageDocumentData!.shipLogBookPageId!, LogBookPageDocumentTypesEnum.TransportationDocument).subscribe({
                next: (result: LogBookPageProductDTO[]) => {
                    this.model = new TransportationLogBookPageEditDTO({
                        commonData: this.shipPageDocumentData!.sourceData,
                        logBookId: this.shipPageDocumentData!.logBookId,
                        //logBookNumber: this.shipPageDocumentData!.logBookNumber,
                        pageNumber: this.shipPageDocumentData!.documentNumber,
                        receiver: this.shipPageDocumentData!.personData,
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
            this.service.getNewTransportationLogBookPage(this.logBookId, this.commonLogBookPageData!.originDeclarationId).subscribe({
                next: (result: TransportationLogBookPageEditDTO) => {
                    this.model = result;
                    this.model.commonData = this.commonLogBookPageData;
                    this.model.logBookPermitLicenseId = this.logBookPermitLicenseId;

                    if (this.pageNumber !== undefined && this.pageNumber !== null) {
                        this.model.pageNumber = this.pageNumber;
                    }

                    if (this.pageStatus !== undefined && this.pageStatus !== null) {
                        this.model.status = this.service.getPageStatusTranslation(this.pageStatus);
                    }

                    for (const product of this.model.products ?? []) {
                        product.hasMissingProperties = true;
                    }

                    this.originPossibleProducts = this.model.originalPossibleProducts?.slice() ?? [];
                    this.model.originalPossibleProducts = []; // за да не се мапират обратно към бекенда

                    if (this.model.commonData!.originDeclarationId === null || this.model.commonData!.originDeclarationId === undefined) {
                        this.canAddProducts = true;
                    }
                    else if (this.originPossibleProducts.length === 0) {
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
        this.commonLogBookPageData = data.commonData;
        this.logBookId = data.logBookId;
        this.logBookTypeId = data.logBookTypeId;
        this.logBookPermitLicenseId = data.logBookPermitLicenseId;
       
        this.pageNumber = data.pageNumber;
        this.pageStatus = data.pageStatus;
        this.shipPageDocumentData = data.shipPageDocumentData;

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

        if (this.form.valid) {
            this.fillModel();
            this.model = CommonUtils.sanitizeModelStrings(this.model);

            this.confirmDialog.open({
                title: this.translationService.getValue('catches-and-sales.complete-page-confirm-dialog-title'),
                message: this.translationService.getValue('catches-and-sales.complete-page-confirm-dialog-message'),
                okBtnLabel: this.translationService.getValue('catches-and-sales.complete-page-confirm-dialog-ok-btn-label')
            }).subscribe({
                next: (ok: boolean) => {
                    if (ok) {
                        if (this.id === null || this.id === undefined) {
                            this.addTransportationLogBookPage(dialogClose);
                        }
                        else {
                            this.service.editTransportationLogBookPage(this.model).subscribe({
                                next: () => {
                                    dialogClose(this.model);
                                },
                                error: (errorResponse: HttpErrorResponse) => {
                                    const error: ErrorModel | undefined = errorResponse.error;

                                    if (error?.code === ErrorCode.SendFLUXSalesFailed) {
                                        if (!IS_PUBLIC_APP) { // show snackbar only when not public app
                                            this.snackbar.open(this.translationService.getValue('catches-and-sales.transportation-page-send-to-flux-sales-error'), undefined, {
                                                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                                                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                                            });
                                        }

                                        dialogClose();
                                    }
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
        if (controlName === 'loadingDateControl') {
            if (errorCode === 'logBookPageDateLocked') {
                const message: string = this.translationService.getValue('catches-and-sales.transportation-page-date-cannot-be-chosen-error');
                return new TLError({ text: message, type: 'error' });
            }
            else if (errorCode === 'mindate') {
                const minDate: Date | undefined = this.getMinLoadingDate(this.commonLogBookPageData);
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

    private buildForm(): void {
        this.form = new FormGroup({
            pageNumberControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)]),
            vehicleIdentificationControl: new FormControl(undefined, [Validators.required, Validators.maxLength(50)]),
            statusControl: new FormControl(),
            loadingLocationControl: new FormControl(undefined, [Validators.required, Validators.maxLength(500)]),
            loadingDateControl: new FormControl(undefined),
            deliveryLocationControl: new FormControl(undefined, Validators.required),

            commonLogBookPageDataControl: new FormControl(),
            receiverPersonControl: new FormControl(undefined, Validators.required),
            productsControl: new FormControl(),

            filesControl: new FormControl()
        });

        this.setMinLoadingDateControlValidators(); 
    }

    private fillModel(): void {
        this.model.pageNumber = this.form.get('pageNumberControl')!.value;
        this.model.vehicleIdentification = this.form.get('vehicleIdentificationControl')!.value;

        this.model.loadingLocation = this.form.get('loadingLocationControl')!.value;
        this.model.loadingDate = this.form.get('loadingDateControl')!.value;
        this.model.deliveryLocation = this.form.get('deliveryLocationControl')!.value;

        this.model.commonData = this.form.get('commonLogBookPageDataControl')!.value;
        this.model.receiver = this.form.get('receiverPersonControl')!.value;
        this.model.products = this.form.get('productsControl')!.value;

        this.model.files = this.form.get('filesControl')!.value;
    }

    private fillForm(): void {
        this.form.get('pageNumberControl')!.setValue(this.model.pageNumber);
        this.form.get('vehicleIdentificationControl')!.setValue(this.model.vehicleIdentification);

        if (!this.isAdd) {
            this.form.get('statusControl')!.setValue(this.model.status);
        }

        this.form.get('loadingLocationControl')!.setValue(this.model.loadingLocation);
        this.form.get('loadingDateControl')!.setValue(this.model.loadingDate);
        this.form.get('deliveryLocationControl')!.setValue(this.model.deliveryLocation);

        this.form.get('commonLogBookPageDataControl')!.setValue(this.model.commonData);
        this.form.get('receiverPersonControl')!.setValue(this.model.receiver);
        this.form.get('productsControl')!.setValue(this.model.products);

        this.form.get('filesControl')!.setValue(this.model.files);
    }

    private addTransportationLogBookPage(dialogClose: DialogCloseCallback): void {
        this.service.addTransportationLogBookPage(this.model, this.hasMissingPagesRangePermission).subscribe({
            next: (id: number) => {
                this.model.id = id;
                dialogClose(this.model);
            },
            error: (response: HttpErrorResponse) => {
                const error: ErrorModel | undefined = response.error;

                if (error?.code === ErrorCode.PageNumberNotInLogbook) {
                    this.snackbar.open(
                        this.translationService.getValue('catches-and-sales.transportation-page-not-in-range-error'),
                        undefined,
                        {
                            duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                            panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                        });
                }
                else if (error?.code === ErrorCode.PageNumberNotInLogBookLicense) {
                    this.snackbar.open(
                        this.translationService.getValue('catches-and-sales.transportation-page-not-in-log-book-license-range-error'),
                        undefined,
                        {
                            duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                            panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                        });
                }
                else if (error?.code === ErrorCode.LogBookPageAlreadySubmitted) {
                    this.snackbar.open(
                        this.translationService.getValue('catches-and-sales.transportation-page-already-submitted-error'),
                        undefined,
                        {
                            duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                            panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                        });
                }
                else if (error?.code === ErrorCode.LogBookPageAlreadySubmittedOtherLogBook) {
                    const message: string = this.translationService.getValue('catches-and-sales.transportation-page-already-submitted-other-logbook-error');
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
                        this.snackbar.open(this.translationService.getValue('catches-and-sales.transportation-page-send-to-flux-sales-error'), undefined, {
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

                    const btnMsg1: string = this.translationService.getValue('catches-and-sales.transportation-page-permit-generate-missing-pages-first-part');
                    const btnMsg2: string = this.translationService.getValue('catches-and-sales.transportation-page-permit-generate-missing-pages-second-part');

                    this.confirmDialog.open({
                        title: this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-dialog-title'),
                        message: message,
                        okBtnLabel: `${btnMsg1} ${diff} ${btnMsg2}`,
                        okBtnColor: 'warn'
                    }).subscribe({
                        next: (ok: boolean | undefined) => {
                            this.hasMissingPagesRangePermission = ok ?? false;

                            if (this.hasMissingPagesRangePermission) {
                                this.addTransportationLogBookPage(dialogClose); // start add method again
                            }
                        }
                    });
                }
            }
        });
    }

    private setMinLoadingDateControlValidators(): void {
        this.form.get('loadingDateControl')!.setValidators([
            Validators.required,
            TLValidators.minDate(undefined, this.getMinLoadingDate(this.commonLogBookPageData)),
            this.checkDateValidityVsLockPeriodsValidator()
        ]);

        this.form.get('loadingDateControl')!.markAsPending({ emitEvent: false });
        this.form.get('loadingDateControl')!.updateValueAndValidity({ emitEvent: false });
    }

    private getMinLoadingDate(commonData: CommonLogBookPageDataDTO | undefined): Date | undefined {
        let result: Date | undefined;

        if (commonData !== undefined && commonData !== null) {
            if (commonData.originDeclarationDate !== undefined && commonData.originDeclarationDate !== null) {
                result = commonData.originDeclarationDate;
            }
        }

        return result;
    }

    private checkDateValidityVsLockPeriodsValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === null || control === undefined) {
                return null;
            }

            if (this.model === null || this.model === undefined) {
                return null;
            }

            const fillDate: Date | undefined = control.value;

            if (fillDate === null || fillDate === undefined) {
                return null;
            }

            const now: Date = new Date();

            const logBookTypeId: number = this.logBookTypeId!;
            const logBookId: number = this.model.logBookId!;

            if (CatchesAndSalesUtils.checkIfPageDateIsUnlocked(this.logBookPageEditExceptions, this.currentUserId, logBookTypeId, logBookId, fillDate, now)) {
                return null;
            }

            if (CatchesAndSalesUtils.pageHasLogBookPageDateLockedViaDaysAfterMonth(fillDate, now, this.lockTransportationLogBookPeriod)) {
                return {
                    logBookPageDateLocked: {
                        lockedPeriod: this.lockTransportationLogBookPeriod,
                        periodType: 'days-after-month'
                    }
                };
            }

            return null;
        }
    }
}