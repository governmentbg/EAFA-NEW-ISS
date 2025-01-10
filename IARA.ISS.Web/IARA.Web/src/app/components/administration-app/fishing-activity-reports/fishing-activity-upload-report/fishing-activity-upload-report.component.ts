import { Component } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { FileInfoDTO } from '@app/models/generated/dtos/FileInfoDTO';
import { FishingActivityReportsService } from '@app/services/administration-app/fishing-activity-reports.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';

@Component({
    selector: 'fishing-activity-upload-report',
    templateUrl: './fishing-activity-upload-report.component.html'
})
export class FishingActivityUploadReportComponent implements IDialogComponent {
    public readonly control: FormControl;

    private reportId!: number;
    private service: FishingActivityReportsService;

    public constructor(service: FishingActivityReportsService) {
        this.service = service;
        this.control = new FormControl(undefined, Validators.required);
    }

    public setData(data: DialogParamsModel, wrapperData: DialogWrapperData): void {
        this.reportId = data.id;
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.control.markAllAsTouched();

        if (this.control.valid) {
            const file: FileInfoDTO = this.control.value;

            this.service.uploadFishingActivityReport(this.reportId, file).subscribe({
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