

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ReduceFishingCapacityBaseRegixDataDTO } from './ReduceFishingCapacityBaseRegixDataDTO';
import { ApplicationSubmittedByRegixDataDTO } from './ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from './ApplicationSubmittedForRegixDataDTO';
import { FishingCapacityFreedActionsRegixDataDTO } from './FishingCapacityFreedActionsRegixDataDTO'; 

export class ReduceFishingCapacityRegixDataDTO extends ReduceFishingCapacityBaseRegixDataDTO {
    public constructor(obj?: Partial<ReduceFishingCapacityRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as ReduceFishingCapacityBaseRegixDataDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(ApplicationSubmittedByRegixDataDTO)
    public submittedBy?: ApplicationSubmittedByRegixDataDTO;

    @StrictlyTyped(ApplicationSubmittedForRegixDataDTO)
    public submittedFor?: ApplicationSubmittedForRegixDataDTO;

    @StrictlyTyped(FishingCapacityFreedActionsRegixDataDTO)
    public freedCapacityAction?: FishingCapacityFreedActionsRegixDataDTO;
}