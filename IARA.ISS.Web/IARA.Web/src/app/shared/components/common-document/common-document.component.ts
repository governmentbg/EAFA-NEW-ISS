import { Component, OnInit, Optional, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { CommonDocumentDTO } from '@app/models/generated/dtos/CommonDocumentDTO';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { CommonDocumentDialogParams } from './models/common-document-dialog-params.model';
import { DateRangeData } from '../input-controls/tl-date-range/tl-date-range.component';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { DateRangeIndefiniteData } from '../date-range-indefinite/date-range-indefinite.component';

@Component({
    selector: 'common-document',
    templateUrl: './common-document.component.html'
})
export class CommonDocumentComponent extends CustomFormControl<CommonDocumentDTO> implements OnInit, IDialogComponent {
    public readonly today: Date = new Date();

    public isDialog: boolean = false;

    private id: number | undefined;
    private isActive: boolean = true;
    private model: CommonDocumentDTO | undefined;
    private viewMode: boolean = false;

    public constructor(@Optional() @Self() ngControl: NgControl) {
        super(ngControl);
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        if (this.model !== undefined && this.model !== null) {
            this.writeValue(this.model);
        }
    }

    public writeValue(value: CommonDocumentDTO): void {
        if (value !== null && value !== undefined) {
            this.id = value.id;
            this.isActive = value.isActive ?? false;

            this.form.get('numControl')!.setValue(value.num);
            this.form.get('issuerControl')!.setValue(value.issuer);
            this.form.get('issueDateControl')!.setValue(value.issueDate);
            this.form.get('commentsControl')!.setValue(value.comments);

            this.form.get('validityControl')!.setValue(new DateRangeIndefiniteData({
                range: new DateRangeData({ start: value.validFrom, end: value.validTo }),
                indefinite: value.isIndefinite ?? false
            }));
        }
    }

    public setData(data: CommonDocumentDialogParams, wrapperData: DialogWrapperData): void {
        this.isDialog = true;
        this.model = data.model;
        this.viewMode = data.viewMode;

        this.buildForm();

        if (this.viewMode) {
            this.form.disable();
        }
    }

    public saveBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.viewMode) {
            dialogClose(this.model);
        }
        else {
            this.form.markAllAsTouched();
            if (this.form.valid) {
                this.model = this.getValue();
                CommonUtils.sanitizeModelStrings(this.model);
                dialogClose(this.model);
            }
        }
    }

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            numControl: new FormControl(null, [Validators.required, Validators.maxLength(50)]),
            issuerControl: new FormControl(null, [Validators.required, Validators.maxLength(500)]),
            issueDateControl: new FormControl(null, Validators.required),
            validityControl: new FormControl(null, Validators.required),
            commentsControl: new FormControl(null, Validators.maxLength(4000))
        });
    }

    protected getValue(): CommonDocumentDTO {
        const result: CommonDocumentDTO = new CommonDocumentDTO({
            id: this.id,
            num: this.form.get('numControl')!.value,
            issuer: this.form.get('issuerControl')!.value,
            issueDate: this.form.get('issueDateControl')!.value,
            comments: this.form.get('commentsControl')!.value,
            isActive: this.isActive
        });

        const validity: DateRangeIndefiniteData | undefined = this.form.get('validityControl')!.value;
        if (validity !== undefined && validity !== null) {
            result.isIndefinite = validity.indefinite;
            result.validFrom = validity.range?.start;
            result.validTo = validity.range?.end;
        }
        else {
            result.isIndefinite = false;
        }

        return result;
    }
}