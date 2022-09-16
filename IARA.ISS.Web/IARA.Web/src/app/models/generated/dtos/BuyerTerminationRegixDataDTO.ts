

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { BuyerTerminationBaseRegixDataDTO } from './BuyerTerminationBaseRegixDataDTO';
import { ApplicationSubmittedByRegixDataDTO } from './ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from './ApplicationSubmittedForRegixDataDTO'; 

export class BuyerTerminationRegixDataDTO extends BuyerTerminationBaseRegixDataDTO {
    public constructor(obj?: Partial<BuyerTerminationRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as BuyerTerminationBaseRegixDataDTO);
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
}