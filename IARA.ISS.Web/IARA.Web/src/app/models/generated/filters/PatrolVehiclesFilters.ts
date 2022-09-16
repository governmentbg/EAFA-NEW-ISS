

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class PatrolVehiclesFilters extends BaseRequestModel {

    constructor(obj?: Partial<PatrolVehiclesFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public name: string | undefined;
    public flagCountryId: number | undefined;
    public externalMark: string | undefined;
    public patrolVehicleTypeId: number | undefined;
    public cfr: string | undefined;
    public uvi: string | undefined;
    public ircsCallSign: string | undefined;
    public mmsi: string | undefined;
    public vesselTypeId: number | undefined;
    public institutionId: number | undefined;
}