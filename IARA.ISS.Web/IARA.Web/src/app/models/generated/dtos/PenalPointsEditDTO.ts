

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { RegixLegalDataDTO } from './RegixLegalDataDTO';
import { PenalPointsAppealDTO } from './PenalPointsAppealDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { PointsTypeEnum } from '@app/enums/points-type.enum';

export class PenalPointsEditDTO { 
    public constructor(obj?: Partial<PenalPointsEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public auanId?: number;

    @StrictlyTyped(Number)
    public decreeId?: number;

    @StrictlyTyped(Number)
    public pointsType?: PointsTypeEnum;

    @StrictlyTyped(String)
    public decreeNum?: string;

    @StrictlyTyped(String)
    public reportNoteNum?: string;

    @StrictlyTyped(Date)
    public reportNoteDate?: Date;

    @StrictlyTyped(Boolean)
    public isIncreasePoints?: boolean;

    @StrictlyTyped(Date)
    public issueDate?: Date;

    @StrictlyTyped(Date)
    public effectiveDate?: Date;

    @StrictlyTyped(Date)
    public deliveryDate?: Date;

    @StrictlyTyped(Number)
    public territoryUnitId?: number;

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(Number)
    public permitId?: number;

    @StrictlyTyped(Boolean)
    public isPermitOwner?: boolean;

    @StrictlyTyped(Number)
    public permitOwnerPersonId?: number;

    @StrictlyTyped(Number)
    public permitOwnerLegalId?: number;

    @StrictlyTyped(Number)
    public qualifiedFisherId?: number;

    @StrictlyTyped(Number)
    public permitLicenseId?: number;

    @StrictlyTyped(RegixPersonDataDTO)
    public personOwner?: RegixPersonDataDTO;

    @StrictlyTyped(RegixLegalDataDTO)
    public legalOwner?: RegixLegalDataDTO;

    @StrictlyTyped(Number)
    public pointsAmount?: number;

    @StrictlyTyped(String)
    public comments?: string;

    @StrictlyTyped(String)
    public issuer?: string;

    @StrictlyTyped(PenalPointsAppealDTO)
    public appealStatuses?: PenalPointsAppealDTO[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}