

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FishingGearDTO } from './FishingGearDTO'; 

export class WaterInspectionFishingGearDTO extends FishingGearDTO {
    public constructor(obj?: Partial<WaterInspectionFishingGearDTO>) {
        if (obj != undefined) {
            super(obj as FishingGearDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public inspectedFishingGearId?: number;

    @StrictlyTyped(Boolean)
    public isTaken?: boolean;

    @StrictlyTyped(Boolean)
    public isStored?: boolean;

    @StrictlyTyped(String)
    public storageLocation?: string;
}