

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class InspectorsFilters extends BaseRequestModel {

    constructor(obj?: Partial<InspectorsFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public username: string | undefined;
    public firstName: string | undefined;
    public lastName: string | undefined;
    public unregPersonName: string | undefined;
    public egnLnc: string | undefined;
    public institution: string | undefined;
    public inspectionSequenceNum: number | undefined;
    public userId: number | undefined;
    public institutionId: number | undefined;
    public inspectorCardNum: string | undefined;
}