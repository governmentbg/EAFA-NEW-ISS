import { DialogParamsModel } from '@app/models/common/dialog-params.model';

export class ChooseLawSectionDialogParams extends DialogParamsModel {
    public isAdding: boolean = false;

    public constructor(obj?: Partial<ChooseLawSectionDialogParams>) {
        super(obj);
        Object.assign(this, obj);
    }
}