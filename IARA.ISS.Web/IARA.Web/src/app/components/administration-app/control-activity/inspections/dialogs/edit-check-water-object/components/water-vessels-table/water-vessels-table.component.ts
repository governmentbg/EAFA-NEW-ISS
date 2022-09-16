import { Component, OnInit, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, NgControl } from '@angular/forms';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { WaterInspectionVesselDTO } from '@app/models/generated/dtos/WaterInspectionVesselDTO';
import { WaterVesselTableParams } from './models/water-vessel-table-params';
import { EditWaterVesselComponent } from '../edit-water-vessel/edit-water-vessel.component';

@Component({
    selector: 'water-vessels-table',
    templateUrl: './water-vessels-table.component.html'
})
export class WaterVesselsTableComponent extends CustomFormControl<WaterInspectionVesselDTO[]> implements OnInit {

    public vessels: WaterInspectionVesselDTO[] = [];

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    private translate: FuseTranslationLoaderService;
    private confirmDialog: TLConfirmDialog;
    private editEntryDialog: TLMatDialog<EditWaterVesselComponent>;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editEntryDialog: TLMatDialog<EditWaterVesselComponent>
    ) {
        super(ngControl);

        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.editEntryDialog = editEntryDialog;

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.control.updateValueAndValidity();
            }
        });
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public writeValue(value: WaterInspectionVesselDTO[]): void {
        if (value !== undefined && value !== null) {
            setTimeout(() => {
                this.vessels = value;
            });
        }
        else {
            setTimeout(() => {
                this.vessels = [];
            });
        }
    }

    public addEditEntry(fishingGear?: WaterInspectionVesselDTO, viewMode?: boolean): void {
        const readOnly: boolean = this.isDisabled || viewMode === true;

        let data: WaterVesselTableParams | undefined;
        let title: string;

        if (fishingGear !== undefined && fishingGear !== null) {
            data = new WaterVesselTableParams({
                model: fishingGear,
                readOnly: readOnly,
            });

            if (readOnly) {
                title = this.translate.getValue('inspections.view-water-vessel-dialog-title');
            }
            else {
                title = this.translate.getValue('inspections.edit-water-vessel-dialog-title');
            }
        }
        else {
            data = new WaterVesselTableParams({
                readOnly: readOnly
            });

            title = this.translate.getValue('inspections.add-water-vessel-dialog-title');
        }

        const dialog = this.editEntryDialog.openWithTwoButtons({
            title: title,
            TCtor: EditWaterVesselComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => closeFn()
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: !readOnly,
            viewMode: readOnly
        }, '800px');

        dialog.subscribe((result: WaterInspectionVesselDTO) => {
            if (result !== undefined && result !== null) {
                if (fishingGear !== undefined) {
                    fishingGear = result;
                }
                else {
                    this.vessels.push(result);
                }

                this.vessels = this.vessels.slice();
                this.onChanged(this.getValue());
                this.control.updateValueAndValidity();
            }
        });
    }

    public deleteEntry(patrolVehicle: GridRow<WaterInspectionVesselDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('inspections.water-vessel-delete-dialog-title'),
            message: this.translate.getValue('inspections.water-vessel-delete-message'),
            okBtnLabel: this.translate.getValue('inspections.water-vessel-delete-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.datatable.softDelete(patrolVehicle);
                    this.vessels.splice(this.vessels.indexOf(patrolVehicle.data), 1);
                    this.onChanged(this.getValue());
                    this.control.updateValueAndValidity();
                }
            }
        });
    }

    protected buildForm(): AbstractControl {
        return new FormControl(null);
    }

    protected getValue(): WaterInspectionVesselDTO[] {
        return this.vessels;
    }
}