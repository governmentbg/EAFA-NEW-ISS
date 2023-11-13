import { Component, Input, OnInit, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, NgControl, ValidationErrors, ValidatorFn } from '@angular/forms';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { VesselDuringInspectionDTO } from '@app/models/generated/dtos/VesselDuringInspectionDTO';
import { PatrolVehicleTableParams } from './models/patrol-vehicle-table-params';
import { EditPatrolVehicleComponent } from '../../dialogs/edit-patrol-vehicle/edit-patrol-vehicle.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { InspectionsService } from '@app/services/administration-app/inspections.service';

@Component({
    selector: 'patrol-vehicles-table',
    templateUrl: './patrol-vehicles-table.component.html'
})
export class PatrolVehiclesTableComponent extends CustomFormControl<VesselDuringInspectionDTO[]> implements OnInit {

    public patrolVehicles: VesselDuringInspectionDTO[] = [];

    @Input()
    public isWaterVehicle?: boolean;

    @Input()
    public hasCoordinates: boolean = true;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    private translate: FuseTranslationLoaderService;
    private confirmDialog: TLConfirmDialog;
    private editEntryDialog: TLMatDialog<EditPatrolVehicleComponent>;
    private readonly service: InspectionsService;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editEntryDialog: TLMatDialog<EditPatrolVehicleComponent>,
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

    public writeValue(value: VesselDuringInspectionDTO[]): void {
        if (value !== undefined && value !== null) {
            this.patrolVehicles = value;
        }
        else {
            setTimeout(() => {
                this.patrolVehicles = [];
            });
        }
    }

    public addEditEntry(patrolVehicle?: VesselDuringInspectionDTO, viewMode?: boolean): void {
        const readOnly: boolean = this.isDisabled || viewMode === true;

        let data: PatrolVehicleTableParams | undefined;
        let title: string;
        let auditBtn: IHeaderAuditButton | undefined;

        if (patrolVehicle !== undefined && patrolVehicle !== null) {
            data = new PatrolVehicleTableParams({
                model: patrolVehicle,
                isWaterVehicle: this.isWaterVehicle,
                readOnly: readOnly,
                isEdit: true,
                excludeIds: this.patrolVehicles.map(f => f.unregisteredVesselId!),
                hasCoordinates: this.hasCoordinates,
            });

            if (patrolVehicle.id !== undefined && patrolVehicle.id !== null) {
                auditBtn = {
                    id: patrolVehicle.id,
                    getAuditRecordData: this.service.getInspectionPatrolVehicleSimpleAudit.bind(this.service),
                    tableName: 'InspectionPatrolVehicles'
                };
            }

            if (readOnly) {
                title = this.translate.getValue('inspections.view-patrol-vehicle-dialog-title');
            }
            else {
                title = this.translate.getValue('inspections.edit-patrol-vehicle-dialog-title');
            }
        }
        else {
            data = new PatrolVehicleTableParams({
                isWaterVehicle: this.isWaterVehicle,
                isEdit: false,
                excludeIds: this.patrolVehicles.map(f => f.unregisteredVesselId!),
                hasCoordinates: this.hasCoordinates
            });

            title = this.translate.getValue('inspections.add-patrol-vehicle-dialog-title');
        }

        const dialog = this.editEntryDialog.openWithTwoButtons({
            title: title,
            TCtor: EditPatrolVehicleComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => closeFn()
            },
            headerAuditButton: auditBtn,
            componentData: data,
            translteService: this.translate,
            disableDialogClose: !readOnly,
            viewMode: readOnly
        }, '800px');

        dialog.subscribe((result: VesselDuringInspectionDTO) => {
            if (result !== undefined && result !== null) {
                if (patrolVehicle !== undefined) {
                    const idx: number = this.patrolVehicles.findIndex(x => x === patrolVehicle);
                    this.patrolVehicles[idx] = result;
                }
                else {
                    this.patrolVehicles.push(result);
                }

                this.patrolVehicles = this.patrolVehicles.slice();
                this.onChanged(this.getValue());
                this.control.updateValueAndValidity();
            }
        });
    }

    public deleteEntry(patrolVehicle: GridRow<VesselDuringInspectionDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('inspections.patrol-vehicle-table-delete-dialog-title'),
            message: this.translate.getValue('inspections.patrol-vehicle-table-delete-message'),
            okBtnLabel: this.translate.getValue('inspections.patrol-vehicle-table-delete-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.datatable.softDelete(patrolVehicle);
                    this.patrolVehicles.splice(this.patrolVehicles.indexOf(patrolVehicle.data), 1);
                    this.onChanged(this.getValue());
                    this.control.updateValueAndValidity();
                }
            }
        });
    }

    protected buildForm(): AbstractControl {
        return new FormControl(undefined, this.patrolVehiclesValidator());
    }

    protected getValue(): VesselDuringInspectionDTO[] {
        return this.patrolVehicles;
    }

    private patrolVehiclesValidator(): ValidatorFn {
        return (): ValidationErrors | null => {
            if (this.patrolVehicles !== undefined && this.patrolVehicles !== null) {
                if (this.patrolVehicles.length === 0) {
                    return { 'atLeastOnePatrolVehicleNeeded': true };
                }
            }
            return null;
        };
    }
}