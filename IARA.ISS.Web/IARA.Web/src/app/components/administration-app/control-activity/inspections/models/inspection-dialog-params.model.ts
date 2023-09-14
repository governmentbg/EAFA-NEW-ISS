import { DialogParamsModel } from '@app/models/common/dialog-params.model';

export class InspectionDialogParamsModel extends DialogParamsModel {
    public canEditNumber: boolean = false;

    public constructor(params?: Partial<InspectionDialogParamsModel>) {
        super(params);
        Object.assign(this, params);
    }
}