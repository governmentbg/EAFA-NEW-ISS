import { Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { MarketCatchTableParams } from '../market-catches-table/models/market-catch-table-params';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { InspectionsService } from '@app/services/administration-app/inspections.service';
import { DeclarationLogBookTypeEnum } from '@app/enums/declaration-log-book-type.enum';
import { InspectedDeclarationCatchDTO } from '@app/models/generated/dtos/InspectedDeclarationCatchDTO';
import { FillDef, MapOptions, SimplePolygonStyleDef, StrokeDef, TLMapViewerComponent } from '@tl/tl-angular-map';
import { TLPopoverComponent } from '@app/shared/components/tl-popover/tl-popover.component';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { forkJoin } from 'rxjs';
import { DeclarationLogBookPageFishDTO } from '@app/models/generated/dtos/DeclarationLogBookPageFishDTO';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { CatchSizeCodesEnum } from '@app/enums/catch-size-codes.enum';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { InspectionLogBookPageNomenclatureDTO } from '@app/models/generated/dtos/InspectionLogBookPageNomenclatureDTO';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { VesselDuringInspectionDTO } from '@app/models/generated/dtos/VesselDuringInspectionDTO';
import { FishCodesEnum } from '@app/enums/fish-codes.enum';

@Component({
    selector: 'edit-market-catch',
    styleUrls: ['./edit-market-catch.component.scss'],
    templateUrl: './edit-market-catch.component.html',
})
export class EditMarketCatchComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;

    protected model: InspectedDeclarationCatchDTO = new InspectedDeclarationCatchDTO();

    public mapOptions: MapOptions;

    public hasCatchType: boolean = true;
    public hasUndersizedCheck: boolean = false;
    public hasDeclaration: boolean = false;
    public readOnly: boolean = false;
    public aquacultureRegistered: boolean = true;
    public showTurbotControl: boolean = false;
    public permitTypeSelected: DeclarationLogBookTypeEnum | undefined;
    public pageDateLabel: string | undefined;

    public readonly declarationLogBookTypeEnum = DeclarationLogBookTypeEnum;

    public ships: ShipNomenclatureDTO[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public vesselTypes: NomenclatureDTO<number>[] = [];
    public fishes: NomenclatureDTO<number>[] = [];
    public types: NomenclatureDTO<number>[] = [];
    public presentations: NomenclatureDTO<number>[] = [];
    public permitTypes: NomenclatureDTO<DeclarationLogBookTypeEnum>[] = [];
    public aquacultures: NomenclatureDTO<number>[] = [];
    public turbotSizeGroups: NomenclatureDTO<number>[] = [];
    public declarationPages: InspectionLogBookPageNomenclatureDTO[] = [];
    public fishErrors: TLError[] = [];

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private readonly service: InspectionsService;
    private readonly nomenclatures: CommonNomenclatures;
    private readonly translate: FuseTranslationLoaderService;

    public constructor(
        service: InspectionsService,
        nomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService
    ) {
        this.buildForm();

        this.service = service;
        this.nomenclatures = nomenclatures;
        this.translate = translate;

        this.permitTypes = [
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.FirstSaleLogBook,
                displayName: translate.getValue('inspections.market-first-sale-log-book'),
                isActive: true,
            }),
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.AdmissionLogBook,
                displayName: translate.getValue('inspections.market-admission-log-book'),
                isActive: true,
            }),
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.TransportationLogBook,
                displayName: translate.getValue('inspections.market-transport-log-book'),
                isActive: true,
            }),
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.ShipLogBook,
                displayName: translate.getValue('inspections.market-ship-log-book'),
                isActive: true,
            }),
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.AquacultureLogBook,
                displayName: translate.getValue('inspections.market-aquaculture-log-book'),
                isActive: true,
            }),
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.Invoice,
                displayName: translate.getValue('inspections.market-other'),
                isActive: true,
            }),
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.NNN,
                displayName: translate.getValue('inspections.market-ship-nnn'),
                isActive: true,
            }),
        ];

        this.mapOptions = new MapOptions();
        this.mapOptions.showGridLayer = true;
        this.mapOptions.gridLayerStyle = this.createCustomGridLayerStyle();
        this.mapOptions.selectGridLayerStyle = this.createCustomSelectGridLayerStyle();
    }

    public async ngOnInit(): Promise<void> {
        if (this.readOnly) {
            this.form.disable();
        }

        const nomenclatureTables = await forkJoin([
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Countries, this.nomenclatures.getCountries.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.VesselTypes, this.nomenclatures.getVesselTypes.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.TurbotSizeGroups, this.nomenclatures.getTurbotSizeGroups.bind(this.nomenclatures), false
            ),
            this.service.getAquacultures()
        ]).toPromise();

        this.ships = nomenclatureTables[0];
        this.countries = nomenclatureTables[1];
        this.vesselTypes = nomenclatureTables[2];
        this.turbotSizeGroups = nomenclatureTables[3];
        this.aquacultures = nomenclatureTables[4];

        if (this.model !== undefined && this.model !== null) {
            this.fillLogBookPageData();
        }

        setTimeout(() => {
            this.fillForm();
        });
    }

    public setData(data: MarketCatchTableParams, wrapperData: DialogWrapperData): void {
        if (data.model !== undefined && data.model !== null) {
            this.model = data.model;
        }

        this.readOnly = data.readOnly;
        this.fishes = data.fishes;
        this.types = data.types;
        this.hasUndersizedCheck = data.hasUndersizedCheck;
        this.presentations = data.presentations;
        this.hasCatchType = data.hasCatchType;

        if (!data.hasCatchType) {
            this.form.get('catchTypeControl')!.disable();
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.readOnly) {
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

    protected buildForm(): void {
        this.form = new FormGroup({
            permitControl: new FormControl(undefined, [Validators.required]),
            typeControl: new FormControl(undefined, [Validators.required]),
            countControl: new FormControl(undefined, [TLValidators.number(0, undefined, 0)]),
            catchTypeControl: new FormControl(undefined, [Validators.required]),
            quantityControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)]),
            presentationControl: new FormControl(undefined, [Validators.required]),
            turbotSizeGroupControl: new FormControl(undefined),
            shipControl: new FormControl(undefined),
            aquacultureRegisteredControl: new FormControl(true),
            aquacultureControl: new FormControl(undefined, [Validators.required]),
            aquacultureTextControl: new FormControl(undefined, [Validators.required, Validators.maxLength(4000)]),
            declarationNumberControl: new FormControl(undefined),
            declarationDateControl: new FormControl(undefined),
            invoiceDataControl: new FormControl(undefined, Validators.maxLength(4000)),
            undersizedControl: new FormControl(false),
            pageNumberControl: new FormControl(undefined),
            logBookNumberControl: new FormControl(undefined),
            pageDateControl: new FormControl(undefined)
        }, this.fishQuantityValidator());

        this.form.get('shipControl')!.disable();
        this.form.get('aquacultureControl')!.disable();
        this.form.get('aquacultureTextControl')!.disable();

        this.form.get('permitControl')!.valueChanges.subscribe({
            next: (value: NomenclatureDTO<DeclarationLogBookTypeEnum> | undefined) => {
                if (value !== undefined && value !== null) {
                    this.permitTypeSelected = value?.value;

                    this.getPageDateLabel();
                    this.pullDeclarations();

                    if (this.permitTypeSelected === DeclarationLogBookTypeEnum.AquacultureLogBook) {
                        this.form.get('aquacultureRegisteredControl')!.setValue(this.aquacultureRegistered);
                    }

                    if (this.permitTypeSelected === DeclarationLogBookTypeEnum.AquacultureLogBook || this.permitTypeSelected === DeclarationLogBookTypeEnum.Invoice || this.permitTypeSelected === DeclarationLogBookTypeEnum.NNN) {
                        this.form.get('shipControl')!.disable();
                    }
                    else if (!this.readOnly) {
                        this.form.get('shipControl')!.enable();
                    }
                }
                else {
                    this.declarationPages = [];

                    this.form.get('pageNumberControl')!.setValue(undefined);
                    this.form.get('pageNumberControl')!.updateValueAndValidity({ onlySelf: true });

                    this.form.get('shipControl')!.reset();
                    this.form.get('shipControl')!.disable();
                }
            }
        });

        this.form.get('pageNumberControl')!.valueChanges.subscribe({
            next: (value: NomenclatureDTO<number> | string | undefined) => {
                this.hasDeclaration = false;

                if (value !== undefined && value !== null) {
                    if (typeof value === 'string') {
                        this.form.get('pageDateControl')!.enable();
                        this.form.get('logBookNumberControl')!.enable();
                    }
                    else if (value instanceof InspectionLogBookPageNomenclatureDTO) {
                        this.hasDeclaration = true;
                        this.form.get('pageDateControl')!.setValue(value.logBookPageDate);
                        this.form.get('logBookNumberControl')!.setValue(value.logBookNum);
                        this.form.get('declarationNumberControl')!.setValue(value.originDeclarationNum);
                        this.form.get('declarationDateControl')!.setValue(value.originDeclarationDate);

                        this.disablePageControls();
                    }
                }
                else {
                    this.form.get('pageDateControl')!.setValue(undefined);
                    this.form.get('logBookNumberControl')!.setValue(undefined);
                    this.form.get('declarationNumberControl')!.setValue(undefined);
                    this.form.get('declarationDateControl')!.setValue(undefined);
                }

                if (this.readOnly === true) {
                    this.disablePageControls();
                }
            }
        });

        this.form.get('aquacultureRegisteredControl')!.valueChanges.subscribe({
            next: (value: boolean) => {
                this.aquacultureRegistered = value;

                if (this.readOnly) {
                    return;
                }

                if (value) {
                    this.form.get('aquacultureControl')!.enable();
                    this.form.get('aquacultureTextControl')!.disable();
                    this.form.get('pageDateControl')!.disable();
                    this.form.get('logBookNumberControl')!.disable();
                }
                else {
                    this.declarationPages = [];
                    this.form.get('aquacultureControl')!.disable();
                    this.form.get('aquacultureTextControl')!.enable();
                    this.form.get('pageDateControl')!.enable();
                    this.form.get('logBookNumberControl')!.enable();
                }
            }
        });

        this.form.get('aquacultureControl')!.valueChanges.subscribe({
            next: (value: NomenclatureDTO<number> | undefined) => {
                if (value !== undefined && value !== null) {
                    this.pullDeclarations();
                }
            }
        });

        this.form.get('typeControl')?.valueChanges.subscribe({
            next: (value: NomenclatureDTO<number> | undefined) => {
                if (value === undefined || value === null || value instanceof NomenclatureDTO) {
                    this.setTurbotFlag(value);
                }
                else {
                    this.setTurbotFlag(undefined);
                }
            }
        });
    }

    protected fillForm(): void {
        this.permitTypeSelected = this.model.logBookType;

        if (this.model.fishTypeId !== undefined && this.model.fishTypeId !== null) {
            const fish: NomenclatureDTO<number> = this.fishes.find(x => x.value === this.model.fishTypeId)!;
            this.form.get('typeControl')!.setValue(fish);
            this.setTurbotFlag(fish);
        }

        this.form.get('countControl')!.setValue(this.model.catchCount);
        this.form.get('quantityControl')!.setValue(this.model.catchQuantity);
        this.form.get('catchTypeControl')!.setValue(this.types.find(f => f.value === this.model.catchTypeId));
        this.form.get('undersizedControl')!.setValue(this.model.undersized);
        this.form.get('turbotSizeGroupControl')!.setValue(this.turbotSizeGroups.find(f => f.value === this.model.turbotSizeGroupId));

        this.form.get('presentationControl')!.setValue(
            this.presentations.find(f => f.value === this.model.presentationId)
            ?? this.presentations.find(f => f.code === 'WHL')
        );

        this.form.get('shipControl')!.setValue(this.model.originShip);
        this.form.get('permitControl')!.setValue(this.permitTypes.find(f => f.value === this.model.logBookType));

        if (this.model.aquacultureId) {
            this.form.get('aquacultureControl')!.setValue(this.aquacultures.find(f => f.value == this.model.aquacultureId));
        }
        else if (this.model.unregisteredEntityData != undefined) {
            this.aquacultureRegistered = false;
            this.form.get('aquacultureRegisteredControl')!.setValue(false);
        }

        if (this.model.logBookType === DeclarationLogBookTypeEnum.Invoice) {
            this.form.get('invoiceDataControl')!.setValue(this.model.unregisteredEntityData);
        }
        else {
            this.form.get('aquacultureTextControl')!.setValue(this.model.unregisteredEntityData);
        }
    }

    protected fillModel(): void {
        const catchCount = this.form.get('countControl')!.value;
        const catchQuantity = this.form.get('quantityControl')!.value;
        this.model.undersized = this.form.get('undersizedControl')!.value;

        if (this.hasUndersizedCheck) {
            this.model.catchTypeId = this.model.undersized === true
                ? this.types.find(f => f.code === CatchSizeCodesEnum[CatchSizeCodesEnum.BMS])?.value
                : this.types.find(f => f.code === CatchSizeCodesEnum[CatchSizeCodesEnum.LSC])?.value;
        }
        else {
            this.model.catchTypeId = this.form.get('catchTypeControl')!.value?.value;
        }

        this.model.fishTypeId = this.form.get('typeControl')!.value?.value;
        this.model.catchCount = catchCount ? Number(catchCount) : undefined;
        this.model.catchQuantity = catchQuantity ? Number(catchQuantity) : undefined;
        this.model.presentationId = this.form.get('presentationControl')!.value?.value;
        this.model.originShip = this.form.get('shipControl')!.value;
        this.model.logBookType = this.form.get('permitControl')!.value?.value;
        this.model.aquacultureId = this.form.get('aquacultureControl')!.value?.value;
        this.model.turbotSizeGroupId = this.form.get('turbotSizeGroupControl')?.value?.value;

        this.model.unregisteredEntityData = this.model.logBookType === DeclarationLogBookTypeEnum.Invoice
            ? this.form.get('invoiceDataControl')!.value
            : this.form.get('aquacultureTextControl')!.value;

        const logBookPage: InspectionLogBookPageNomenclatureDTO | string = this.form.get('pageNumberControl')!.value;

        if (typeof logBookPage === 'string') {
            this.model.unregisteredPageNum = logBookPage;
            this.model.unregisteredLogBookNum = this.form.get('logBookNumberControl')!.value;
            this.model.unregisteredPageDate = this.form.get('pageDateControl')!.value;
            this.model.logBookPageId = undefined;
        }
        else if (logBookPage !== null && logBookPage !== undefined) {
            const page = this.declarationPages.find(f => f.value === logBookPage.value);

            this.model.logBookPageId = this.form.get('pageNumberControl')!.value!.value;
            this.model.unregisteredPageNum = page?.logPageNum?.toString();
            this.model.unregisteredPageDate = page?.logBookPageDate;
            this.model.unregisteredLogBookNum = page?.logBookNum;
        }
    }

    private createCustomGridLayerStyle(): SimplePolygonStyleDef {
        const layerStyle: SimplePolygonStyleDef = new SimplePolygonStyleDef();
        layerStyle.fill = new FillDef('rgba(255,255,255,0.4)');
        layerStyle.stroke = new StrokeDef('rgb(0,116,2,1)', 1);

        return layerStyle;
    }

    private createCustomSelectGridLayerStyle(): SimplePolygonStyleDef {
        const layerStyle: SimplePolygonStyleDef = new SimplePolygonStyleDef();
        layerStyle.fill = new FillDef('rgb(255,177,34, 0.1)');
        layerStyle.stroke = new StrokeDef('rgb(255,177,34,1)', 3);

        return layerStyle;
    }

    private getPageDateLabel(): void {
        switch (this.permitTypeSelected) {
            case DeclarationLogBookTypeEnum.AdmissionLogBook:
                this.pageDateLabel = this.translate.getValue('inspections.market-handover-date');
                break;
            case DeclarationLogBookTypeEnum.FirstSaleLogBook:
                this.pageDateLabel = this.translate.getValue('inspections.market-sale-date');
                break;
            case DeclarationLogBookTypeEnum.TransportationLogBook:
                this.pageDateLabel = this.translate.getValue('inspections.market-loading-date');
                break;
            case DeclarationLogBookTypeEnum.ShipLogBook:
                this.pageDateLabel = this.translate.getValue('inspections.market-declaration-date');
                break;
            case DeclarationLogBookTypeEnum.AquacultureLogBook:
            default:
                this.pageDateLabel = this.translate.getValue('inspections.market-filling-date');
                break;
        }
    }

    private fillLogBookPageData(): void {
        if (this.model.logBookPageId !== undefined && this.model.logBookPageId !== null) {
            if (((this.model.originShip?.shipId !== null && this.model.originShip?.shipId !== undefined)
                || (this.model.aquacultureId !== null && this.model.aquacultureId !== undefined))
                && this.model.logBookType !== null && this.model.logBookType !== undefined
                && this.model.logBookType !== DeclarationLogBookTypeEnum.Invoice
                && this.model.logBookType !== DeclarationLogBookTypeEnum.NNN
            ) {
                this.service.getLogBookPages(this.model.logBookType, this.model.originShip?.shipId, this.model.aquacultureId).subscribe({
                    next: (pages: InspectionLogBookPageNomenclatureDTO[]) => {
                        this.declarationPages = pages;
                        const page: InspectionLogBookPageNomenclatureDTO | undefined = pages.find(f => f.value === this.model.logBookPageId);

                        this.form.get('pageNumberControl')!.setValue(page);
                        this.form.get('logBookNumberControl')!.setValue(page?.logBookNum);
                        this.form.get('pageDateControl')!.setValue(page?.logBookPageDate);
                        this.form.get('declarationNumberControl')!.setValue(page?.originDeclarationNum);
                        this.form.get('declarationDateControl')!.setValue(page?.originDeclarationDate);

                        this.disablePageControls();
                    }
                });
            }
            else {
                this.form.get('pageNumberControl')!.setValue(this.model.unregisteredPageNum);
                this.form.get('logBookNumberControl')!.setValue(this.model.unregisteredLogBookNum);
                this.form.get('pageDateControl')!.setValue(this.model.unregisteredPageDate);
            }
        }
        else {
            this.form.get('pageNumberControl')!.setValue(this.model.unregisteredPageNum);
            this.form.get('logBookNumberControl')!.setValue(this.model.unregisteredLogBookNum);
            this.form.get('pageDateControl')!.setValue(this.model.unregisteredPageDate);
        }
    }

    private disablePageControls(): void {
        this.form.get('logBookNumberControl')!.disable();
        this.form.get('pageDateControl')!.disable();
        this.form.get('declarationNumberControl')!.disable();
        this.form.get('declarationDateControl')!.disable();
    }

    private pullDeclarations(vessel: VesselDuringInspectionDTO | undefined = undefined): void {
        if (this.permitTypeSelected !== undefined
            && this.permitTypeSelected !== null
            && this.permitTypeSelected !== DeclarationLogBookTypeEnum.Invoice
            && this.permitTypeSelected !== DeclarationLogBookTypeEnum.NNN
        ) {
            let ship: VesselDuringInspectionDTO | undefined = this.form.get('shipControl')!.value;
            const aquaculture: number | undefined = this.form.get('aquacultureControl')!.value?.value;

            if (vessel !== undefined && vessel !== null) {
                ship = vessel;
            }

            if ((ship !== undefined && ship !== null && ship.shipId !== undefined && ship.shipId !== null)
                || (aquaculture !== undefined && aquaculture !== null)
            ) {
                this.service.getLogBookPages(this.permitTypeSelected!, ship?.shipId, aquaculture).subscribe({
                    next: (result: InspectionLogBookPageNomenclatureDTO[]) => {
                        if (result !== undefined && result !== null) {
                            this.declarationPages = result;
                        }
                    }
                });
            }
        }
    }

    private setTurbotFlag(value: NomenclatureDTO<number> | undefined): void {
        if (value !== undefined && value !== null) {
            if (FishCodesEnum[value.code as keyof typeof FishCodesEnum] === FishCodesEnum.TUR) {
                this.showTurbotControl = true;

                if (this.readOnly) {
                    this.form.get('turbotSizeGroupControl')!.disable();
                }
            }
            else {
                this.showTurbotControl = false;
            }
        }
        else {
            this.showTurbotControl = false;
        }
    }

    private fishQuantityValidator(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            const permit: NomenclatureDTO<number> | string = form.get('pageNumberControl')!.value;
            const fishCatch: NomenclatureDTO<number> = form.get('typeControl')!.value;
            const quantity: number = form.get('quantityControl')!.value;

            if (typeof permit === 'object' && permit && fishCatch && quantity) {
                this.fishErrors = [];
                let fishes = this.declarationPages.find(x => x.value === permit.value)?.logBookProducts ?? [];

                const presentation: NomenclatureDTO<number> = form.get('presentationControl')!.value;

                if (presentation) {
                    fishes = fishes.filter(f => f.presentationId == null || f.presentationId === presentation.value);
                }

                if (fishes.length > 0) {
                    const fishQuantities: number[] = fishes.filter(f => f.fishId === fishCatch.value).map(f => f.quantity!);
                    const quantitiesSum: number = this.sum(fishQuantities);

                    if (quantitiesSum < quantity) {
                        this.fishErrors.push({
                            text: this.translate.getValue('inspections.declaration-fish-quantity-error')
                                .replace('{0}', `${fishCatch.displayName} : ${quantitiesSum}`)
                                .replace('{1}', (quantity - quantitiesSum).toString()),
                            type: 'warn'
                        });
                    }
                    else {
                        this.fishErrors.push({
                            text: this.translate.getValue('inspections.declaration-fish-quantity-warning')
                                .replace('{0}', `${fishCatch.displayName} : ${quantitiesSum}`),
                            type: 'warn'
                        });
                    }
                }
            }

            return null;
        }
    }

    private sum(nums: number[]): number {
        return nums.reduce((sum: number, current: number) => { return sum + current; }, 0);
    }
}