import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FuseTranslationLoaderService } from '../../services/translation-loader.service';

@Component({
    selector: 'fuse-confirm-dialog',
    templateUrl: './confirm-dialog.component.html',
    styleUrls: ['./confirm-dialog.component.scss']
})
export class FuseConfirmDialogComponent implements OnInit {
    public confirmMessage: string = this._translationLoader.getValue('common.do-you-confirm-the-action-message');

    /**
     * Constructor
     *
     * @param {MatDialogRef<FuseConfirmDialogComponent>} dialogRef
     */
    constructor(public dialogRef: MatDialogRef<FuseConfirmDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public message: string,
        private _translationLoader: FuseTranslationLoaderService) {
    }

    public ngOnInit(): void {
        if (this.message !== null && this.message !== undefined && this.message.length > 0) {
            this.confirmMessage = this.message;
        }
    }
}