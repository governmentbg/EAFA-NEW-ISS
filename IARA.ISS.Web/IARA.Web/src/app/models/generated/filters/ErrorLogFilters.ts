

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class ErrorLogFilters extends BaseRequestModel {

    constructor(obj?: Partial<ErrorLogFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public errorLogDateFrom: Date | undefined;
    public errorLogDateTo: Date | undefined;
    public severity: string[] | undefined;
    public class: string | undefined;
    public errorLogId: string | undefined;
    public userId: number | undefined;
    public method: string | undefined;
    public message: string | undefined;
}