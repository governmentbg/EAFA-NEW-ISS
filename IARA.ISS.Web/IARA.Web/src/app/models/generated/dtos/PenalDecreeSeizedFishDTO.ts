

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AuanConfiscatedFishDTO } from './AuanConfiscatedFishDTO'; 

export class PenalDecreeSeizedFishDTO extends AuanConfiscatedFishDTO {
    public constructor(obj?: Partial<PenalDecreeSeizedFishDTO>) {
        if (obj != undefined) {
            super(obj as AuanConfiscatedFishDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public storageTerritoryUnitId?: number;
}