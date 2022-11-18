import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CurrencyPipe } from '@angular/common';
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

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private model!: FirstSaleLogBookPageEditDTO;
    private id: number | undefined;
    private pageNumber: number | undefined;
    private pageStatus: LogBookPageStatusesEnum | undefined;
    private currencyPipe: CurrencyPipe;

    private logBookId!: number;
    private commonLogBookPageData: CommonLogBookPageDataDTO | undefined;
    /** Has value when the component is open from a declaration of a ship log book page */
    private shipPageDocumentData: BasicLogBookPageDocumentDataDTO | undefined;
    private translationService: FuseTranslationLoaderService;
    private snackbar: MatSnackBar;

    public constructor(
        translationService: FuseTranslationLoaderService,
        currencyPipe: CurrencyPipe,
        snackbar: MatSnackBar
    ) {
        this.translationService = translationService;
        this.currencyPipe = currencyPipe;
        this.snackbar = snackbar;
    }

    public async ngOnInit(): Promise<void> {
        this.registeredBuyers = await this.service.getRegisteredBuyersNomenclature().toPromise();

        if (this.id !== null && this.id !== undefined) {
            this.service.getFirstSaleLogBookPage(this.id).subscribe({
                next: (result: FirstSaleLogBookPageEditDTO) => {
                    this.model = result;
                    this.model.status = this.service.getPageStatusTranslation(LogBookPageStatusesEnum[this.model.status! as keyof typeof LogBookPageStatusesEnum]);

                    this.originPossibleProducts = this.model.originalPossibleProducts ?? [];
                    this.model.originalPossibleProducts = []; // за да не се мапират обратно към бекенда

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
        // TODO
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
                this.service.addFirstSaleLogBookPage(this.model).subscribe({
                    next: (id: number) => {
                        this.model.id = id;
                        dialogClose(this.model);
                    },
                    error: (response: HttpErrorResponse) => {
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
                        else if (error?.code === ErrorCode.SendFLUXSalesFailed) {
                            if (!IS_PUBLIC_APP) { // show snackbar only when not public app
                                this.snackbar.open(this.translationService.getValue('catches-and-sales.first-sale-page-send-to-flux-sales-error'), undefined, {
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
                this.service.editFirstSaleLogBookPage(this.model).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    },
                    error: (response: HttpErrorResponse) => {
                        const error: ErrorModel | undefined = response.error;

                        if (error?.code === ErrorCode.SendFLUXSalesFailed) {
                            if (!IS_PUBLIC_APP) { // show snackbar only when not public app
                                this.snackbar.open(this.translationService.getValue('catches-and-sales.first-sale-page-send-to-flux-sales-error'), undefined, {
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
            logBookNumberControl: new FormControl(),
            pageNumberControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)]),
            statusControl: new FormControl(),
            saleDateControl: new FormControl(undefined, Validators.required),
            saleContractNumberControl: new FormControl(undefined, Validators.maxLength(100)),
            saleLocationControl: new FormControl(undefined, [Validators.maxLength(500), Validators.required]),
            saleContractDateControl: new FormControl(),

            commonLogBookPageDataControl: new FormControl(),
            buyerControl: new FormControl(null, Validators.required),
            productsControl: new FormControl(),
            productsTotalValueControl: new FormControl(),

            filesControl: new FormControl()
        });
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
}