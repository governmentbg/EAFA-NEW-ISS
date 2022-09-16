import { IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { Observable } from 'rxjs';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { ApplicationRegisterDTO } from '../generated/dtos/ApplicationRegisterDTO';

export class ApplicationsRegisterData<T extends IDialogComponent> {
    public editDialog!: TLMatDialog<T>;
    public editDialogTCtor!: new (...agrs: any[]) => T;
    public createRegisterCallback?: (application: ApplicationRegisterDTO) => Observable<any> | void;

    public editApplicationDialogTitle!: string;
    public editRegixDataDialogTitle!: string;
    public addRegisterDialogTitle!: string;
    public viewApplicationDialogTitle!: string;
    public viewRegixDataDialogTitle!: string;
    public viewRegisterDialogTitle!: string;
    public viewAndConfrimDataRegularityTitle!: string;

    public customRightSideActionButtons: IActionInfo[] | undefined;
    public customLeftSideActionButtons: IActionInfo[] | undefined;

    public constructor(obj?: Partial<ApplicationsRegisterData<T>>) {
        Object.assign(this, obj);
    }
}