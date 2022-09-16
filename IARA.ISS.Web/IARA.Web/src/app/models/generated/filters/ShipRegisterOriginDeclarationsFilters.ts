

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class ShipRegisterOriginDeclarationsFilters extends BaseRequestModel {

    constructor(obj?: Partial<ShipRegisterOriginDeclarationsFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public shipUID: number | undefined;
}