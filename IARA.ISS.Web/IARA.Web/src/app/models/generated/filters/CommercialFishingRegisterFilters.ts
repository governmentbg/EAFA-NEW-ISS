

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class CommercialFishingRegisterFilters extends BaseRequestModel {

    constructor(obj?: Partial<CommercialFishingRegisterFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public permitTypeId: number | undefined;
    public permitLicenseTypeId: number | undefined;
    public number: string | undefined;
    public issuedOnRangeStartDate: Date | undefined;
    public issuedOnRangeEndDate: Date | undefined;
    public shipId: number | undefined;
    public shipName: string | undefined;
    public shipCfr: string | undefined;
    public shipExternalMarking: string | undefined;
    public shipRegistrationCertificateNumber: string | undefined;
    public poundNetName: string | undefined;
    public poundNetNumber: string | undefined;
    public fishingGearTypeId: number | undefined;
    public fishingGearMarkNumber: string | undefined;
    public fishingGearPingerNumber: string | undefined;
    public submittedForName: string | undefined;
    public submittedForIdentifier: string | undefined;
    public logbookNumber: string | undefined;
    public permitTerritoryUnitId: number | undefined;
    public permitIsSuspended: boolean | undefined;
    public permitIsExpired: boolean | undefined;
    public permitLicenseTerritoryUnitId: number | undefined;
    public permitLicenseIsSuspended: boolean | undefined;
    public permitLicenseIsExpired: boolean | undefined;
    public personId: number | undefined;
    public legalId: number | undefined;
}