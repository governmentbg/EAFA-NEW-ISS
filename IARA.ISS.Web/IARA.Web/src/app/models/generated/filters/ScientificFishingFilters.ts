

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';
import { ScientificPermitStatusEnum } from '@app/enums/scientific-permit-status.enum';

export class ScientificFishingFilters extends BaseRequestModel {

    constructor(obj?: Partial<ScientificFishingFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public eventisNum: string | undefined;
    public permitNumber: string | undefined;
    public creationDateFrom: Date | undefined;
    public creationDateTo: Date | undefined;
    public validFrom: Date | undefined;
    public validTo: Date | undefined;
    public permitReasonIds: number[] | undefined;
    public permitLegalReasonIds: number[] | undefined;
    public permitRequesterName: string | undefined;
    public permitOwnerName: string | undefined;
    public permitOwnerEgn: string | undefined;
    public scientificOrganizationName: string | undefined;
    public researchWaterArea: string | undefined;
    public aquaticOrganismType: string | undefined;
    public gearType: string | undefined;
    public statuses: ScientificPermitStatusEnum[] | undefined;
    public territoryUnitId: number | undefined;
    public personId: number | undefined;
    public legalId: number | undefined;
}