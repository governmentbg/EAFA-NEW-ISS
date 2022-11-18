import { DialogParamsModel } from '@app/models/common/dialog-params.model';

export class EditAuanDialogParams extends DialogParamsModel {
    public inspectionId!: number;
    public isThirdParty: boolean = false;

    public constructor(obj?: Partial<EditAuanDialogParams>) {
        super(obj);
        Object.assign(this, obj);
    }
}