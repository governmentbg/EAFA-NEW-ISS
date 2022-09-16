

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { SimpleFishingCapacityDTO } from './SimpleFishingCapacityDTO';

export class FishingCapacityStatisticsDTO { 
    public constructor(obj?: Partial<FishingCapacityStatisticsDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(SimpleFishingCapacityDTO)
    public maximumFishingCapacity?: SimpleFishingCapacityDTO;

    @StrictlyTyped(SimpleFishingCapacityDTO)
    public totalCapacityFromActiveCertificates?: SimpleFishingCapacityDTO;

    @StrictlyTyped(SimpleFishingCapacityDTO)
    public totalActiveShipFishingCapacity?: SimpleFishingCapacityDTO;

    @StrictlyTyped(SimpleFishingCapacityDTO)
    public totalUnusedFishingCapacity?: SimpleFishingCapacityDTO;
}