import { Component, OnInit } from '@angular/core';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';

const SHOW_SPINNER_MS = 3000;

@Component({
    selector: 'wait-external-checks-to-finish',
    templateUrl: './wait-external-checks-to-finish.component.html'
})
export class WaitExternalChecksToFinishComponent implements OnInit, IDialogComponent {
    public message: string = '';
    public warnMessage: string = '';

    public showSpinner: boolean = true;

    private translate: FuseTranslationLoaderService;

    public constructor(translate: FuseTranslationLoaderService) {
        this.translate = translate;

        this.message = this.translate.getValue('applications-register.wait-external-checks-to-finish-dialog-message');
        this.warnMessage = this.translate.getValue('applications-register.wait-external-checks-to-finish-dialog-warn-admin-message');
    }

    public ngOnInit(): void {
        setTimeout(() => {
            this.showSpinner = false;
        }, SHOW_SPINNER_MS);
    }

    public setData(data: unknown, wrapperData: DialogWrapperData): void {
        // nothing for now
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

}