import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { PointsTypeEnum } from '@app/enums/points-type.enum';

export class EditPenalPointsDialogParams extends DialogParamsModel {
    public penalDecreeId!: number;
    public type!: PointsTypeEnum;

    public constructor(obj?: Partial<EditPenalPointsDialogParams>) {
        super(obj);
        Object.assign(this, obj);
    }
}