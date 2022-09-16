

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class NewsFilters extends BaseRequestModel {

    constructor(obj?: Partial<NewsFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public dateFrom: Date | undefined;
    public dateTo: Date | undefined;
    public districtsIds: number[] | undefined;
}