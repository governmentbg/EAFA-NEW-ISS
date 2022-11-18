import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { DialogWrapperComponent } from './dialog-wrapper.component';
import { IDialogComponent } from './interfaces/dialog-content.interface';
import { IDialogData } from './interfaces/dialog-data.interface';

@Injectable({
    providedIn: 'root'
})
export class TLMatDialog<T extends IDialogComponent> {
    constructor(private dialog: MatDialog) { }

    public open(data: IDialogData<T>, dialogWidth: string = '1600px'): Observable<any> {
        const dialogRef = this.dialog.open<DialogWrapperComponent<T>, IDialogData<T>, any>(DialogWrapperComponent, {
            width: dialogWidth,
            data
        });

        return dialogRef.afterClosed();
    }

    public openWithTwoButtons(data: IDialogData<T>, dialogWidth: string = "1600px"): Observable<any> {
        if (data.saveBtn === null || data.saveBtn === undefined) {
            data.saveBtn = {
                id: 'save-button-id',
                translateValue: data.translteService.getValue('common.save'),
                color: 'accent'
            };
        }

        if (data.cancelBtn === null || data.cancelBtn === undefined) {
            data.cancelBtn = {
                id: 'cancel-button-id',
                translateValue: data.translteService.getValue('common.cancel'),
                color: 'primary'
            };
        }
        
        const dialogRef = this.dialog.open<DialogWrapperComponent<T>, IDialogData<T>, any>(DialogWrapperComponent, {
            width: dialogWidth,
            data
        });

        return dialogRef.afterClosed();
    }
}