

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AuanInspectedEntityDTO } from './AuanInspectedEntityDTO';
import { PenalDecreeSeizedFishDTO } from './PenalDecreeSeizedFishDTO';
import { PenalDecreeSeizedFishingGearDTO } from './PenalDecreeSeizedFishingGearDTO';
import { AuanViolatedRegulationDTO } from './AuanViolatedRegulationDTO';

export class PenalDecreeAuanDataDTO { 
    public constructor(obj?: Partial<PenalDecreeAuanDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public auanNum?: string;

    @StrictlyTyped(Date)
    public draftDate?: Date;

    @StrictlyTyped(Number)
    public territoryUnitId?: number;

    @StrictlyTyped(String)
    public drafter?: string;

    @StrictlyTyped(String)
    public locationDescription?: string;

    @StrictlyTyped(String)
    public offenderComments?: string;

    @StrictlyTyped(String)
    public constatationComments?: string;

    @StrictlyTyped(AuanInspectedEntityDTO)
    public inspectedEntity?: AuanInspectedEntityDTO;

    @StrictlyTyped(PenalDecreeSeizedFishDTO)
    public confiscatedFish?: PenalDecreeSeizedFishDTO[];

    @StrictlyTyped(PenalDecreeSeizedFishDTO)
    public confiscatedAppliance?: PenalDecreeSeizedFishDTO[];

    @StrictlyTyped(PenalDecreeSeizedFishingGearDTO)
    public confiscatedFishingGear?: PenalDecreeSeizedFishingGearDTO[];

    @StrictlyTyped(AuanViolatedRegulationDTO)
    public violatedRegulations?: AuanViolatedRegulationDTO[];
}