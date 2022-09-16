import { Injectable } from '@angular/core';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { SpinnerComponent } from './spinner.component';

@Injectable({
    providedIn: 'root'
})
export class SpinnerService {
    private static readonly SPINNER_DELAY_MS = 500;
    private dialogRef?: MatDialogRef<SpinnerComponent> = undefined;
    private isSpinnerDisplayed: boolean = false;
    private spinnerCount: number = 0;

    constructor(private dialog: MatDialog) { }

    public show(): void {
        if (!this.isSpinnerDisplayed) {
            this.isSpinnerDisplayed = true;
            setTimeout(() => {
                if (this.dialogRef === undefined && this.spinnerCount > 0) {
                    this.dialogRef = this.dialog.open(SpinnerComponent, {
                        disableClose: true,
                        panelClass: 'spinner-panel'
                    });
                }
            }, SpinnerService.SPINNER_DELAY_MS);
        }
        ++this.spinnerCount;
    }

    public hide(): void {
        if (this.spinnerCount > 0) {
            --this.spinnerCount;

            if (this.spinnerCount === 0) {
                this.isSpinnerDisplayed = false;

                if (this.dialogRef !== undefined) {
                    this.dialogRef.close();
                    this.dialogRef = undefined;
                }
            }
        }
    }
}