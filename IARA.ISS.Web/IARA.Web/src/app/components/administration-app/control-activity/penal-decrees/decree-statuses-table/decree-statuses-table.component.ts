import { Component, Input, OnInit, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, NgControl } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IPenalDecreesService } from '@app/interfaces/administration-app/penal-decrees.interface';
import { PenalDecreeStatusDTO } from '@app/models/generated/dtos/PenalDecreeStatusDTO';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { EditPenalDecreeStatusComponent } from '../edit-penal-decree-status/edit-penal-decree-status.component';
import { EditPenalDecreeStatusDialogParams } from '../models/edit-penal-decree-status-params.model';
import { PenalDecreeTypeEnum } from '@app/enums/penal-decree-type.enum';

@Component({
    selector: 'decree-statuses-table',
    templateUrl: './decree-statuses-table.component.html'
})
export class DecreeStatusesTableComponent extends CustomFormControl<PenalDecreeStatusDTO[]> implements OnInit {
    @Input()
    public viewMode: boolean = false;

    @Input()
    public service!: IPenalDecreesService;

    @Input()
    public decreeType!: PenalDecreeTypeEnum;

    public statuses: PenalDecreeStatusDTO[] = [];
    public isDisabled: boolean = false;

    @ViewChild('decreeStatusesTable')
    private decreeStatusesTable!: TLDataTableComponent;

    private readonly translate: FuseTranslationLoaderService;
    private editStatusDialog: TLMatDialog<EditPenalDecreeStatusComponent>;

    private confirmDialog!: TLConfirmDialog;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editStatusDialog: TLMatDialog<EditPenalDecreeStatusComponent>
    ) {
        super(ngControl, false);

        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.editStatusDialog = editStatusDialog;
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public writeValue(value: PenalDecreeStatusDTO[]): void {
        if (value !== undefined && value !== null) {
            this.statuses = value;
        }
    }

    public addEditStatus(status: PenalDecreeStatusDTO | undefined, viewMode: boolean = false): void {
        const readOnly: boolean = this.isDisabled || viewMode === true;

        let data: EditPenalDecreeStatusDialogParams | undefined;
        let auditBtn: IHeaderAuditButton | undefined;
        let title: string;

        if (status !== undefined) {
            data = new EditPenalDecreeStatusDialogParams(this.service, status, this.decreeType, this.viewMode || viewMode);

            if (status.id !== undefined) {
                auditBtn = {
                    id: status.id,
                    getAuditRecordData: this.service.getPenalDecreeStatusAudit.bind(this.service),
                    tableName: 'PenalDecreeStatus'
                }
            }

            if (this.decreeType === PenalDecreeTypeEnum.PenalDecree) {
                if (readOnly) {
                    title = this.translate.getValue('penal-decrees.view-penal-decree-status-dialog-title');
                }
                else {
                    title = this.translate.getValue('penal-decrees.edit-penal-decree-status-dialog-title');

                }
            }

            else if (this.decreeType === PenalDecreeTypeEnum.Agreement) {
                if (readOnly) {
                    title = this.translate.getValue('penal-decrees.view-agreement-status-dialog-title');
                }
                else {
                    title = this.translate.getValue('penal-decrees.edit-agreement-status-dialog-title');

                }
            }

            else {
                if (readOnly) {
                    title = this.translate.getValue('penal-decrees.view-warning-status-dialog-title');
                }
                else {
                    title = this.translate.getValue('penal-decrees.edit-warning-status-dialog-title');

                }
            }
        }
        else {
            data = new EditPenalDecreeStatusDialogParams(this.service, undefined, this.decreeType, false);

            if (this.decreeType === PenalDecreeTypeEnum.PenalDecree) {
                title = this.translate.getValue('penal-decrees.add-penal-decree-status-dialog-title');
            }

            else if (this.decreeType === PenalDecreeTypeEnum.Agreement) {
                title = this.translate.getValue('penal-decrees.add-agreement-status-dialog-title');
            }

            else  {
                title = this.translate.getValue('penal-decrees.add-warning-status-dialog-title');
            }
        }

        const dialog = this.editStatusDialog.openWithTwoButtons({
            title: title,
            TCtor: EditPenalDecreeStatusComponent,
            headerAuditButton: auditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditStatusDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            viewMode: viewMode
        }, '1200px');

        dialog.subscribe({
            next: (result: PenalDecreeStatusDTO | undefined) => {
                if (result !== undefined) {
                    //editing
                    if (status !== undefined) {
                        status = result;
                    }
                    //adding
                    else {
                        this.statuses.push(result);
                    }

                    this.statuses = this.statuses.slice();
                    this.onChanged(this.statuses);
                }
            }
        });
    }

    public deleteStatus(status: GridRow<PenalDecreeStatusDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('penal-decrees.delete-status-dialog-title'),
            message: this.translate.getValue('penal-decrees.delete-status-dialog-message'),
            okBtnLabel: this.translate.getValue('penal-decrees.delete-status-dialog-ok-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.decreeStatusesTable.softDelete(status);
                    this.onChanged(this.statuses);
                }
            }
        })
    }

    public undoDeleteStatus(status: GridRow<PenalDecreeStatusDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.decreeStatusesTable.softUndoDelete(status);
                    this.onChanged(this.statuses);
                }
            }
        })
    }

    private closeEditStatusDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    protected getValue(): PenalDecreeStatusDTO[] {
        return this.statuses;
    }

    protected buildForm(): AbstractControl {
        return new FormControl();
    }
}