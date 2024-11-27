

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class PenalDecreesFilters extends BaseRequestModel {

    constructor(obj?: Partial<PenalDecreesFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public penalDecreeNum: string | undefined;
    public drafterId: number | undefined;
    public drafterName: string | undefined;
    public issuerId: number | undefined;
    public issuerName: string | undefined;
    public auanNum: string | undefined;
    public territoryUnitId: number | undefined;
    public departmentId: number | undefined;
    public issueDateFrom: Date | undefined;
    public issueDateTo: Date | undefined;
    public effectiveDateFrom: Date | undefined;
    public effectiveDateTo: Date | undefined;
    public sanctionTypeIds: number[] | undefined;
    public statusTypeIds: number[] | undefined;
    public deliveryConfirmationTypeIds: number[] | undefined;
    public deliveryTypeIds: number[] | undefined;
    public fishingGearId: number | undefined;
    public fishId: number | undefined;
    public applianceId: number | undefined;
    public locationDescription: string | undefined;
    public inspectedEntityFirstName: string | undefined;
    public inspectedEntityMiddleName: string | undefined;
    public inspectedEntityLastName: string | undefined;
    public identifier: string | undefined;
    public fineAmountFrom: number | undefined;
    public fineAmountTo: number | undefined;
    public auanId: number | undefined;
}