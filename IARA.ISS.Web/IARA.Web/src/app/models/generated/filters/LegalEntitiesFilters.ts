

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class LegalEntitiesFilters extends BaseRequestModel {

    constructor(obj?: Partial<LegalEntitiesFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public legalName: string | undefined;
    public eik: string | undefined;
    public registeredDateFrom: Date | undefined;
    public registeredDateTo: Date | undefined;
    public territoryUnitId: number | undefined;
}