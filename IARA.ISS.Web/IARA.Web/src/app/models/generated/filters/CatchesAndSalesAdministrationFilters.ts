

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class CatchesAndSalesAdministrationFilters extends BaseRequestModel {

    constructor(obj?: Partial<CatchesAndSalesAdministrationFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public pageNumber: number | undefined;
    public onlinePageNumber: string | undefined;
    public logBookTypeId: number | undefined;
    public logBookNumber: string | undefined;
    public documentNumber: number | undefined;
    public showOnlyExistingPages: boolean | undefined;
    public shipId: number | undefined;
    public aquacultureId: number | undefined;
    public registeredBuyerId: number | undefined;
    public ownerEngEik: string | undefined;
    public logBookStatusIds: number[] | undefined;
    public logBookValidityStartDate: Date | undefined;
    public logBookValidityEndDate: Date | undefined;
    public territoryUnitId: number | undefined;
    public filterFishLogBookTeritorryUnitId: boolean | undefined;
    public filterFirstSaleLogBookTeritorryUnitId: boolean | undefined;
    public filterAdmissionLogBookTeritorryUnitId: boolean | undefined;
    public filterTransportationLogBookTeritorryUnitId: boolean | undefined;
    public filterAquacultureLogBookTeritorryUnitId: boolean | undefined;
    public personId: number | undefined;
}