import { Component } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';

import { IRecreationalFishingService } from '@app/interfaces/common-app/recreational-fishing.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { EnterOnlineTicketNumberParams } from '../../models/enter-online-ticket-number-params.model';

@Component({
    selector: 'enter-online-ticket-number',
    templateUrl: './enter-online-ticket-number.component.html'
})
export class EnterOnlineTicketNumberComponent implements IDialogComponent {
    public control: FormControl;

    private ticketId!: number;
    private service!: IRecreationalFishingService;

    public constructor() {
        this.control = new FormControl(undefined, [Validators.required, Validators.maxLength(50), TLValidators.number(1, undefined, 0)]);
    }

    public setData(data: EnterOnlineTicketNumberParams, wrapperData: DialogWrapperData): void {
        this.ticketId = data.ticketId;
        this.service = data.service;
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.saveTicket(false, dialogClose);
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (actionInfo.id === 'save-and-print') {
            this.saveTicket(true, dialogClose);
        }
        else {
            dialogClose();
        }
    }

    private saveTicket(print: boolean = false, dialogClose: DialogCloseCallback): void {
        this.control.markAsTouched();

        if (this.control.valid) {
            const ticketNum: string = this.control.value;

            this.service.enterOnlineTicketOfflineNumber(this.ticketId, ticketNum).subscribe({
                next: () => {
                    if (print) {
                        this.service.downloadFishingTicket(this.ticketId).subscribe({
                            next: () => {
                                dialogClose(ticketNum);
                            }
                        });
                    }
                    else {
                        dialogClose(ticketNum);
                    }
                }
            });
        }
    }
}