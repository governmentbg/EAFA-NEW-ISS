

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class FLUXVMSRequestFilters extends BaseRequestModel {

    constructor(obj?: Partial<FLUXVMSRequestFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public webServiceName: string | undefined;
    public requestDateFrom: Date | undefined;
    public requestDateTo: Date | undefined;
    public responseDateFrom: Date | undefined;
    public responseDateTo: Date | undefined;
    public requestUUID: string | undefined;
    public responseUUID: string | undefined;
    public responseStatuses: string[] | undefined;
    public domainNames: string[] | undefined;
}