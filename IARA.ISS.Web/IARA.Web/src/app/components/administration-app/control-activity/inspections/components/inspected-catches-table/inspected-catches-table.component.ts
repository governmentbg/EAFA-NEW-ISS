import { Component, Input, OnChanges, OnInit, Self, SimpleChanges, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { FillDef, MapOptions, SimplePolygonStyleDef, StrokeDef, TLMapViewerComponent } from '@tl/tl-angular-map';

import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { InspectedCatchTableModel } from '../../models/inspected-catch-table.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { TLPopoverComponent } from '@app/shared/components/tl-popover/tl-popover.component';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { InspectionCatchMeasureDTO } from '@app/models/generated/dtos/InspectionCatchMeasureDTO';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { InspectedShipComponent } from '../inspected-ship/inspected-ship.component';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { InspectedShipParams } from '../inspected-ship/models/inspected-ship-params.model';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { VesselDuringInspectionDTO } from '@app/models/generated/dtos/VesselDuringInspectionDTO';
import { CatchSizeCodesEnum } from '@app/enums/catch-size-codes.enum';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';

function groupBy(array: any[], f: any): any[][] {
    const groups: any = {};
    array.forEach(function (o) {
        const group = JSON.stringify(f(o));
        groups[group] = groups[group] || [];
        groups[group].push(o);
    });
    return Object.keys(groups).map(function (group) {
        return groups[group];
    })
}

@Component({
    selector: 'inspected-catches-table',
    styleUrls: ['./inspected-catches-table.component.scss'],
    templateUrl: './inspected-catches-table.component.html',
})
export class InspectedCatchesTableComponent extends CustomFormControl<InspectionCatchMeasureDTO[]> implements OnInit, OnChanges {
    public catchesFormGroup!: FormGroup;

    @Input()
    public hasShip: boolean = false;

    @Input()
    public hasUnloadedQuantity: boolean = false;

    @Input()
    public hasAverageSize: boolean = false;

    @Input()
    public hasAllowedDeviation: boolean = true;

    @Input()
    public hasSex: boolean = false;

    @Input()
    public hasCatchZone: boolean = true;

    @Input()
    public hasTurbotSizeGroups: boolean = true;

    @Input()
    public hasCatchType: boolean = true;

    @Input()
    public ships: ShipNomenclatureDTO[] = [];

    @Input()
    public fishes: NomenclatureDTO<number>[] = [];

    @Input()
    public types: NomenclatureDTO<number>[] = [];

    @Input()
    public catchZones: NomenclatureDTO<number>[] = [];

    @Input()
    public turbotSizeGroups: NomenclatureDTO<number>[] = [];

    @Input()
    public fishSex: NomenclatureDTO<number>[] = [];

    @Input()
    public hasUndersizedCheck: boolean = false;

    @Input()
    public requiresFish: boolean = true;

    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(TLMapViewerComponent)
    private mapViewer!: TLMapViewerComponent;

    @ViewChild(TLPopoverComponent)
    private mapPopover!: TLPopoverComponent;

    public mapOptions: MapOptions;

    public isMapPopoverOpened: boolean = false;

    public catches: InspectedCatchTableModel[] = [];
    public catchQuantities: Map<number, number> = new Map<number, number>();

    private temporarySelectedGridSector: NomenclatureDTO<number> | undefined;

    private readonly translate: FuseTranslationLoaderService;
    private readonly editShipDialog: TLMatDialog<InspectedShipComponent>;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        editShipDialog: TLMatDialog<InspectedShipComponent>
    ) {
        super(ngControl);

        this.translate = translate;
        this.editShipDialog = editShipDialog;

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.control.updateValueAndValidity();
            },
        });

        this.mapOptions = new MapOptions();
        this.mapOptions.showGridLayer = true;
        this.mapOptions.gridLayerStyle = this.createCustomGridLayerStyle();
        this.mapOptions.selectGridLayerStyle = this.createCustomSelectGridLayerStyle();
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if ('requiresFish' in changes) {
            if (!this.requiresFish) {
                this.control.setValidators([this.catchesValidator(), this.missingCatchInspectionTypeValidator(), this.missingUnloadedQuantityValidator()]);
            }
        }

        if ('hasCatchType' in changes) {
            if (this.hasCatchType) {
                this.catchesFormGroup.controls.catchInspectionTypeIdControl.enable();
            }
            else {
                this.catchesFormGroup.controls.catchInspectionTypeIdControl.disable();
            }
        }
    }

    public writeValue(value: InspectionCatchMeasureDTO[]): void {
        if (value !== undefined && value !== null) {
            const catches: InspectedCatchTableModel[] = value.map(x => new InspectedCatchTableModel({
                id: x.id,
                fishId: x.fishId,
                action: x.action,
                allowedDeviation: x.allowedDeviation,
                averageSize: x.averageSize,
                catchCount: x.catchCount,
                catchInspectionTypeId: x.catchInspectionTypeId,
                catchQuantity: x.catchQuantity,
                catchZoneId: x.catchZoneId,
                fishSexId: x.fishSexId,
                shipLogBookPageId: x.shipLogBookPageId,
                undersized: x.undersized,
                originShip: x.originShip,
                storageLocation: x.storageLocation,
                unloadedQuantity: x.unloadedQuantity,
                fish: this.fishes.find(s => s.value === x.fishId),
                type: this.types.find(s => s.value === x.catchInspectionTypeId),
                catchZone: this.catchZones.find(s => s.value === x.catchZoneId),
                turbotSizeGroup: this.turbotSizeGroups.find(s => s.value === x.turbotSizeGroupId),
                turbotSizeGroupId: x.turbotSizeGroupId,
                hasMissingProperties: this.checkIfCatchHasMissingProperties(x)
            }));

            setTimeout(() => {
                this.catches = catches;
                this.recalculateCatchQuantitySums();
                this.onChanged(this.getValue());
            });
        }
        else {
            setTimeout(() => {
                this.catches = [];
                this.recalculateCatchQuantitySums();
                this.onChanged(this.getValue());
            });
        }
    }

    public onEditRecord(row: GridRow<InspectedCatchTableModel>): void {
        if (this.hasUnloadedQuantity) {
            this.catchesFormGroup.get('unloadedQuantityControl')!.setValidators([Validators.required, TLValidators.number(0)]);
        }

        if (row !== null && row !== undefined) {
            this.catchesFormGroup.get('catchZoneControl')!.setValue(row.data.catchZone);
            this.recalculateCatchQuantitySums();
        }
    }

    public catchRecordChanged(event: RecordChangedEventArgs<InspectedCatchTableModel>): void {
        event.Record.catchZoneId = this.catchesFormGroup.get('catchZoneControl')!.value?.value;
        event.Record.catchZone = this.catchesFormGroup.get('catchZoneControl')!.value;
        event.Record.hasMissingProperties = false;

        this.catches = this.datatable.rows;
        this.onChanged(this.getValue());
        this.control.updateValueAndValidity();

        this.catchesFormGroup.get('catchZoneControl')!.setValue(null);
        this.recalculateCatchQuantitySums();
    }

    public onPopoverToggled(isOpened: boolean): void {
        this.isMapPopoverOpened = isOpened;

        setTimeout(() => {
            if (this.isMapPopoverOpened === true) {
                this.mapViewer.selectedGridSectorsChangeEvent.subscribe({
                    next: (selectedGridSectors: string[] | undefined) => {
                        if (!CommonUtils.isNullOrEmpty(selectedGridSectors)) {
                            this.temporarySelectedGridSector = this.catchZones.find(x => x.code === selectedGridSectors![0])!;
                        }
                    }
                });

                const quadrant: string | null | undefined = this.catchesFormGroup.get('catchZoneControl')!.value?.code;
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
        });
    }

    public onQuadrantChosenBtnClicked(): void {
        this.catchesFormGroup.get('catchZoneControl')!.setValue(this.temporarySelectedGridSector);
        this.temporarySelectedGridSector = undefined;
        this.mapViewer.selectGridSectors([]);
        this.mapPopover.closePopover(true);
    }

    public onMapPopoverCancelBtnClicked(): void {
        this.temporarySelectedGridSector = undefined;
        this.mapViewer.selectGridSectors([]);
        this.mapPopover.closePopover(true);
    }

    public onShipDialogOpen(row: GridRow<InspectedCatchTableModel>): void {
        const readOnly: boolean = this.isDisabled;

        let data: InspectedShipParams | undefined;
        let title: string;

        if (row.data.originShip !== undefined && row.data.originShip !== null) {
            data = new InspectedShipParams({
                model: row.data.originShip,
                ships: this.ships,
                readOnly: readOnly,
                hasMap: false,
            });

            if (readOnly) {
                title = this.translate.getValue('inspections.view-catch-ship-dialog-title');
            }
            else {
                title = this.translate.getValue('inspections.edit-catch-ship-dialog-title');
            }
        }
        else {
            data = new InspectedShipParams({
                readOnly: readOnly,
                ships: this.ships,
                hasMap: false,
            });

            title = this.translate.getValue('inspections.add-catch-ship-dialog-title');
        }

        const dialog = this.editShipDialog.openWithTwoButtons({
            title: title,
            TCtor: InspectedShipComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => closeFn()
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: !readOnly,
            viewMode: readOnly
        }, '1200px');

        dialog.subscribe((result: VesselDuringInspectionDTO | undefined) => {
            if (result !== undefined && result !== null) {
                row.data.originShip = result;
            }
        });
    }

    protected buildForm(): AbstractControl {
        this.catchesFormGroup = new FormGroup({
            fishIdControl: new FormControl(undefined, [Validators.required]),
            catchInspectionTypeIdControl: new FormControl(undefined, [Validators.required]),
            catchCountControl: new FormControl(undefined, [TLValidators.number(0, undefined, 0)]),
            catchQuantityControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)]),
            unloadedQuantityControl: new FormControl(undefined, [TLValidators.number(0)]),
            averageSizeControl: new FormControl(undefined, [TLValidators.number(0)]),
            allowedDeviationControl: new FormControl(undefined, [TLValidators.number(0, 100)]),
            catchZoneControl: new FormControl(undefined),
            shipControl: new FormControl(undefined),
            turbotSizeGroupIdControl: new FormControl(undefined),
            fishSexIdControl: new FormControl(undefined),
            undersizedControl: new FormControl(false)
        });

        return new FormControl(undefined, [
            this.catchesValidator(),
            this.missingCatchInspectionTypeValidator(),
            this.missingUnloadedQuantityValidator()
        ]);
    }

    protected getValue(): InspectionCatchMeasureDTO[] {
        const bms = this.types.find(x => x.code === CatchSizeCodesEnum[CatchSizeCodesEnum.BMS])?.value;
        const lsc = this.types.find(x => x.code === CatchSizeCodesEnum[CatchSizeCodesEnum.LSC])?.value;

        return this.catches.map(x => new InspectionCatchMeasureDTO({
            id: x.id,
            action: x.action,
            fishId: x.fishId,
            catchCount: x.catchCount,
            catchQuantity: x.catchQuantity,
            catchZoneId: x.catchZoneId,
            fishSexId: x.fishSexId,
            originShip: x.originShip,
            storageLocation: x.storageLocation,
            catchInspectionTypeId: this.hasUndersizedCheck ? (x.undersized === true ? bms : lsc) : x.catchInspectionTypeId,
            allowedDeviation: this.hasAllowedDeviation ? x.allowedDeviation : undefined,
            averageSize: this.hasAverageSize ? x.averageSize : undefined,
            unloadedQuantity: this.hasUnloadedQuantity ? x.unloadedQuantity : undefined,
            shipLogBookPageId: x.shipLogBookPageId ?? undefined,
            turbotSizeGroupId: x.turbotSizeGroupId ?? undefined,
            hasMissingProperties: this.checkIfCatchHasMissingProperties(x)
        }));
    }

    private checkIfCatchHasMissingProperties(catchRecord: InspectionCatchMeasureDTO): boolean {
        if (this.hasUnloadedQuantity && (catchRecord.unloadedQuantity === undefined || catchRecord.unloadedQuantity === null)) {
            return true;
        }

        if (this.hasCatchType && (catchRecord.catchInspectionTypeId === undefined || catchRecord.catchInspectionTypeId === null)) {
            return true;
        }

        return false;
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

    private catchesValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.catches !== undefined && this.catches !== null) {
                const result = groupBy(this.catches, ((o: InspectedCatchTableModel) => ([o.fishId, o.catchInspectionTypeId, o.catchZoneId, o.turbotSizeGroupId])));

                if (result.find((f: any[]) => f.length > 1)) {
                    return { 'catchesMatch': true };
                }
            }

            return null;
        };
    }

    private missingCatchInspectionTypeValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.hasCatchType && this.catches !== undefined && this.catches !== null && this.catches.length > 0) {
                for (const catchRecord of this.catches) {
                    if (catchRecord.catchInspectionTypeId === undefined || catchRecord.catchInspectionTypeId === null) {
                        return { 'missingCatchInspectionType': true };
                    }
                }
            }

            return null;
        }
    }

    private missingUnloadedQuantityValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.hasUnloadedQuantity && this.catches !== undefined && this.catches !== null && this.catches.length > 0) {
                for (const catchRecord of this.catches) {
                    if (catchRecord.unloadedQuantity === undefined || catchRecord.unloadedQuantity === null) {
                        return { 'missingUnloadedQuantity': true };
                    }
                }
            }

            return null;
        }
    }

    private recalculateCatchQuantitySums(): void {
        const recordsGroupedByFish: Record<number, InspectedCatchTableModel[]> = CommonUtils.groupBy(this.catches, x => x.fishId!);
        this.catchQuantities.clear();

        for (const fishGroupId in recordsGroupedByFish) {
            const quantity: number = recordsGroupedByFish[fishGroupId].reduce((sum, current) => Number(sum) + (this.hasUnloadedQuantity ? Number(current.unloadedQuantity!) : Number(current.catchQuantity!)), 0);
            this.catchQuantities.set(Number(fishGroupId), quantity);
        }
    }
}
