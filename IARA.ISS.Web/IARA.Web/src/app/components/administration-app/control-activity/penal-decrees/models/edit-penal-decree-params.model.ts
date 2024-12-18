import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { AuanStatusEnum } from '@app/enums/auan-status.enum';

export class EditPenalDecreeDialogParams extends DialogParamsModel {
    public auanId: number | undefined;
    public typeId!: number;
    public status: AuanStatusEnum | undefined;

    public constructor(obj?: Partial<EditPenalDecreeDialogParams>) {
        super(obj);
        Object.assign(this, obj);
    }
}