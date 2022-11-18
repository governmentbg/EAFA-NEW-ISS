

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class UserManagementFilters extends BaseRequestModel {

    constructor(obj?: Partial<UserManagementFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public firstName: string | undefined;
    public middleName: string | undefined;
    public lastName: string | undefined;
    public roleId: number | undefined;
    public email: string | undefined;
    public registeredDateFrom: Date | undefined;
    public registeredDateTo: Date | undefined;
    public isRequestedAccess: boolean | undefined;
}