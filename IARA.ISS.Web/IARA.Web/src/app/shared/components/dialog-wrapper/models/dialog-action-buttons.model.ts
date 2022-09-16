import { MatDialogRef } from '@angular/material/dialog';
import { DialogWrapperComponent } from '../dialog-wrapper.component';
import { IActionInfo } from '../interfaces/action-info.interface';
import { IDialogComponent } from '../interfaces/dialog-content.interface';

export class DialogWrapperData {
    public leftSideActions?: Array<IActionInfo>;
    public rightSideActions?: Array<IActionInfo>;
    public dialogRef?: MatDialogRef<DialogWrapperComponent<IDialogComponent>>;

    public constructor(obj?: Partial<DialogWrapperData>) {
        Object.assign(this, obj);
    }
}