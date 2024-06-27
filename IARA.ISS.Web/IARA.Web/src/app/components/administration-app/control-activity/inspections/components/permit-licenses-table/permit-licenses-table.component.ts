import { Component, Input, OnInit, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormGroup, NgControl } from '@angular/forms';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { EditInspectorComponent } from '../../dialogs/edit-inspector/edit-inspector.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { PermitLicensesTableParams } from './models/permit-licenses-table.params';
import { PermitLicenseTableModel } from '../../models/permit-license-table.model';

@Component({
    selector: 'permit-licenses-table',
    templateUrl: './permit-licenses-table.component.html'
})
export class PermitLicensesTableComponent extends CustomFormControl<PermitLicenseTableModel[]> implements OnInit {

    public permitLicenses: PermitLicenseTableModel[] = [];

    @Input()
    public isEdit: boolean = false;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    private readonly translate: FuseTranslationLoaderService;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly editEntryDialog: TLMatDialog<EditInspectorComponent>;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editEntryDialog: TLMatDialog<EditInspectorComponent>
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

    public async ngOnInit(): Promise<void> {
        this.initCustomFormControl();
    }

    public writeValue(value: PermitLicenseTableModel[]): void {
        if (value !== undefined && value !== null) {
            setTimeout(() => {
                this.permitLicenses = value;
                this.onChanged(this.getValue());
            });
        }
        else {
            setTimeout(() => {
                this.permitLicenses = [];
                this.onChanged(this.getValue());
            });
        }
    }

    public addEditEntry(permitLicense?: PermitLicenseTableModel, viewMode?: boolean): void {
        const readOnly: boolean = this.isDisabled || viewMode === true;

        let data: PermitLicensesTableParams | undefined;
        let title: string;

        if (permitLicense !== undefined && permitLicense !== null) {
            data = new PermitLicensesTableParams({
                model: permitLicense,
                readOnly: readOnly,
                isEdit: true,
            });

            if (readOnly) {
                title = this.translate.getValue('inspections.view-permit-license-dialog-title');
            }
            else {
                title = this.translate.getValue('inspections.edit-permit-license-dialog-title');
            }
        }
        else {
            data = new PermitLicensesTableParams();

            title = this.translate.getValue('inspections.add-permit-license-dialog-title');
        }

        const dialog = this.editEntryDialog.openWithTwoButtons({
            title: title,
            TCtor: EditInspectorComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => closeFn()
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: !readOnly,
            viewMode: readOnly
        }, '800px');

        dialog.subscribe((result: PermitLicenseTableModel) => {
            if (result !== undefined && result !== null) {
                if (permitLicense !== undefined) {
                    permitLicense = result;
                }
                else {
                    this.permitLicenses.push(result);
                }

                this.permitLicenses = this.permitLicenses.slice();
                this.onChanged(this.getValue());
            }
        });
    }

    public deleteEntry(permitLicense: GridRow<PermitLicenseTableModel>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('inspections.permit-license-table-delete-dialog-title'),
            message: this.translate.getValue('inspections.permit-license-table-delete-message'),
            okBtnLabel: this.translate.getValue('inspections.permit-license-table-delete-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.datatable.softDelete(permitLicense);
                    this.permitLicenses.splice(this.permitLicenses.indexOf(permitLicense.data), 1);
                    this.onChanged(this.getValue());
                }
            }
        });
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({});
    }

    protected getValue(): PermitLicenseTableModel[] {
        return this.permitLicenses;
    }
}