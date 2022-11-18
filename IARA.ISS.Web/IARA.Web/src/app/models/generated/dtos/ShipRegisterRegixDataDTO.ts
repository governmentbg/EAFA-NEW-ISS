

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ShipRegisterBaseRegixDataDTO } from './ShipRegisterBaseRegixDataDTO';
import { ApplicationSubmittedByRegixDataDTO } from './ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from './ApplicationSubmittedForRegixDataDTO';
import { ShipOwnerRegixDataDTO } from './ShipOwnerRegixDataDTO';
import { FishingCapacityFreedActionsRegixDataDTO } from './FishingCapacityFreedActionsRegixDataDTO'; 

export class ShipRegisterRegixDataDTO extends ShipRegisterBaseRegixDataDTO {
    public constructor(obj?: Partial<ShipRegisterRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as ShipRegisterBaseRegixDataDTO);
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

    @StrictlyTyped(ShipOwnerRegixDataDTO)
    public owners?: ShipOwnerRegixDataDTO[];

    @StrictlyTyped(FishingCapacityFreedActionsRegixDataDTO)
    public remainingCapacityAction?: FishingCapacityFreedActionsRegixDataDTO;
}