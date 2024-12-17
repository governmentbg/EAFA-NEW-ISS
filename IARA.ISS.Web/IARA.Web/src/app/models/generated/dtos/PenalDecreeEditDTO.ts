

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PenalDecreeSeizedFishDTO } from './PenalDecreeSeizedFishDTO';
import { PenalDecreeSeizedFishingGearDTO } from './PenalDecreeSeizedFishingGearDTO';
import { PenalDecreeFishCompensationDTO } from './PenalDecreeFishCompensationDTO';
import { AuanViolatedRegulationDTO } from './AuanViolatedRegulationDTO';
import { AuanDeliveryDataDTO } from './AuanDeliveryDataDTO';
import { PenalDecreeAuanDataDTO } from './PenalDecreeAuanDataDTO';
import { PenalDecreeResolutionDTO } from './PenalDecreeResolutionDTO';
import { PenalDecreeStatusEditDTO } from './PenalDecreeStatusEditDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { AuanStatusEnum } from '@app/enums/auan-status.enum';

export class PenalDecreeEditDTO { 
    public constructor(obj?: Partial<PenalDecreeEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public auanId?: number;

    @StrictlyTyped(Number)
    public typeId?: number;

    @StrictlyTyped(Number)
    public issuerUserId?: number;

    @StrictlyTyped(String)
    public decreeNum?: string;

    @StrictlyTyped(Date)
    public issueDate?: Date;

    @StrictlyTyped(Date)
    public effectiveDate?: Date;

    @StrictlyTyped(Number)
    public sanctionTypeIds?: number[];

    @StrictlyTyped(Number)
    public fineAmount?: number;

    @StrictlyTyped(Number)
    public penalDecreeStatus?: AuanStatusEnum;

    @StrictlyTyped(PenalDecreeSeizedFishDTO)
    public seizedFish?: PenalDecreeSeizedFishDTO[];

    @StrictlyTyped(PenalDecreeSeizedFishDTO)
    public seizedAppliance?: PenalDecreeSeizedFishDTO[];

    @StrictlyTyped(PenalDecreeSeizedFishingGearDTO)
    public seizedFishingGear?: PenalDecreeSeizedFishingGearDTO[];

    @StrictlyTyped(PenalDecreeFishCompensationDTO)
    public fishCompensations?: PenalDecreeFishCompensationDTO[];

    @StrictlyTyped(AuanViolatedRegulationDTO)
    public auanViolatedRegulations?: AuanViolatedRegulationDTO[];

    @StrictlyTyped(AuanViolatedRegulationDTO)
    public decreeViolatedRegulations?: AuanViolatedRegulationDTO[];

    @StrictlyTyped(AuanViolatedRegulationDTO)
    public fishCompensationViolatedRegulations?: AuanViolatedRegulationDTO[];

    @StrictlyTyped(Number)
    public compensationAmount?: number;

    @StrictlyTyped(Boolean)
    public isRecurrentViolation?: boolean;

    @StrictlyTyped(Number)
    public appealCourtId?: number;

    @StrictlyTyped(String)
    public issuerPosition?: string;

    @StrictlyTyped(String)
    public sanctionDescription?: string;

    @StrictlyTyped(String)
    public comments?: string;

    @StrictlyTyped(String)
    public constatationComments?: string;

    @StrictlyTyped(String)
    public minorCircumstancesDescription?: string;

    @StrictlyTyped(String)
    public evidenceComments?: string;

    @StrictlyTyped(AuanDeliveryDataDTO)
    public deliveryData?: AuanDeliveryDataDTO;

    @StrictlyTyped(PenalDecreeAuanDataDTO)
    public auanData?: PenalDecreeAuanDataDTO;

    @StrictlyTyped(PenalDecreeResolutionDTO)
    public resolutionData?: PenalDecreeResolutionDTO;

    @StrictlyTyped(PenalDecreeStatusEditDTO)
    public statuses?: PenalDecreeStatusEditDTO[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}