
import { BaseRequestModel } from '../../common/BaseRequestModel';

export class CatchRecordPublicFilters extends BaseRequestModel {

    constructor(obj?: Partial<CatchRecordPublicFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public ticketId: number | undefined;
}