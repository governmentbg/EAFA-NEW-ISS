

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';

export class FishingCapacityCertificatesFilters extends BaseRequestModel {

    constructor(obj?: Partial<FishingCapacityCertificatesFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public certificateId: number | undefined;
    public certificateNum: number | undefined;
    public validFrom: Date | undefined;
    public validTo: Date | undefined;
    public holderNames: string | undefined;
    public holderEgnEik: string | undefined;
    public grossTonnageFrom: number | undefined;
    public grossTonnageTo: number | undefined;
    public powerFrom: number | undefined;
    public powerTo: number | undefined;
    public isCertificateActive: boolean | undefined;
    public personId: number | undefined;
    public legalId: number | undefined;
}