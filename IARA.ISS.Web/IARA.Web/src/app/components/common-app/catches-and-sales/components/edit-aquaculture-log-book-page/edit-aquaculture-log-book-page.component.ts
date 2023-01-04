import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
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
import { CatchesAndSalesDialogParamsModel } from '../../models/catches-and-sales-dialog-params.model';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';


@Component({
    selector: 'edit-aquaculture-log-book-page',
    templateUrl: './edit-aquaculture-log-book-page.component.html'
})
export class EditAquacultureLogBookPageComponent implements OnInit, IDialogComponent {
    public readonly pageCode: PageCodeEnum = PageCodeEnum.AquacultureLogBookPage;
    public readonly logBookType: LogBookTypesEnum = LogBookTypesEnum.Aquaculture;
    public readonly currentDate: Date = new Date();
    public readonly dateTimeControlHint: string;

    public form!: FormGroup;
    public viewMode!: boolean;
    public model!: AquacultureLogBookPageEditDTO;
    public service!: ICatchesAndSalesService;
    public isAdd: boolean = false;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private id: number | undefined;
    private logBookId!: number;

    private translationService: FuseTranslationLoaderService;
    private snackbar: MatSnackBar;
    private confirmDialog: TLConfirmDialog;
    private hasMissingPagesRangePermission: boolean = false;

    public constructor(translationService: FuseTranslationLoaderService, snackbar: MatSnackBar, confirmDialog: TLConfirmDialog) {
        this.translationService = translationService;
        this.snackbar = snackbar;
        this.confirmDialog = confirmDialog;

        this.dateTimeControlHint = this.translationService.getValue('common.date-time-control-format-hint');
    }

    public ngOnInit(): void {
        if (this.id !== null && this.id !== undefined) {
            this.service.getAquacultureLogBookPage(this.id).subscribe({
                next: (value: AquacultureLogBookPageEditDTO) => {
                    this.model = value;
                    this.model.status = this.service.getPageStatusTranslation(LogBookPageStatusesEnum[this.model.status! as keyof typeof LogBookPageStatusesEnum]);

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

        if (this.form.valid) {
            this.fillModel();
            this.model = CommonUtils.sanitizeModelStrings(this.model);

            if (this.id === null || this.id === undefined) {
                this.addAquacultureLogBookPage(dialogClose);
            }
            else {
                this.service.editAquacultureLogBookPage(this.model).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    }
                });
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            pageNumberControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)]),
            statusControl: new FormControl(),
            fillDateControl: new FormControl(undefined, Validators.required),
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
            }
        });
    }
}