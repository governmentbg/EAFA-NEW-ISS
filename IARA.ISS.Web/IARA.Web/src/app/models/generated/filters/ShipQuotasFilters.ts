

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class ShipQuotasFilters extends BaseRequestModel {

    constructor(obj?: Partial<ShipQuotasFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public shipId: number | undefined;
    public year: number | undefined;
    public fishId: number | undefined;
    public association: string | undefined;
    public cfr: string | undefined;
    public shipQuotaId: number | undefined;
}