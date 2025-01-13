import { Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { AquacultureLogBookPageEditDTO } from '@app/models/generated/dtos/AquacultureLogBookPageEditDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { CatchesAndSalesDialogParamsModel } from '@app/components/common-app/catches-and-sales/models/catches-and-sales-dialog-params.model';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { SystemParametersService } from '@app/services/common-app/system-parameters.service';
import { CatchesAndSalesUtils } from '@app/components/common-app/catches-and-sales/utils/catches-and-sales.utils';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { SystemPropertiesDTO } from '@app/models/generated/dtos/SystemPropertiesDTO';
import { LogBookPageEditExceptionDTO } from '@app/models/generated/dtos/LogBookPageEditExceptionDTO';
import { SecurityService } from '@app/services/common-app/security.service';

@Component({
    selector: 'edit-aquaculture-log-book-page',
    templateUrl: './edit-aquaculture-log-book-page.component.html'
})
export class EditAquacultureLogBookPageComponent implements OnInit, IDialogComponent {
    public readonly pageCode: PageCodeEnum = PageCodeEnum.AquacultureLogBookPage;
    public readonly logBookType: LogBookTypesEnum = LogBookTypesEnum.Aquaculture;
    public readonly currentDate: Date = new Date();
    public readonly dateTimeControlHint: string;

    public getControlErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.getControlErrorLabelText.bind(this);

    public form!: FormGroup;
    public viewMode!: boolean;
    public model!: AquacultureLogBookPageEditDTO;
    public service!: ICatchesAndSalesService;
    public isAdd: boolean = false;
    public isLogBookPageDateLockedError: boolean = false;
    public logBookPageDateLockedViaDaysAfterMonth: boolean = false;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private id: number | undefined;
    private logBookId!: number;

    private hasMissingPagesRangePermission: boolean = false;
    private lockAquacultureLogBookPeriod!: number;
    private logBookPageEditExceptions: LogBookPageEditExceptionDTO[] = [];
    private currentUserId: number;

    private readonly translationService: FuseTranslationLoaderService;
    private readonly snackbar: MatSnackBar;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly systemParametersService: SystemParametersService;

    public constructor(
        translationService: FuseTranslationLoaderService,
        snackbar: MatSnackBar,
        confirmDialog: TLConfirmDialog,
        systemParametersService: SystemParametersService,
        authService: SecurityService
    ) {
        this.translationService = translationService;
        this.snackbar = snackbar;
        this.confirmDialog = confirmDialog;
        this.systemParametersService = systemParametersService;

        this.currentUserId = authService.User!.userId;

        this.dateTimeControlHint = this.translationService.getValue('common.date-time-control-format-hint');
    }

    public async ngOnInit(): Promise<void> {
        const systemParameters: SystemPropertiesDTO = await this.systemParametersService.systemParameters();
        this.lockAquacultureLogBookPeriod = systemParameters.addAquaculturePagesDaysTolerance!;

        if (!this.viewMode) {
            this.logBookPageEditExceptions = await this.service.getLogBookPageEditExceptions().toPromise();
        }

        if (this.id !== null && this.id !== undefined) {
            this.service.getAquacultureLogBookPage(this.id).subscribe({
                next: (value: AquacultureLogBookPageEditDTO) => {
                    this.model = value;
                    const pageStatus: LogBookPageStatusesEnum = LogBookPageStatusesEnum[this.model.status! as keyof typeof LogBookPageStatusesEnum];
                    this.model.status = this.service.getPageStatusTranslation(pageStatus);

                    if (pageStatus === LogBookPageStatusesEnum.Missing) {
                        this.isAdd = true;
                    }

                    this.fillForm();
                }
            });
        }
        else {
            this.service.getNewAquacultureLogBookPage(this.logBookId).subscribe({
                next: (result: AquacultureLogBookPageEditDTO) => {
                    this.model = result;
                    this.fillForm();
                }
            });
        }
    }

    public setData(data: CatchesAndSalesDialogParamsModel, buttons: DialogWrapperData): void {
        this.id = data.id;
        this.service = data.service as ICatchesAndSalesService;
        this.viewMode = data.viewMode;
        this.logBookId = data.logBookId;

        if (this.id === null || this.id === undefined) {
            this.isAdd = true;
        }
        else {
            this.isAdd = false;
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
                            this.addAquacultureLogBookPage(dialogClose);
                        }
                        else {
                            this.service.editAquacultureLogBookPage(this.model).subscribe({
                                next: () => {
                                    dialogClose(this.model);
                                },
                                error: (response: HttpErrorResponse) => {
                                    this.addOrEditAquacultureLogBookPageErrorHandle(response, dialogClose);
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
        if (controlName === 'fillDateControl') {
            if (errorCode === 'logBookPageDateLocked') {
                const message: string = this.translationService.getValue('catches-and-sales.aquaculture-page-date-cannot-be-chosen-error');
                if (this.isLogBookPageDateLockedError || this.logBookPageDateLockedViaDaysAfterMonth) {
                    return new TLError({ text: message, type: 'error' });
                }
                else {
                    return new TLError({ text: message, type: 'warn' });
                }
            }
        }

        return undefined;
    }

    private buildForm(): void {
        this.form = new FormGroup({
            pageNumberControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)]),
            statusControl: new FormControl(),
            fillDateControl: new FormControl(undefined, [Validators.required, this.checkDateValidityVsLockPeriodsValidator()]),
            iaraAcceptanceDateTimeControl: new FormControl(undefined, Validators.required),
            aquacultureFacilityControl: new FormControl(undefined, Validators.required),
            buyerPersonControl: new FormControl(null, Validators.required),
            productsControl: new FormControl(),
            filesControl: new FormControl()
        });
    }

    private fillForm(): void {
        this.form.get('pageNumberControl')!.setValue(this.model.pageNumber);
        this.form.get('fillDateControl')!.setValue(this.model.fillingDate);
        this.form.get('iaraAcceptanceDateTimeControl')!.setValue(this.model.iaraAcceptanceDateTime);
        this.form.get('aquacultureFacilityControl')!.setValue(this.model.aquacultureFacilityName);
        this.form.get('buyerPersonControl')!.setValue(this.model.buyer);
        this.form.get('productsControl')!.setValue(this.model.products);
        this.form.get('filesControl')!.setValue(this.model.files);

        if (!this.isAdd) {
            this.form.get('statusControl')!.setValue(this.model.status);
        }
    }

    private fillModel(): void {
        this.model.pageNumber = this.form.get('pageNumberControl')!.value;
        this.model.fillingDate = this.form.get('fillDateControl')!.value;
        this.model.iaraAcceptanceDateTime = this.form.get('iaraAcceptanceDateTimeControl')!.value;
        this.model.buyer = this.form.get('buyerPersonControl')!.value;
        this.model.products = this.form.get('productsControl')!.value;
        this.model.files = this.form.get('filesControl')!.value;
    }

    private addAquacultureLogBookPage(dialogClose: DialogCloseCallback): void {
        this.service.addAquacultureLogBookPage(this.model, this.hasMissingPagesRangePermission).subscribe({
            next: (id: number) => {
                this.model.id = id;
                dialogClose(this.model);
            },
            error: (response: HttpErrorResponse) => {
                this.addOrEditAquacultureLogBookPageErrorHandle(response, dialogClose);
            }
        });
    }

    private addOrEditAquacultureLogBookPageErrorHandle(response: HttpErrorResponse, dialogClose: DialogCloseCallback): void {
        const error: ErrorModel | undefined = response.error;

        if (error?.code === ErrorCode.PageNumberNotInLogbook) {
            this.snackbar.open(this.translationService.getValue('catches-and-sales.aquaculture-page-not-in-range-error'), undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
            });
        }
        else if (error?.code === ErrorCode.LogBookPageAlreadySubmitted) {
            this.snackbar.open(this.translationService.getValue('catches-and-sales.aquaculture-page-already-submitted-error'), undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
            });
        }
        else if (error?.code === ErrorCode.LogBookPageAlreadySubmittedOtherLogBook) {
            const message: string = this.translationService.getValue('catches-and-sales.aquaculture-page-already-submitted-other-logbook-error');
            this.snackbar.open(
                `${message}: ${error.messages[0]}`,
                undefined,
                {
                    duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                });
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

            const btnMsg1: string = this.translationService.getValue('catches-and-sales.aquaculture-page-permit-generate-missing-pages-first-part');
            const btnMsg2: string = this.translationService.getValue('catches-and-sales.aquaculture-page-permit-generate-missing-pages-second-part');

            this.confirmDialog.open({
                title: this.translationService.getValue('catches-and-sales.aquaculture-page-generate-missing-pages-permission-dialog-title'),
                message: message,
                okBtnLabel: `${btnMsg1} ${diff} ${btnMsg2}`,
                okBtnColor: 'warn'
            }).subscribe({
                next: (ok: boolean | undefined) => {
                    this.hasMissingPagesRangePermission = ok ?? false;

                    if (this.hasMissingPagesRangePermission) {
                        this.addAquacultureLogBookPage(dialogClose); // start add method again
                    }
                }
            });
        }
        else if (error?.code === ErrorCode.CannotAddEditLockedAquaculturePage) {
            const msg1: string = this.translationService.getValue('catches-and-sales.aquaculture-page-cannot-add-page-for-chosen-locked-fill-date-error');
            const msg2: string = this.translationService.getValue('catches-and-sales.aquaculture-page-because-days-have-past-since-pervious-month');
            const msg3: string = this.translationService.getValue('catches-and-sales.aquaculture-page-to-add-page-after-locked-period-contanct-admin');

            const msg: string = `${msg1} ${this.lockAquacultureLogBookPeriod} ${msg2}. ${msg3}`;

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

            const fillDate: Date | undefined = control.value;

            if (fillDate === null || fillDate === undefined) {
                return null;
            }

            const now: Date = new Date();

            const logBookTypeId: number = this.model.logBookTypeId!;
            const logBookId: number = this.model.logBookId!;

            if (CatchesAndSalesUtils.checkIfPageDateIsUnlocked(this.logBookPageEditExceptions, this.currentUserId, logBookTypeId, logBookId, fillDate, now)) {
                return null;
            }

            if (CatchesAndSalesUtils.pageHasLogBookPageDateLockedViaDaysAfterMonth(fillDate, now, this.lockAquacultureLogBookPeriod)) {
                this.logBookPageDateLockedViaDaysAfterMonth = true;

                return {
                    logBookPageDateLocked: {
                        lockedPeriod: this.lockAquacultureLogBookPeriod,
                        periodType: 'days-after-month'
                    }
                };
            }

            this.logBookPageDateLockedViaDaysAfterMonth = false;
            return null;
        }
    }

    private isFormValid(): boolean {
        if (this.form.valid) {
            return true;
        }
        else {
            const errors: ValidationErrors = {};

            for (const key of Object.keys(this.form.controls)) {
                if (key === 'fillDateControl' && !this.isLogBookPageDateLockedError && !this.logBookPageDateLockedViaDaysAfterMonth) {
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
}