

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class AuanRegisterFilters extends BaseRequestModel {

    constructor(obj?: Partial<AuanRegisterFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public id: number | undefined;
    public auanNum: string | undefined;
    public drafterId: number | undefined;
    public drafterName: string | undefined;
    public territoryUnitId: number | undefined;
    public draftDateFrom: Date | undefined;
    public draftDateTo: Date | undefined;
    public inspectionTypeId: number | undefined;
    public fishingGearId: number | undefined;
    public fishId: number | undefined;
    public applianceId: number | undefined;
    public locationDescription: string | undefined;
    public inspectedEntityFirstName: string | undefined;
    public inspectedEntityMiddleName: string | undefined;
    public inspectedEntityLastName: string | undefined;
    public identifier: string | undefined;
    public isDelivered: boolean | undefined;
    public personId: number | undefined;
    public legalId: number | undefined;
    public inspectionId: number | undefined;
}