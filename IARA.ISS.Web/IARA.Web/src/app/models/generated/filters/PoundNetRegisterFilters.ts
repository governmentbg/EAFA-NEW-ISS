

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class PoundNetRegisterFilters extends BaseRequestModel {

    constructor(obj?: Partial<PoundNetRegisterFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public number: string | undefined;
    public name: string | undefined;
    public muncipalityId: number | undefined;
    public seasonTypeId: number | undefined;
    public categoryTypeId: number | undefined;
    public registeredDateFrom: Date | undefined;
    public registeredDateTo: Date | undefined;
    public statusId: number | undefined;
}