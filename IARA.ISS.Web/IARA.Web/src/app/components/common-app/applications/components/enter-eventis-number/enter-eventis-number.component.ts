import { Component } from '@angular/core';
import { Form, FormControl, FormGroup, Validators } from '@angular/forms';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ApplicationsAdministrationService } from '@app/services/administration-app/applications-administration.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';


@Component({
    selector: 'enter-eventis-number',
    templateUrl: './enter-eventis-number.component.html'
})
export class EnterEventisNumberComponent implements IDialogComponent {

    public eventisFormGroup: FormGroup;

    private service: ApplicationsAdministrationService;
    private applicationId?: number;

    public constructor(service: ApplicationsAdministrationService) {
        this.service = service;
        this.eventisFormGroup = new FormGroup({
            eventisNumber: new FormControl('', Validators.required)
        });
    }

    public setData(data: unknown, buttons: DialogWrapperData): void {
        const params = (data as DialogParamsModel);
        this.applicationId = params.id;
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.eventisFormGroup.markAllAsTouched();

        if (this.eventisFormGroup.valid) {
            const eventisNumber: string = this.eventisFormGroup.controls.eventisNumber.value;
            this.service.enterEventisNumber(this.applicationId!, eventisNumber).subscribe(() => {
                dialogClose();
            });
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }


}
