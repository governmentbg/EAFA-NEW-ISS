

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class ReportParameterDefinitionFilters extends BaseRequestModel {

    constructor(obj?: Partial<ReportParameterDefinitionFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public validFrom: Date | undefined;
    public validTo: Date | undefined;
}