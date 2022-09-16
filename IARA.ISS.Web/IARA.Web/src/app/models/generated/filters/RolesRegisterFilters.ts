
import { BaseRequestModel } from '../../common/BaseRequestModel';

export class RolesRegisterFilters extends BaseRequestModel {

    constructor(obj?: Partial<RolesRegisterFilters>) {
        if (obj != undefined) {
            super((obj as BaseRequestModel));
            Object.assign(this, obj);
        } else {
            super();
        }
    }


    public code: string | undefined;
    public name: string | undefined;
    public permissionId: number | undefined;
    public validFrom: Date | undefined;
    public validTo: Date | undefined;
}