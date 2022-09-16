

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class FluxFlapRequestFilters extends BaseRequestModel {

    constructor(obj?: Partial<FluxFlapRequestFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public shipId: number | undefined;
    public shipIdentifier: string | undefined;
    public shipName: string | undefined;
    public requestUuid: string | undefined;
    public responseUuid: string | undefined;
    public requestDateFrom: Date | undefined;
    public requestDateTo: Date | undefined;
}