import { DialogParamsModel } from '@app/models/common/dialog-params.model';

export class EditLogBookPageExceptionParameters extends DialogParamsModel {
    public isCopy: boolean = false;

    public constructor(obj?: Partial<EditLogBookPageExceptionParameters>) {
        super(obj);
        Object.assign(this, obj);
    }
}