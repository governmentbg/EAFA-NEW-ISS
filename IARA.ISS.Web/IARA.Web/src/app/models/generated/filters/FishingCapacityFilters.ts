

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class FishingCapacityFilters extends BaseRequestModel {

    constructor(obj?: Partial<FishingCapacityFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public shipCfr: string | undefined;
    public shipName: string | undefined;
    public grossTonnageFrom: number | undefined;
    public grossTonnageTo: number | undefined;
    public powerFrom: number | undefined;
    public powerTo: number | undefined;
    public territoryUnitId: number | undefined;
}