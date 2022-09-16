import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';

import { IAuanRegisterService } from '@app/interfaces/administration-app/auan-register.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { AuanRegisterService } from '@app/services/administration-app/auan-register.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { AuanRegisterEditDTO } from '@app/models/generated/dtos/AuanRegisterEditDTO';
import { EditAuanDialogParams } from '../models/edit-auan-dialog-params.model';
import { EditAuanComponent } from '../edit-auan/edit-auan.component';

@Component({
    selector: 'edit-auan-inspection-picker',
    templateUrl: './edit-auan-inspection-picker.component.html'
})
export class EditAuanInspectionPickerComponent implements OnInit, IDialogComponent {
    public inspectionReportControl: FormControl;

    public inspectionReports: NomenclatureDTO<number>[] = [];

    private service!: IAuanRegisterService;
    private translate: FuseTranslationLoaderService;
    private editDialog: TLMatDialog<EditAuanComponent>;

    public constructor(
        service: AuanRegisterService,
        translate: FuseTranslationLoaderService,
        editDialog: TLMatDialog<EditAuanComponent>
    ) {
        this.service = service;
        this.translate = translate;
        this.editDialog = editDialog;

        this.inspectionReportControl = new FormControl(null, Validators.required);
    }

    public ngOnInit(): void {
        this.service.getAllInspectionReports().subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.inspectionReports = result;
            }
        });
    }

    public setData(data: unknown, wrapperData: DialogWrapperData): void {
        // nothing to do
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.inspectionReportControl.markAllAsTouched();
        if (this.inspectionReportControl.valid) {
            this.openEditAuanDialog().subscribe({
                next: (auan: AuanRegisterEditDTO | undefined) => {
                    dialogClose(auan);
                }
            });
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private openEditAuanDialog(): Observable<AuanRegisterEditDTO | undefined> {
        const dialog = this.editDialog.openWithTwoButtons({
            title: this.translate.getValue('auan-register.add-auan-dialog-title'),
            TCtor: EditAuanComponent,
            headerAuditButton: undefined,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: new EditAuanDialogParams({
                inspectionId: this.inspectionReportControl.value!.value,
                isReadonly: false
            }),
            rightSideActionsCollection: [{
                id: 'print',
                color: 'accent',
                translateValue: 'auan-register.save-print'
            }],
            translteService: this.translate,
            disableDialogClose: true
        }, '1400px');

        return dialog;
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}