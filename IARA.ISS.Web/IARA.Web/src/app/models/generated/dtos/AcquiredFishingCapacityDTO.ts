

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AcquiredCapacityMannerEnum } from '@app/enums/acquired-capacity-manner.enum';

export class AcquiredFishingCapacityDTO { 
    public constructor(obj?: Partial<AcquiredFishingCapacityDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public acquiredManner?: AcquiredCapacityMannerEnum;

    @StrictlyTyped(Number)
    public grossTonnage?: number;

    @StrictlyTyped(Number)
    public power?: number;

    @StrictlyTyped(Number)
    public capacityLicenceIds?: number[];
}