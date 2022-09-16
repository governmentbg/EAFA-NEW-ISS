

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { TransferFishingCapacityBaseRegixDataDTO } from './TransferFishingCapacityBaseRegixDataDTO';
import { ApplicationSubmittedByRegixDataDTO } from './ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from './ApplicationSubmittedForRegixDataDTO';
import { FishingCapacityHolderRegixDataDTO } from './FishingCapacityHolderRegixDataDTO'; 

export class TransferFishingCapacityRegixDataDTO extends TransferFishingCapacityBaseRegixDataDTO {
    public constructor(obj?: Partial<TransferFishingCapacityRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as TransferFishingCapacityBaseRegixDataDTO);
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

    @StrictlyTyped(FishingCapacityHolderRegixDataDTO)
    public holders?: FishingCapacityHolderRegixDataDTO[];
}