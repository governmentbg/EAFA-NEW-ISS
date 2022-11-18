

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AuanInspectedEntityDTO } from './AuanInspectedEntityDTO';
import { AuanWitnessDTO } from './AuanWitnessDTO';
import { AuanViolatedRegulationDTO } from './AuanViolatedRegulationDTO';
import { AuanConfiscatedFishDTO } from './AuanConfiscatedFishDTO';
import { AuanConfiscatedFishingGearDTO } from './AuanConfiscatedFishingGearDTO';
import { AuanDeliveryDataDTO } from './AuanDeliveryDataDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { AuanObjectionResolutionTypesEnum } from '@app/enums/auan-objection-resolution-types.enum';

export class AuanRegisterEditDTO { 
    public constructor(obj?: Partial<AuanRegisterEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public inspectionId?: number;

    @StrictlyTyped(String)
    public auanNum?: string;

    @StrictlyTyped(Date)
    public draftDate?: Date;

    @StrictlyTyped(String)
    public locationDescription?: string;

    @StrictlyTyped(AuanInspectedEntityDTO)
    public inspectedEntity?: AuanInspectedEntityDTO;

    @StrictlyTyped(AuanWitnessDTO)
    public auanWitnesses?: AuanWitnessDTO[];

    @StrictlyTyped(AuanViolatedRegulationDTO)
    public violatedRegulations?: AuanViolatedRegulationDTO[];

    @StrictlyTyped(AuanConfiscatedFishDTO)
    public confiscatedFish?: AuanConfiscatedFishDTO[];

    @StrictlyTyped(AuanConfiscatedFishDTO)
    public confiscatedAppliance?: AuanConfiscatedFishDTO[];

    @StrictlyTyped(AuanConfiscatedFishingGearDTO)
    public confiscatedFishingGear?: AuanConfiscatedFishingGearDTO[];

    @StrictlyTyped(String)
    public constatationComments?: string;

    @StrictlyTyped(String)
    public offenderComments?: string;

    @StrictlyTyped(AuanDeliveryDataDTO)
    public deliveryData?: AuanDeliveryDataDTO;

    @StrictlyTyped(Boolean)
    public hasObjection?: boolean;

    @StrictlyTyped(Date)
    public objectionDate?: Date;

    @StrictlyTyped(Number)
    public resolutionType?: AuanObjectionResolutionTypesEnum;

    @StrictlyTyped(String)
    public resolutionNum?: string;

    @StrictlyTyped(Date)
    public resolutionDate?: Date;

    @StrictlyTyped(Number)
    public statusId?: number;

    @StrictlyTyped(Number)
    public inspectorId?: number;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}