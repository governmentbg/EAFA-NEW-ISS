

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AuanInspectionDTO } from './AuanInspectionDTO';
import { AuanInspectedEntityDTO } from './AuanInspectedEntityDTO';
import { PenalDecreeSeizedFishDTO } from './PenalDecreeSeizedFishDTO';
import { PenalDecreeSeizedFishingGearDTO } from './PenalDecreeSeizedFishingGearDTO';
import { AuanViolatedRegulationDTO } from './AuanViolatedRegulationDTO'; 

export class PenalDecreeAuanDataDTO extends AuanInspectionDTO {
    public constructor(obj?: Partial<PenalDecreeAuanDataDTO>) {
        if (obj != undefined) {
            super(obj as AuanInspectionDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(String)
    public auanNum?: string;

    @StrictlyTyped(Date)
    public draftDate?: Date;

    @StrictlyTyped(String)
    public drafter?: string;

    @StrictlyTyped(String)
    public inspectorName?: string;

    @StrictlyTyped(String)
    public locationDescription?: string;

    @StrictlyTyped(String)
    public offenderComments?: string;

    @StrictlyTyped(String)
    public constatationComments?: string;

    @StrictlyTyped(Boolean)
    public isExternal?: boolean;

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