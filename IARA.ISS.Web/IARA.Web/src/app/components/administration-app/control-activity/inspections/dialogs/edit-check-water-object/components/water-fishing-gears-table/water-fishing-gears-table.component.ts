import { Component, OnInit, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, NgControl } from '@angular/forms';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { InspectedFishingGearDTO } from '@app/models/generated/dtos/InspectedFishingGearDTO';
import { WaterFishingGearTableParams } from './models/water-fishing-gear-table-params';
import { WaterInspectionFishingGearDTO } from '@app/models/generated/dtos/WaterInspectionFishingGearDTO';
import { EditWaterFishingGearComponent } from '../edit-water-fishing-gear/edit-water-fishing-gear.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { InspectionsService } from '@app/services/administration-app/inspections.service';

@Component({
    selector: 'water-fishing-gears-table',
    templateUrl: './water-fishing-gears-table.component.html'
})
export class WaterFishingGearsTableComponent extends CustomFormControl<WaterInspectionFishingGearDTO[]> implements OnInit {

    public fishingGears: WaterInspectionFishingGearDTO[] = [];

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    private readonly translate: FuseTranslationLoaderService;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly editEntryDialog: TLMatDialog<EditWaterFishingGearComponent>;
    private readonly service: InspectionsService;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editEntryDialog: TLMatDialog<EditWaterFishingGearComponent>,
        service: InspectionsService
    ) {
        super(ngControl);

        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.editEntryDialog = editEntryDialog;
        this.service = service;

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.control.updateValueAndValidity();
            }
        });
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public writeValue(value: WaterInspectionFishingGearDTO[]): void {
        if (value !== undefined && value !== null) {
            setTimeout(() => {
                this.fishingGears = value;
                this.onChanged(this.getValue());
            });
        }
        else {
            setTimeout(() => {
                this.fishingGears = [];
                this.onChanged(this.getValue());
            });
        }
    }

    public addEditEntry(fishingGear?: WaterInspectionFishingGearDTO, viewMode?: boolean): void {
        const readOnly: boolean = this.isDisabled || viewMode === true;

        let data: WaterFishingGearTableParams | undefined;
        let title: string;
        let auditBtn: IHeaderAuditButton | undefined;

        if (fishingGear !== undefined && fishingGear !== null) {
            data = new WaterFishingGearTableParams({
                model: fishingGear,
                readOnly: readOnly,
            });

            if (fishingGear.inspectedFishingGearId !== undefined && fishingGear.inspectedFishingGearId !== null) {
                auditBtn = {
                    id: fishingGear.inspectedFishingGearId,
                    getAuditRecordData: this.service.getInspectedFishingGearSimpleAudit.bind(this.service),
                    tableName: 'RInsp.InspectedFishingGears'
                };
            }

            if (readOnly) {
                title = this.translate.getValue('inspections.view-water-fishing-gear-dialog-title');
            }
            else {
                title = this.translate.getValue('inspections.edit-water-fishing-gear-dialog-title');
            }
        }
        else {
            data = new WaterFishingGearTableParams({
                readOnly: readOnly
            });

            title = this.translate.getValue('inspections.add-water-fishing-gear-dialog-title');
        }

        const dialog = this.editEntryDialog.openWithTwoButtons({
            title: title,
            TCtor: EditWaterFishingGearComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => closeFn()
            },
            headerAuditButton: auditBtn,
            componentData: data,
            translteService: this.translate,
            disableDialogClose: !readOnly,
            viewMode: readOnly
        }, '800px');

        dialog.subscribe((result: WaterInspectionFishingGearDTO) => {
            if (result !== undefined && result !== null) {
                if (fishingGear !== undefined) {
                    const idx: number = this.fishingGears.findIndex(x => x === fishingGear);
                    this.fishingGears[idx] = result;
                }
                else {
                    this.fishingGears.push(result);
                }

                this.fishingGears = this.fishingGears.slice();
                this.onChanged(this.getValue());
                this.control.updateValueAndValidity();
            }
        });
    }

    public deleteEntry(fishingGear: GridRow<WaterInspectionFishingGearDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('inspections.water-fishing-gear-delete-dialog-title'),
            message: this.translate.getValue('inspections.water-fishing-gear-delete-message'),
            okBtnLabel: this.translate.getValue('inspections.water-fishing-gear-delete-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.datatable.softDelete(fishingGear);
                    this.fishingGears.splice(this.fishingGears.indexOf(fishingGear.data), 1);
                    this.onChanged(this.getValue());
                    this.control.updateValueAndValidity();
                }
            }
        });
    }

    protected buildForm(): AbstractControl {
        return new FormControl(null);
    }

    protected getValue(): InspectedFishingGearDTO[] {
        return this.fishingGears;
    }
}