import { Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { Environment } from '../../../../../environments/environment';
import { RequestProperties } from '../../../services/request-properties';
import { IActionInfo } from '../../dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '../../dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '../../dialog-wrapper/models/dialog-action-buttons.model';
import { EGovOfflinePaymentDataDialogParams } from './models/egov-offline-payment-data-dialog-params.model';

@Component({
    selector: 'egov-offline-payment-data',
    templateUrl: './egov-offline-payment-data.component.html'
})
export class EGovOfflinePaymenDataComponent implements IDialogComponent {
    public paymentDataFormGroup: FormGroup;
    public egovPaymentHref: string;

    private buttons!: DialogWrapperData;
    private snackbar: MatSnackBar;
    private translate: FuseTranslationLoaderService;

    public constructor(snackbar: MatSnackBar, translate: FuseTranslationLoaderService) {
        this.snackbar = snackbar;
        this.translate = translate;

        this.paymentDataFormGroup = new FormGroup({
            referenceNumber: new FormControl('')
        });

        this.egovPaymentHref = Environment.Instance.egovPaymentHref;
    }

    public setData(data: EGovOfflinePaymentDataDialogParams, buttons: DialogWrapperData): void {
        this.buttons = buttons;
        this.paymentDataFormGroup.controls.referenceNumber.setValue(data.referenceNumber);

        if (this.buttons.dialogRef?.componentInstance) {
            this.buttons.dialogRef.componentInstance.viewMode = true;
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose(this.egovPaymentHref);
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public copyToClipboard(): string {
        return this.paymentDataFormGroup.controls.referenceNumber.value;
    }

    public codeCopied(copied: boolean): void {
        if (copied) {
            if (this.buttons.dialogRef?.componentInstance) {
                this.buttons.dialogRef.componentInstance.viewMode = false;
            }

            this.snackbar.open(
                this.translate.getValue('online-payment-data.code-copied-successfully'),
                undefined,
                {
                    duration: RequestProperties.DEFAULT.showExceptionDurationSucc,
                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassSucc
                }
            );
        }
        else {
            this.snackbar.open(
                this.translate.getValue('online-payment-data.code-copy-failed'),
                undefined,
                {
                    duration: RequestProperties.DEFAULT.showExceptionDurationSucc,
                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassSucc
                }
            );
        }
    }
}