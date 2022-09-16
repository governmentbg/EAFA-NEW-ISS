

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class MaximumFishingCapacityFilters extends BaseRequestModel {

    constructor(obj?: Partial<MaximumFishingCapacityFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public dateFrom: Date | undefined;
    public dateTo: Date | undefined;
    public regulation: string | undefined;
}