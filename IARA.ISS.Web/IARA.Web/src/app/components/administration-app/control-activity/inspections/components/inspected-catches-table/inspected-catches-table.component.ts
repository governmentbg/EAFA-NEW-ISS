import { Component, Input, OnInit, Self, ViewChild } from '@angular/core';
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
export class InspectedCatchesTableComponent extends CustomFormControl<InspectionCatchMeasureDTO[]> implements OnInit {
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
    public ships: NomenclatureDTO<number>[] = [];

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

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(TLMapViewerComponent)
    private mapViewer!: TLMapViewerComponent;

    @ViewChild(TLPopoverComponent)
    private mapPopover!: TLPopoverComponent;

    public mapOptions: MapOptions;

    public isMapPopoverOpened: boolean = false;

    public catches: InspectedCatchTableModel[] = [];

    private temporarySelectedGridSector: NomenclatureDTO<number> | undefined;

    private readonly translate: FuseTranslationLoaderService;
    private readonly editShipDialog: TLMatDialog<InspectedShipComponent>;

    public constructor(@Self() ngControl: NgControl,
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

    public writeValue(value: InspectionCatchMeasureDTO[]): void {
        if (value !== undefined && value !== null) {
            const catches: InspectedCatchTableModel[] = value.map(f => new InspectedCatchTableModel({
                action: f.action,
                allowedDeviation: f.allowedDeviation,
                averageSize: f.averageSize,
                catchCount: f.catchCount,
                catchInspectionTypeId: f.catchInspectionTypeId,
                catchQuantity: f.catchQuantity,
                catchZoneId: f.catchZoneId,
                fishId: f.fishId,
                fishSexId: f.fishSexId,
                id: f.id,
                originShip: f.originShip,
                storageLocation: f.storageLocation,
                unloadedQuantity: f.unloadedQuantity,
                fish: this.fishes.find(s => s.value === f.fishId),
                type: this.types.find(s => s.value === f.catchInspectionTypeId),
                catchZone: this.catchZones.find(s => s.value === f.catchZoneId),
                turbotSizeGroup: this.turbotSizeGroups.find(s => s.value === f.turbotSizeGroupId)
            }));

            setTimeout(() => {
                this.catches = catches;
            });
        } else {
            setTimeout(() => {
                this.catches = [];
            });
        }
    }

    public onEditRecord(row: GridRow<InspectedCatchTableModel>): void {
        if (row !== null && row !== undefined) {
            this.catchesFormGroup.get('catchZoneControl')!.setValue(row.data.catchZone);
        }
    }

    public catchRecordChanged(event: RecordChangedEventArgs<InspectedCatchTableModel>): void {
        event.Record.catchZoneId = this.catchesFormGroup.get('catchZoneControl')!.value?.value;
        event.Record.catchZone = this.catchesFormGroup.get('catchZoneControl')!.value;

        this.catches = this.datatable.rows;
        this.onChanged(this.getValue());
        this.control.updateValueAndValidity();

        this.catchesFormGroup.get('catchZoneControl')!.setValue(null);
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
            catchCountControl: new FormControl(undefined, [TLValidators.number(1)]),
            catchQuantityControl: new FormControl(undefined, [Validators.required, TLValidators.number(1)]),
            unloadedQuantityControl: new FormControl(undefined, [TLValidators.number(1)]),
            allowedDeviationControl: new FormControl(undefined, [TLValidators.number(0, 100)]),
            catchZoneControl: new FormControl(undefined),
            shipControl: new FormControl(undefined),
            turbotSizeGroupIdControl: new FormControl(undefined),
            fishSexIdControl: new FormControl(undefined),
        });

        return new FormControl(undefined, this.catchesValidator());
    }

    protected getValue(): InspectionCatchMeasureDTO[] {
        return this.catches.map(f => new InspectionCatchMeasureDTO({
            action: f.action,
            allowedDeviation: f.allowedDeviation,
            averageSize: f.averageSize,
            catchCount: f.catchCount,
            catchInspectionTypeId: f.catchInspectionTypeId,
            catchQuantity: f.catchQuantity,
            catchZoneId: f.catchZoneId,
            fishId: f.fishId,
            fishSexId: f.fishSexId,
            id: f.id,
            originShip: f.originShip,
            storageLocation: f.storageLocation,
            unloadedQuantity: f.unloadedQuantity,
            turbotSizeGroupId: f.turbotSizeGroupId,
        }));
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
        return (): ValidationErrors | null => {
            if (this.catches !== undefined && this.catches !== null) {
                const result = groupBy(this.catches, ((o: InspectedCatchTableModel) => ([o.fishId, o.catchInspectionTypeId, o.catchZoneId])));

                if (result.find((f: any[]) => f.length > 1)) {
                    return { 'catchesMatch': true };
                }
            }
            return null;
        };
    }
}
