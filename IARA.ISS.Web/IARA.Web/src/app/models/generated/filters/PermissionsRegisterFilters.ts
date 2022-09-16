
import { BaseRequestModel } from '../../common/BaseRequestModel';

export class PermissionsRegisterFilters extends BaseRequestModel {

    constructor(obj?: Partial<PermissionsRegisterFilters>) {
        if (obj != undefined) {
            super((obj as BaseRequestModel));
            Object.assign(this, obj);
        } else {
            super();
        }
    }


    public name: string | undefined;
    public groupId: number | undefined;
    public typeIds: number[] | undefined;
    public roleId: number | undefined;
}