

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FishingGearInspectionDTO } from './FishingGearInspectionDTO';
import { FishingGearMarkInspectionDTO } from './FishingGearMarkInspectionDTO';

export class FishingGearWithMarksDTO { 
    public constructor(obj?: Partial<FishingGearWithMarksDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(FishingGearInspectionDTO)
    public fishingGears?: FishingGearInspectionDTO[];

    @StrictlyTyped(FishingGearMarkInspectionDTO)
    public fishingGearMarks?: FishingGearMarkInspectionDTO[];
}