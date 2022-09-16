import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { ReasonData } from '@app/models/common/reason-data.model';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { ReasonDialogParams } from './models/reason-dialog-params.model';


@Component({
    selector: 'enter-reason',
    templateUrl: './enter-reason.component.html'
})
export class EnterReasonComponent implements IDialogComponent {

    public reasonControl: FormControl;

    private saveReasonServiceMethod?: (recordId: number, reason: string) => Observable<void>;
    private recordId!: number;

    public constructor() {
        this.reasonControl = new FormControl('', [Validators.required, Validators.maxLength(500)]);
    }

    public setData(data: unknown, buttons: DialogWrapperData): void {
        const dialogData = data as ReasonDialogParams;
        this.saveReasonServiceMethod = dialogData.saveReasonServiceMethod;
        this.recordId = dialogData.id;
        if (dialogData.reasonFieldValue !== undefined) {
            this.reasonControl.setValue(dialogData.reasonFieldValue);
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.reasonControl.markAllAsTouched();

        if (this.reasonControl.valid) {
            const reason: string = this.reasonControl.value;
            if (this.saveReasonServiceMethod !== undefined) {
                this.saveReasonServiceMethod(this.recordId, reason).subscribe(() => {
                    dialogClose(new ReasonData({ recordId: this.recordId, reason: reason }));
                });
            }
            else {
                dialogClose(new ReasonData({ recordId: this.recordId, reason: reason }));
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }


}
