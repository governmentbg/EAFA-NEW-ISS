import { Component, } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { Observable } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { JustifiedCancellationDialogParams, JustifiedCancellationMethod } from './models/justified-cancellation-dialog-params.model';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { LogBookPageCancellationReasonDTO } from '@app/models/generated/dtos/LogBookPageCancellationReasonDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';


@Component({
    selector: 'justified-cancellation',
    templateUrl: './justified-cancellation.component.html'
})
export class JustifiedCancellationComponent implements IDialogComponent {
    public reasonControl: FormControl;
    public reasonControlLabel: string;
    public tooltipResourceName: string;

    private model!: LogBookPageCancellationReasonDTO;
    private logBookType!: LogBookTypesEnum;
    private cancellationServiceMethod!: JustifiedCancellationMethod;

    private readonly translationService: FuseTranslationLoaderService;

    public constructor(
        translate: FuseTranslationLoaderService
    ) {
        this.translationService = translate;

        this.reasonControl = new FormControl('', [Validators.required, Validators.maxLength(4000)]);
        this.reasonControlLabel = this.translationService.getValue('catches-and-sales.justified-cancellation-reason-default-label');
        this.tooltipResourceName = this.translationService.getValue('catches-and-sales.justified-cancellation-reason-default-helper')
    }

    public setData(data: JustifiedCancellationDialogParams, wrapperData: DialogWrapperData): void {
        this.cancellationServiceMethod = data.cancellationServiceMethod;
        this.logBookType = data.logBookType;

        if (!CommonUtils.isNullOrEmpty(data.reasonControlLabel)) {
            this.reasonControlLabel = data.reasonControlLabel!;
        }

        if (!CommonUtils.isNullOrEmpty(data.reasonControlTooltipResouce)) {
            this.tooltipResourceName = data.reasonControlTooltipResouce!;
        }

        this.model = data.model ?? new LogBookPageCancellationReasonDTO();
        this.reasonControl.setValue(this.model.reason);
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.reasonControl.markAsTouched();

        if (this.reasonControl.valid) {
            this.model.reason = this.reasonControl.value!;

            this.cancellationServiceMethod(this.model, this.logBookType).subscribe({
                next: () => {
                    dialogClose(this.model);
                }
            });
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }
}