

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class CrossCheckResultsFilters extends BaseRequestModel {

    constructor(obj?: Partial<CrossCheckResultsFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public checkCode: string | undefined;
    public checkName: string | undefined;
    public checkTableName: string | undefined;
    public tableId: string | undefined;
    public errorDescription: string | undefined;
    public resolutionIds: number[] | undefined;
    public resolutionTypeId: number | undefined;
    public validFrom: Date | undefined;
    public validTo: Date | undefined;
    public assignedUserId: number | undefined;
    public crossCheckResultId: number | undefined;
    public groupIds: number[] | undefined;
}