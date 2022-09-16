

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { IncreaseFishingCapacityBaseRegixDataDTO } from './IncreaseFishingCapacityBaseRegixDataDTO';
import { ApplicationSubmittedByRegixDataDTO } from './ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from './ApplicationSubmittedForRegixDataDTO';
import { FishingCapacityFreedActionsRegixDataDTO } from './FishingCapacityFreedActionsRegixDataDTO'; 

export class IncreaseFishingCapacityRegixDataDTO extends IncreaseFishingCapacityBaseRegixDataDTO {
    public constructor(obj?: Partial<IncreaseFishingCapacityRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as IncreaseFishingCapacityBaseRegixDataDTO);
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
    public remainingCapacityAction?: FishingCapacityFreedActionsRegixDataDTO;
}