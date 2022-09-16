

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FishingCapacityHolderRegixDataDTO } from './FishingCapacityHolderRegixDataDTO';
import { FishingCapacityRemainderActionEnum } from '@app/enums/fishing-capacity-remainder-action.enum';

export class FishingCapacityFreedActionsRegixDataDTO { 
    public constructor(obj?: Partial<FishingCapacityFreedActionsRegixDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public action?: FishingCapacityRemainderActionEnum;

    @StrictlyTyped(FishingCapacityHolderRegixDataDTO)
    public holders?: FishingCapacityHolderRegixDataDTO[];
}