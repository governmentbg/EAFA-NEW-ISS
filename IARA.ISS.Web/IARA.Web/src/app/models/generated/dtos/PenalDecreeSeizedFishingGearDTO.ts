

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AuanConfiscatedFishingGearDTO } from './AuanConfiscatedFishingGearDTO'; 

export class PenalDecreeSeizedFishingGearDTO extends AuanConfiscatedFishingGearDTO {
    public constructor(obj?: Partial<PenalDecreeSeizedFishingGearDTO>) {
        if (obj != undefined) {
            super(obj as AuanConfiscatedFishingGearDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public storageTerritoryUnitId?: number;
}