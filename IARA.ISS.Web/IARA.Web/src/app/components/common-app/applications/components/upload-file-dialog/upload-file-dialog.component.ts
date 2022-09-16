import { Component } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { ApplicationStatusesEnum } from '@app/enums/application-statuses.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { PageCodeEnum } from '../../../../../enums/page-code.enum';
import { UploadFileDialogParams } from './models/upload-file-dialog-params.model';

@Component({
    selector: 'upload-file-dialog',
    templateUrl: './upload-file-dialog.component.html'
})
export class UploadFileDialogComponent implements IDialogComponent {
    public fileControl: FormControl;
    public service!: IApplicationsService;
    public applicationId!: number;

    public readonly pageCode: PageCodeEnum = PageCodeEnum.OnlineAppl;

    public constructor() {
        this.fileControl = new FormControl(null, Validators.required);
    }

    public setData(data: UploadFileDialogParams, buttons: DialogWrapperData): void {
        this.service = data.service;
        this.applicationId = data.applicationId;
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.fileControl.markAllAsTouched();

        if (this.fileControl.valid) {
            const fileToUpload = this.fileControl.value[0];
            this.service.uploadSignedApplication(this.applicationId, fileToUpload).subscribe((newStatus: ApplicationStatusesEnum) => {
                dialogClose();
            });
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }
}
