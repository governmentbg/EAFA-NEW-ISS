import { Component } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { FileInfoDTO } from '@app/models/generated/dtos/FileInfoDTO';
import { FluxVmsRequestsService } from '@app/services/administration-app/flux-vms-requests.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';

@Component({
    selector: 'upload-flux-acdr-request',
    templateUrl: './upload-flux-acdr-request.component.html'
})
export class UploadFluxAcdrRequestsComponent implements IDialogComponent {
    public fileControl: FormControl;

    private requestId!: number;
    private readonly service: FluxVmsRequestsService;

    public constructor(service: FluxVmsRequestsService) {
        this.service = service;
        this.fileControl = new FormControl(null, Validators.required);
    }

    public setData(data: DialogParamsModel, wrapperData: DialogWrapperData): void {
        this.requestId = data.id;
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.fileControl.markAllAsTouched();

        if (this.fileControl.valid) {
            const fileToUpload: FileInfoDTO = this.fileControl.value;

            this.service.importAcdrQueryRequest(this.requestId, fileToUpload).subscribe({
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
}