import { DialogParamsModel } from '@app/models/common/dialog-params.model';

export class AssignApplicationByUserParameters extends DialogParamsModel {
    public hasAllTerritoryUnitsPermission: boolean = false;

    public constructor(obj?: Partial<AssignApplicationByUserParameters>) {
        super(obj);
        Object.assign(this, obj);
    }
}