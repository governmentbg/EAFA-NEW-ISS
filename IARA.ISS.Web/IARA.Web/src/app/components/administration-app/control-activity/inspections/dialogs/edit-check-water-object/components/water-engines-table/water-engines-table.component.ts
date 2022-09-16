import { Component, OnInit, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, NgControl } from '@angular/forms';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { WaterEngineTableParams } from './models/water-engine-table-params';
import { WaterInspectionEngineDTO } from '@app/models/generated/dtos/WaterInspectionEngineDTO';
import { EditWaterEngineComponent } from '../edit-water-engine/edit-water-engine.component';

@Component({
    selector: 'water-engines-table',
    templateUrl: './water-engines-table.component.html'
})
export class WaterEnginesTableComponent extends CustomFormControl<WaterInspectionEngineDTO[]> implements OnInit {

    public engines: WaterInspectionEngineDTO[] = [];

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    private translate: FuseTranslationLoaderService;
    private confirmDialog: TLConfirmDialog;
    private editEntryDialog: TLMatDialog<EditWaterEngineComponent>;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editEntryDialog: TLMatDialog<EditWaterEngineComponent>
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

    public writeValue(value: WaterInspectionEngineDTO[]): void {
        if (value !== undefined && value !== null) {
            setTimeout(() => {
                this.engines = value;
            });
        }
        else {
            setTimeout(() => {
                this.engines = [];
            });
        }
    }

    public addEditEntry(fishingGear?: WaterInspectionEngineDTO, viewMode?: boolean): void {
        const readOnly: boolean = this.isDisabled || viewMode === true;

        let data: WaterEngineTableParams | undefined;
        let title: string;

        if (fishingGear !== undefined && fishingGear !== null) {
            data = new WaterEngineTableParams({
                model: fishingGear,
                readOnly: readOnly,
            });

            if (readOnly) {
                title = this.translate.getValue('inspections.view-water-engine-dialog-title');
            }
            else {
                title = this.translate.getValue('inspections.edit-water-engine-dialog-title');
            }
        }
        else {
            data = new WaterEngineTableParams({
                readOnly: readOnly
            });

            title = this.translate.getValue('inspections.add-water-engine-dialog-title');
        }

        const dialog = this.editEntryDialog.openWithTwoButtons({
            title: title,
            TCtor: EditWaterEngineComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => closeFn()
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: !readOnly,
            viewMode: readOnly
        }, '800px');

        dialog.subscribe((result: WaterInspectionEngineDTO) => {
            if (result !== undefined && result !== null) {
                if (fishingGear !== undefined) {
                    fishingGear = result;
                }
                else {
                    this.engines.push(result);
                }

                this.engines = this.engines.slice();
                this.onChanged(this.getValue());
                this.control.updateValueAndValidity();
            }
        });
    }

    public deleteEntry(patrolVehicle: GridRow<WaterInspectionEngineDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('inspections.water-engine-delete-dialog-title'),
            message: this.translate.getValue('inspections.water-engine-delete-message'),
            okBtnLabel: this.translate.getValue('inspections.water-engine-delete-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.datatable.softDelete(patrolVehicle);
                    this.engines.splice(this.engines.indexOf(patrolVehicle.data), 1);
                    this.onChanged(this.getValue());
                    this.control.updateValueAndValidity();
                }
            }
        });
    }

    protected buildForm(): AbstractControl {
        return new FormControl(null);
    }

    protected getValue(): WaterInspectionEngineDTO[] {
        return this.engines;
    }
}