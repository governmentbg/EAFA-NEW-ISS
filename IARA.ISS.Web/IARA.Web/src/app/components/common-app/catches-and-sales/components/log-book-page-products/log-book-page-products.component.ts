﻿import { CurrencyPipe } from '@angular/common';
import { AfterViewInit, Component, Input, OnInit, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, NgControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { forkJoin, Observable, Subscription } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { EditLogBookPageProductComponent } from './components/edit-log-book-page-product/edit-log-book-page-product.component';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { LogBookPageProductDTO } from '@app/models/generated/dtos/LogBookPageProductDTO';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { EditLogBookPageProductDialogParamsModel } from './models/edit-log-book-page-product-dialog-params.model';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { LogBookPageProductUtils } from './utils/log-book-page-product.utils';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { FishNomenclatureDTO } from '@app/models/generated/dtos/FishNomenclatureDTO';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FishGroupedQuantitiesModel } from '../../models/fish-grouped-quantities.model';

@Component({
    selector: 'log-book-page-products',
    templateUrl: './log-book-page-products.component.html'
})
export class LogBookPageProductsComponent extends CustomFormControl<LogBookPageProductDTO[]> implements OnInit, AfterViewInit {
    @Input()
    public isReadonly: boolean = false;

    @Input()
    public service!: ICatchesAndSalesService;

    @Input()
    public logBookType!: LogBookTypesEnum;

    @Input()
    public showAddButton: boolean = false;

    @Input()
    public originProducts: LogBookPageProductDTO[] = [];

    @Input()
    public softDeleteRecords: boolean = true;

    public readonly logBookTypesEnum: typeof LogBookTypesEnum = LogBookTypesEnum;
    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    public hasPrice: boolean = false;
    public isTouched: boolean = false;

    public fishTypes: FishNomenclatureDTO[] = [];
    public productPresentations: NomenclatureDTO<number>[] = [];
    public productFreshness: NomenclatureDTO<number>[] = [];
    public productPurposes: NomenclatureDTO<number>[] = [];
    public turbotSizeGroups: NomenclatureDTO<number>[] = [];

    public products: LogBookPageProductDTO[] = [];
    public fishQuantities: Map<string, string> = new Map<string, string>();
    public fishQuantityText: string | undefined;

    @ViewChild('productsTable')
    private productsTable!: TLDataTableComponent;

    private translate: FuseTranslationLoaderService;
    private nomenclatures: CommonNomenclatures;
    private editLogBookPageProductDialog: TLMatDialog<EditLogBookPageProductComponent>;
    private confirmDialog: TLConfirmDialog;
    private currencyPipe: CurrencyPipe;

    private readonly loader: FormControlDataLoader;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        editLogBookPageProductDialog: TLMatDialog<EditLogBookPageProductComponent>,
        confirmDialog: TLConfirmDialog,
        currencyPipe: CurrencyPipe
    ) {
        super(ngControl);

        this.translate = translate;
        this.nomenclatures = nomenclatures;
        this.editLogBookPageProductDialog = editLogBookPageProductDialog;
        this.confirmDialog = confirmDialog;
        this.currencyPipe = currencyPipe;

        this.loader = new FormControlDataLoader(this.loadNomenclatures.bind(this));

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.isTouched = true;
                this.control.updateValueAndValidity({ onlySelf: true });
            }
        });
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
        this.loader.load();

        this.hasPrice = this.logBookType === LogBookTypesEnum.Aquaculture || this.logBookType === LogBookTypesEnum.FirstSale;
    }

    public ngAfterViewInit(): void {
        this.productsTable.recordChanged.subscribe({
            next: (event: RecordChangedEventArgs<LogBookPageProductDTO>) => {
                this.isTouched = true;
            }
        });
    }

    public writeValue(value: LogBookPageProductDTO[]): void {
        if (value !== null && value !== undefined) {
            this.loader.load(() => {
                if (this.hasPrice) {
                    for (const product of value) {
                        product.totalPrice = LogBookPageProductUtils.formatTotalProductPrice(
                            this.currencyPipe,
                            product.unitCount ?? product.quantityKg,
                            product.unitPrice
                        ) ?? undefined;
                    }
                }

                setTimeout(() => {
                    this.products = value.slice();
                    this.recalculateFishQuantitySums();
                });
            });
        }
        else {
            setTimeout(() => {
                this.products = [];
                this.recalculateFishQuantitySums();
            });
        }
    }

    public generateProductsFromOriginCatchRecords(): void {
        this.products = this.products.filter(x => x.id !== null && x.id !== undefined);

        for (const product of this.products) {
            product.isActive = false;
        }

        for (const product of this.originProducts) {
            const newProduct: LogBookPageProductDTO = new LogBookPageProductDTO();
            Object.assign(newProduct, product);
            newProduct.hasMissingProperties = true;
            this.products.push(newProduct);
        }
        this.onChanged(this.products);
        this.recalculateFishQuantitySums();

        this.isTouched = true;
        this.control.updateValueAndValidity({ emitEvent: false, onlySelf: true });
    }

    public addEditProduct(product?: LogBookPageProductDTO, viewMode: boolean = false): void {
        let data: EditLogBookPageProductDialogParamsModel | undefined;
        let headerAuditBtn: IHeaderAuditButton | undefined;
        let title: string = '';

        if (product !== null && product !== undefined) {
            data = new EditLogBookPageProductDialogParamsModel({
                model: product,
                viewMode: this.isReadonly || viewMode,
                service: this.service,
                logBookType: this.logBookType,
                hasPrice: this.hasPrice
            });

            if (product.id !== null && product.id !== undefined) {
                headerAuditBtn = {
                    id: product.id,
                    getAuditRecordData: this.service.getLogBookPageProductAudit.bind(this.service),
                    tableName: 'LogBookPageProduct'
                };
            }

            if (this.isReadonly || viewMode) {
                title = this.translate.getValue('catches-and-sales.page-product-view-product-dialog-title');
            }
            else {
                title = this.translate.getValue('catches-and-sales.page-product-edit-product-dialog-title');
            }
        }
        else if (!this.isReadonly) {
            data = new EditLogBookPageProductDialogParamsModel({
                service: this.service,
                logBookType: this.logBookType,
                viewMode: false,
                hasPrice: this.hasPrice
            });

            title = this.translate.getValue('catches-and-sales.page-product-add-product-dialog-title');
        }

        this.openLogBookPageProductDialog(product, title, headerAuditBtn, data, viewMode || this.isReadonly)
            .subscribe({
                next: (result: LogBookPageProductDTO | null | undefined) => {
                    if (result !== null && result !== undefined) {
                        result.hasMissingProperties = false;

                        if (product !== null && product !== undefined) {
                            product = result;
                        }
                        else {
                            this.products.push(result);
                        }

                        this.products = this.products.slice();
                        this.onChanged(this.products);
                        this.recalculateFishQuantitySums();

                        this.isTouched = true;
                        this.control.updateValueAndValidity({ emitEvent: false, onlySelf: true });
                    }
                }
            });
    }

    public copyProduct(product: LogBookPageProductDTO): void {
        const title: string = this.translate.getValue('catches-and-sales.page-product-add-product-dialog-title');
        const productCopy: LogBookPageProductDTO = new LogBookPageProductDTO({
            catchLocation: product.catchLocation,
            fishId: product.fishId,
            fishSizeCategoryId: product.fishSizeCategoryId,
            minSize: product.minSize,
            originDeclarationFishId: product.originDeclarationFishId,
            originProductId: product.originProductId,
            productFreshnessId: product.productFreshnessId,
            productPresentationId: product.productPresentationId,
            productPurposeId: product.productPurposeId,
            turbotSizeGroupId: product.turbotSizeGroupId,
            hasMissingProperties: true,
            isActive: true
        });

        const data: EditLogBookPageProductDialogParamsModel = new EditLogBookPageProductDialogParamsModel({
            model: productCopy,
            viewMode: this.isReadonly,
            service: this.service,
            logBookType: this.logBookType,
            hasPrice: this.hasPrice
        });

        this.openLogBookPageProductDialog(productCopy, title, undefined, data, false)
            .subscribe({
                next: (result: LogBookPageProductDTO | undefined) => {
                    if (result !== null && result !== undefined) {
                        result.hasMissingProperties = false;
                        this.products.push(result);

                        this.products = this.products.slice();
                        this.onChanged(this.products);
                        this.recalculateFishQuantitySums();

                        this.isTouched = true;
                        this.control.updateValueAndValidity({ emitEvent: false, onlySelf: true });
                    }
                }
            });
    }

    public deleteProduct(row: GridRow<LogBookPageProductDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('catches-and-sales.page-product-delete-dialog-label'),
            message: this.translate.getValue('catches-and-sales.page-product-confirm-delete-message'),
            okBtnLabel: this.translate.getValue('catches-and-sales.page-product-delete-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.productsTable.softDelete(row);
                    this.handleChangedProductsTable();
                }
            }
        });
    }

    public undoDeleteLogBook(row: GridRow<LogBookPageProductDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.productsTable.softUndoDelete(row);
                    this.handleChangedProductsTable();
                }
            }
        });
    }

    public removeProduct(row: GridRow<LogBookPageProductDTO>): void {
        const indexToDelete: number = this.products.findIndex(x => x === row.data);
        this.products.splice(indexToDelete, 1);
        this.products = this.products.slice();
        this.recalculateFishQuantitySums();

        this.isTouched = true;
        this.control.updateValueAndValidity({ emitEvent: false, onlySelf: true });
    }

    public validate(control: AbstractControl): ValidationErrors | null {
        const errors: ValidationErrors = {};

        //productsQuantityNotMatch is warning
        if (this.control.errors !== null && this.control.errors !== undefined) {
            if (this.control.errors['missingProperties']) {
                errors['missingProperties'] = this.control.errors['missingProperties'];
            }

            if (this.control.errors['noProducts']) {
                errors['noProducts'] = this.control.errors['noProducts'];
            }
        }

        return Object.keys(errors).length === 0 ? null : errors;
    }

    protected buildForm(): AbstractControl {
        return new FormControl(null, [
            this.missingPropertiesValidator(),
            this.atLeastOneProductValidator(),
            this.productsQuantitiesNotGreaterThanOriginal()
        ]);
    }

    protected getValue(): LogBookPageProductDTO[] {
        return this.products;
    }

    private openLogBookPageProductDialog(
        product: LogBookPageProductDTO | undefined,
        title: string,
        headerAuditBtn: IHeaderAuditButton | undefined,
        data: EditLogBookPageProductDialogParamsModel | undefined,
        viewMode: boolean
    ): Observable<any> {
        return this.editLogBookPageProductDialog.openWithTwoButtons({
            title: title,
            TCtor: EditLogBookPageProductComponent,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditLogBookPageProductDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: true,
            viewMode: this.isReadonly || viewMode
        }, '1300px');
    }

    private handleChangedProductsTable(): void {
        this.products = this.productsTable.rows;
        this.onChanged(this.products);
        this.recalculateFishQuantitySums();

        this.isTouched = true;
        this.control.updateValueAndValidity({ emitEvent: false, onlySelf: true });
    }

    private loadNomenclatures(): Subscription {
        return forkJoin(
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Fishes, this.nomenclatures.getFishTypes.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.CatchPresentations, this.nomenclatures.getCatchPresentations.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.CatchStates, this.service.getCatchStates.bind(this.service), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.FishPurposes, this.service.getFishPurposes.bind(this.service), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.TurbotSizeGroups, this.service.getTurbotSizeGroups.bind(this.service), false
            )
        ).subscribe({
            next: (nomenclatures: NomenclatureDTO<number>[][]) => {
                this.fishTypes = nomenclatures[0];
                this.productPresentations = nomenclatures[1];
                this.productFreshness = nomenclatures[2];
                this.productPurposes = nomenclatures[3];
                this.turbotSizeGroups = nomenclatures[4];

                this.loader.complete();
            }
        });
    }

    private missingPropertiesValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.products !== undefined && this.products !== null) {

                const zeroQuantityProducts: LogBookPageProductDTO[] = this.products.filter(x => x.isActive && x.quantityKg === 0);
                if (zeroQuantityProducts.length > 0) {
                    return { 'missingProperties': true };
                }


                for (const product of this.products) {
                    if (product.hasMissingProperties) {
                        return { 'missingProperties': true };
                    }
                }
            }

            return null;
        };
    }

    private atLeastOneProductValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === null || control === undefined) {
                return null;
            }

            if (this.products === null || this.products === undefined || this.products.filter(x => x.isActive).length === 0) {
                return { 'noProducts': true };
            }

            return null;
        }
    }

    private productsQuantitiesNotGreaterThanOriginal(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === null || control === undefined) {
                return null;
            }

            if (this.products === null || this.products === undefined || this.products.filter(x => x.isActive).length === 0) {
                return null;
            }

            if (this.originProducts === null || this.originProducts === undefined || this.originProducts.length === 0) {
                return null;
            }

            const originalProductsGrouped: FishGroupedQuantitiesModel[] = this.getProductQuantitiesGrouped(this.originProducts);
            const productsGrouped: FishGroupedQuantitiesModel[] = this.getProductQuantitiesGrouped(this.products.filter(x => x.isActive));

            for (const product of productsGrouped) {
                const fishId: number = Number(product.fishId);
                const productQuantity: number = Number(product.quantity);

                const originalProduct: FishGroupedQuantitiesModel | undefined = originalProductsGrouped.find(x => x.fishId === fishId
                    && (((x.turbotSizeGroupId === undefined || x.turbotSizeGroupId === null) && (product.turbotSizeGroupId === undefined || product.turbotSizeGroupId === null)) || Number(x.turbotSizeGroupId) === Number(product.turbotSizeGroupId)));

                if (originalProduct !== null && originalProduct !== undefined) {
                    const originalProductQuantity: number = Number(originalProduct.quantity);

                    if (productQuantity > originalProductQuantity) {
                        return { 'productsQuantityNotMatch': true };
                    }
                }
            }

            return null;
        }
    }

    private recalculateFishQuantitySums(): void {
        const count: string = this.translate.getValue('catches-and-sales.ship-page-declaration-count');
        const quantityKg: string = this.translate.getValue('catches-and-sales.ship-page-declaration-kg');
        const continentalCatch: string = this.translate.getValue('catches-and-sales.ship-page-catch-record-is-continental-catch');

        const productsGroupedByCatchZone: Record<number, LogBookPageProductDTO[]> = CommonUtils.groupBy(this.products.filter(x => x.isActive), x => (x.catchLocation!));
        this.fishQuantities.clear();

        for (const catchZone in productsGroupedByCatchZone) {
            const productsGrouped: FishGroupedQuantitiesModel[] = this.getProductQuantitiesGrouped(productsGroupedByCatchZone[catchZone]).filter(x => x.quantity! > 0);

            if (productsGrouped.length > 0) {
                const catchLocation: string = CommonUtils.isNullOrEmpty(catchZone) ? continentalCatch : catchZone;

                const fishQuantityText: string = productsGrouped.map(x =>
                    `${x.fishName}
                     ${x.turbotSizeGroupName !== undefined && x.turbotSizeGroupName !== null ? ` - ${x.turbotSizeGroupName}` : ''}:
                     ${x.turbotCount !== undefined && x.turbotCount !== null ? ` ${x.turbotCount} ${count} - ` : ''}
                     ${x.quantity?.toFixed(2)} ${quantityKg}`
                ).join('; ');

                this.fishQuantities.set(catchLocation, fishQuantityText);
            }
        }
    }

    private getProductQuantitiesGrouped(fishes: LogBookPageProductDTO[]): FishGroupedQuantitiesModel[] {
        const result: FishGroupedQuantitiesModel[] = [];

        for (const fish of fishes) {
            const index: number = result.findIndex(x => x.fishId === fish.fishId
                && (((x.turbotSizeGroupId === undefined || x.turbotSizeGroupId === null) && (fish.turbotSizeGroupId === undefined || fish.turbotSizeGroupId === null)) || Number(x.turbotSizeGroupId) === Number(fish.turbotSizeGroupId)));

            if (index !== -1) {
                result[index].quantity! += (fish.quantityKg ?? 0);

                if (fish.unitCount !== undefined && fish.unitCount !== null) {
                    result[index].turbotCount! += (fish.unitCount ?? 0);
                }
            }
            else {
                const product: FishGroupedQuantitiesModel = new FishGroupedQuantitiesModel({
                    fishId: fish.fishId,
                    turbotSizeGroupId: fish.turbotSizeGroupId,
                    turbotCount: fish.unitCount,
                    quantity: (fish.quantityKg ?? 0),
                    fishName: this.fishTypes.find(x => x.value === fish.fishId)!.displayName,
                    turbotSizeGroupName: this.turbotSizeGroups.find(x => x.value === fish.turbotSizeGroupId)?.displayName
                });

                result.push(product);
            }
        }

        return result.filter(x => x.quantity !== undefined && x.quantity !== null);
    }

    private closeEditLogBookPageProductDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}