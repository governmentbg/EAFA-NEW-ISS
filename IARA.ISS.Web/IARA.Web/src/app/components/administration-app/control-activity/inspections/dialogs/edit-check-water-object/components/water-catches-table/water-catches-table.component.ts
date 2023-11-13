import { Component, Input, OnInit, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, NgControl } from '@angular/forms';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { InspectionCatchMeasureDTO } from '@app/models/generated/dtos/InspectionCatchMeasureDTO';
import { WaterCatchTableParams } from './models/water-catch-table-params';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { EditWaterCatchComponent } from '../edit-water-catch/edit-water-catch.component';
import { InspectionsService } from '@app/services/administration-app/inspections.service';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';

@Component({
    selector: 'water-catches-table',
    templateUrl: './water-catches-table.component.html'
})
export class WaterCatchesTableComponent extends CustomFormControl<InspectionCatchMeasureDTO[]> implements OnInit {

    public catches: InspectionCatchMeasureDTO[] = [];

    @Input()
    public fishes: NomenclatureDTO<number>[] = [];

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    private confirmDialog: TLConfirmDialog;
    private editEntryDialog: TLMatDialog<EditWaterCatchComponent>;

    private readonly translate: FuseTranslationLoaderService;
    private readonly service: InspectionsService;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editEntryDialog: TLMatDialog<EditWaterCatchComponent>,
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

    public writeValue(value: InspectionCatchMeasureDTO[]): void {
        if (value !== undefined && value !== null) {
            setTimeout(() => {
                this.catches = value;
            });
        }
        else {
            setTimeout(() => {
                this.catches = [];
            });
        }
    }

    public addEditEntry(catchMeasure?: InspectionCatchMeasureDTO, viewMode?: boolean): void {
        const readOnly: boolean = this.isDisabled || viewMode === true;

        let data: WaterCatchTableParams | undefined;
        let title: string;
        let auditBtn: IHeaderAuditButton | undefined;

        if (catchMeasure !== undefined && catchMeasure !== null) {
            data = new WaterCatchTableParams({
                model: catchMeasure,
                readOnly: readOnly,
                fishes: this.fishes,
            });

            if (catchMeasure.id !== undefined && catchMeasure.id !== null) {
                auditBtn = {
                    id: catchMeasure.id,
                    getAuditRecordData: this.service.getInspectionCatchMeasureSimpleAudit.bind(this.service),
                    tableName: 'InspectionCatchMeasures'
                };
            }

            if (readOnly) {
                title = this.translate.getValue('inspections.view-water-catch-dialog-title');
            }
            else {
                title = this.translate.getValue('inspections.edit-water-catch-dialog-title');
            }
        }
        else {
            data = new WaterCatchTableParams({
                readOnly: readOnly,
                fishes: this.fishes,
            });

            title = this.translate.getValue('inspections.add-water-catch-dialog-title');
        }

        const dialog = this.editEntryDialog.openWithTwoButtons({
            title: title,
            TCtor: EditWaterCatchComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => closeFn()
            },
            headerAuditButton: auditBtn,
            componentData: data,
            translteService: this.translate,
            disableDialogClose: !readOnly,
            viewMode: readOnly
        }, '800px');

        dialog.subscribe((result: InspectionCatchMeasureDTO) => {
            if (result !== undefined && result !== null) {
                if (catchMeasure !== undefined) {
                    catchMeasure = result;
                }
                else {
                    this.catches.push(result);
                }

                this.catches = this.catches.slice();
                this.onChanged(this.getValue());
                this.control.updateValueAndValidity();
            }
        });
    }

    public deleteEntry(catchMeasure: GridRow<InspectionCatchMeasureDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('inspections.water-catch-delete-dialog-title'),
            message: this.translate.getValue('inspections.water-catch-delete-message'),
            okBtnLabel: this.translate.getValue('inspections.water-catch-delete-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.datatable.softDelete(catchMeasure);
                    this.catches.splice(this.catches.indexOf(catchMeasure.data), 1);
                    this.onChanged(this.getValue());
                    this.control.updateValueAndValidity();
                }
            }
        });
    }

    protected buildForm(): AbstractControl {
        return new FormControl(null);
    }

    protected getValue(): InspectionCatchMeasureDTO[] {
        return this.catches;
    }
}