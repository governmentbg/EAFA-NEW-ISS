import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
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

    public noAvailableProducts: boolean = false;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private model!: TransportationLogBookPageEditDTO;
    private id: number | undefined;
    private commonLogBookPageData: CommonLogBookPageDataDTO | undefined;
    private shipPageDocumentData: BasicLogBookPageDocumentDataDTO | undefined;
    private logBookId!: number;
    /** Needed only for Transportation log book pages, when the log book is for Person/Legal */
    public logBookPermitLicenseId: number | undefined;
    private pageNumber: number | undefined;
    private pageStatus: LogBookPageStatusesEnum | undefined;
    private translationService: FuseTranslationLoaderService;
    private snackbar: MatSnackBar;

    public constructor(translationService: FuseTranslationLoaderService, snackbar: MatSnackBar) {
        this.translationService = translationService;
        this.snackbar = snackbar;
    }

    public ngOnInit(): void {
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

            if (this.id === null || this.id === undefined) {
                this.service.addTransportationLogBookPage(this.model).subscribe({
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
                            this.snackbar.open(
                                `${this.translationService.getValue('catches-and-sales.transportation-page-already-submitted-other-logbook-error')}: ${error.messages[0]}`,
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
                    }
                });
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

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            pageNumberControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)]),
            vehicleIdentificationControl: new FormControl(undefined, [Validators.required, Validators.maxLength(50)]),
            statusControl: new FormControl(),
            loadingLocationControl: new FormControl(undefined, [Validators.required, Validators.maxLength(500)]),
            loadingDateControl: new FormControl(undefined, Validators.required),
            deliveryLocationControl: new FormControl(undefined, Validators.required),

            commonLogBookPageDataControl: new FormControl(),
            receiverPersonControl: new FormControl(undefined, Validators.required),
            productsControl: new FormControl(),

            filesControl: new FormControl()
        });
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
}