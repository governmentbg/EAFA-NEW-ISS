﻿

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class ScientificFishingPublicFilters extends BaseRequestModel {

    constructor(obj?: Partial<ScientificFishingPublicFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public eventisNum: string | undefined;
    public permitNumber: string | undefined;
    public creationDateFrom: Date | undefined;
    public creationDateTo: Date | undefined;
    public validFrom: Date | undefined;
    public validTo: Date | undefined;
}