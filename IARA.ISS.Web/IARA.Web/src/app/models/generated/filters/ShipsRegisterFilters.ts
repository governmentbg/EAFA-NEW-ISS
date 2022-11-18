

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class ShipsRegisterFilters extends BaseRequestModel {

    constructor(obj?: Partial<ShipsRegisterFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public eventTypeId: number | undefined;
    public eventDateFrom: Date | undefined;
    public eventDateTo: Date | undefined;
    public cancellationReasonId: number | undefined;
    public isCancelled: boolean | undefined;
    public isForbidden: boolean | undefined;
    public isThirdPartyShip: boolean | undefined;
    public shipOwnerName: string | undefined;
    public shipOwnerEgnLnc: string | undefined;
    public fleetId: number | undefined;
    public vesselTypeId: number | undefined;
    public hasCommercialFishingLicence: boolean | undefined;
    public hasVMS: boolean | undefined;
    public cfr: string | undefined;
    public name: string | undefined;
    public externalMark: string | undefined;
    public ircsCallSign: string | undefined;
    public publicAidTypeId: number | undefined;
    public totalLengthFrom: number | undefined;
    public totalLengthTo: number | undefined;
    public grossTonnageFrom: number | undefined;
    public grossTonnageTo: number | undefined;
    public netTonnageFrom: number | undefined;
    public netTonnageTo: number | undefined;
    public mainEnginePowerFrom: number | undefined;
    public mainEnginePowerTo: number | undefined;
    public mainFishingGearId: number | undefined;
    public additionalFishingGearId: number | undefined;
    public foodLawLicenceDateFrom: Date | undefined;
    public foodLawLicenceDateTo: Date | undefined;
    public allowedForQuotaFishId: number | undefined;
    public shipAssociationId: number | undefined;
    public territoryUnitId: number | undefined;
    public personId: number | undefined;
    public legalId: number | undefined;
}