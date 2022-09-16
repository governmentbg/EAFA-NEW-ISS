

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class CatchesAndSalesPublicFilters extends BaseRequestModel {

    constructor(obj?: Partial<CatchesAndSalesPublicFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public pageNumber: number | undefined;
    public onlinePageNumber: string | undefined;
    public documentNumber: number | undefined;
    public logBookTypeId: number | undefined;
    public logBookNumber: string | undefined;
    public logBookStatusIds: number[] | undefined;
    public logBookValidityStartDate: Date | undefined;
    public logBookValidityEndDate: Date | undefined;
}