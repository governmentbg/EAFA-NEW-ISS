

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ShipDeregistrationBaseRegixDataDTO } from './ShipDeregistrationBaseRegixDataDTO';
import { ApplicationSubmittedByRegixDataDTO } from './ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from './ApplicationSubmittedForRegixDataDTO';
import { FishingCapacityFreedActionsRegixDataDTO } from './FishingCapacityFreedActionsRegixDataDTO'; 

export class ShipDeregistrationRegixDataDTO extends ShipDeregistrationBaseRegixDataDTO {
    public constructor(obj?: Partial<ShipDeregistrationRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as ShipDeregistrationBaseRegixDataDTO);
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