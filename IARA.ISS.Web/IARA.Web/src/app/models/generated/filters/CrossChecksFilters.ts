

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class CrossChecksFilters extends BaseRequestModel {

    constructor(obj?: Partial<CrossChecksFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public name: string | undefined;
    public checkedTable: string | undefined;
    public dataSource: string | undefined;
    public reportGroupName: string | undefined;
    public errorLevels: number[] | undefined;
    public autoExecFrequencyCodes: string[] | undefined;
}