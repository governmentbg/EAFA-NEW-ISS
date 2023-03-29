

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class LogBookPageEditExceptionFilters extends BaseRequestModel {

    constructor(obj?: Partial<LogBookPageEditExceptionFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public userId: number | undefined;
    public logBookTypeIds: number[] | undefined;
    public logBookId: number | undefined;
    public exceptionActiveDateFrom: Date | undefined;
    public exceptionActiveDateTo: Date | undefined;
    public editPageDateFrom: Date | undefined;
    public editPageDateTo: Date | undefined;
}