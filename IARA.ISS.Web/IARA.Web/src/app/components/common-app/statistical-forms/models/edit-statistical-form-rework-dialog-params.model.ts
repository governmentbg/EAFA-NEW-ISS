import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { StatisticalFormReworkEditDTO } from '@app/models/generated/dtos/StatisticalFormReworkEditDTO';


export class EditStatisticalFormReworkDialogParams extends DialogParamsModel {
    public model: StatisticalFormReworkEditDTO | undefined;

    public constructor(params?: Partial<EditStatisticalFormReworkDialogParams>) {
        super(params);
        Object.assign(this, params);
    }
}