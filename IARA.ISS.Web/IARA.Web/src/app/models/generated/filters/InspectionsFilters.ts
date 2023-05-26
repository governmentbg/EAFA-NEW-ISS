

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class InspectionsFilters extends BaseRequestModel {

    constructor(obj?: Partial<InspectionsFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public shipId: number | undefined;
    public territoryNode: number | undefined;
    public inspector: string | undefined;
    public inspectionTypeId: number | undefined;
    public reportNumber: string | undefined;
    public dateFrom: Date | undefined;
    public dateTo: Date | undefined;
    public subjectIsLegal: boolean | undefined;
    public subjectName: string | undefined;
    public subjectEIK: string | undefined;
    public subjectEGN: string | undefined;
    public updatedAfter: Date | undefined;
    public showBothActiveAndInactive: boolean | undefined;
    public inspectedPersonId: number | undefined;
    public inspectedLegalId: number | undefined;
}