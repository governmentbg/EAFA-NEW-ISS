

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class RecreationalFishingTicketApplicationFilters extends BaseRequestModel {

    constructor(obj?: Partial<RecreationalFishingTicketApplicationFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public ticketNum: string | undefined;
    public typeIds: number[] | undefined;
    public periodIds: number[] | undefined;
    public ticketHolderName: string | undefined;
    public ticketHolderEGN: string | undefined;
    public validFrom: Date | undefined;
    public validTo: Date | undefined;
    public issueDateFrom: Date | undefined;
    public issueDateTo: Date | undefined;
    public ticketIssuerName: string | undefined;
    public isDuplicate: boolean | undefined;
    public statusIds: number[] | undefined;
    public showOnlyNotFinished: boolean | undefined;
    public personId: number | undefined;
    public territoryUnitId: number | undefined;
    public showExpired: boolean | undefined;
}