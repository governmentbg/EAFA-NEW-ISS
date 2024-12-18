

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectedFishingGearDTO } from './InspectedFishingGearDTO';
import { InspectionCatchMeasureDTO } from './InspectionCatchMeasureDTO';

export class InspectionShipLogBookPageDataDTO { 
    public constructor(obj?: Partial<InspectionShipLogBookPageDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(InspectedFishingGearDTO)
    public fishingGear?: InspectedFishingGearDTO;

    @StrictlyTyped(InspectionCatchMeasureDTO)
    public catchRecords?: InspectionCatchMeasureDTO[];
}