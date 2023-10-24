

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class FishingActivityReportsFilters extends BaseRequestModel {

    constructor(obj?: Partial<FishingActivityReportsFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public tripIdentifier: string | undefined;
    public shipId: number | undefined;
    public startTime: Date | undefined;
    public endTime: Date | undefined;
}