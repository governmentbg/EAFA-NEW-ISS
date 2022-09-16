

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class QualifiedFishersFilters extends BaseRequestModel {

    constructor(obj?: Partial<QualifiedFishersFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public name: string | undefined;
    public egn: string | undefined;
    public registeredDateFrom: Date | undefined;
    public registeredDateTo: Date | undefined;
    public registrationNum: string | undefined;
    public diplomaNr: string | undefined;
    public personId: number | undefined;
    public territoryUnitId: number | undefined;
}