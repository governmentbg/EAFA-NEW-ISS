

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class PrintConfigurationFilters extends BaseRequestModel {

    constructor(obj?: Partial<PrintConfigurationFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public territoryUnitIds: number[] | undefined;
    public userEgnLnch: string | undefined;
    public userNames: string | undefined;
    public applicationTypeIds: number[] | undefined;
    public substituteReason: string | undefined;
}