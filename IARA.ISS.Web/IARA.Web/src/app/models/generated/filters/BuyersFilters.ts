

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class BuyersFilters extends BaseRequestModel {

    constructor(obj?: Partial<BuyersFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public entryTypeId: number | undefined;
    public urorrNumber: string | undefined;
    public registrationNumber: string | undefined;
    public registeredDateFrom: Date | undefined;
    public registeredDateTo: Date | undefined;
    public utilityName: string | undefined;
    public populatedAreaId: number | undefined;
    public districtId: number | undefined;
    public ownerName: string | undefined;
    public ownerEIK: string | undefined;
    public organizerName: string | undefined;
    public organizerEGN: string | undefined;
    public logBookNumber: string | undefined;
    public territoryUnitId: number | undefined;
    public statusIds: number[] | undefined;
    public personId: number | undefined;
    public legalId: number | undefined;
}