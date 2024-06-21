import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { CatchesAndSalesDialogParamsModel } from '../../models/catches-and-sales-dialog-params.model';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { FirstSaleLogBookPageEditDTO } from '@app/models/generated/dtos/FirstSaleLogBookPageEditDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { CommonLogBookPageDataDTO } from '@app/models/generated/dtos/CommonLogBookPageDataDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { LogBookPageProductDTO } from '@app/models/generated/dtos/LogBookPageProductDTO';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { BasicLogBookPageDocumentDataDTO } from '@app/models/generated/dtos/BasicLogBookPageDocumentDataDTO';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { LogBookPageDocumentTypesEnum } from '../../enums/log-book-page-document-types.enum';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { DateDifference } from '@app/models/common/date-difference.model';
import { DateUtils } from '@app/shared/utils/date.utils';
import { LockFirstSaleLogBookPeriodsModel } from './models/lock-first-sale-log-book-periods.model';
import { SystemParametersService } from '@app/services/common-app/system-parameters.service';
import { SystemPropertiesDTO } from '@app/models/generated/dtos/SystemPropertiesDTO';
import { CatchesAndSalesUtils } from '@app/components/common-app/catches-and-sales/utils/catches-and-sales.utils';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { LogBookPageEditExceptionDTO } from '@app/models/generated/dtos/LogBookPageEditExceptionDTO';
import { SecurityService } from '@app/services/common-app/security.service';

@Component({
    selector: 'edit-first-sale-log-book-page',
    templateUrl: './edit-first-sale-log-book-page.component.html'
})
export class EditFirstSaleLogBookPageComponent implements OnInit, AfterViewInit, IDialogComponent {
    public readonly pageCode: PageCodeEnum = PageCodeEnum.FirstSaleLogBookPage;
    public readonly logBookType: LogBookTypesEnum = LogBookTypesEnum.FirstSale;
    public readonly currentDate: Date = new Date();

    public form!: FormGroup;
    public viewMode!: boolean;
    public service!: ICatchesAndSalesService;
    public originPossibleProducts: LogBookPageProductDTO[] = [];
    public registeredBuyers: NomenclatureDTO<number>[] = [];
    public isAdd: boolean = false;

    public noAvailableProducts: boolean = false;
    public isLogBookPageDateLockedError: boolean = false;

    public getControlErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.getControlErrorLabelText.bind(this);

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private model!: FirstSaleLogBookPageEditDTO;
    private id: number | undefined;
    private pageNumber: number | undefined;
    private pageStatus: LogBookPageStatusesEnum | undefined;

    private logBookId!: number;
    private logBookTypeId!: number;
    private commonLogBookPageData: CommonLogBookPageDataDTO | undefined;
    /** Has value when the component is open from a declaration of a ship log book page */
    private shipPageDocumentData: BasicLogBookPageDocumentDataDTO | undefined;
    private hasMissingPagesRangePermission: boolean = false;

    private lockFirstSaleLogBookPeriods!: LockFirstSaleLogBookPeriodsModel;
    private logBookPageEditExceptions: LogBookPageEditExceptionDTO[] = [];
    private currentUserId: number;

    private readonly currencyPipe: CurrencyPipe;
    private readonly translationService: FuseTranslationLoaderService;
    private readonly snackbar: MatSnackBar;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly datePipe: DatePipe;
    private readonly systemParametersService: SystemParametersService;

    public constructor(
        translationService: FuseTranslationLoaderService,
        currencyPipe: CurrencyPipe,
        snackbar: MatSnackBar,
        confirmDialog: TLConfirmDialog,
        datePipe: DatePipe,
        systemParametersService: SystemParametersService,
        authService: SecurityService
    ) {
        this.translationService = translationService;
        this.currencyPipe = currencyPipe;
        this.snackbar = snackbar;
        this.confirmDialog = confirmDialog;
        this.datePipe = datePipe;
        this.systemParametersService = systemParametersService;

        this.currentUserId = authService.User!.userId;
    }

    public async ngOnInit(): Promise<void> {
        this.registeredBuyers = await this.service.getRegisteredBuyersNomenclature().toPromise();

        if (!this.viewMode) {
            this.logBookPageEditExceptions = await this.service.getLogBookPageEditExceptions().toPromise();
        }

        const systemParameters: SystemPropertiesDTO = await this.systemParametersService.systemParameters();
        this.lockFirstSaleLogBookPeriods = new LockFirstSaleLogBookPeriodsModel({
            lockFirstSaleBelow200KLogBookAfterHours: systemParameters.lockFirstSaleBelow200KLogBookAfterHours,
            lockFirstSaleAbove200KLogBookAfterHours: systemParameters.lockFirstSaleAbove200KLogBookAfterHours,
            lockFirstSaleLogBookPeriod: systemParameters.addFirstSalePagesDaysTolerance
        });

        if (this.id !== null && this.id !== undefined) {
            this.service.getFirstSaleLogBookPage(this.id).subscribe({
                next: (result: FirstSaleLogBookPageEditDTO) => {
                    this.model = result;
                    this.model.status = this.service.getPageStatusTranslation(LogBookPageStatusesEnum[this.model.status! as keyof typeof LogBookPageStatusesEnum]);

                    this.originPossibleProducts = this.model.originalPossibleProducts ?? [];
                    this.model.originalPossibleProducts = []; // за да не се мапират обратно към бекенда

                    this.commonLogBookPageData = this.model.commonData;

                    if (!this.viewMode) {
                        this.setSaleDateControlValidators();
                    }

                    this.fillForm();
                }
            });
        }
        else if (this.shipPageDocumentData !== null && this.shipPageDocumentData !== undefined) {
            this.service.getPossibleProducts(this.shipPageDocumentData!.shipLogBookPageId!, LogBookPageDocumentTypesEnum.FirstSaleDocument).subscribe({
                next: (result: LogBookPageProductDTO[]) => {
                    this.model = new FirstSaleLogBookPageEditDTO({
                        buyerId: this.shipPageDocumentData!.registeredBuyerId,
                        buyerName: this.registeredBuyers.find(x => x.value === this.shipPageDocumentData!.registeredBuyerId!)!.displayName,
                        commonData: this.shipPageDocumentData!.sourceData,
                        logBookId: this.shipPageDocumentData!.logBookId,
                        logBookNumber: this.shipPageDocumentData!.logBookNumber,
                        pageNumber: this.shipPageDocumentData!.documentNumber,
                        originalPossibleProducts: result.slice(),
                        products: JSON.parse(JSON.stringify(result.slice()))
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
            let admissionDocumentId: number | undefined;

            if (this.commonLogBookPageData.admissionDocumentId !== null && this.commonLogBookPageData.admissionDocumentId !== undefined) {
                admissionDocumentId = this.commonLogBookPageData.admissionDocumentId;
            }
            else if (this.commonLogBookPageData.transportationDocumentId !== null && this.commonLogBookPageData.transportationDocumentId !== undefined) {
                transportationDocumentId = this.commonLogBookPageData.transportationDocumentId;
            }
            else {
                originDeclarationId = this.commonLogBookPageData.originDeclarationId;
            }

            this.service.getNewFirstSaleLogBookPage(this.logBookId, originDeclarationId, transportationDocumentId, admissionDocumentId).subscribe({
                next: (result: FirstSaleLogBookPageEditDTO) => {
                    this.model = result;
                    this.model.commonData = this.commonLogBookPageData;

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

    public ngAfterViewInit(): void {
        this.form.get('productsControl')!.valueChanges.subscribe({
            next: (products: LogBookPageProductDTO[] | undefined) => {
                this.updateProductsTotalPrice(products);
            }
        });
    }

    public setData(data: CatchesAndSalesDialogParamsModel, buttons: DialogWrapperData): void {
        this.id = data.id;
        this.logBookId = data.logBookId;
        this.service = data.service as ICatchesAndSalesService;
        this.viewMode = data.viewMode;
        this.commonLogBookPageData = data.commonData;
        this.pageNumber = data.pageNumber;
        this.pageStatus = data.pageStatus;
        this.shipPageDocumentData = data.shipPageDocumentData;
        this.logBookTypeId = data.logBookTypeId;

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
                next: (ok: boolean) => {
                    if (ok) {
                        if (this.id === null || this.id === undefined) {
                            this.addFirstSaleLogBookPage(dialogClose);
                        }
                        else {
                            this.service.editFirstSaleLogBookPage(this.model).subscribe({
                                next: () => {
                                    dialogClose(this.model);
                                },
                                error: (response: HttpErrorResponse) => {
                                    this.addOrEditFirstSaleLogBookPageErrorHandle(response, dialogClose);
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
        if (controlName === 'saleDateControl') {
            if (errorCode === 'logBookPageDateLocked' || errorCode === 'logBookPageDatePeriodLocked') {
                const message: string = this.translationService.getValue('catches-and-sales.first-sale-date-cannot-be-chosen-error');
                if (this.isLogBookPageDateLockedError || errorCode === 'logBookPageDatePeriodLocked') {
                    return new TLError({ text: message, type: 'error' });
                }
                else {
                    return new TLError({ text: message, type: 'warn' });
                }
            }
            else if (errorCode === 'mindate') {
                const minDate: Date | undefined = this.getMinSaleDate();
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
            logBookNumberControl: new FormControl(),
            pageNumberControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)]),
            statusControl: new FormControl(),
            saleDateControl: new FormControl(undefined),
            saleContractNumberControl: new FormControl(undefined, Validators.maxLength(100)),
            saleLocationControl: new FormControl(undefined, [Validators.maxLength(500), Validators.required]),
            saleContractDateControl: new FormControl(),

            commonLogBookPageDataControl: new FormControl(),
            buyerControl: new FormControl(null, Validators.required),
            productsControl: new FormControl(),
            productsTotalValueControl: new FormControl(),

            filesControl: new FormControl()
        });

        this.setSaleDateControlValidators();
    }

    private fillForm(): void {
        this.form.get('logBookNumberControl')!.setValue(this.model.logBookNumber);
        this.form.get('pageNumberControl')!.setValue(this.model.pageNumber);
        this.form.get('saleDateControl')!.setValue(this.model.saleDate);
        this.form.get('saleContractNumberControl')!.setValue(this.model.saleContractNumber);
        this.form.get('saleLocationControl')!.setValue(this.model.saleLocation);
        this.form.get('saleContractDateControl')!.setValue(this.model.saleContractDate);

        this.form.get('commonLogBookPageDataControl')!.setValue(this.model.commonData);

        if (this.model.buyerId !== null && this.model.buyerId !== undefined) {
            const buyer: NomenclatureDTO<number> = this.registeredBuyers.find(x => x.value === this.model.buyerId)!;
            this.form.get('buyerControl')!.setValue(buyer);
        }

        this.form.get('productsControl')!.setValue(this.model.products);

        if (this.model.productsTotalPrice !== null && this.model.productsTotalPrice !== undefined) {
            const formattedTotalPrice: string | null = this.currencyPipe.transform(this.model.productsTotalPrice, 'BGN', 'symbol', '0.2-2', 'bg-BG');
            this.form.get('productsTotalValueControl')!.setValue(formattedTotalPrice);
        }
        else {
            this.updateProductsTotalPrice(this.model.products);
        }

        this.form.get('filesControl')!.setValue(this.model.files);

        if (!this.isAdd) {
            this.form.get('statusControl')!.setValue(this.model.status);
        }
    }

    private fillModel(): void {
        this.model.pageNumber = this.form.get('pageNumberControl')!.value;
        this.model.saleDate = this.form.get('saleDateControl')!.value;
        this.model.saleContractNumber = this.form.get('saleContractNumberControl')!.value;
        this.model.saleLocation = this.form.get('saleLocationControl')!.value;
        this.model.saleContractDate = this.form.get('saleContractDateControl')!.value;
        this.model.buyerId = this.form.get('buyerControl')!.value?.value;
        this.model.products = this.form.get('productsControl')!.value;
        this.model.productsTotalPrice = this.form.get('productsTotalValueControl')!.value;
        this.model.files = this.form.get('filesControl')!.value;
    }

    private addFirstSaleLogBookPage(dialogClose: DialogCloseCallback): void {
        this.service.addFirstSaleLogBookPage(this.model, this.hasMissingPagesRangePermission).subscribe({
            next: (id: number) => {
                this.model.id = id;
                dialogClose(this.model);
            },
            error: (response: HttpErrorResponse) => {
                this.addOrEditFirstSaleLogBookPageErrorHandle(response, dialogClose);
            }
        });
    }

    private addOrEditFirstSaleLogBookPageErrorHandle(response: HttpErrorResponse, dialogClose: DialogCloseCallback): void {
        const error: ErrorModel | undefined = response.error;

        if (error?.code === ErrorCode.PageNumberNotInLogbook) {
            this.snackbar.open(this.translationService.getValue('catches-and-sales.first-sale-page-not-in-range-error'), undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
            });
        }
        else if (error?.code === ErrorCode.LogBookPageAlreadySubmitted) {
            this.snackbar.open(this.translationService.getValue('catches-and-sales.first-sale-page-already-submitted-error'), undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
            });
        }
        else if (error?.code === ErrorCode.LogBookPageAlreadySubmittedOtherLogBook) {
            const message: string = this.translationService.getValue('catches-and-sales.first-sale-page-already-submitted-other-logbook-error');
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
                this.snackbar.open(this.translationService.getValue('catches-and-sales.first-sale-page-send-to-flux-sales-error'), undefined, {
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

            const btnMsg1: string = this.translationService.getValue('catches-and-sales.first-sale-page-permit-generate-missing-pages-first-part');
            const btnMsg2: string = this.translationService.getValue('catches-and-sales.first-sale-page-permit-generate-missing-pages-second-part');

            this.confirmDialog.open({
                title: this.translationService.getValue('catches-and-sales.first-sale-page-generate-missing-pages-permission-dialog-title'),
                message: message,
                okBtnLabel: `${btnMsg1} ${diff} ${btnMsg2}`,
                okBtnColor: 'warn'
            }).subscribe({
                next: (ok: boolean | undefined) => {
                    this.hasMissingPagesRangePermission = ok ?? false;

                    if (this.hasMissingPagesRangePermission) {
                        this.addFirstSaleLogBookPage(dialogClose); // start add method again
                    }
                }
            });
        }
        else if (error?.code === ErrorCode.CannotAddEditFirstSalePageAboveLimitTurnover) {
            const msg1: string = this.translationService.getValue('catches-and-sales.first-sale-cannot-add-page-turnover-above-limit-error');
            const msg2: string = this.translationService.getValue('catches-and-sales.first-sale-hours-after-sale');
            const msg3: string = this.translationService.getValue('catches-and-sales.first-sale-to-add-page-after-locked-period-contanct-admin');

            const msg = `${msg1} ${this.lockFirstSaleLogBookPeriods.lockFirstSaleAbove200KLogBookAfterHours} ${msg2}. ${msg3}.`;

            this.snackbar.open(msg, undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
            });
        }
        else if (error?.code === ErrorCode.CannotAddEditFirstSalePageBelowLimitTurnover) {
            const msg1: string = this.translationService.getValue('catches-and-sales.first-sale-cannot-add-page-turnover-below-limit-error');
            const msg2: string = this.translationService.getValue('catches-and-sales.first-sale-hours-after-sale');
            const msg3: string = this.translationService.getValue('catches-and-sales.first-sale-to-add-page-after-locked-period-contanct-admin');

            const msg = `${msg1} ${this.lockFirstSaleLogBookPeriods.lockFirstSaleBelow200KLogBookAfterHours} ${msg2}. ${msg3}.`;

            this.snackbar.open(msg, undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
            });
        }
    }

    private updateProductsTotalPrice(products: LogBookPageProductDTO[] | undefined): void {
        const totalPrice: number | undefined = this.calculateProductsTotalPrice(products);
        const formattedTotalPrice: string | null = this.currencyPipe.transform(totalPrice?.toString(), 'BGN', 'symbol', '0.2-2', 'bg-BG');
        this.form.get('productsTotalValueControl')!.setValue(formattedTotalPrice);
    }

    private calculateProductsTotalPrice(products: LogBookPageProductDTO[] | undefined): number | undefined {
        if (products !== null && products !== undefined && products.length > 0) {
            const totalPrice: number = products.reduce((sum, current) => sum + (current.quantityKg! * current.unitPrice!), 0);
            return totalPrice;
        }
        else {
            return undefined;
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

            const saleDate: Date | undefined = control.value;

            if (saleDate === null || saleDate === undefined) {
                return null;
            }

            const now: Date = new Date();

            saleDate.setHours(now.getHours());
            saleDate.setMinutes(now.getMinutes());
            saleDate.setSeconds(now.getSeconds());
            saleDate.setMilliseconds(now.getMilliseconds());

            const logBookTypeId: number = this.logBookTypeId!;
            const logBookId: number = this.model.logBookId!;

            const difference: DateDifference | undefined = DateUtils.getDateDifference(saleDate, now);

            if (difference === null || difference === undefined) {
                return null;
            }

            if (difference.minutes === 0 && difference.hours === 0 && difference.days === 0) {
                return null;
            }

            const hoursDifference: number = CatchesAndSalesUtils.convertDateDifferenceToHours(difference);

            if (CatchesAndSalesUtils.pageHasLogBookPageDateLockedViaDaysAfterMonth(saleDate, now, this.lockFirstSaleLogBookPeriods.lockFirstSaleLogBookPeriod)
                && !CatchesAndSalesUtils.checkIfPageDateIsUnlocked(this.logBookPageEditExceptions, this.currentUserId, logBookTypeId, logBookId, saleDate, now)
            ) {
                return {
                    logBookPageDatePeriodLocked: {
                        hasAboveLimitAnnualTurnOver: false,
                        lockedPeriod: this.lockFirstSaleLogBookPeriods.lockFirstSaleLogBookPeriod,
                        periodType: 'days-after-month'
                    }
                };
            }

            //предупреждения
            if (this.model.hasAbove200KAnnualTurnover === true) {
                if (hoursDifference > this.lockFirstSaleLogBookPeriods.lockFirstSaleAbove200KLogBookAfterHours) {
                    return {
                        logBookPageDateLocked: {
                            hasAboveLimitAnnualTurnOver: true,
                            lockedPeriod: this.lockFirstSaleLogBookPeriods.lockFirstSaleAbove200KLogBookAfterHours,
                            periodType: 'hours'
                        }
                    };
                }
            }
            else {
                if (hoursDifference > this.lockFirstSaleLogBookPeriods.lockFirstSaleBelow200KLogBookAfterHours) {
                    return {
                        logBookPageDateLocked: {
                            hasAboveLimitAnnualTurnOver: false,
                            lockedPeriod: this.lockFirstSaleLogBookPeriods.lockFirstSaleBelow200KLogBookAfterHours,
                            periodType: 'hours'
                        }
                    };
                }
            }
            return null;
        }
    }

    private setSaleDateControlValidators(): void {
        this.form.get('saleDateControl')!.setValidators([
            Validators.required,
            TLValidators.minDate(undefined, this.getMinSaleDate()),
            this.checkDateValidityVsLockPeriodsValidator()
        ]);

        this.form.get('saleDateControl')!.markAsPending({ emitEvent: false });
        this.form.get('saleDateControl')!.updateValueAndValidity({ emitEvent: false });
    }

    private getMinSaleDate(): Date | undefined {
        let result: Date | undefined;

        if (this.commonLogBookPageData !== undefined && this.commonLogBookPageData !== null) {
            if (this.commonLogBookPageData.admissionHandoverDate !== undefined && this.commonLogBookPageData.admissionHandoverDate !== null) {
                result = this.commonLogBookPageData.admissionHandoverDate;
            }
            else if (this.commonLogBookPageData.transportationDocumentDate !== undefined && this.commonLogBookPageData.transportationDocumentDate !== null) {
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
                if (key === 'saleDateControl' && !this.isLogBookPageDateLockedError) {
                    if (!this.isLogBookPageDateLockedError) {
                        for (const error in this.form.controls[key].errors) {
                            if (error !== 'logBookPageDateLocked') {
                                errors[key] = this.form.controls[key].errors![error];
                            }
                        }
                    }
                    else {
                        for (const error in this.form.controls[key].errors) {
                            if (error !== 'logBookPageDatePeriodLocked') {
                                errors[key] = this.form.controls[key].errors![error];
                            }
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
}