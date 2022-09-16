

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class StatisticalFormsFilters extends BaseRequestModel {

    constructor(obj?: Partial<StatisticalFormsFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public registryNumber: string | undefined;
    public submissionDateFrom: Date | undefined;
    public submissionDateTo: Date | undefined;
    public processUserId: number | undefined;
    public submissionUserId: number | undefined;
    public formTypeIds: number[] | undefined;
    public formObject: string | undefined;
    public personId: number | undefined;
    public legalId: number | undefined;
    public territoryUnitId: number | undefined;
    public filterAquaFarmTerritoryUnit: boolean | undefined;
    public filterReworkTerritoryUnit: boolean | undefined;
    public filterFishVesselTerritoryUnit: boolean | undefined;
}