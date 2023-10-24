import { Component } from '@angular/core';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { FormControl, Validators } from '@angular/forms';
import { FluxAcdrRequestEditDTO } from '@app/models/generated/dtos/FluxAcdrRequestEditDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { FluxVmsRequestsService } from '@app/services/administration-app/flux-vms-requests.service';

@Component({
    selector: 'edit-flux-acdr-request',
    templateUrl: './edit-flux-acdr-request.component.html'
})
export class EditFluxAcdrRequestsComponent implements IDialogComponent {
    public monthControl: FormControl;

    public previousMonth: Date;

    private model: FluxAcdrRequestEditDTO = new FluxAcdrRequestEditDTO();
    private readonly service: FluxVmsRequestsService;

    public constructor(service: FluxVmsRequestsService) {
        this.service = service;
        const today: Date = new Date();
        this.previousMonth = new Date(today.getFullYear(), today.getMonth() - 1, 1);

        this.monthControl = new FormControl(undefined, Validators.required);
    }

    public setData(data: unknown, wrapperData: DialogWrapperData): void {
        //nothing to do
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.monthControl.markAllAsTouched();

        if (this.monthControl.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            this.service.addAcdrQueryRequest(this.model).subscribe({
                next: () => {
                    dialogClose(true);
                },
                error: () => {
                    dialogClose(false);
                }
            });
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private fillModel(): void {
        const fromDate: Date = this.monthControl.value;

        this.model.fromDate = fromDate;
        this.model.toDate = new Date(fromDate.getFullYear(), fromDate.getMonth() + 1, 0);
    }
}