
import { BaseRequestModel } from '../../common/BaseRequestModel';

export class NomenclaturesFilters extends BaseRequestModel {

    constructor(obj?: Partial<NomenclaturesFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public tableId: number | undefined;
    public name: string | undefined;
    public code: string | undefined;
    public validityDateFrom: Date | undefined;
    public validityDateTo: Date | undefined;
}