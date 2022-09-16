import { IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';

export class EditDialogInfo {
    public editDialog!: TLMatDialog<IDialogComponent>;
    public editDialogTCtor!: any;
    public viewTitle!: string;
    public viewRegisterTitle!: string;
    public viewRegixDataTitle!: string;
    public editRegixDataTitle!: string;
    public editApplicationTitle!: string;
    public viewApplicationDataAndConfrimRegularityTitle!: string;

    constructor(obj?: Partial<EditDialogInfo>) {
        Object.assign(this, obj);
    }
}