

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class FluxAcdrRequestFilters extends BaseRequestModel {

    constructor(obj?: Partial<FluxAcdrRequestFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public requestDateFrom: Date | undefined;
    public requestDateTo: Date | undefined;
    public responseDateFrom: Date | undefined;
    public responseDateTo: Date | undefined;
    public requestMonthDateFrom: Date | undefined;
    public requestUUID: string | undefined;
    public responseUUID: string | undefined;
    public requestContent: string | undefined;
    public responseContent: string | undefined;
    public responseStatuses: string[] | undefined;
    public domainNames: string[] | undefined;
    public reportStatuses: string[] | undefined;
    public isModified: boolean | undefined;
}