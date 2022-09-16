

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class YearlyQuotasFilters extends BaseRequestModel {

    constructor(obj?: Partial<YearlyQuotasFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public year: number | undefined;
    public fishId: number | undefined;
}