import { PenalPointsAppealDTO } from '@app/models/generated/dtos/PenalPointsAppealDTO';
import { IPenalPointsService } from '@app/interfaces/administration-app/penal-points.interface';

export class EditPenalPointsComplaintDialogParams {
    public model: PenalPointsAppealDTO | undefined;
    public viewMode: boolean = false;
    public service!: IPenalPointsService;

    public constructor(params?: Partial<EditPenalPointsComplaintDialogParams>) {
        Object.assign(this, params);
    }
}