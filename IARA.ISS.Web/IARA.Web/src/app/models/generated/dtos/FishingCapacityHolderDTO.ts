

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FishingCapacityHolderRegixDataDTO } from './FishingCapacityHolderRegixDataDTO'; 

export class FishingCapacityHolderDTO extends FishingCapacityHolderRegixDataDTO {
    public constructor(obj?: Partial<FishingCapacityHolderDTO>) {
        if (obj != undefined) {
            super(obj as FishingCapacityHolderRegixDataDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public transferredTonnage?: number;

    @StrictlyTyped(Number)
    public transferredPower?: number;
}