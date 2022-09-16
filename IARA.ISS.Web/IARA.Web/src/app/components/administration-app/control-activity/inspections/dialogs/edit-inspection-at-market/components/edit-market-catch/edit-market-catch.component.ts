import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

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
import { VesselDuringInspectionDTO } from '@app/models/generated/dtos/VesselDuringInspectionDTO';
import { DeclarationLogBookPageDTO } from '@app/models/generated/dtos/DeclarationLogBookPageMobileDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { forkJoin } from 'rxjs';

@Component({
    selector: 'edit-market-catch',
    styleUrls: ['./edit-market-catch.component.scss'],
    templateUrl: './edit-market-catch.component.html',
})
export class EditMarketCatchComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;

    protected model: InspectedDeclarationCatchDTO = new InspectedDeclarationCatchDTO();

    @ViewChild(TLMapViewerComponent)
    private mapViewer!: TLMapViewerComponent;

    @ViewChild(TLPopoverComponent)
    private mapPopover!: TLPopoverComponent;

    public mapOptions: MapOptions;

    public hasCatchType: boolean = true;
    public isMapPopoverOpened: boolean = false;
    public readOnly: boolean = false;
    public permitTypeSelected: DeclarationLogBookTypeEnum | undefined;

    public readonly declarationLogBookTypeEnum = DeclarationLogBookTypeEnum;

    public ships: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public vesselTypes: NomenclatureDTO<number>[] = [];
    public fishes: NomenclatureDTO<number>[] = [];
    public types: NomenclatureDTO<number>[] = [];
    public catchZones: NomenclatureDTO<number>[] = [];
    public presentations: NomenclatureDTO<number>[] = [];
    public permitTypes: NomenclatureDTO<DeclarationLogBookTypeEnum>[] = [];
    public declarations: NomenclatureDTO<number>[] = [];

    private temporarySelectedGridSector: NomenclatureDTO<number> | undefined;
    private logBookPages: DeclarationLogBookPageDTO[] = [];

    private readonly service: InspectionsService;
    private readonly nomenclatures: CommonNomenclatures;

    public constructor(
        service: InspectionsService,
        nomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService
    ) {
        this.buildForm();

        this.service = service;
        this.nomenclatures = nomenclatures;

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
                value: DeclarationLogBookTypeEnum.Invoice,
                displayName: translate.getValue('inspections.market-ship-invoice'),
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
            )
        ]).toPromise();

        this.ships = nomenclatureTables[0];
        this.countries = nomenclatureTables[1];
        this.vesselTypes = nomenclatureTables[2];

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
        this.catchZones = data.catchZones;
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

    public onPopoverToggled(isOpened: boolean): void {
        this.isMapPopoverOpened = isOpened;
        setTimeout(() => {
            if (this.isMapPopoverOpened === true) {
                this.mapViewer.selectedGridSectorsChangeEvent.subscribe({
                    next: (selectedGridSectors: string[] | undefined) => {
                        if (!CommonUtils.isNullOrEmpty(selectedGridSectors)) {
                            this.temporarySelectedGridSector = this.catchZones.find(f => f.code === selectedGridSectors![0])!;
                        }
                    }
                });

                const quadrant: string | null | undefined = this.form.get('catchZoneControl')!.value;
                if (quadrant !== null && quadrant !== undefined) {
                    this.mapViewer.gridLayerIsRenderedEvent.subscribe({
                        next: (isMapRendered: boolean) => {
                            if (isMapRendered === true) {
                                this.mapViewer.selectGridSectors([quadrant]);
                            }
                        }
                    });
                }
            }
        }, 1000);
    }

    public onQuadrantChosenBtnClicked(): void {
        this.form.get('catchZoneControl')!.setValue(this.temporarySelectedGridSector);
        this.mapPopover.closePopover(true);
    }

    public onMapPopoverCancelBtnClicked(): void {
        this.temporarySelectedGridSector = undefined;
        this.mapViewer.selectGridSectors([]);
        this.mapPopover.closePopover(true);
    }

    protected buildForm(): void {
        this.form = new FormGroup({
            typeControl: new FormControl(undefined, [Validators.required]),
            countControl: new FormControl(undefined, [TLValidators.number()]),
            catchTypeControl: new FormControl(undefined, [Validators.required]),
            quantityControl: new FormControl(undefined, [Validators.required]),
            presentationControl: new FormControl(undefined, [Validators.required]),
            unloadedQuantityControl: new FormControl(undefined, [Validators.required]),
            catchZoneControl: new FormControl(undefined, [Validators.required]),
            shipControl: new FormControl(undefined),
            permitControl: new FormControl(undefined),
            declarationNumberControl: new FormControl(undefined),
            declarationDateControl: new FormControl(undefined),
        });

        this.form.get('declarationNumberControl')!.valueChanges.subscribe({
            next: async (value: NomenclatureDTO<number> | undefined) => {
                if (typeof value === 'string') {
                    this.form.get('declarationDateControl')!.enable();
                } else {
                    const page = this.logBookPages.find(f => f.id === value!.value);

                    this.form.get('declarationDateControl')!.setValue(page!.date);
                    this.form.get('declarationDateControl')!.disable();
                }

                if (this.readOnly === true) {
                    this.form.get('declarationDateControl')!.disable();
                }
            }
        });

        this.form.get('permitControl')!.valueChanges.subscribe({
            next: async (value: NomenclatureDTO<DeclarationLogBookTypeEnum>) => {
                this.permitTypeSelected = value?.value;

                if (this.permitTypeSelected === DeclarationLogBookTypeEnum.Invoice || this.permitTypeSelected === DeclarationLogBookTypeEnum.NNN) {
                    this.form.get('catchZoneControl')!.clearValidators();
                    this.form.get('catchZoneControl')!.markAsPending({ emitEvent: false });
                    return;
                }

                this.form.get('catchZoneControl')!.setValidators([Validators.required]);
                this.form.get('catchZoneControl')!.markAsPending({ emitEvent: false });

                const ship: VesselDuringInspectionDTO = this.form.get('shipControl')!.value;

                if (ship !== null && ship !== undefined && this.permitTypeSelected !== null && this.permitTypeSelected !== undefined) {
                    const result = await this.service.getDeclarationLogBookPages(this.permitTypeSelected, ship.shipId!).toPromise();

                    this.logBookPages = result;
                    this.declarations = result.map(f => new NomenclatureDTO({
                        value: f.id,
                        displayName: f.num,
                        isActive: true,
                    }));
                }
            }
        });

        this.form.get('shipControl')!.valueChanges.subscribe({
            next: async (value: VesselDuringInspectionDTO) => {
                if (this.permitTypeSelected === DeclarationLogBookTypeEnum.Invoice || this.permitTypeSelected === DeclarationLogBookTypeEnum.NNN) {
                    return;
                }

                if (this.permitTypeSelected !== null && this.permitTypeSelected !== undefined) {
                    const result = await this.service.getDeclarationLogBookPages(this.permitTypeSelected, value.shipId!).toPromise();

                    this.logBookPages = result;
                    this.declarations = result.map(f => new NomenclatureDTO({
                        value: f.id,
                        displayName: f.num,
                        isActive: true,
                    }));
                }
            }
        });
    }

    protected async fillForm(): Promise<void> {
        this.form.get('typeControl')!.setValue(this.fishes.find(f => f.value === this.model.fishTypeId));
        this.form.get('catchTypeControl')!.setValue(this.types.find(f => f.value === this.model.catchTypeId));
        this.form.get('quantityControl')!.setValue(this.model.catchQuantity);
        this.form.get('presentationControl')!.setValue(
            this.presentations.find(f => f.value === this.model.presentationId)
            ?? this.presentations.find(f => f.code === 'WHL')
        );
        this.form.get('unloadedQuantityControl')!.setValue(this.model.unloadedQuantity);
        this.form.get('catchZoneControl')!.setValue(this.catchZones.find(f => f.value === this.model.catchZoneId));
        this.form.get('shipControl')!.setValue(this.model.originShip, { emitEvent: false });

        this.permitTypeSelected = this.model.logBookType;

        this.form.get('permitControl')!.setValue(this.permitTypes.find(f => f.value === this.model.logBookType), { emitEvent: false });

        if (this.model.originShip?.shipId !== null && this.model.originShip?.shipId !== undefined
            && this.permitTypeSelected !== null && this.permitTypeSelected !== undefined
            && this.permitTypeSelected !== DeclarationLogBookTypeEnum.Invoice && this.permitTypeSelected !== DeclarationLogBookTypeEnum.NNN) {
            const result = await this.service.getDeclarationLogBookPages(this.permitTypeSelected, this.model.originShip.shipId!).toPromise();

            this.logBookPages = result;
            this.declarations = result.map(f => new NomenclatureDTO({
                value: f.id,
                displayName: f.num,
                isActive: true,
            }));

            const page = this.logBookPages.find(f => f.id === this.model.logBookPageId);
            const nomPage = this.declarations.find(f => f.value === this.model.logBookPageId);

            this.form.get('declarationNumberControl')!.setValue(nomPage ?? this.model.unregisteredPageNum, { emitEvent: false });
            this.form.get('declarationDateControl')!.setValue(page?.date ?? this.model.unregisteredPageDate);
            this.form.get('declarationDateControl')!.disable();
        } else {
            this.form.get('declarationNumberControl')!.setValue(this.model.unregisteredPageNum, { emitEvent: false });
            this.form.get('declarationDateControl')!.setValue(this.model.unregisteredPageDate);
        }
    }

    protected fillModel(): void {
        this.model.fishTypeId = this.form.get('typeControl')!.value?.value;
        this.model.catchTypeId = this.form.get('catchTypeControl')!.value?.value;
        this.model.catchQuantity = Number(this.form.get('quantityControl')!.value);
        this.model.presentationId = this.form.get('presentationControl')!.value?.value;
        this.model.unloadedQuantity = Number(this.form.get('unloadedQuantityControl')!.value);
        this.model.catchZoneId = this.form.get('catchZoneControl')!.value?.value;
        this.model.originShip = this.form.get('shipControl')!.value;
        this.model.logBookType = this.form.get('permitControl')!.value?.value;

        const permit: NomenclatureDTO<number> | string = this.form.get('declarationNumberControl')!.value;

        if (typeof permit === 'string') {
            this.model.unregisteredPageNum = permit;
            this.model.unregisteredPageDate = this.form.get('declarationDateControl')!.value;
        } else if (permit !== null && permit !== undefined) {
            const page = this.logBookPages.find(f => f.id === permit.value);

            this.model.logBookPageId = page?.id;
            this.model.unregisteredPageNum = page?.num;
            this.model.unregisteredPageDate = page?.date;
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
}