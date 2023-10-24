

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FishingGearDTO } from './FishingGearDTO';
import { InspectedFishingGearEnum } from '@app/enums/inspected-fishing-gear.enum';

export class InspectedFishingGearDTO { 
    public constructor(obj?: Partial<InspectedFishingGearDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(FishingGearDTO)
    public permittedFishingGear?: FishingGearDTO;

    @StrictlyTyped(FishingGearDTO)
    public inspectedFishingGear?: FishingGearDTO;

    @StrictlyTyped(Boolean)
    public hasAttachedAppliances?: boolean;

    @StrictlyTyped(Number)
    public checkInspectedMatchingRegisteredGear?: InspectedFishingGearEnum;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}