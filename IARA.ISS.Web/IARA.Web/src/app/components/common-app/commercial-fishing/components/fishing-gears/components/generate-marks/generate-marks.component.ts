import { Component } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { RangeInputData } from '@app/shared/components/input-controls/tl-range-input/range-input.component';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { MarksRangeData } from '../../models/marks-range.model';

const MARK_NUMBERS_RANGE_WARNING_DIFFERENCE = 100;

@Component({
    selector: 'generate-marks',
    templateUrl: './generate-marks.component.html'
})
export class GenerateMarksComponent implements IDialogComponent {
    public markNumbersRangeControl: FormControl;
    public readonly markNumbersRangeMinValue = 0;

    private readonly translateService: FuseTranslationLoaderService;
    private readonly confirmationDialog: TLConfirmDialog;

    public constructor(translateService: FuseTranslationLoaderService, confirmationDialog: TLConfirmDialog) {
        this.translateService = translateService;
        this.confirmationDialog = confirmationDialog;

        this.markNumbersRangeControl = new FormControl(undefined, Validators.required);
    }

    public setData(data: unknown, wrapperData: DialogWrapperData): void {
        //
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.markNumbersRangeControl.markAllAsTouched();

        if (this.markNumbersRangeControl.valid) {
            const marksRange: RangeInputData | undefined = this.markNumbersRangeControl.value;

            if (marksRange !== null
                && marksRange !== undefined
                && marksRange.start !== null
                && marksRange.start !== undefined
                && marksRange.end !== null
                && marksRange.end !== undefined
            ) {
                const difference: number = marksRange.end - marksRange.start;

                if (difference > MARK_NUMBERS_RANGE_WARNING_DIFFERENCE) {
                    const message: string = this.translateService.getValue('fishing-gears.generate-marks-from-range-confirm-message');
                    this.confirmationDialog.open({
                        okBtnLabel: this.translateService.getValue('fishing-gears.generate'),
                        title: this.translateService.getValue('fishing-gears.generate-marks-from-range-confirm-dialog-title'),
                        message: `${message}: ${difference}`
                    }).subscribe({
                        next: (ok: boolean) => {
                            if (ok) {
                                this.markNumbersRangeControl.reset();
                                dialogClose(new MarksRangeData(marksRange.start!, marksRange.end!))
                            }
                        }
                    });
                }
                else {
                    this.markNumbersRangeControl.reset();
                    dialogClose(new MarksRangeData(marksRange.start!, marksRange.end!))
                }
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }
}