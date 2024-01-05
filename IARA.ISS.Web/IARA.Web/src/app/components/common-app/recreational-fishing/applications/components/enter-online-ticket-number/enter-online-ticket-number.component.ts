import { Component } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IRecreationalFishingService } from '@app/interfaces/common-app/recreational-fishing.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { EnterOnlineTicketNumberParams } from '../../models/enter-online-ticket-number-params.model';

@Component({
    selector: 'enter-online-ticket-number',
    templateUrl: './enter-online-ticket-number.component.html'
})
export class EnterOnlineTicketNumberComponent implements IDialogComponent {
    public control: FormControl;

    public getControlErrorLabelText: GetControlErrorLabelTextCallback;

    private ticketId!: number;
    private service!: IRecreationalFishingService;

    private readonly translate: FuseTranslationLoaderService;

    public constructor(translate: FuseTranslationLoaderService) {
        this.translate = translate;

        this.control = new FormControl(undefined, [Validators.required, Validators.maxLength(50), TLValidators.number(1, undefined, 0)]);
        this.getControlErrorLabelText = this.getControlErrorLabelTextMethod.bind(this);
    }

    public setData(data: EnterOnlineTicketNumberParams, wrapperData: DialogWrapperData): void {
        this.ticketId = data.ticketId;
        this.service = data.service;
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.control.markAsTouched();

        if (this.control.valid) {
            const ticketNum: string = this.control.value;

            this.service.enterOnlineTicketOfflineNumber(this.ticketId, ticketNum).subscribe({
                next: (result: boolean) => {
                    if (result) {
                        dialogClose(ticketNum);
                    }
                    else {
                        this.control.setErrors({ alreadyInUse: true });
                        this.control.markAsTouched();
                    }
                }
            });
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public getControlErrorLabelTextMethod(controlName: string, error: unknown, errorCode: string): TLError | undefined {
        if (errorCode === 'alreadyInUse') {
            return new TLError({
                text: this.translate.getValue('recreational-fishing.online-ticket-number-already-in-use'),
                type: 'error'
            });
        }
        return undefined;
    }
}