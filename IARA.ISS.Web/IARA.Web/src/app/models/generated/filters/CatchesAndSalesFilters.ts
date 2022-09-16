

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class CatchesAndSalesFilters extends BaseRequestModel {

    constructor(obj?: Partial<CatchesAndSalesFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public logBookTypeId: number | undefined;
    public documentNumber: string | undefined;
    public shipId: number | undefined;
    public dateFrom: Date | undefined;
    public dateTo: Date | undefined;
}