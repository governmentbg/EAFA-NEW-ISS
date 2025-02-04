

import { BaseRequestModel } from '@app/models/common/BaseRequestModel';
import { PointsTypeEnum } from '../../../enums/points-type.enum';

export class PenalPointsFilters extends BaseRequestModel {

    constructor(obj?: Partial<PenalPointsFilters>) {
      if (obj != undefined) { 
        super((obj as BaseRequestModel));
        Object.assign(this, obj);
      } else {
        super();
      }
    }

    
    public penalDecreeNum: string | undefined;
    public decreeNum: string | undefined;
    public permitNum: string | undefined;
    public permitLicenseNum: string | undefined;
    public decreeDateFrom: Date | undefined;
    public decreeDateTo: Date | undefined;
    public penalDecreeDateFrom: Date | undefined;
    public penalDecreeDateTo: Date | undefined;
    public pointsAmountFrom: number | undefined;
    public pointsAmountTo: number | undefined;
    public statusIds: number[] | undefined;
    public shipId: number | undefined;
    public shipRegistrationCertificateNumber: string | undefined;
    public permitOwnerName: string | undefined;
    public permitOwnerIdentifier: string | undefined;
    public captainName: string | undefined;
    public captainIdentifier: string | undefined;
    public isIncreasePoints: boolean | undefined;
    public pointsType: PointsTypeEnum | undefined;
    public year: number | undefined;
    public personId: number | undefined;
    public legalId: number | undefined;
    public penalDecreeId: number | undefined;
}