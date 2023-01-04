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

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private model!: AdmissionLogBookPageEditDTO;
    private id: number | undefined;
    /** Needed only for Admission log book pages, when the log book is for Person/Legal */
    public logBookPermitLicenseId: number | undefined;
    private shipPageDocumentData: BasicLogBookPageDocumentDataDTO | undefined;
    private commonLogBookPageData: CommonLogBookPageDataDTO | undefined;
    private logBookId!: number;
    private pageNumber: number | undefined;
    private pageStatus: LogBookPageStatusesEnum | undefined;
    private translationService: FuseTranslationLoaderService;
    private snackbar: MatSnackBar;
    private confirmDialog: TLConfirmDialog;
    private hasMissingPagesRangePermission: boolean = false;

    public constructor(translationService: FuseTranslationLoaderService, snackbar: MatSnackBar, confirmDialog: TLConfirmDialog) {
        this.translationService = translationService;
        this.snackbar = snackbar;
        this.confirmDialog = confirmDialog;
    }

    public ngOnInit(): void {
        if (this.id !== null && this.id !== undefined) {
            this.service.getAdmissionLogBookPage(this.id).subscribe({
                next: (result: AdmissionLogBookPageEditDTO) => {
                    this.model = result;
                    this.model.status = this.service.getPageStatusTranslation(LogBookPageStatusesEnum[this.model.status! as keyof typeof LogBookPageStatusesEnum]);

                    this.originPossibleProducts = this.model.originalPossibleProducts ?? [];
                    this.model.originalPossibleProducts = []; // за да не се мапират обратно към бекенда

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
        this.pageNumber = data.pageNumber;
        this.pageStatus = data.pageStatus;
        this.commonLogBookPageData = data.commonData;
        this.logBookPermitLicenseId = data.logBookPermitLicenseId;

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

            if (this.id === null || this.id === undefined) {
                this.addAdmissionLogBookPage(dialogClose);
            }
            else {
                this.service.editAdmissionLogBookPage(this.model).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    },
                    error: (errorResponse: HttpErrorResponse) => {
                        const error: ErrorModel | undefined = errorResponse.error;

                        if (error?.code === ErrorCode.SendFLUXSalesFailed) {
                            if (!IS_PUBLIC_APP) { // show snackbar only when not public app
                                this.snackbar.open(this.translationService.getValue('catches-and-sales.admission-page-send-to-flux-sales-error'), undefined, {
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
            handoverDateControl: new FormControl(undefined, Validators.required),
            storageLocationControl: new FormControl(undefined, [Validators.required, Validators.maxLength(500)]),

            commonLogBookPageDataControl: new FormControl(),
            acceptingPersonControl: new FormControl(undefined, Validators.required),
            productsControl: new FormControl(),

            filesControl: new FormControl()
        });
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
            }
        });
    }
}