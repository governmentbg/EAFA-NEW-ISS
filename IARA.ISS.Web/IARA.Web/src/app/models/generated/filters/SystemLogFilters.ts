
import { BaseRequestModel } from '../../common/BaseRequestModel';

export class SystemLogFilters extends BaseRequestModel {

    constructor(obj?: Partial<SystemLogFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public actionTypeId: number | undefined;
    public registeredDateFrom: Date | undefined;
    public registeredDateTo: Date | undefined;
    public userId: number | undefined;
    public tableId: string | undefined;
    public tableName: string | undefined;
}