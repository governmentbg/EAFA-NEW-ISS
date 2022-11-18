import { DialogParamsModel } from '@app/models/common/dialog-params.model';

export class EditPenalDecreeDialogParams extends DialogParamsModel {
    public auanId: number | undefined;
    public typeId!: number;

    public constructor(obj?: Partial<EditPenalDecreeDialogParams>) {
        super(obj);
        Object.assign(this, obj);
    }
}