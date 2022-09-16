

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class ShipRegisterLogBookPagesFilters extends BaseRequestModel {

    constructor(obj?: Partial<ShipRegisterLogBookPagesFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public shipUID: number | undefined;
}