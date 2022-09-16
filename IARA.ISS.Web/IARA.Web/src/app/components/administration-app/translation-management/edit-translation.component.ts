import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { TranslationManagementService } from '@app/services/administration-app/translation-management.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { TranslationManagementEditDTO } from '@app/models/generated/dtos/TranslationManagementEditDTO';
import { ITranslationManagementService } from '@app/interfaces/administration-app/translation-management.interface';
import { EditTranslationDialogParams } from './models/edit-translation-dialog-params.model';

@Component({
    selector: 'edit-translation-component',
    templateUrl: './edit-translation.component.html',
})
export class EditTranslationComponent implements IDialogComponent, OnInit {
    public form!: FormGroup;

    private viewMode: boolean = false;

    private id: number | undefined;
    private key: string | undefined;
    private model!: TranslationManagementEditDTO;

    private dialogData!: DialogWrapperData;

    private readonly service: ITranslationManagementService;

    public constructor(service: TranslationManagementService) {
        this.service = service;

        this.buildForm();
    }

    public ngOnInit(): void {
        if (this.id !== undefined && this.id !== null) {
            this.service.get(this.id).subscribe({
                next: (result: TranslationManagementEditDTO) => {
                    this.model = result;
                    this.fillForm();
                }
            });
        }
        else if (this.key !== undefined && this.key !== null) {
            this.service.getByKey(this.key).subscribe({
                next: (result: TranslationManagementEditDTO) => {
                    if (result !== undefined && result !== null) {
                        this.model = result;
                        this.id = this.model.id;

                        if (this.dialogData.dialogRef) {
                            this.dialogData.dialogRef.componentInstance.headerAuditBtn = {
                                id: this.model.id!,
                                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                                tableName: 'NTranslationResource'
                            };
                        }
                    }
                    else {
                        const parts: string[] = this.key!.split('.');

                        this.model = new TranslationManagementEditDTO({
                            code: parts[1],
                            groupCode: parts[0]
                        });
                    }

                    this.fillForm();
                }
            });
        }
        else {
            this.model = new TranslationManagementEditDTO();
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();

        if (this.form.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            if (this.id === null || this.id === undefined) {
                this.service.add(this.model).subscribe({
                    next: (id: number) => {
                        this.model.id = id;
                        dialogClose(this.model);
                    }
                });
            }
            else {
                this.service.edit(this.model).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    }
                });
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public setData(data: EditTranslationDialogParams, dialogData: DialogWrapperData): void {
        this.id = data.id;
        this.key = data.key;
        this.viewMode = data.viewMode;
        this.dialogData = dialogData;
    }

    public fillForm(): void {
        this.form.get('groupCodeControl')!.setValue(this.model.groupCode);
        this.form.get('resourceCodeControl')!.setValue(this.model.code);
        this.form.get('valueBgControl')!.setValue(this.model.valueBg);
        this.form.get('valueEnControl')!.setValue(this.model.valueEn);

        if (this.viewMode) {
            this.form.disable();
        }
    }

    public fillModel(): void {
        this.model.valueBg = this.form.get('valueBgControl')!.value;
        this.model.valueEn = this.form.get('valueEnControl')!.value;
    }

    private buildForm(): void {
        this.form = new FormGroup({
            groupCodeControl: new FormControl({ value: undefined, disabled: true }),
            resourceCodeControl: new FormControl({ value: undefined, disabled: true }),
            valueBgControl: new FormControl(undefined, [Validators.required, Validators.maxLength(4000)]),
            valueEnControl: new FormControl(undefined, [Validators.required, Validators.maxLength(4000)])
        });
    }
}