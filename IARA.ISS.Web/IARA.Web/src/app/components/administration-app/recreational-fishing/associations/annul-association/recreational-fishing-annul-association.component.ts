import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { AssociationAnnulmentResult } from '../models/association-annulment-result.model';

@Component({
    selector: 'recreational-fishing-annul-association',
    templateUrl: './recreational-fishing-annul-association.component.html'
})
export class RecreationalFishingAnnulAssociationComponent implements IDialogComponent {
    public form: FormGroup;

    private annuling!: boolean;

    public constructor() {
        this.form = new FormGroup({
            annulmentDateControl: new FormControl(new Date(), Validators.required),
            annulmentReasonControl: new FormControl('', [Validators.maxLength(4000), Validators.required])
        });
    }

    public saveBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        if (this.form.valid || this.form.disabled) {
            dialogClose(this.buildResult());
        }
    }

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public setData(data: AssociationAnnulmentResult, buttons: DialogWrapperData): void {
        this.annuling = !data.canceled;

        if (!this.annuling) {
            this.form.get('annulmentDateControl')!.setValue(data.date);
            this.form.get('annulmentReasonControl')!.setValue(data.reason);

            this.form.disable();
        }
    }

    private buildResult(): AssociationAnnulmentResult {
        return new AssociationAnnulmentResult(
            this.annuling,
            this.form.get('annulmentDateControl')!.value,
            this.form.get('annulmentReasonControl')!.value
        );
    }
}
