

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FishingCapacityHolderDTO } from './FishingCapacityHolderDTO';
import { FishingCapacityRemainderActionEnum } from '@app/enums/fishing-capacity-remainder-action.enum';

export class FishingCapacityFreedActionsDTO { 
    public constructor(obj?: Partial<FishingCapacityFreedActionsDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public action?: FishingCapacityRemainderActionEnum;

    @StrictlyTyped(FishingCapacityHolderDTO)
    public holders?: FishingCapacityHolderDTO[];
}