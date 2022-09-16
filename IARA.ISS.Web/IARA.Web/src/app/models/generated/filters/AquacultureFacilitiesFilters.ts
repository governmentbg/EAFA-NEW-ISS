

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class AquacultureFacilitiesFilters extends BaseRequestModel {

    constructor(obj?: Partial<AquacultureFacilitiesFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public regNum: string | undefined;
    public urorNum: string | undefined;
    public name: string | undefined;
    public eik: string | undefined;
    public registrationDateFrom: Date | undefined;
    public registrationDateTo: Date | undefined;
    public statusIds: number[] | undefined;
    public territoryUnitId: number | undefined;
    public waterAreaTypeIds: number[] | undefined;
    public populatedAreaId: number | undefined;
    public location: string | undefined;
    public waterSalinityTypes: string[] | undefined;
    public waterTemperatureTypes: string[] | undefined;
    public systemTypes: string[] | undefined;
    public aquaticOrganismId: number | undefined;
    public powerSupplyTypeId: number | undefined;
    public installationTypeIds: number[] | undefined;
    public totalWaterAreaFrom: number | undefined;
    public totalWaterAreaTo: number | undefined;
    public totalProductionCapacityFrom: number | undefined;
    public totalProductionCapacityTo: number | undefined;
    public personId: number | undefined;
    public legalId: number | undefined;
}