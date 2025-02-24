import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { CurrencyPipe } from '@angular/common';
import { forkJoin } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { EditLogBookPageProductDialogParamsModel } from '../../models/edit-log-book-page-product-dialog-params.model';
import { LogBookPageProductDTO } from '@app/models/generated/dtos/LogBookPageProductDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { FishNomenclatureDTO } from '@app/models/generated/dtos/FishNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { FishCodesEnum } from '@app/enums/fish-codes.enum';
import { LogBookPageProductUtils } from '../../utils/log-book-page-product.utils';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';

const DEFAULT_SIZE_CATEGORY_CODE: string = 'N/A'; // TODO this as a parameter or enum ???
const DEFAULT_DUNABE_LOCATION: string = 'р. Дунав';

@Component({
    selector: 'edit-log-book-page-product',
    templateUrl: './edit-log-book-page-product.component.html'
})
export class EditLogBookPageProductComponent implements AfterViewInit, OnInit, IDialogComponent {
    public readonly logBookTypesEnum: typeof LogBookTypesEnum = LogBookTypesEnum;
    public form!: FormGroup;

    public aquaticOrganismTypes: FishNomenclatureDTO[] = [];
    public presentations: NomenclatureDTO<number>[] = [];
    public freshnessCategories: NomenclatureDTO<number>[] = [];
    public purposes: NomenclatureDTO<number>[] = [];
    public turbotSizeGroups: NomenclatureDTO<number>[] = [];
    public sizeCategories: NomenclatureDTO<number>[] = [];
    public quantityCountOptions: NomenclatureDTO<boolean>[] = [];

    public readOnly: boolean = false;
    public hasPrice: boolean = true;
    public hasUnitCount: boolean = false;
    public showUnitCountControl: boolean = false;
    public showTurbotControls: boolean = false;
    public showFishCategoryControl: boolean = false;
    public isAquaculturePage: boolean = false;
    public isContinentalCatch: boolean = false;
    public model!: LogBookPageProductDTO;
    public logBookType!: LogBookTypesEnum;

    private readonly UNIT_COUNT_PURPOSE_CODES: string[] = ['6'];

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private service!: ICatchesAndSalesService;
    private nomenclatures: CommonNomenclatures;
    private currencyPipe: CurrencyPipe;

    public constructor(translate: FuseTranslationLoaderService, nomenclatures: CommonNomenclatures, currencyPipe: CurrencyPipe) {
        this.nomenclatures = nomenclatures;
        this.currencyPipe = currencyPipe;

        this.quantityCountOptions = [
            new NomenclatureDTO<boolean>({
                value: false,
                displayName: translate.getValue('catches-and-sales.page-product-quantity-kg'),
                isActive: true
            }),
            new NomenclatureDTO<boolean>({
                value: true,
                displayName: translate.getValue('catches-and-sales.page-product-count'),
                isActive: true
            })
        ];
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures: (NomenclatureDTO<number>[] | FishNomenclatureDTO[])[] = await forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Fishes, this.nomenclatures.getFishTypes.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.CatchPresentations, this.nomenclatures.getCatchPresentations.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.CatchStates, this.service.getCatchStates.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.FishPurposes, this.service.getFishPurposes.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TurbotSizeGroups, this.service.getTurbotSizeGroups.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.FishSizeCategories, this.service.getFishSizeCategories.bind(this.service), false)
        ).toPromise();

        this.aquaticOrganismTypes = nomenclatures[0];
        this.presentations = nomenclatures[1];
        this.freshnessCategories = nomenclatures[2];
        this.purposes = nomenclatures[3];
        this.turbotSizeGroups = nomenclatures[4];
        this.sizeCategories = nomenclatures[5];

        if (this.logBookType !== LogBookTypesEnum.Aquaculture) {
            this.form.get('catchLocationControl')!.setValidators([Validators.required, Validators.maxLength(500)]);
            this.form.get('catchLocationControl')!.markAsPending({ emitEvent: false });
            this.form.get('catchLocationControl')!.updateValueAndValidity({ emitEvent: false });

            if (this.isContinentalCatch) {
                this.form.get('catchLocationControl')!.setValue(DEFAULT_DUNABE_LOCATION);
            }

            this.form.get('freshnessControl')!.setValidators(Validators.required);
            this.form.get('freshnessControl')!.markAsPending({ emitEvent: false });
            this.form.get('freshnessControl')!.updateValueAndValidity({ emitEvent: false });

            this.form.get('sizeCategoryControl')!.setValidators(Validators.required);
            this.form.get('sizeCategoryControl')!.markAsPending({ emitEvent: false });
            this.form.get('sizeCategoryControl')!.updateValueAndValidity({ emitEvent: false });

            const defaultSizeCategory: NomenclatureDTO<number> | undefined = this.sizeCategories.find(x => x.code === DEFAULT_SIZE_CATEGORY_CODE);
            this.form.get('sizeCategoryControl')!.setValue(defaultSizeCategory);

            this.form.get('minimumSizeControl')!.setValidators([Validators.required, TLValidators.number(0)]);
            this.form.get('minimumSizeControl')!.markAsPending({ emitEvent: false });
            this.form.get('minimumSizeControl')!.updateValueAndValidity({ emitEvent: false });

            this.isAquaculturePage = false;
        }
        else {
            this.form.get('averageUnitWeightKgControl')!.setValidators([Validators.required, TLValidators.number(0)]);
            this.form.get('averageUnitWeightKgControl')!.markAsPending({ emitEvent: false });
            this.form.get('averageUnitWeightKgControl')!.updateValueAndValidity({ emitEvent: false });

            this.isAquaculturePage = true;
        }

        if (this.readOnly) {
            this.form.disable();
        }

        this.fillForm();
    }

    public ngAfterViewInit(): void {
        this.form.get('quantityKgControl')!.valueChanges.subscribe({
            next: (quantityKg: number | undefined) => {
                if (this.hasPrice) {
                    const unitPrice: number | undefined = Number(this.form.get('unitPriceControl')!.value);
                    const unitCount: number | undefined = Number(this.form.get('unitCountControl')!.value);

                    const formattedTotalPrice: string | null = this.hasUnitCount && this.showUnitCountControl && !this.showTurbotControls
                        ? LogBookPageProductUtils.formatTotalProductPrice(this.currencyPipe, Number(unitCount), unitPrice)
                        : LogBookPageProductUtils.formatTotalProductPrice(this.currencyPipe, Number(quantityKg), unitPrice);

                    this.form.get('totalPriceControl')!.setValue(formattedTotalPrice);
                }

                if (this.hasUnitCount) {
                    const unitCount: number | undefined = Number(this.form.get('unitCountControl')!.value);

                    if (!this.isAquaculturePage) {
                        if (quantityKg && unitCount && unitCount > 0) {
                            const averageUnitWeightKg: string | undefined = (quantityKg / unitCount).toFixed(3);
                            this.form.get('averageUnitWeightKgControl')!.setValue(averageUnitWeightKg);
                        }
                        else {
                            this.form.get('averageUnitWeightKgControl')!.setValue(undefined);
                        }
                    }
                }
            }
        });

        this.form.get('unitCountControl')!.valueChanges.subscribe({
            next: (unitCount: number | undefined) => {
                const quantityKg: number | undefined = Number(this.form.get('quantityKgControl')!.value);

                if (!this.isAquaculturePage) {
                    if (quantityKg && unitCount && unitCount > 0) {
                        const averageUnitWeightKg: string | undefined = (Number(quantityKg) / Number(unitCount)).toFixed(3);
                        this.form.get('averageUnitWeightKgControl')!.setValue(averageUnitWeightKg);
                    }
                    else {
                        this.form.get('averageUnitWeightKgControl')!.setValue(undefined);
                    }
                }

                if (this.hasPrice) {
                    const unitPrice: number | undefined = Number(this.form.get('unitPriceControl')!.value);

                    const formattedTotalPrice: string | null = this.showTurbotControls || !this.showUnitCountControl
                        ? LogBookPageProductUtils.formatTotalProductPrice(this.currencyPipe, Number(this.form.get('quantityKgControl')!.value), unitPrice)
                        : LogBookPageProductUtils.formatTotalProductPrice(this.currencyPipe, Number(unitCount), unitPrice);
                    this.form.get('totalPriceControl')!.setValue(formattedTotalPrice);
                }
            }
        });

        if (this.hasPrice) {
            this.form.get('unitPriceControl')!.valueChanges.subscribe({
                next: (unitPrice: number | undefined) => {
                    const quantityKg: number | undefined = Number(this.form.get('quantityKgControl')!.value);
                    const unitCount: number | undefined = Number(this.form.get('unitCountControl')!.value);

                    const formattedTotalPrice: string | null = this.hasUnitCount && this.showUnitCountControl && !this.showTurbotControls
                        ? LogBookPageProductUtils.formatTotalProductPrice(this.currencyPipe, Number(unitCount), unitPrice)
                        : LogBookPageProductUtils.formatTotalProductPrice(this.currencyPipe, Number(quantityKg), unitPrice);

                    this.form.get('totalPriceControl')!.setValue(formattedTotalPrice);
                }
            });
        }

        this.form.get('purposeControl')!.valueChanges.subscribe({
            next: (purpose: NomenclatureDTO<number> | undefined) => {
                this.form.get('quantityCountControl')!.clearValidators();

                if (this.logBookType === LogBookTypesEnum.Aquaculture) {
                    if (purpose && typeof purpose !== 'string') {
                        if (this.UNIT_COUNT_PURPOSE_CODES.includes(purpose.code!)) {
                            this.hasUnitCount = true;
                            this.form.get('quantityCountControl')!.setValidators(Validators.required);
                        }
                        else {
                            this.hasUnitCount = this.hasOrganismUnitCount();
                            this.form.get('unitCountControl')!.setValue(undefined);
                        }
                    }
                    else {
                        this.hasUnitCount = this.hasOrganismUnitCount();
                        this.form.get('unitCountControl')!.setValue(undefined);
                    }

                    if (!this.readOnly) {
                        this.updateQuantityCountValidators();
                    }
                }

                this.form.get('quantityCountControl')!.updateValueAndValidity({ emitEvent: false });
            }
        });

        this.form.get('aquaticOrganismTypeControl')!.valueChanges.subscribe({
            next: (aquaticOrganism: FishNomenclatureDTO | undefined) => {
                this.setTurbotFlagAndValidators(aquaticOrganism);
                this.setMinCatchSize(aquaticOrganism);

                if (!this.readOnly) {
                    this.updateQuantityCountValidators();
                }
            }
        });

        this.form.get('quantityCountControl')!.valueChanges.subscribe({
            next: (value: NomenclatureDTO<boolean> | undefined) => {
                if (value !== undefined && value !== null) {
                    this.showUnitCountControl = value.value ?? false;

                    if (this.showUnitCountControl) {
                        this.form.get('quantityKgControl')!.setValue(undefined);
                    }
                    else {
                        this.form.get('unitCountControl')!.setValue(undefined);
                    }

                    if (!this.readOnly) {
                        this.updateQuantityCountValidators();
                    }
                }
            }
        });
    }

    public setData(data: EditLogBookPageProductDialogParamsModel, buttons: DialogWrapperData): void {
        this.readOnly = data.viewMode;
        this.service = data.service;
        this.logBookType = data.logBookType;
        this.hasPrice = data.hasPrice;

        this.buildForm();

        if (data.model === null || data.model === undefined) {
            this.model = new LogBookPageProductDTO({ isActive: true, logBookType: this.logBookType });
        }
        else {
            if (this.readOnly) {
                this.form.disable();
            }

            this.model = data.model;
            this.model.logBookType = this.logBookType;

            if (CommonUtils.isNullOrEmpty(this.model.catchLocation) || this.model.catchLocation?.includes(DEFAULT_DUNABE_LOCATION)) {
                this.isContinentalCatch = true;
            }
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.readOnly === true) {
            dialogClose(this.model);
        }
        else {
            this.form.markAllAsTouched();
            this.validityCheckerGroup.validate();

            if (this.form.valid) {
                this.fillModel();
                CommonUtils.sanitizeModelStrings(this.model);
                dialogClose(this.model);
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
            aquaticOrganismTypeControl: new FormControl(undefined, Validators.required),
            catchLocationControl: new FormControl(undefined, [Validators.maxLength(500)]),

            minimumSizeControl: new FormControl(undefined, [TLValidators.number(0)]),
            averageUnitWeightKgControl: new FormControl(undefined),

            presentationControl: new FormControl(undefined, Validators.required),
            sizeCategoryControl: new FormControl(),

            freshnessControl: new FormControl(undefined),
            purposeControl: new FormControl(undefined, Validators.required),

            quantityKgControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)]),

            turbotSizeGroupControl: new FormControl(),
            unitCountControl: new FormControl(),

            quantityCountControl: new FormControl()
        });

        if (this.hasPrice) {
            this.form.addControl('unitPriceControl', new FormControl(undefined, [Validators.required, TLValidators.number(0)]));
            this.form.addControl('totalPriceControl', new FormControl());
        }
    }

    private fillForm(): void {
        if (this.model.fishId !== null && this.model.fishId !== undefined) {
            const aquaticOrganism: FishNomenclatureDTO = this.aquaticOrganismTypes.find(x => x.value === this.model.fishId)!;
            this.form.get('aquaticOrganismTypeControl')!.setValue(aquaticOrganism);
            this.setTurbotFlagAndValidators(aquaticOrganism);
        }
        else {
            this.setTurbotFlagAndValidators(undefined);
        }

        if (this.logBookType !== LogBookTypesEnum.Aquaculture) {
            if (!this.isContinentalCatch) {
                this.form.get('catchLocationControl')!.setValue(this.model.catchLocation);
            }

            if (this.model.productFreshnessId !== null && this.model.productFreshnessId !== undefined) {
                const freshness: NomenclatureDTO<number> = this.freshnessCategories.find(x => x.value === this.model.productFreshnessId)!;
                this.form.get('freshnessControl')!.setValue(freshness);
            }
        }

        if (this.model.productPresentationId !== null && this.model.productPresentationId !== undefined) {
            const presentation: NomenclatureDTO<number> = this.presentations.find(x => x.value === this.model.productPresentationId)!;
            this.form.get('presentationControl')!.setValue(presentation);
        }

        if (this.model.fishSizeCategoryId !== null && this.model.fishSizeCategoryId !== undefined) {
            const sizeCategory: NomenclatureDTO<number> = this.sizeCategories.find(x => x.value === this.model.fishSizeCategoryId)!;
            this.form.get('sizeCategoryControl')!.setValue(sizeCategory);
        }
        else {
            const defaultSizeCategory: NomenclatureDTO<number> | undefined = this.sizeCategories.find(x => x.code === DEFAULT_SIZE_CATEGORY_CODE);
            this.form.get('sizeCategoryControl')!.setValue(defaultSizeCategory);
        }

        this.form.get('minimumSizeControl')!.setValue(this.model.minSize);

        this.form.get('averageUnitWeightKgControl')!.setValue(this.model.averageUnitWeightKg);

        if (this.model.productPurposeId !== null && this.model.productPurposeId !== undefined) {
            const purpose: NomenclatureDTO<number> = this.purposes.find(x => x.value === this.model.productPurposeId)!;
            this.form.get('purposeControl')!.setValue(purpose);
        }

        if (this.model.quantityKg !== undefined && this.model.quantityKg !== null && this.model.quantityKg > 0) {
            this.form.get('quantityKgControl')!.setValue(this.model.quantityKg);
        }

        if (this.hasPrice) {
            this.form.get('unitPriceControl')!.setValue(this.model.unitPrice);
            const formattedTotalPrice: string | null = this.hasUnitCount
                ? LogBookPageProductUtils.formatTotalProductPrice(this.currencyPipe, this.model.unitCount, this.model.unitPrice)
                : LogBookPageProductUtils.formatTotalProductPrice(this.currencyPipe, this.model.quantityKg, this.model.unitPrice);

            this.form.get('totalPriceControl')!.setValue(formattedTotalPrice);
        }

        if (this.hasUnitCount) {
            if (!this.showTurbotControls) {
                if (this.model.unitCount !== undefined && this.model.unitCount !== null) {
                    this.form.get('quantityCountControl')!.setValue(this.quantityCountOptions.find(x => x.value === true));
                }
                else {
                    this.form.get('quantityCountControl')!.setValue(this.quantityCountOptions.find(x => x.value === false));
                }
            }

            if (this.showTurbotControls || this.showUnitCountControl) {
                this.form.get('unitCountControl')!.setValue(this.model.unitCount);
            }
        }

        if (this.model.turbotSizeGroupId !== null && this.model.turbotSizeGroupId !== undefined) {
            const turbotSizeGroup: NomenclatureDTO<number> = this.turbotSizeGroups.find(x => x.value === this.model!.turbotSizeGroupId)!;
            this.form.get('turbotSizeGroupControl')!.setValue(turbotSizeGroup);
        }
    }

    private fillModel(): void {
        this.model.fishId = this.form.get('aquaticOrganismTypeControl')!.value.value;
        this.model.productPresentationId = this.form.get('presentationControl')!.value.value;
        this.model.productPurposeId = this.form.get('purposeControl')!.value.value;

        if (this.hasPrice) {
            this.model.unitPrice = this.form.get('unitPriceControl')!.value;
            this.model.totalPrice = this.form.get('totalPriceControl')!.value;
        }

        if (this.logBookType !== LogBookTypesEnum.Aquaculture) {
            this.model.catchLocation = this.form.get('catchLocationControl')!.value;
            this.model.productFreshnessId = this.form.get('freshnessControl')!.value.value;
            this.model.fishSizeCategoryId = this.form.get('sizeCategoryControl')!.value.value;
            this.model.minSize = this.form.get('minimumSizeControl')!.value;
        }

        this.model.averageUnitWeightKg = this.form.get('averageUnitWeightKgControl')!.value;

        this.model.quantityKg = this.form.get('quantityKgControl')!.value;
        this.model.unitCount = this.showUnitCountControl || this.showTurbotControls ? this.form.get('unitCountControl')!.value : undefined;
        this.model.turbotSizeGroupId = this.form.get('turbotSizeGroupControl')!.value?.value;

    }

    private setMinCatchSize(value: FishNomenclatureDTO | undefined): void {
        if (this.logBookType !== LogBookTypesEnum.Aquaculture) {
            this.form.get('minimumSizeControl')!.setValue(value?.minCatchSize);
        }
    }

    private setTurbotFlagAndValidators(value: FishNomenclatureDTO | undefined): void {
        if (value !== null && value !== undefined) {
            if (FishCodesEnum[value.code as keyof typeof FishCodesEnum] === FishCodesEnum.TUR) {
                this.showTurbotControls = true;
                this.hasUnitCount = true;
                this.showFishCategoryControl = false;
            }
            else {
                this.showTurbotControls = false;
                this.hasUnitCount = this.hasPurposeUnitCount();

                if (this.logBookType !== LogBookTypesEnum.Aquaculture) {
                    this.showFishCategoryControl = true;
                }
                else {
                    this.showFishCategoryControl = false;
                }
            }
        }
        else {
            this.showTurbotControls = false;
            this.hasUnitCount = this.hasPurposeUnitCount();
            this.showFishCategoryControl = false;
        }

        if (!this.readOnly) {
            this.setTurbotControlsValidators();
        }
    }

    private setTurbotControlsValidators(): void {
        if (this.showTurbotControls === true) {
            this.form.get('turbotSizeGroupControl')!.setValidators(Validators.required);
            this.form.get('turbotSizeGroupControl')!.markAsPending();
        }
        else {
            this.form.get('turbotSizeGroupControl')!.setValidators(null);
        }
    }

    private updateQuantityCountValidators(): void {
        const quantityKgValidators: ValidatorFn[] = [];
        const unitCountValidators: ValidatorFn[] = [];

        if (!this.showUnitCountControl || this.showTurbotControls) {
            quantityKgValidators.push(Validators.required, TLValidators.number(0));
        }

        if (this.showUnitCountControl || this.showTurbotControls) {
            unitCountValidators.push(Validators.required, TLValidators.number(0));
        }

        this.form.get('quantityKgControl')!.setValidators(quantityKgValidators);
        this.form.get('unitCountControl')!.setValidators(unitCountValidators);

        this.form.get('quantityKgControl')!.markAsPending();
        this.form.get('unitCountControl')!.markAsPending();

        this.form.get('quantityKgControl')!.updateValueAndValidity();
        this.form.get('unitCountControl')!.updateValueAndValidity();
    }

    private hasPurposeUnitCount(): boolean {
        const purpose: NomenclatureDTO<number> | string | undefined = this.form.get('purposeControl')!.value;

        if (purpose && typeof purpose !== 'string') {
            return this.UNIT_COUNT_PURPOSE_CODES.includes(purpose.code!);
        }

        return false;
    }

    private hasOrganismUnitCount(): boolean {
        const organism: FishNomenclatureDTO | string | undefined = this.form.get('aquaticOrganismTypeControl')!.value;

        if (organism && typeof organism !== 'string') {
            if (FishCodesEnum[organism.code as keyof typeof FishCodesEnum] === FishCodesEnum.TUR) {
                return true;
            }
        }

        return false;
    }
}