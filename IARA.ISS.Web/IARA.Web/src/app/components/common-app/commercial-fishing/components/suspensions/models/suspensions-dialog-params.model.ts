import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ISuspensionService } from '@app/interfaces/common-app/suspension.interface';

export class SuspensionsDialogParams {
    public isPermitLicense!: boolean;
    public recordId!: number;
    public pageCode!: PageCodeEnum;
    public viewMode!: boolean;
    public service!: ISuspensionService;
    public postOnAdd!: boolean;

    public constructor(obj?: Partial<SuspensionsDialogParams>) {
        Object.assign(this, obj);
    }
}