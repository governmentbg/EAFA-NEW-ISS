import { FuseTranslationLoaderService } from "@/@fuse/services/translation-loader.service";
import { Component, Inject } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { TLTranslatePipe } from '../../pipes/tl-translate.pipe';
import { IConfirmDialogData } from "./confirmation-dialog-data.interface";

@Component({
    selector: 'tl-confirmation-dialog',
    templateUrl: './confirmation-dialog.component.html',
    styleUrls: ['./confirmation-dialog.component.scss']
})
export class ConfirmationDialogComponent {
    public message: string;
    public title: string;
    public okBtnLabel: string;
    public cancelBtnLabel: string;
    private dialogRef: MatDialogRef<ConfirmationDialogComponent>;
    public translationService: FuseTranslationLoaderService;
    private tlTranslatePipe: TLTranslatePipe;

    constructor(@Inject(MAT_DIALOG_DATA) data: IConfirmDialogData,
        dialogRef: MatDialogRef<ConfirmationDialogComponent>,
        translationService: FuseTranslationLoaderService,
        tlTranslatePipe: TLTranslatePipe) {
        this.dialogRef = dialogRef;
        this.translationService = translationService;
        this.tlTranslatePipe = tlTranslatePipe;
        if (data != undefined) {
            if (data.message != undefined) {
                this.message = data.message;
            } else {
                this.message = this.translationService.getValue('common.do-you-confirm-the-action-message');
            }

            if (data.title != undefined) {
                this.title = data.title;
            } else {
                this.title = this.tlTranslatePipe.transform(this.translationService.getValue('common.confirmation'), 'cap');
            }

            if (data.okBtnLabel != undefined) {
                this.okBtnLabel = data.okBtnLabel;
            } else {
                this.okBtnLabel = this.tlTranslatePipe.transform(this.translationService.getValue('common.confirm'), 'cap');
            }

            if (data.cancelBtnLabel != undefined) {
                this.cancelBtnLabel = data.cancelBtnLabel;
            } else {
                this.cancelBtnLabel = this.tlTranslatePipe.transform(this.translationService.getValue('common.cancel'), 'cap');
            }
        } else {
            this.message = this.translationService.getValue('common.do-you-confirm-the-action-message');
            this.title = this.tlTranslatePipe.transform(this.translationService.getValue('common.confirmation'), 'cap');
            this.okBtnLabel = this.tlTranslatePipe.transform(this.translationService.getValue('common.confirm'), 'cap');
            this.cancelBtnLabel = this.tlTranslatePipe.transform(this.translationService.getValue('common.cancel'), 'cap');
        }
    }

    public cancel(): void {
        this.dialogRef.close(false);
    }

    public confirm(): void {
        this.dialogRef.close(true);
    }
}